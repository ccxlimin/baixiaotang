using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.BLL
{
    public interface IOrderService
    {
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="userid">当前购买者</param>
        /// <param name="fee">费用</param>
        /// <param name="mainId">主键</param>
        /// <param name="orderType">订单类型</param>
        /// <param name="orerDesc">订单描述</param>
        /// <param name="seq">订单编号开头序列</param>
        /// <param name="count">购买数量</param>
        /// <param name="now">时间</param>
        /// <param name="tran"></param>
        /// <param name="result">订单内部ID</param>
        /// <param name="payorderid">订单编号</param>
        /// <returns></returns>
        string CreateOrder(long userid, decimal fee, long mainId, int orderType, string orerDesc, string seq, int count, DateTime now, SqlTransaction tran, out int result, out string payorderid);
    }
}
