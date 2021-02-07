using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public enum PVTableEnum
    {
        /// <summary>
        /// 无效
        /// </summary>
        None = 0,
        /// <summary>
        /// 问题
        /// </summary>
        Question = 1,
        /// <summary>
        /// 文章
        /// </summary>
        Article = 2,
        /// <summary>
        /// 活动
        /// </summary>
        Activity = 3,
        /// <summary>
        /// 礼物/数据分析/课程
        /// </summary>
        Gift = 4,
        /// <summary>
        /// 导航
        /// </summary>
        SoftLink = 5,
        /// <summary>
        /// 招聘
        /// </summary>
        ZhaoPin = 6,
        /// <summary>
        /// 求职
        /// </summary>
        QiuZhi = 7,
        /// <summary>
        /// 产品服务
        /// </summary>
        Product = 8,
        /// <summary>
        /// 主页
        /// </summary>
        About = 10,
        /// <summary>
        /// 新闻
        /// </summary>
        News = 11,
    }
}
