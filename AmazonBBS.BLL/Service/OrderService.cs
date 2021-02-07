using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.BLL
{
    public class OrderService : IOrderService
    {
        private readonly AmazonBBSDBContext _amazonBBSDBContext;

        public OrderService(AmazonBBSDBContext amazonBBSDBContext)
        {
            _amazonBBSDBContext = amazonBBSDBContext;
        }

        public string CreateOrder(long userid, decimal fee, long mainId, int orderType, string orerDesc, string seq, int count, DateTime now, SqlTransaction tran, out int result, out string payorderid)
        {
            result = 0;
            string msg = string.Empty;

            var bll = BXTOrderBLL.Instance;
            payorderid = seq + mainId.ToString() + now.ToString("yyyyMMddHHmmssfff") + orderType.ToString();

            BXTOrder order = new BXTOrder()
            {
                Fee = fee * count,//计算总费用
                CreateTime = now,
                CreateUser = userid.ToString(),
                IsDelete = 0,
                IsPay = 0,
                ItemID = mainId,
                OrderType = orderType,
                OrerDesc = orerDesc,
                PayOrderID = payorderid,
                BuyCount = count,
            };
            if ((result = bll.Add(order, tran)) <= 0)
            {
                msg = "创建订单时失败，请重试！";
            }
            return msg;
        }
    }
}
