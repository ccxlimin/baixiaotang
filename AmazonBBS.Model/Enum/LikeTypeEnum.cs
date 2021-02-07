using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 关注类型
    /// </summary>
    public enum LikeTypeEnum
    {
        None = 0,
        /// <summary>
        /// 关注问题
        /// </summary>
        Like_BBS = 1,

        /// <summary>
        /// 关注文章
        /// </summary>
        Like_Article = 2,

        /// <summary>
        /// 关注用户
        /// </summary>
        Like_User = 3,
    }
}
