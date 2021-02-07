using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class _MyComments : _Comment
    {
        ///// <summary>
        ///// 回答内容
        ///// </summary>
        //public string CommentContent { get; set; }

        ///// <summary>
        ///// 回答时间
        ///// </summary>
        //public DateTime CommentTime { get; set; }

        ///// <summary>
        ///// 回复数
        ///// </summary>
        //public int ReplyCount { get; set; }

        ///// <summary>
        ///// 赞数
        ///// </summary>
        //public int PrisedCount { get; set; }

        /// <summary>
        /// 问题标题
        /// </summary>
        public string QuestionTitle { get; set; }

        /// <summary>
        /// 问题ID
        /// </summary>
        public long QuestionId { get; set; }
    }
}
