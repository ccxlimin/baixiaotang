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
    /// 活动报名表
    /// </summary>
    public class ActivityJoinDAL : Auto_ActivityJoinDAL
    {
        public object JoinCount(long id)
        {
            return new SqlQuickBuild(@"select count(1) from ActivityJoin where ActivityId=@id")
                .AddParams("@id", SqlDbType.BigInt, id)
                .GetSingleStr();
        }

        public string IsJoined(long userId, long aid)
        {
            return new SqlQuickBuild(@"select count(1) from ActivityJoin where ActivityId=@aid and JoinUserID=@userId and IsFeed=1")
                .AddParams("@userId", SqlDbType.BigInt, userId)
                .AddParams("@aid", SqlDbType.BigInt, aid)
                .GetSingleStr();
        }

        public DataTable GetJoinedUsers(long id)
        {
            return new SqlQuickBuild(@"select b.UserName,b.UserID [Uid],b.HeadUrl from ActivityJoin a
                    left join UserBase b on a.JoinUserID=b.UserID where a.ActivityId=@id and a.IsFeed=1 order by JoinTime desc")
                    .AddParams("@id", SqlDbType.BigInt, id)
                    .GetTable();
        }

        public DataSet GetJoinList(long id)
        {
            return new SqlQuickBuild(@"select UserID,ActivityId,[Title] from Activity where ActivityId=@id and IsDelete=0;
               select a.*,b.FeeName,b.Fee ItemSourceFee from ActivityJoin a
               left join ActivityFee b on b.ActivityFeeId=a.ActivityFeeId
                where a.ActivityId=@id;
               select * from JoinItemQuestionExt where IsDelete=0 and MainID=@id and MainType=1;
               select * from JoinItemAnswerExt where JoinMainID=@id and JoinType=1;")
.AddParams("@id", SqlDbType.BigInt, id)
.Query();
        }

        public DataTable GetDetailJoinInfo(long userid, long itemID, SqlTransaction tran)
        {
            return new SqlQuickBuild(@"select * from ActivityJoin where ActivityId=@id and JoinUserID=@userid ")
                .AddParams("@userid", SqlDbType.BigInt, userid)
                .AddParams("@id", SqlDbType.BigInt, itemID)
                .GetTable(tran);
        }
    }

}

