using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 评论类型(1贴吧 2文章 3活动 4礼物 5招聘 6求职 7产品服务 8数据分析 9课程)
    /// </summary>
    public enum CommentEnumType
    {
        /// <summary>
        /// 不存在 （异常）
        /// </summary>
        None = 0,
        /// <summary>
        /// 贴吧
        /// </summary>
        [Description("帖子")]
        BBS = 1,
        /// <summary>
        /// 文章
        /// </summary>
        [Description("文章")]
        Article = 2,
        /// <summary>
        /// 活动
        /// </summary>
        [Description("活动")]
        Party = 3,
        /// <summary>
        /// 礼物
        /// </summary>
        [Description("礼物")]
        Gift = 4,
        /// <summary>
        /// 招聘
        /// </summary>
        [Description("招聘")]
        ZhaoPin = 5,
        /// <summary>
        /// 求职
        /// </summary>
        [Description("求职")]
        QiuZhi = 6,
        /// <summary>
        /// 产品服务
        /// </summary>
        [Description("产品")]
        Product = 7,
        /// <summary>
        /// 数据分析
        /// </summary>
        [Description("数据")]
        DataAnalysis = 8,
        /// <summary>
        /// 课程
        /// </summary>
        [Description("课程")]
        KeCheng = 9
        /*
         1、问题 2、问题的评论回复 
         3、文章 4、文章的评论回复
         5、活动 6、文章的评论回复
         7、礼物 8、文章的评论回复
         9、招聘 10、文章的评论回复
         11、求职 12、文章的评论回复
         13、产品服务 14、文章的评论回复
         15、数据分析 16、文章的评论回复
         17、课程 18、文章的评论回复
         */
    }
}
