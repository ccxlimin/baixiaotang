using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public enum OrderCheckEnumType
    {
        /// <summary>
        /// 未确认收货
        /// </summary>
        NoCheck = 0,

        /// <summary>
        /// 已确认收货
        /// </summary>
        Checked = 1,

        /// <summary>
        /// 退款中
        /// </summary>
        Returning = 2,

        /// <summary>
        /// 已退款
        /// </summary>
        Returned = 3
    }
}
