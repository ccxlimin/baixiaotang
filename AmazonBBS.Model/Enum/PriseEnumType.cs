using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 点赞类型
    /// </summary>
    public enum PriseEnumType
    {
        /// <summary>
        /// 贴吧
        /// </summary>
        BBS = 1,
        /// <summary>
        /// 贴吧的评论或回复
        /// </summary>
        BBSComment = 2,
        /// <summary>
        /// 文章
        /// </summary>
        Article = 3,
        /// <summary>
        /// 文章的评论或回复
        /// </summary>
        ArticleComment = 4,
        /// <summary>
        /// 活动
        /// </summary>
        Party = 5,
        /// <summary>
        /// 活动的评论或回复
        /// </summary>
        PartyComment = 6,
        /// <summary>
        /// 礼物
        /// </summary>
        Gift = 7,
        /// <summary>
        /// 礼物的评论或回复
        /// </summary>
        GiftComment = 8,
        /// <summary>
        /// 招聘
        /// </summary>
        ZhaoPin = 9,
        /// <summary>
        /// 招聘的评论或回复
        /// </summary>
        ZhaoPinComment = 10,
        /// <summary>
        /// 求职
        /// </summary>
        QiuZhi = 11,
        /// <summary>
        /// 求职的评论或回复
        /// </summary>
        QiuZhiComment = 12,
        /// <summary>
        /// 产品服务
        /// </summary>
        Product = 13,
        /// <summary>
        /// 产品服务的评论或回复
        /// </summary>
        ProductComment = 14,
        /// <summary>
        /// 数据分析
        /// </summary>
        DataAnalysis = 15,
        /// <summary>
        /// 数据分析的评论或回复
        /// </summary>
        DataComment = 16,
        /// <summary>
        /// 课程
        /// </summary>
        KeCheng = 17,
        /// <summary>
        /// 课程的评论或回复
        /// </summary>
        KeChengCommen = 18,
    }
}
