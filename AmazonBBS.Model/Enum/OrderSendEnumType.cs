using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 订单发货状态
    /// </summary>
    public enum OrderSendEnumType
    {
        /// <summary>
        /// 未发货
        /// </summary>
        NoSend = 0,

        /// <summary>
        /// 已发货
        /// </summary>
        Sended = 1,
    }
}
