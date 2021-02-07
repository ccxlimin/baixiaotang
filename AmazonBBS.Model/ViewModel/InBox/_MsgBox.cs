using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class _MsgBox : Chat
    {
        public string HeadUrl { get; set; }
        /// <summary>
        /// 发信人姓名
        /// </summary>
        public string FromUserName
        {
            get;
            set;
        }

        /// <summary>
        /// 收信人姓名
        /// </summary>
        public string ToUserName
        {
            get;
            set;
        }
    }
}
