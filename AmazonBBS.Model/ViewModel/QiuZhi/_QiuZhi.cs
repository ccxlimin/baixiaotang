using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class _QiuZhi : QiuZhi
    {
        /// <summary>
        /// 发布者头像
        /// </summary>
        public string HeadUrl { get; set; }

        /// <summary>
        /// 评论人数
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 评论列表
        /// </summary>
        public _Comments Comments { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 作者VIP
        /// </summary>
        public int VIP { get; set; }

        /// <summary>
        /// VIP有效时间
        /// </summary>
        public DateTime VIPExpiryTime { get; set; }

        /// <summary>
        /// 作者是否管理员
        /// </summary>
        public bool IsMaster { get; set; }

        /// <summary>
        /// 用户认证
        /// </summary>
        public int UserV { get; set; }

        /// <summary>
        /// 用户最后登录时间
        /// </summary>
        public DateTime LastLogonTime { get; set; }

        /// <summary>
        /// 当前登录人是否已关注此招聘
        /// </summary>
        public bool IsLiked { get; set; }
    }
}
