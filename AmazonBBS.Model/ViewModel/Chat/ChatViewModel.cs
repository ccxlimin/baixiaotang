using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class ChatViewModel
    {
        /// <summary>
        /// 未读消息数
        /// </summary>
        public int UnReadCount { get; set; }

        /// <summary>
        /// 发送人
        /// </summary>
        public long FromID { get; set; }

        /// <summary>
        /// 发送人姓名
        /// </summary>
        public string FromUserName { get; set; }

        ///// <summary>
        ///// 发送人头像
        ///// </summary>
        //public string Head { get; set; }

        ///// <summary>
        ///// 签名
        ///// </summary>
        //public string Sign { get; set; }

        ///// <summary>
        ///// 最近发送消息时间
        ///// </summary>
        //public DateTime LastSendTime { get; set; }
    }

    public class ChatBox : Chat
    {
        public string Head { get; set; }
        public string Sign { get; set; }
    }
}
