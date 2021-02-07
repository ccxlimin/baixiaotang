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
    /// 活动费用表
    /// </summary>
    public class ActivityFeeDAL : Auto_ActivityFeeDAL
    {
        public bool UpdateCount(int joinCount, long activityFeeId, SqlTransaction tran)
        {
            return new SqlQuickBuild(@"update ActivityFee set FeeCount=FeeCount-@num where ActivityFeeId=@id")
                .AddParams("@num", SqlDbType.Int, joinCount)
                .AddParams("@id", SqlDbType.BigInt, activityFeeId)
                .ExecuteSql(tran);
        }

        public DataTable GetFeeList(int id)
        {
            return new SqlQuickBuild(@"select * from ActivityFee where ActivityId=@id")
                .AddParams("@id", SqlDbType.Int, id)
                .GetTable();
        }
    }

}

