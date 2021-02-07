using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class _Answer : Answer
    {
        /// <summary>
        /// 是否已购买答案
        /// </summary>
        public bool IsFeeAnswer { get; set; }

        /// <summary>
        /// 是否已赞
        /// </summary>
        public bool IsPrised { get; set; }

        /// <summary>
        /// 赞同数
        /// </summary>
        public int PrisedCount { get; set; }

        /// <summary>
        /// 该评论的回复条数
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 评论者头像
        /// </summary>
        public string HeadUrl { get; set; }

        /// <summary>
        /// 用户个性签名
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 评论者名称
        /// </summary>
        public string UserName { get; set; }

        ///// <summary>
        ///// 评论者个性签名
        ///// </summary>
        //public string MySign { get; set; }

        public List<_ReplyAnswer> ReplyList { get; set; }
    }

    public class _ReplyAnswer : Answer
    {
        /// <summary>
        /// 回复给谁 的名称
        /// </summary>
        public string Reply2UserName { get; set; }

        /// <summary>
        /// 回复给谁 的ID
        /// </summary>
        public long Reply2UserID { get; set; }

        /// <summary>
        /// 是否已赞
        /// </summary>
        public bool IsPrised { get; set; }

        /// <summary>
        /// 赞同数
        /// </summary>
        public int PrisedCount { get; set; }

        /// <summary>
        /// 该回复的回复条数
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 回复者头像
        /// </summary>
        public string HeadUrl { get; set; }

        /// <summary>
        /// 用户个性签名
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 回复者名称
        /// </summary>
        public string UserName { get; set; }

        ///// <summary>
        ///// 回复者个性签名
        ///// </summary>
        //public string MySign { get; set; }
    }
}
