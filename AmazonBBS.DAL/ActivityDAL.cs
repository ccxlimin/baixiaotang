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
    /// 活动表
    /// </summary>
    public class ActivityDAL : Auto_ActivityDAL
    {
        public DataSet GetAllActivits(int startIndex, int endIndex, string key)
        {
            StringBuilder sb = new StringBuilder();
            var sql = new SqlQuickBuild();
            if (key.IsNotNullOrEmpty())
            {
                sb.Append(@"
                            select count(1) from Activity where IsDelete=0 and IsChecked=2 and IsDelete=0 and (Title like @key or UserName like @key);

                            select * from (
                            select ROW_NUMBER() over(order by ActivityCreateTIme desc)rid,
                            --a.ActivityId,a.Title,a.ActivityIMG,a.BeginTime,a.[Address]
                            a.*
                            ,b.FeeType,b.Fee from Activity a
                            left join (select * from (select ROW_NUMBER() over(partition by activityid order by activityFeeid desc)rid,* from ActivityFee)T where T.rid=1) b on b.ActivityId=a.ActivityId
                            where a.IsDelete=0 and a.IsChecked=2 
                                and (a.Title like @key or a.UserName like @key)
                            ) T where T.rid BETWEEN  @si and @ei");
                sql.AddParams("@key", SqlDbType.VarChar, "%{0}%".FormatWith(key));
            }
            else
            {
                sb.Append(@"
                            select count(1) from Activity a
left join UserBase b on b.UserID=a.UserID
where a.IsDelete=0 and a.IsChecked=2;

                            select * from (
                            select ROW_NUMBER() over(order by ActivityCreateTIme desc)rid,
                            --a.ActivityId,a.Title,a.ActivityIMG,a.BeginTime,a.[Address]
                            a.*
                            ,b.FeeType,b.Fee from Activity a
                         left join (select * from (select ROW_NUMBER() over(partition by activityid order by activityFeeid desc)rid,* from ActivityFee)T where T.rid=1) b on b.ActivityId=a.ActivityId
                            where a.IsDelete=0 and a.IsChecked=2
                            ) T where T.rid BETWEEN  @si and @ei");
            }
            sql.Cmd = sb.ToString();
            return sql.AddParams("@si", SqlDbType.Int, startIndex)
                            .AddParams("@ei", SqlDbType.Int, endIndex)
                            .Query();
        }

        public DataSet GetDetail(long id, long userid, bool searchMyJoinItem)
        {
            //return new SqlQuickBuild(@"select 
            //                    a.*,
            //                    (select count(1) from ActivityJoin where ActivityId=a.ActivityId and IsFeed=1) JoinCount 
            //                    from Activity a
            //                    where a.ActivityId=@id and a.IsChecked=2;
            //                    select * from ActivityFee b where b.ActivityId=@id;
            //                    select ItemName,JoinItemQuestionExtId from JoinItemQuestionExt c where c.MainID=@id and c.MainType=1")
            //                   .AddParams("@id", SqlDbType.BigInt, id)
            //                   .Query();
            var sql = new SqlQuickBuild();
            StringBuilder sb = new StringBuilder(@"select 
                                a.*,
                                (select count(1) from ActivityJoin where ActivityId = a.ActivityId and IsFeed = 1) JoinCount
                                    from Activity a
                                where a.ActivityId = @id and a.IsChecked = 2;
            select * from ActivityFee b where b.ActivityId = @id;
            select ItemName, JoinItemQuestionExtId from JoinItemQuestionExt c where c.MainID = @id and c.MainType = 1;");
            if (searchMyJoinItem)
            {
                sb.Append(@"select b.*,a.JoinCount from ActivityJoin a 
                            left join ActivityFee b on b.ActivityFeeId=a.ActivityFeeId
                            where a.JoinUserID = @userid and a.ActivityId = @id and a.IsFeed=1;");
                sql.AddParams("@userid", SqlDbType.BigInt, userid);
            }
            sql.AddParams("@id", SqlDbType.BigInt, id);
            sql.Cmd = sb.ToString();
            return sql.Query();
        }

        public DataTable GetCheckList()
        {
            return new SqlQuickBuild(@"
                                        select 
                        b.UserName,b.HeadUrl,
                        a.* from Activity a 
                        left join UserBase b on a.UserID=b.UserID 
                        where a.IsChecked=1 and a.IsDelete=0 and b.IsDelete=0")
                .GetTable();
        }

        public bool PVCount(long activityId)
        {
            return new SqlQuickBuild(@"update Activity set PVCount=PVCount+1 where ActivityId=@id")
                .AddParams("@id", SqlDbType.BigInt, activityId)
                .ExecuteSql();
        }

        public DataSet GetEditDetail(long id)
        {
            return new SqlQuickBuild(@"select * from Activity where IsDelete=0 and ActivityId=@id;
                                        select * from ActivityFee where ActivityId=@id;
                                        select * from JoinItemQuestionExt where IsDelete=0 and MainID=@id and MainType=1;")
                                        .AddParams("@id", SqlDbType.BigInt, id)
                                        .Query();
        }
    }
}

