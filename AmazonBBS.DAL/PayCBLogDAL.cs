using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using AmazonBBS.Common;
using AmazonBBS.Model;

namespace AmazonBBS.DAL
{
    /// <summary>
    /// 支付回调日志
    /// </summary>
    public class PayCBLogDAL : Auto_PayCBLogDAL
    {
        public DataTable Search(string out_trade_no)
        {
            return new SqlQuickBuild("select * from PayCBLog where TradeNo=@tradeno and IsPay=1")
              .AddParams("@tradeno", SqlDbType.VarChar, out_trade_no)
              .GetTable();
        }
    }
}

