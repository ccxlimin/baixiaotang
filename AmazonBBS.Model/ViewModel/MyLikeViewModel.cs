using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class MyLikeViewModel
    {
        public List<_MyLikeInfo> MyLikes { get; set; }

        public Paging MyLikePage { get; set; }
    }

    public class _MyLikeInfo
    {
        /// <summary>
        /// 关注类型
        /// </summary>
        public int LikeType { get; set; }

        /// <summary>
        /// 关注问题ID
        /// </summary>
        public long QuestionId { get; set; }

        /// <summary>
        /// 关注文章ID
        /// </summary>
        public long ArticleId { get; set; }

        /// <summary>
        /// 被关注用户ID
        /// </summary>
        public long BeLikedUserID { get; set; }

        /// <summary>
        /// 被关注的 问题标题/文章标题/用户姓名
        /// </summary>
        public string BeLikedName { get; set; }
    }
}
