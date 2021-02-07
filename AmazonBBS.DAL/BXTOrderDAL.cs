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
    /// 订单
    /// </summary>
    public class BXTOrderDAL : Auto_BXTOrderDAL
    {
        public DataTable SearchByRows(string out_trade_no)
        {
            return new SqlQuickBuild("select * from BXTOrder where PayOrderID=@tradeno ")
                .AddParams("@tradeno", SqlDbType.VarChar, out_trade_no)
                .GetTable();
        }
    }

}

