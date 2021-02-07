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
    /// 礼物费用表
    /// </summary>
    public class GiftFeeDAL : Auto_GiftFeeDAL
    {
        public bool UpdateCount(int buycount, long giftfeeid, SqlTransaction tran)
        {
            return new SqlQuickBuild(@"update GiftFee set FeeCount=FeeCount-@num where GiftFeeId=@id")
               .AddParams("@num", SqlDbType.Int, buycount)
               .AddParams("@id", SqlDbType.BigInt, giftfeeid)
               .ExecuteSql(tran);
        }
    }

}
