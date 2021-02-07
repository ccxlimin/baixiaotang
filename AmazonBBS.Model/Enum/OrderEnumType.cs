using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 订单类型
    /// </summary>
    public enum OrderEnumType
    {
        /// <summary>
        /// 充值
        /// </summary>
        TopUp = 1,
        /// <summary>
        /// 礼物
        /// </summary>
        Gift = 2,
        /// <summary>
        /// 活动
        /// </summary>
        Party = 3,
        /// <summary>
        /// 数据
        /// </summary>
        Data = 4,
        /// <summary>
        /// 招聘
        /// </summary>
        ZhaoPin = 5,
        /// <summary>
        /// 求职
        /// </summary>
        QiuZhi = 6,
        /// <summary>
        /// 课程
        /// </summary>
        KeCheng = 8
    }
}
