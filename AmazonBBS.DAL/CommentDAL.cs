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
    /// 评论表
    /// </summary>
    public class CommentDAL : Auto_CommentDAL
    {
        public long FindUserIDByCommentID(long id, SqlTransaction tran)
        {
            return Convert.ToInt64(new SqlQuickBuild(@"select CommentUserID from Comment where CommentId=@cid and IsDelete=0")
              .AddParams("@cid", SqlDbType.BigInt, id)
              .GetSingleStr(tran));
        }

        /// <summary>
        /// 根据评论对象类型获取所有评论
        /// </summary>
        /// <param name="mainID">指定该评论的对象ID </param>
        /// <param name="userID">当前登录用户ID</param>
        /// <param name="mainType">评论对象类型(1贴吧 2文章 3活动 4礼物 5招聘 6求职 7产品服务 8数据分析 9课程)</param>
        /// <param name="sbiMainType"></param>
        /// <param name="priseType">点赞类型</param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public DataSet GetCommentsByType(long mainID, long userID, int mainType, int sbiMainType, int priseType, int startIndex, int endIndex)
        {
            return new SqlQuickBuild(@"
            --获取评论数
            select count(1) from Comment where MainID=@mid and MainType=@mainType and CommentOrReplyType=1 and IsDelete=0;
            
            --分页获取具体评论情况
            select * from (
            select ROW_NUMBER() over(order by a.CreateTime) rid,
            (select count(1) from Comment _a where _a.ReplyTopCommentId=a.CommentId and _a.MainType=@mainType) ReplyCount,
            (select count(1) from Prise _p where _p.TargetID=a.CommentId and _p.Type=@prisetype and _p.IsDelete=0) PrisedCount,
            b.UserName,
            isnull(b.HeadUrl,'/Content/img/head_default.gif') HeadUrl,
            --b.Sign,
			ISNULL(d.FeeAnswerLogId,0) IsFeeAnswer,
            ISNULL(c.PriseId,0) IsPrised
            ,a.*,
            e.VIP,e.VIPExpiryTime,
            --e.TotalScore,
            --e.TotalCoin,
            
             --(select count(1) from ScoreCoinLog where CoinSource=@coinsource and UserID=a.CommentUserID) SignCount,
             --(select count(1) from Question ques where ques.UserID=a.CommentUserID and ques.IsDelete=0) User_BBS_Count,
             --(select count(1) from Article art where art.UserID=a.CommentUserID and art.IsDelete=0) User_Article_Count,
             --(select count(1) from UserLike userlike where userlike.IsDelete=0 and userlike.LikeType=@liketype and userlike.LikeTargetID=a.CommentUserID) User_Fans_Count,
             (case when @sbiMainType>0 then ( case when a.CommentUserID = @userid then (select count(1) from ScoreBeloneItem sbi where sbi.MainId=@mid and sbi.MainType=@sbiMainType and sbi.UserID=@userid and DATEDIFF(day,sbi.CreateTime,getdate())<8) else 0 end) else 0 end) ShowCoinTips,

             (select count(1) from prise where TargetID = a.CommentId and [Type]=@prisetype and isdelete=0) PriseCount,
             (select count(1) from against where TargetID = a.CommentId and [Type]=@prisetype and isdelete=0) AgainstCount,
             (select count(1) from Against _a where _a.TargetID = a.CommentId and _a.type=@prisetype and _a.IsDelete=0 and _a.UserID=@userid) IsAgainst,

            --(select EnumDesc from BBSEnum where e.LevelName=BBSEnumId and EnumType=3) LevelName,
            f.EnumDesc LevelName ,
            f.Url LevelNameUrls,
            --(select EnumDesc from BBSEnum where e.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,
            g.EnumDesc OnlyLevelName ,
            e.HeadNameShowType 

            from Comment a
            left join UserBase b on a.CommentUserID=b.UserID
            left join Prise c on c.TargetID=a.CommentId and c.Type=@prisetype and c.IsDelete=0 and c.UserID=@userid
            left join FeeAnswerLog d on a.CommentId = d.AnswerId and d.UserID=@userid
            left join UserExt e on e.UserID=b.UserID

            left join BBSEnum f on e.LevelName = f.BBSENumId and f.EnumType = @LevelName 
            left join BBSEnum g on e.OnlyLevelName = g.BBSENumId and g.EnumType = @OnlyLevelName 

            where a.MainID=@mid and a.MainType=@mainType and a.CommentOrReplyType=1 and a.IsDelete=0
            ) T where T.rid BETWEEN @startindex and @endindex;")
             .AddParams("@mid", SqlDbType.BigInt, mainID)
                .AddParams("@mainType", SqlDbType.Int, mainType)
                .AddParams("@prisetype", SqlDbType.Int, priseType)
                .AddParams("@userid", SqlDbType.BigInt, userID)
                .AddParams("@startindex", SqlDbType.Int, startIndex)
                .AddParams("@endindex", SqlDbType.Int, endIndex)
                .AddParams("@coinsource", SqlDbType.Int, CoinSourceEnum.Sign.GetHashCode())
                .AddParams("@liketype", SqlDbType.Int, LikeTypeEnum.Like_User.GetHashCode())
                .AddParams("@sbiMainType", SqlDbType.Int, sbiMainType)
                .AddParams("@LevelName", SqlDbType.Int, BBSEnumType.LevelName.GetHashCode())
                .AddParams("@OnlyLevelName", SqlDbType.Int, BBSEnumType.OnlyLevelName.GetHashCode())
            .Query();
        }

        public DataTable GetReplyList(long replyTopCommentID, long userid, int mainType, int priseType)
        {
            return new SqlQuickBuild(@"
                                select * from (
                                select ROW_NUMBER() over(order by a.CreateTime) rid,
                                (select count(1) from Comment _a where _a.ReplyToUserID=a.CommentUserID and _a.ReplyToCommentID=a.CommentId and _a.ReplyTopCommentId=a.ReplyTopCommentId and _a.MainType=@mainType) ReplyCount,
                                (select count(1) from Prise _p where _p.TargetID=a.CommentId and _p.Type=@prisetype and _p.IsDelete=0 ) PrisedCount,
                                isnull(b.HeadUrl,'/Content/img/head_default.gif') HeadUrl,
                                b.UserName,
                                c.UserName Reply2UserName,c.UserID Reply2UserID,
                                ISNULL(d.PriseId,0) IsPrised,
                                a.*,
                                e.VIP,e.VIPExpiryTime,
                                --e.TotalScore,
                                --e.TotalCoin,
                                (select EnumDesc from BBSEnum where e.LevelName=BBSEnumId and EnumType=3) LevelName,
                                (select EnumDesc from BBSEnum where e.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,

             --(select count(1) from ScoreCoinLog where CoinSource=@coinsource and UserID=a.CommentUserID) SignCount,
             --(select count(1) from Question ques where ques.UserID=a.CommentUserID and ques.IsDelete=0) User_BBS_Count,
             --(select count(1) from Article art where art.UserID=a.CommentUserID and art.IsDelete=0) User_Article_Count,
             --(select count(1) from UserLike userlike where userlike.IsDelete=0 and userlike.LikeType=@liketype and userlike.LikeTargetID=a.CommentUserID) User_Fans_Count,

             (select count(1) from prise where TargetID = a.CommentId and [Type]=@prisetype and isdelete=0) PriseCount,
             (select count(1) from against where TargetID = a.CommentId and [Type]=@prisetype and isdelete=0) AgainstCount,
             (select count(1) from Against _a where _a.TargetID = a.CommentId and _a.type=@prisetype and _a.IsDelete=0 and _a.UserID=@userid) IsAgainst,

                                e.HeadNameShowType 
                                from Comment a
                                left join UserBase b on a.CommentUserID=b.UserID
                                left join UserBase c on a.ReplyTouserid=c.UserID
                                left join Prise d on d.UserID=@userid and d.TargetID=a.CommentId and d.Type=@prisetype and d.IsDelete=0 
                                left join UserExt e on e.UserID=b.UserID
                                where a.CommentOrReplyType=2 and a.MainType=@mainType and a.ReplyTopCommentId =@replyTopCommentID and a.IsDelete=0
                                ) T 
                                ")
                .AddParams("@replyTopCommentID", SqlDbType.BigInt, replyTopCommentID)
                .AddParams("@mainType", SqlDbType.Int, mainType)
                .AddParams("@prisetype", SqlDbType.Int, priseType)
                .AddParams("@userid", SqlDbType.BigInt, userid)
                .AddParams("@coinsource", SqlDbType.Int, CoinSourceEnum.Sign.GetHashCode())
                .AddParams("@liketype", SqlDbType.Int, LikeTypeEnum.Like_User.GetHashCode())
                .GetTable();
        }

        public string GetAuthorID(long id, int mainType)
        {
            return new SqlQuickBuild(@"select CommentuserID from Comment where CommentId=@id and MainType=@mainType and IsDelete=0;")
                .AddParams("@id", SqlDbType.BigInt, id)
                .AddParams("@mainType", SqlDbType.Int, mainType)
                .GetSingleStr();
        }

        public DataTable GetCommentInfoAndMainTitleByMainID(long id, int mainType, string sql)
        {
            return new SqlQuickBuild(sql)
                .AddParams("@id", SqlDbType.BigInt, id)
                .AddParams("@mainType", SqlDbType.Int, mainType)
                .GetTable();
        }

        public long FindUserIDByAnswerID(long id)
        {
            return Convert.ToInt64(new SqlQuickBuild(@"select CommentUserId from Comment where CommentId=@aid and IsDelete=0")
                .AddParams("@aid", SqlDbType.BigInt, id)
                .GetSingleStr());
        }

        public DataSet GetCommentListByUserid(long userID, int mainType, int startIndex, int endIndex, long currentLoginUserID, bool currentLoginUserIsMaster)
        {
            bool withoutAnonymous = false;//查询时是否排除 匿名的
            bool isNotMe = userID != currentLoginUserID;
            string filed = string.Empty;
            string conditionJoin = string.Empty;
            if (isNotMe)
            {
                withoutAnonymous = currentLoginUserIsMaster ? false : true;
                filed = " ISNULL(feeanswer.FeeAnswerLogId,0) IsFeeAnswer ,";
                conditionJoin = " left join FeeAnswerLog as feeanswer on feeanswer.AnswerId=a.CommentId and feeanswer.UserID=@currentUid";
            }
            string anonymous = withoutAnonymous ? " and a.IsAnonymous=0 " : string.Empty;
            var sqlquick = new SqlQuickBuild(@"
                                select count(1) from Comment a 
                                left join Question b on b.QuestionId=a.MainId
                                where a.CommentUserId=@userid and a.MainType=@mainType and a.IsDelete=0 and b.IsDelete=0;

                                select * from (
                                select row_Number() over(order by a.CreateTime desc) rid,a.CommentContent,a.CreateTime,b.Title QuestionTitle,
                                b.QuestionId,a.IsHideOrFeeToSee,
                                {0}
                                (select count(1) from Comment _a where _a.ReplyTopCommentId=a.CommentId and _a.MainType=@mainType) ReplyCount,--回答人数
                                (select count(1) from Prise _p where _p.TargetID=a.CommentId and _p.[TYpe]=2) PrisedCount--点赞人数
                                from Comment a
                                left join Question b on a.MainId=b.QuestionId
                                {1}
                                where a.CommentUserId=@userid and a.IsDelete=0 and a.MainType=@mainType and b.IsDelete=0 {2}
                                ) T where T.rid BETWEEN @startindex and @endindex;
                                select HeadUrl as CommentHeadUrl from UserBase where UserID=@userid and IsDelete=0;"
                                .FormatWith(filed, conditionJoin, anonymous)
                                );
            sqlquick.AddParams("@userid", SqlDbType.BigInt, userID)
                                .AddParams("@mainType", SqlDbType.Int, mainType)
                                .AddParams("@startindex", SqlDbType.Int, startIndex)
                                .AddParams("@endindex", SqlDbType.Int, endIndex);
            if (isNotMe)
            {
                sqlquick.AddParams("@currentUid", SqlDbType.BigInt, currentLoginUserID);
            }
            return sqlquick.Query();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="priseType"></param>
        /// <param name="bestornice">1最佳解答 2最优解答</param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public DataSet GetBestAnswersByUserID(long userID, int priseType, int bestornice, int startIndex, int endIndex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select count(1) from Comment a
                        left join Question b on b.{0} = a.CommentId
                        where a.CommentUserId=@userid and a.IsDelete=0 and b.IsDelete=0;");
            sb.Append(@" select 
                         T.*,
                         (select count(1) from Comment _c where _c.ReplyTopCommentId=T.CommentId and _c.IsDelete=0) ReplyCount, --回复人数
                         (select count(1) from Prise _p where _p.TargetID=T.CommentId and _p.[Type]=@priseType and _p.IsDelete=0) PrisedCount--点赞人数
                         from (
                         select ROW_NUMBER() over(order by c.CreateTime desc)rid,
                         qt.QuestionId,qt.Title QuestionTitle,c.CommentContent,c.CreateTime,c.CommentId
                         from Comment c 
                         left join Question qt on qt.{0}=c.CommentId
                         where c.CommentUserId=@userid and c.IsDelete=0 and qt.IsDelete=0 
                         ) T where T.rid BETWEEN @startindex and @endindex;

                         select HeadUrl as CommentHeadUrl from UserBase where UserID=@userid and IsDelete=0;");

            return new SqlQuickBuild(sb.ToString().FormatWith(bestornice == 1 ? "BestAnswerId" : "NiceAnswerId"))
                                .AddParams("@userid", SqlDbType.BigInt, userID)
                                .AddParams("@priseType", SqlDbType.Int, priseType)
                                .AddParams("@startindex", SqlDbType.Int, startIndex)
                                .AddParams("@endindex", SqlDbType.Int, endIndex)
                                .Query();
        }

        public bool DeleteComment(int mainType, long id)
        {
            return new SqlQuickBuild("update Comment set IsDelete=1 where MainType=@mainType and CommentId=@id")
                             .AddParams("@mainType", SqlDbType.Int, mainType)
                             .AddParams("@id", SqlDbType.BigInt, id)
             .ExecuteSql();
        }

        public bool EditComment(int mainType, long id, string content)
        {
            return new SqlQuickBuild("update Comment set CommentContent=@content where MainType=@mainType and CommentId=@id")
                                .AddParams("@content", SqlDbType.Text, content)
                                .AddParams("@mainType", SqlDbType.Int, mainType)
                                .AddParams("@id", SqlDbType.BigInt, id)
                .ExecuteSql();
        }
    }

}

