using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket
{
    interface ISocket
    {
        /// <summary>
        /// 启动服务器，监听客户端请求
        /// </summary>
        /// <param name="port">服务器端进程口号</param>
        void Run(int port);

        /// <summary>
        /// 在独立线程中不停地向所有客户端广播消息
        /// </summary>
        void Broadcast();

        /// <summary>
        /// 把客户端消息打包处理（拼接上谁什么时候发的什么消息）
        /// </summary>
        /// <returns>The message.</returns>
        /// <param name="sm">Sm.</param>
        byte[] PackageMessage(SocketMessage sm);

        /// <summary>
        /// 处理客户端连接请求,成功后把客户端加入到clientPool
        /// </summary>
        /// <param name="result">Result.</param>
        void Accept(IAsyncResult result);

        /// <summary>
        /// 处理客户端发送的消息，接收成功后加入到msgPool，等待广播
        /// </summary>
        /// <param name="result">Result.</param>
        void Recieve(IAsyncResult result);
    }
}
