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
    /// 用户扩展表
    /// </summary>
    public class UserExtDAL : Auto_UserExtDAL
    {
        public string Exist(long userid)
        {
            return new SqlQuickBuild("select count(1) from UserExt  where UserID=@userid")
                .AddParams("@userid", SqlDbType.BigInt, userid)
                .GetSingleStr();
        }

        public DataTable GetExtInfo(long userID)
        {
            return new SqlQuickBuild("select * from UserExt where Userid=@userid ")
                 .AddParams("@userid", SqlDbType.BigInt, userID)
                 .GetTable();
        }

        public bool AddScoreCoin(long userid, int num, int cointype, SqlTransaction tran)
        {
            string sql = string.Empty;
            if (cointype == 1)
            {
                sql = "update UserExt set TotalScore=TotalScore+@num where UserID=@userid;";
            }
            else if (cointype == 2)
            {
                sql = "update UserExt set TotalCoin=TotalCoin+@num where UserID=@userid;";
            }
            return new SqlQuickBuild(sql)
                .AddParams("@userid", SqlDbType.BigInt, userid)
                .AddParams("@num", SqlDbType.Int, num)
                .ExecuteSql(tran);
        }

        public bool SubScore(long userid, int num, int type, SqlTransaction tran)
        {
            string sql = string.Empty;
            //积分
            if (type == 1)
            {
                sql = "update UserExt set TotalScore=TotalScore-@num where UserID=@userid";
            }
            else
            {
                //金钱
                sql = "update UserExt set TotalCoin=TotalCoin-@num where UserID=@userid";
            }
            return new SqlQuickBuild(sql)
                .AddParams("@num", SqlDbType.Int, num)
                .AddParams("@userid", SqlDbType.BigInt, userid)
                .ExecuteSql(tran);
        }

        public bool ReSetCloseAccount(long userID)
        {
            return new SqlQuickBuild(@"update userext set CloseTime=null where UserID=@uid;
                                       update UserBase set IsDelete=0 where UserID=@uid;")
                .AddParams("@uid", SqlDbType.BigInt, userID)
                .ExecuteSql();
        }

        public DataSet GetFaRenRenZheng(int startIndex, int endIndex)
        {
            return new SqlQuickBuild(@"
select count(1) from (
select a.*,b.UserName from UserExt a
left join UserBase b on a.UserID=b.UserID
where b.IsDelete=0 and (a.UserV=0 or a.UserV=1 or a.UserV=2 or a.UserV=3) and a.RealName is not null
) T where 1=1;
select TT.* from (
(select ROW_NUMBER() over(order by T.UserID) rid,T.* from(
select a.*,b.UserName from UserExt a
left join UserBase b on a.UserID=b.UserID
where b.IsDelete=0 and (a.UserV=0 or a.UserV=1 or a.UserV=2 or a.UserV=3) and a.RealName is not null
)T )) TT where TT.rid between @s and @e
")
.AddParams("@s", SqlDbType.Int, startIndex)
.AddParams("@e", SqlDbType.Int, endIndex)
.Query();
        }

        public bool UpdateSkin(long userID, string skin)
        {
            return new SqlQuickBuild("update UserExt set UserCenterSkin=@skin where UserID=@uid")
                .AddParams("@uid", SqlDbType.BigInt, userID)
                .AddParams("@skin", SqlDbType.NVarChar, skin)
                .ExecuteSql();
        }

        public DataTable FindNearCloseAccount()
        {
            return new SqlQuickBuild(@"select a.UserID,a.CloseTime from UserExt a
                                left join UserBase b on b.IsDelete=0 and b.UserID=a.UserID
                                where a.CloseTime = (select max(CloseTime) from UserExt)")
                                .GetTable();
        }

        public bool UpdateLevelName(long userid, int levelName, bool only)
        {
            string sql = string.Empty;
            if (only)
            {
                sql = "update UserExt set OnlyLevelName=@level where UserID=@userid";
            }
            else
            {
                sql = "update UserExt set LevelName=@level where UserID=@userid";
            }
            return new SqlQuickBuild(sql).AddParams("@userid", SqlDbType.BigInt, userid)
                .AddParams("@level", SqlDbType.Int, levelName)
                .ExecuteSql();
        }

        public DataTable GetLevelNameForUser(long userid)
        {
            return new SqlQuickBuild(@"select 
                                    ISNULL((select top 1 * from (select e.SortIndex - (select count(1) from ScoreCoinLog where CoinSource=3 and UserID=10006 and IsDelete=0) needCount from BBSEnum e where e.EnumType=3)T where T.needCount>0 order by T.needCount asc),0) needSignCount,
                                    c.EnumDesc no,
                                    d.EnumDesc,
                                    a.HeadNameShowType o,
                                    c.Url levelNameUrls
                                    from UserExt a
                                    left join BBSEnum c on a.LevelName=c.BBSEnumId and c.EnumType=3 
                                    left join BBSEnum d on a.OnlyLevelName=d.BBSEnumId and d.EnumType=4  
                                    where UserID=@userid")
                .AddParams("@userid", SqlDbType.BigInt, userid)
                .GetTable();
        }

        public bool CloseAccount(long userid, DateTime closetime)
        {
            return new SqlQuickBuild(@"update UserExt set closetime = @closetime where UserID=@uid;
                                       update UserBase set IsDelete=3 where UserID=@uid")
                .AddParams("@closetime", SqlDbType.DateTime, closetime)
                .AddParams("@uid", SqlDbType.BigInt, userid)
                .ExecuteSql();
        }

        public DataTable GetHotUsers(int count)
        {
            return new SqlQuickBuild(@"select top (@count) b.UserName,b.UserID,b.HeadUrl,a.UserV from UserExt a 
                                        left join UserBase b on b.UserID = a.UserID
                                        where b.IsDelete=0
                                        order by a.TotalScore desc")
                                        .AddParams("@count", SqlDbType.Int, count)
                                        .GetTable();
        }

        public DataTable GetNewUsers(int days)
        {
            return new SqlQuickBuild(@"select b.UserName,b.UserID,b.HeadUrl,a.UserV from UserExt a 
                                        left join UserBase b on b.UserID = a.UserID
                                        where DATEDIFF(day, b.CreateTime , GETDATE())<=@days and b.IsDelete=0
                                        order by a.TotalScore desc")
                                        .AddParams("@days", SqlDbType.Int, days)
                                        .GetTable();
        }

        public bool CheckBBS(long id, int type)
        {
            return new SqlQuickBuild("update UserExt set CheckBBS=@check where UserID=@uid")
                                        .AddParams("@check", SqlDbType.Int, type)
                                        .AddParams("@uid", SqlDbType.BigInt, id)
                                        .ExecuteSql();
        }

        public DataTable GetOldUsers(int days, int count)
        {
            return new SqlQuickBuild(@"select * from (
select ROW_NUMBER() over(order by a.TotalScore desc) rid, b.UserName,b.UserID,b.HeadUrl,a.UserV,
(select count(1) from ScoreCoinLog where UserID=a.UserID and CoinSource=3) SignCount,--签到次数 
(select count(1) from Comment where MainType=1 and CommentUserID=a.UserID and IsDelete=0) CommentCount --回答问题次数
from UserExt a 
left join UserBase b on b.UserID = a.UserID
where DATEDIFF(day, b.CreateTime , GETDATE())>=@days and b.IsDelete=0)T 
where T.rid >@count")
                                        .AddParams("@days", SqlDbType.Int, days)
                                        .AddParams("@count", SqlDbType.Int, count)
                                        .GetTable();
        }

        public DataTable GetALLSignAndLevel(SqlTransaction tran)
        {
            return new SqlQuickBuild(@"
                                select UserID,LevelName,
                                (select count(1) from ScoreCoinLog where UserID=a.UserID and CoinSource=3) SignCount 
                                from userext a ").GetTable(tran);
        }

        public DataTable GetSignAndLevelByUserID(SqlTransaction tran, long userID)
        {
            return new SqlQuickBuild(@"select UserID,LevelName,
                                (select count(1) from ScoreCoinLog where UserID=a.UserID and CoinSource=3) SignCount 
                                from userext a where a.UserID=@uid")
                                .AddParams("@uid", SqlDbType.BigInt, userID)
                                .GetTable(tran);
        }

        public bool RejectUserAuth(long userId)
        {
            return new SqlQuickBuild(@"update UserExt set RealName=null,CardID=null,CompanyName=null,CompanyTel=null,CardPic=null,FaRenPic=null where UserID=@uid")
                .AddParams("@uid", SqlDbType.BigInt, userId)
                .ExecuteSql();
        }
    }

}

