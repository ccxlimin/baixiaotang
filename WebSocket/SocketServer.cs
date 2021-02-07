using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocket
{
    public class SocketServer : ISocket
    {
        private Dictionary<Socket, ClientInfo> clientPool = new Dictionary<Socket, ClientInfo>();
        private List<SocketMessage> msgPool = new List<SocketMessage>();

        public void Accept(IAsyncResult result)
        {
            /*
             BeginRecieve方法的MSDN有解释，和Accept一样也是异步处理，接收客户端消息，放入第一个参数中，它也传入了一个回调函数的委托，和带有socket state的对象，用于处理下一次接收。我们把接收成功地客户端socket及其对应信息存放到clientPool中
             */
            Socket server = result.AsyncState as Socket;
            Socket client = server.EndAccept(result);
            try
            {
                //处理下一个客户端连接
                server.BeginAccept(new AsyncCallback(Accept), server);
                byte[] buffer = new byte[1024];
                //接收客户端消息
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Recieve), client);
                ClientInfo info = new ClientInfo();
                info.Id = client.RemoteEndPoint;
                info.handle = client.Handle;
                info.buffer = buffer;
                //把客户端存入clientPool
                this.clientPool.Add(client, info);
                Console.WriteLine(string.Format("Client {0} connected", client.RemoteEndPoint));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error :\r\n\t" + ex.ToString());
            }
        }

        public void Broadcast()
        {
            /*
             Broadcast()方法启用了一个新线程，循环检测msgPool是否为空，当不为空的时候遍历所有客户端，调用send方法发送msgPool里面的第一条消息，然后清除该消息继续检测，直到消息广播完，其实这就是一个阉割版的观察者模式 ，顺便看一下打包数据方法
             */
            Thread broadcast = new Thread(() =>
            {
                while (true)
                {
                    if (msgPool.Count > 0)
                    {
                        byte[] msg = PackageMessage(msgPool[0]);
                        foreach (KeyValuePair<Socket, ClientInfo> cs in clientPool)
                        {
                            Socket client = cs.Key;
                            if (client.Connected)
                            {
                                client.Send(msg, msg.Length, SocketFlags.None);
                            }
                        }
                        msgPool.RemoveAt(0);
                    }
                }
            });
            broadcast.Start();
        }

        public byte[] PackageMessage(SocketMessage sm)
        {
            StringBuilder packagedMsg = new StringBuilder();
            if (!sm.isLogin) //消息是login信息
            {
                packagedMsg.AppendFormat("{0} @ {1}:\r\n    ", sm.Client.Name, sm.Time.ToShortTimeString());
                packagedMsg.Append(sm.Message);
            }
            else //处理普通消息
            {
                packagedMsg.AppendFormat("{0} login @ {1}", sm.Client.Name, sm.Time.ToShortTimeString());
            }

            return Encoding.UTF8.GetBytes(packagedMsg.ToString());
        }

        public void Recieve(IAsyncResult result)
        {
            /*
             我加入了用户名处理用于广播客户端消息的时候显示客户端自定义的昵称而不是生硬的ip地址+端口号，当然这里需要客户端配合
             */
            Socket client = result.AsyncState as Socket;

            if (client == null || !clientPool.ContainsKey(client))
            {
                return;
            }

            try
            {
                int length = client.EndReceive(result);
                byte[] buffer = clientPool[client].buffer;

                //接收消息
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Recieve), client);
                string msg = Encoding.UTF8.GetString(buffer, 0, length);
                SocketMessage sm = new SocketMessage();
                sm.Client = clientPool[client];
                sm.Time = DateTime.Now;

                Regex reg = new Regex(@"{<(.*?)>}");
                Match m = reg.Match(msg);
                if (m.Value != "") //处理客户端传来的用户名
                {
                    clientPool[client].NickName = Regex.Replace(m.Value, @"{<(.*?)>}", "$1");
                    sm.isLogin = true;
                    sm.Message = "login!";
                    Console.WriteLine("{0} login @ {1}", client.RemoteEndPoint, DateTime.Now);
                }
                else //处理客户端传来的普通消息
                {
                    sm.isLogin = false;
                    sm.Message = msg;
                    Console.WriteLine("{0} @ {1}\r\n    {2}", client.RemoteEndPoint, DateTime.Now, msg);
                }
                msgPool.Add(sm);
            }
            catch
            {
                //把客户端标记为关闭，并在clientPool中清除
                client.Disconnect(true);
                Console.WriteLine("Client {0} disconnet", clientPool[client].Name);
                clientPool.Remove(client);
            }
        }

        /// <summary>
        /// 这是该类唯一提供的共有方法，供外界调用，来根据port参数创建一个socket
        /// </summary>
        /// <param name="port"></param>
        public void Run(int port)
        {
            /*
             1.在一个新线程中创建服务器socket，最多允许10个客户端连接。
2.在方法最后调用Broadcast()方法用于向所有客户端广播消息
3.BeginAccept方法，MSDN上有权威解释，但是觉得不够接地气，简单说一下我的理解，首先这个方法是异步的，用于服务器接受一个客户端的连接，第一个参数实际上是回调函数，在C#中使用委托，在回调函数中通过调用EndAccept就可以获得尝试连接的客户端socket，第二个参数是包含请求state的对象，传入server socket对象本身就可以了
             */
            Thread serverSocketThraed = new Thread(() =>
            {
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Any, port));
                server.Listen(10);
                server.BeginAccept(new AsyncCallback(Accept), server);
            });

            serverSocketThraed.Start();
            Console.WriteLine("Server is ready");
            Broadcast();
        }


        //IPEndPoint serverPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8256);


        /// <summary>
        /// 打包服务器握手数据
        /// </summary>
        /// <returns>The hand shake data.</returns>
        /// <param name="handShakeBytes">Hand shake bytes.</param>
        /// <param name="length">Length.</param>
        private byte[] PackageHandShakeData(byte[] handShakeBytes, int length)
        {
            string handShakeText = Encoding.UTF8.GetString(handShakeBytes, 0, length);
            string key = string.Empty;
            Regex reg = new Regex(@"Sec\-WebSocket\-Key:(.*?)\r\n");
            Match m = reg.Match(handShakeText);
            if (m.Value != "")
            {
                key = Regex.Replace(m.Value, @"Sec\-WebSocket\-Key:(.*?)\r\n", "$1").Trim();
            }

            byte[] secKeyBytes = SHA1.Create().ComputeHash(
                                     Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
            string secKey = Convert.ToBase64String(secKeyBytes);

            var responseBuilder = new StringBuilder();
            responseBuilder.Append("HTTP/1.1 101 Switching Protocols" + "\r\n");
            responseBuilder.Append("Upgrade: websocket" + "\r\n");
            responseBuilder.Append("Connection: Upgrade" + "\r\n");
            responseBuilder.Append("Sec-WebSocket-Accept: " + secKey + "\r\n\r\n");

            return Encoding.UTF8.GetBytes(responseBuilder.ToString());
        }

        /// <summary>
        /// 解析客户端发送来的数据
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="recBytes">Rec bytes.</param>
        /// <param name="length">Length.</param>
        private string AnalyzeClientData(byte[] recBytes, int length)
        {
            if (length < 2)
            {
                return string.Empty;
            }

            bool fin = (recBytes[0] & 0x80) == 0x80; // 1bit，1表示最后一帧  
            if (!fin)
            {
                return string.Empty;// 超过一帧暂不处理 
            }

            bool mask_flag = (recBytes[1] & 0x80) == 0x80; // 是否包含掩码  
            if (!mask_flag)
            {
                return string.Empty;// 不包含掩码的暂不处理
            }

            int payload_len = recBytes[1] & 0x7F; // 数据长度  

            byte[] masks = new byte[4];
            byte[] payload_data;

            if (payload_len == 126)
            {
                Array.Copy(recBytes, 4, masks, 0, 4);
                payload_len = (UInt16)(recBytes[2] << 8 | recBytes[3]);
                payload_data = new byte[payload_len];
                Array.Copy(recBytes, 8, payload_data, 0, payload_len);

            }
            else if (payload_len == 127)
            {
                Array.Copy(recBytes, 10, masks, 0, 4);
                byte[] uInt64Bytes = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    uInt64Bytes[i] = recBytes[9 - i];
                }
                UInt64 len = BitConverter.ToUInt64(uInt64Bytes, 0);

                payload_data = new byte[len];
                for (UInt64 i = 0; i < len; i++)
                {
                    payload_data[i] = recBytes[i + 14];
                }
            }
            else
            {
                Array.Copy(recBytes, 2, masks, 0, 4);
                payload_data = new byte[payload_len];
                Array.Copy(recBytes, 6, payload_data, 0, payload_len);

            }

            for (var i = 0; i < payload_len; i++)
            {
                payload_data[i] = (byte)(payload_data[i] ^ masks[i % 4]);
            }

            return Encoding.UTF8.GetString(payload_data);
        }

        /// <summary>
        /// 把客户端消息打包处理（拼接上谁什么时候发的什么消息）
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="message">Message.</param>
        private byte[] PackageServerData(SocketMessage sm)
        {
            StringBuilder msg = new StringBuilder();
            if (!sm.isLoginMessage)
            { //消息是login信息
                msg.AppendFormat("{0} @ {1}:\r\n    ", sm.Client.Name, sm.Time.ToShortTimeString());
                msg.Append(sm.Message);
            }
            else
            { //处理普通消息
                msg.AppendFormat("{0} login @ {1}", sm.Client.Name, sm.Time.ToShortTimeString());
            }


            byte[] content = null;
            byte[] temp = Encoding.UTF8.GetBytes(msg.ToString());

            if (temp.Length < 126)
            {
                content = new byte[temp.Length + 2];
                content[0] = 0x81;
                content[1] = (byte)temp.Length;
                Array.Copy(temp, 0, content, 2, temp.Length);
            }
            else if (temp.Length < 0xFFFF)
            {
                content = new byte[temp.Length + 4];
                content[0] = 0x81;
                content[1] = 126;
                content[2] = (byte)(temp.Length & 0xFF);
                content[3] = (byte)(temp.Length >> 8 & 0xFF);
                Array.Copy(temp, 0, content, 4, temp.Length);
            }
            else
            {
                // 暂不处理超长内容  
            }

            return content;
        }
    }
}
