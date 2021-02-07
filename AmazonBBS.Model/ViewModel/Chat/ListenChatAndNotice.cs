using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class ListenChatAndNotice
    {
        /// <summary>
        /// 未读私信数量
        /// </summary>
        public List<ChatViewModel> Chats { get; set; }

        /// <summary>
        /// 通知数量
        /// </summary>
        public int Notices { get; set; }
    }
}
