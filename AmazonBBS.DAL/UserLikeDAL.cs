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
    /// 关注列表
    /// </summary>
    public class UserLikeDAL : Auto_UserLikeDAL
    {
        public string IsLiked(long id, int type, long userID)
        {
            return new SqlQuickBuild(@"select isnull(UserLikeId,0) from UserLike where UserID=@userid and LikeType=@type and LikeTargetID=@targetid and IsDelete=0;")
                .AddParams("@targetid", SqlDbType.BigInt, id)
                .AddParams("@type", SqlDbType.Int, type)
                .AddParams("@userid", SqlDbType.BigInt, userID)
                .GetSingleStr();
        }

        public bool UnLike(int likeId)
        {
            return new SqlQuickBuild(@"update UserLike set IsDelete=1 where UserLikeId=@likeid")
                .AddParams("@likeid", SqlDbType.BigInt, likeId)
                .ExecuteSql();
        }

        public string FindUserIdByID(string queryTable, string uesrColumn, int id)
        {
            string sql = @"select {2} from {0} where {0}id = {1}".FormatWith(queryTable, id, uesrColumn);
            return new SqlQuickBuild(sql)
                .GetSingleStr();
        }

        public DataTable FindLikerListByLikeType(long likeTargetID, int likeType)
        {
            return new SqlQuickBuild(@"select b.UserID,b.UserName,b.Account from userlike a
                                    left join UserBase b on a.UserID=b.UserID and b.IsDelete=0 and b.Source=2 
                                     where a.LikeTargetID=@likeTargetID and a.LikeType=@likeType and a.IsDelete=0;")
                .AddParams("@likeTargetID", SqlDbType.BigInt, likeTargetID)
                .AddParams("@likeType", SqlDbType.Int, likeType)
                .GetTable();
        }

        public DataSet GetLikesByUserID(long userID, int type, int startindex = 0, int endindex = 0)
        {
            StringBuilder sb = new StringBuilder();
            string pageSql = string.Empty;
            string pageSqlEnd = string.Empty;

            SqlQuickBuild sqlExe = new SqlQuickBuild();

            if (startindex > 0)
            {
                pageSql = "  select * from (  select ROW_NUMBER() over(order by a.LikeTime desc) rid,";
                pageSqlEnd = " ) T where T.rid BETWEEN @startindex and @endindex;";
                sqlExe.AddParams("@startindex", SqlDbType.Int, startindex)
                      .AddParams("@endindex", SqlDbType.Int, endindex);
            }
            if (type == 1)
            {
                sb.Append(@"
                select count(1) from UserLike a 
                    left join Question b on a.LikeTargetID=b.QuestionId
                    where a.UserID=@userid and a.IsDelete=0 and a.LikeType=1 and b.IsDelete=0;
                {0}
                b.QuestionId,b.Title BeLikedName,a.LikeType from UserLike a 
                left join Question  b on a.LikeTargetID=b.QuestionId
                where a.UserID=@userid and a.IsDelete=0 and a.LikeType=1 and b.IsDelete=0
                {1}");
            }
            else if (type == 2)
            {
                sb.Append(@"
                select count(1) from UserLike a
                    left join Article b on a.LikeTargetID=b.ArticleId
                    where a.UserID=@userid and a.IsDelete=0 and a.LikeType=2 and b.IsDelete=0;
                    {0}
                            b.ArticleId,b.Title BeLikedName,a.LikeType from UserLike a 
                            left join Article b on a.LikeTargetID=b.ArticleId
                            where a.UserID=@userid and a.IsDelete=0 and a.LikeType=2 and b.IsDelete=0
                     {1}");
            }
            else if (type == 3)
            {
                sb.Append(@"
                select count(1) from UserLike a
                    left join UserBase b on b.UserID=a.LikeTargetID
                    where a.UserID=@userid and a.IsDelete=0 and a.LikeType=3 and b.IsDelete=0;
               {0}
                            b.UserName BeLikedName,b.UserID BeLikedUserID,a.LikeType from UserLike a 
                            left join UserBase b on a.LikeTargetID=b.UserID
                            where a.UserID=@userid and a.IsDelete=0 and a.LikeType=3 and b.IsDelete=0
                {1}");
            }
            sqlExe.Cmd = sb.ToString().FormatWith(pageSql, pageSqlEnd);
            sqlExe.AddParams("@userid", SqlDbType.BigInt, userID);
            return sqlExe.Query();
        }

        public DataTable GetFansByUserID(long userId, int likeType)
        {
            return new SqlQuickBuild(@"
                            select 
                            b.UserName UserName,b.UserID UserID from UserLike a 
                            left join UserBase b on a.UserID=b.UserID
                            where a.LikeTargetID=@userid and a.IsDelete=0 and a.LikeType=@likeType and b.IsDelete=0
                            ")
                            .AddParams("@userid", SqlDbType.BigInt, userId)
                            .AddParams("@likeType", SqlDbType.Int, likeType)
                            .GetTable();
        }
    }

}

