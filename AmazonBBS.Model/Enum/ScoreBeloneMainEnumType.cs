using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 发布文章 帖子 和评论 操作时的积分对应
    /// </summary>
    public enum ScoreBeloneMainEnumType
    {
        None = 0,

        Publish_BBS = 1,

        Comment_BBS = 2,

        Publish_Article = 3,

        Comment_Article = 4
    }
}
