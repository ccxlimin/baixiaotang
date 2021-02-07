using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public enum ReportEnumType
    {
        BBS = 1,
        Article = 2,

        /// <summary>
        /// 帖子的评论
        /// </summary>
        BBSComment = 3,

        /// <summary>
        /// 文章的评论
        /// </summary>
        ArticleComment = 4,
    }
}
