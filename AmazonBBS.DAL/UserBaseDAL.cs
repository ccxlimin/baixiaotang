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
    /// 用户基本表
    /// </summary>
    public class UserBaseDAL : Auto_UserBaseDAL
    {
        public string ExistAccount(string account)
        {
            return new SqlQuickBuild("select count(1) from UserBase where Account=@account;").AddParams("@account", SqlDbType.VarChar, account).GetSingleStr();
        }

        public string ExistUserName(string username)
        {
            return new SqlQuickBuild("select UserID from UserBase where UserName=@username and IsDelete=0;").AddParams("@username", SqlDbType.VarChar, username).GetSingleStr();
        }

        public DataTable GetUserDetail<T>(T id, long currentUserId)
        {
            string col = "UserID";
            SqlDbType dbtype = SqlDbType.BigInt;
            if (typeof(T).ToString() == typeof(string).ToString())
            {
                col = "UserName";
                dbtype = SqlDbType.NVarChar;
            }
            return new SqlQuickBuild(@"select 
                        (select count(1) from UserLike ul where ul.LikeTargetID=a.UserID and ul.LikeType=3 and ul.UserID=@currentUserId and ul.IsDelete=0) IsLiked,--是否关注
                        (select count(1) from UserLike ull
                            left join UserBase ub_ on ub_.UserID=ull.UserID
                            where ull.LikeTargetID=a.UserID and ull.LikeType=3 and ull.IsDelete=0 and ub_.IsDelete=0 )LikeCount,--关注人数
                        (select count(1) from Question q where q.UserID=a.UserID and q.IsDelete=0)QuestionCount,--问题数量
                        (select count(1) from Article at where at.UserID=a.UserID and at.IsDelete=0)ArticltCount,--文章数量
                        (select count(1) from Comment co 
                            left join Question qt on qt.BestAnswerId=co.CommentID
                            where qt.IsDelete=0 and co.CommentUserId=a.UserID
                            ) BsetAnswerCount,--最佳解答数量
                        (select count(1) from Comment co 
                            left join Question qt on qt.NiceAnswerId=co.CommentID
                            where qt.IsDelete=0 and co.CommentUserId=a.UserID
                            ) NiceAnswerCount,--优秀解答数量
                        b.TotalScore,b.TotalCoin,
                        b.UserV,--用户认证
                        b.HeadNameShowType,--头衔1/专属头衔2显示切换
                        c.EnumDesc LevelName,--头衔名称
                        c.Url LevelNameUrls,
                        d.EnumDesc OnlyLevelName,--专属头衔
                        ISNULL(e.MasterId,0) IsMaster,--是否管理员
                        ISNULL(e.IsRoot,0) IsRoot,--是否超级管理员
                        a.*
                        from UserBase a
                        left join UserExt b on  a.UserID=b.UserID
                        left join BBSEnum c on b.LevelName=c.BBSEnumId and c.IsDelete=0
                        left join BBSEnum d on b.OnlyLevelName=d.BBSEnumId and d.IsDelete=0
                        left join Master e on a.UserID=e.UserID and e.IsDelete=0
                        where a.{0}=@userid and a.IsDelete=0".FormatWith(col))
                .AddParams("@userid", dbtype, id)
                .AddParams("@currentUserId", SqlDbType.BigInt, currentUserId)
                .GetTable();
        }

        public bool UpdateGender(long userID, int gender)
        {
            return new SqlQuickBuild("update UserBase set Gender=@gender where UserID=@uid")
                .AddParams("@uid", SqlDbType.BigInt, userID)
                .AddParams("@gender", SqlDbType.Int, gender)
                .ExecuteSql();
        }

        public bool UpdateBirth(long userID, string birth)
        {
            return new SqlQuickBuild("update UserBase set Birth=@birth where UserID=@uid")
            .AddParams("@uid", SqlDbType.BigInt, userID)
            .AddParams("@birth", SqlDbType.VarChar, birth)
            .ExecuteSql();
        }

        public bool UpdateAreas(long userID, string province, string city, string county)
        {
            return new SqlQuickBuild("update UserBase set Province=@province,City=@city,County=@county where UserID=@uid")
            .AddParams("@uid", SqlDbType.BigInt, userID)
            .AddParams("@province", SqlDbType.NVarChar, province)
            .AddParams("@city", SqlDbType.NVarChar, city)
            .AddParams("@county", SqlDbType.NVarChar, county)
            .ExecuteSql();
        }


        public DataTable GetUserInfo(string account)
        {
            return new SqlQuickBuild("select Account,Password,UserName from UserBase where Account=@account")
                .AddParams("@account", SqlDbType.VarChar, account)
                .GetTable();
        }

        public DataTable GetUserInfo(long uid)
        {
            return new SqlQuickBuild(@"select * from UserBase where UserID=@uid and IsDelete=0")
                .AddParams("@uid", SqlDbType.BigInt, uid)
                .GetTable();
        }

        public DataTable GetUserInfoALL(long uid)
        {
            return new SqlQuickBuild(@"select * from UserBase a 
                                       left join UserExt b on b.UserID=a.UserID
                                       where a.IsDelete=0 and a.UserID=@uid")
                                       .AddParams("@uid", SqlDbType.BigInt, uid)
                                       .GetTable();
        }

        public DataSet QueryAllUserInfo(int st, int en, long me)
        {
            return new SqlQuickBuild(@"
                            --select count(1) from UserBase where IsDelete=0 and UserID != @me;
                            select count(1) from UserBase where IsDelete=0;
                            select * from (
                            --select ROW_NUMBER() over (order by createtime desc) rid,* from UserBase where IsDelete=0 and UserID!=@me
                            select ROW_NUMBER() over (order by createtime desc) rid,* from UserBase where IsDelete=0 
                            ) a 
                            left join UserExt b on a.UserID=b.UserID
                            where a.rid between @s and @e  
                            order by a.CreateTime desc;")
                            .AddParams("@s", SqlDbType.Int, st)
                            .AddParams("@e", SqlDbType.Int, en)
                            //.AddParams("@me", SqlDbType.BigInt, me)
                            .Query();
        }

        public DataTable GetUserByKey(string key)
        {
            return new SqlQuickBuild(@"
                            select * from (select * from UserBase where IsDelete=0 and UserName like @key) a 
                            left join UserExt b on a.UserID=b.UserID")
                           .AddParams("@key", SqlDbType.VarChar, "%{0}%".FormatWith(key))
                           .GetTable();
        }

        public DataTable GetUserInfo(string account, string password)
        {
            return new SqlQuickBuild(@"select * from UserBase where Account=@account and Password=@pwd; ").AddParams("@account", SqlDbType.VarChar, account
                ).AddParams("@pwd", SqlDbType.VarChar, password).GetTable();
        }

        public bool UploadHeadUrl(string url, long userID, SqlTransaction tran)
        {
            return new SqlQuickBuild(@"update UserBase set HeadUrl = @url where UserID=@userid and IsDelete=0")
                  .AddParams("@userid", SqlDbType.BigInt, userID)
                  .AddParams("@url", SqlDbType.VarChar, url)
                  .ExecuteSql(tran);
        }

        public bool UpdateSign(long userID, string sign)
        {
            return new SqlQuickBuild("update UserBase set Sign=@sign where UserID=@userid and IsDelete=0")
                .AddParams("@userid", SqlDbType.BigInt, userID)
                .AddParams("@sign", SqlDbType.VarChar, sign)
                .ExecuteSql();
        }

        public string GetUserNameByUserID(long userID)
        {
            return new SqlQuickBuild("select UserName from UserBase where UserID=@uid and IsDelete=0")
                .AddParams("@uid", SqlDbType.BigInt, userID)
                .GetSingleStr();
        }

        public bool CloseAccount(long userid)
        {
            return new SqlQuickBuild("update UserBase set IsDelete=2 where UserID=@userid")
                .AddParams("@userid", SqlDbType.BigInt, userid)
                .ExecuteSql();
        }

        public string HasTopicMaster(int bbsEnumid, long userid)
        {
            return new SqlQuickBuild("select count(1) from [Master] where (UserID=@userid and BBSMenuId=@bbsenumid) or (UserID=@userid and IsRoot=1) and IsDelete=0;")
                .AddParams("@userid", SqlDbType.BigInt, userid)
                .AddParams("@bbsenumid", SqlDbType.Int, bbsEnumid)
                .GetSingleStr();
        }

        public bool UpdateUserName(string nickname, long userID)
        {
            return new SqlQuickBuild(@"update UserBase set UserName=@uname where UserID=@uid")
                .AddParams("@uname", SqlDbType.NVarChar, nickname)
                .AddParams("@uid", SqlDbType.BigInt, userID)
                .ExecuteSql();
        }

        public DataTable FindUserInfoByOpenID(string openid, int source)
        {
            return new SqlQuickBuild("select * from UserBase where Openid=@openid and [Source]=@source")
                .AddParams("@openid", SqlDbType.VarChar, openid)
                .AddParams("@source", SqlDbType.Int, source)
                .GetTable();
        }

        public bool DeleteUser(long userId, SqlTransaction tran)
        {
            return new SqlQuickBuild("update UserBase set IsDelete=1 where UserID=@uid")
                .AddParams("@uid", SqlDbType.BigInt, userId)
                .ExecuteSql(tran);
        }

        public bool ReCoverUser(long userId, SqlTransaction tran)
        {
            return new SqlQuickBuild("update UserBase set IsDelete=0 where UserID=@uid")
                .AddParams("@uid", SqlDbType.BigInt, userId)
                .ExecuteSql(tran);
        }

        public DataTable GetSignUsers(DateTime startDt, DateTime endDt)
        {
            return new SqlQuickBuild(@"
                        select * from (
                        select 
                        --(select count(1) from ScoreCoinLog where CoinSource=3 and UserID=a.UserID) SignCount,
                        a.CoinTime SignTime, 1 SignToday,
                        b.UserID,b.UserName from ScoreCoinLog a
                        left join UserBase b on a.UserID=b.UserID
                        where a.CoinSource=3 and (a.CoinTime between @startDt and @endDt)
                        )T order by T.SignTime;")
                        .AddParams("@startDt", SqlDbType.DateTime, startDt)
                        .AddParams("@endDt", SqlDbType.DateTime, endDt)
                        .GetTable();
        }

        public DataTable GetSignUsersByMonth(DateTime startDt, DateTime endDt)
        {
            return new SqlQuickBuild(@"
                select 
                a.*,
                isnull(c.UserMonthSignContinueCount,0) MonthContinueSignCount,
                (select count(1) from ScoreCoinLog where CoinSource=3 and UserID=a.UserID) SignTotalCount,
                b.UserName from (select count(1) SignCount,
                UserID from ScoreCoinLog 
                where CoinSource=3 and (CoinTime between @startDt and @endDt) group by UserID) a
                left join UserBase b on b.UserID=a.UserID
                left join UserExt c on c.UserID=a.UserID
                order by a.SignCount desc;")
                .AddParams("@startDt", SqlDbType.DateTime, startDt)
                        .AddParams("@endDt", SqlDbType.DateTime, endDt)
                        .GetTable();
        }

        public DataTable GetCustomers()
        {
            return new SqlQuickBuild(@"
select * from Customer where IsDelete=0 order by CreateTime desc;
")
                .GetTable();
        }
    }

}

