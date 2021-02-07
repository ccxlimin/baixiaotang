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
    /// 提问表
    /// </summary>
    public class QuestionDAL : Auto_QuestionDAL
    {
        public DataSet GetQuestionList(int id, int startIndex, int endIndex)
        {
            return new SqlQuickBuild(@"
--选择置顶贴
select a.*,b.UserName,b.HeadUrl,
(select count(1) from UserLike ul where ul.LikeTargetId=a.QuestionId and ul.LikeType=1 and ul.IsDelete=0) LikeCount,
(select count(1) from Answer aa where aa.QuestionId=a.QuestionId and aa.IsDelete=0) CommentCount 
from Question a
left join UserBase b on a.UserID=b.UserID
where a.TopicID=@id and a.IsTop=1 and a.IsDelete=0;
--计算总量
                                    select count(1) from Question where TopicID=@id and IsDelete=0 and IsChecked=2;
                                    select * from (
                                        select ROW_NUMBER() over(order by a.createtime desc) rid,
                        b.UserName,b.HeadUrl,
(select count(1) from UserLike ul where ul.LikeTargetId=a.QuestionId and ul.LikeType=1 and ul.IsDelete=0) LikeCount,
                        (select count(1) from Answer aa where aa.QuestionId=a.QuestionId and aa.IsDelete=0) CommentCount,
                        a.* from Question a 
                        left join UserBase b on a.UserID=b.UserID 
                        where a.TopicID=@id and a.IsTop=0 and a.IsDelete=0 and IsChecked=2) T where T.rid BETWEEN @startindex and @endindex")
                .AddParams("@id", SqlDbType.Int, id)
                .AddParams("@startindex", SqlDbType.Int, startIndex)
                .AddParams("@endindex", SqlDbType.Int, endIndex)
                .Query();
        }

        public DataTable GetListForIndex(int top)
        {
            return new SqlQuickBuild(@"
                                    select top (@top) * from Question a
                                    left join UserBase b on a.UserID=b.UserID
                                    where a.IsDelete=0 and a.IsChecked=2 order by a.PVCount desc")
                .AddParams("@top", SqlDbType.Int, top)
                .GetTable();
        }

        public DataSet GetCheckList(int startIndex, int endIndex)
        {
            return new SqlQuickBuild(@"
--计算总量
                                    select count(1) from Question where TopicID in (select BBSEnumID from BBSEnum where EnumType=1) and IsDelete=0 and IsChecked=1;
                                    select * from (
                                        select ROW_NUMBER() over(order by a.createtime desc) rid,
                        b.UserName,b.HeadUrl,
(select count(1) from UserLike ul where ul.LikeTargetId=a.QuestionId and ul.LikeType=1 and ul.IsDelete=0) LikeCount,
                        (select count(1) from Answer aa where aa.QuestionId=a.QuestionId and aa.IsDelete=0) CommentCount,
                        a.* from Question a 
                        left join UserBase b on a.UserID=b.UserID 
                        where a.TopicID in (select BBSEnumID from BBSEnum where EnumType=1) and a.IsTop=0 and a.IsDelete=0 and IsChecked=1) T where T.rid BETWEEN @startindex and @endindex")
               .AddParams("@startindex", SqlDbType.Int, startIndex)
               .AddParams("@endindex", SqlDbType.Int, endIndex)
               .Query();
        }

        public DataTable GetTop5QuesForIndex(int top)
        {
            return new SqlQuickBuild(@"
                        select 
                        (select count(1) from Answer aa where aa.QuestionId=tq.QuestionId and aa.IsDelete=0) CommentCount,
                        u.UserName,tq.*
                        from (
                        select top (@count)
                        --(select count(1) from Answer a where a.QuestionId=q.QuestionId and a.AType=1 and a.IsDelete=0) Commentcount   
                        (select count(1) from Answer a where a.QuestionId=q.QuestionId and a.IsDelete=0) Commentcount
                        ,q.*
                         from Question q where IsChecked=2 order by Commentcount desc,q.CreateTime desc
                         ) tq
                         left join userbase u on u.userid=tq.UserID
                        ")
                        .AddParams("@count", SqlDbType.Int, top)
                        .GetTable();
        }

        public DataSet GetQuestionListByUserid(long userID, int startIndex, int endIndex)
        {
            return new SqlQuickBuild(@"
                                    select count(1) from Question where UserID=@userid and IsDelete=0;
                                    select * from (
                                        select ROW_NUMBER() over(order by a.createtime desc) rid,
                                    (select count(1) from UserLike ul where ul.LikeTargetId=a.QuestionId and ul.LikeType=1 and ul.IsDelete=0) LikeCount,
                                    (select count(1) from Answer aa where aa.QuestionId=a.QuestionId and aa.IsDelete=0) CommentCount,
                        a.* from Question a 
                        where a.UserID=@userid and a.IsDelete=0) T where T.rid BETWEEN @startindex and @endindex;
                                    select HeadUrl as QuestionHeadUrl from UserBase where UserID=@userid;")
               .AddParams("@userid", SqlDbType.BigInt, userID)
               .AddParams("@startindex", SqlDbType.Int, startIndex)
               .AddParams("@endindex", SqlDbType.Int, endIndex)
               .Query();
        }

        public bool PVUpdate(long id)
        {
            return new SqlQuickBuild("update Question set PVCount=PVCount+1 where QuestionId=@id")
                .AddParams("@id", SqlDbType.BigInt, id)
                .ExecuteSql();
        }

        public DataSet GetQuestionDetail(int id, long userid, int startindex, int endindex)
        {
            return new SqlQuickBuild(@"
            select b.UserName,
            (select isnull(PriseId,0) from Prise _p where _p.type=1 and _p.IsDelete=0 and _p.UserID=@userid and _p.TargetID=@qid) IsPrised,
             (select count(1) from Answer _answer where _answer.QuestionId = a.QuestionId and _answer.IsDelete=0) CommentCount,
             (select count(1) from UserLike ul where ul.LikeTargetId=a.QuestionId and ul.LikeType=1 and ul.UserID=@userid and ul.IsDelete=0) IsLiked,
             (select count(1) from UserLike ull where ull.LikeTargetId=a.QuestionId and ull.LikeType=1 and ull.IsDelete=0)LikeCount,
             a.* from Question a
            left join UserBase b on a.UserID = b.UserID
              where QuestionId = @qid and b.IsDelete = 0;
            
            --获取评论数
            select count(1) from Answer where QuestionId=@qid and AType=1 and IsDelete=0;
            
            --分页获取具体评论情况
            select * from (
            select ROW_NUMBER() over(order by a.CreateTime) rid,
            (select count(1) from Answer _a where _a.ReplyTopAnswerId=a.AnswerId) ReplyCount,
            (select count(1) from Prise _p where _p.TargetID=a.AnswerId and _p.Type=2 and _p.IsDelete=0) PrisedCount,
            b.UserName,b.HeadUrl,b.Sign,
			ISNULL(d.FeeAnswerLogId,0) IsFeeAnswer,ISNULL(c.PriseId,0) IsPrised,a.* 
            from Answer a
            left join UserBase b on a.AnswerUserId=b.UserID
            left join Prise c on c.TargetID=a.AnswerId and c.Type=2 and c.IsDelete=0 and c.UserID=@userid
            left join FeeAnswerLog d on a.AnswerId = d.AnswerId and d.UserID=@userid
            where a.QuestionId=@qid and a.AType=1 and a.IsDelete=0
            ) T where T.rid BETWEEN @startindex and @endindex    
            ")
                .AddParams("@qid", SqlDbType.BigInt, id)
                .AddParams("@startindex", SqlDbType.Int, startindex)
                .AddParams("@endindex", SqlDbType.Int, endindex)
                .AddParams("@userid", SqlDbType.BigInt, userid)
                .Query();
        }

        public DataTable GetReplyList(long sr, long userid)
        {
            return new SqlQuickBuild(@"
                                select * from (
                                select ROW_NUMBER() over(order by a.CreateTime) rid,
                                (select count(1) from Answer _a where _a.ReplyToUserID=a.AnswerUserId and _a.replyToAnswerId=a.AnswerId and _a.ReplyTopAnswerId=a.ReplyTopAnswerId) ReplyCount,
                                (select count(1) from Prise _p where _p.TargetID=a.AnswerId) PrisedCount,
                                b.UserName,c.UserName Reply2UserName,c.UserID Reply2UserID,
                                ISNULL(d.PriseId,0) IsPrised,a.*
                                from Answer a
                                left join UserBase b on a.AnswerUserId=b.UserID
                                left join UserBase c on a.ReplyTouserid=c.UserID
                                left join Prise d on d.UserID=@userid and d.TargetID=a.AnswerId and d.IsDelete=0 
                                where a.AType=2 and a.ReplyTopAnswerId =@sr and a.IsDelete=0
                                ) T 
                                ")
                .AddParams("@sr", SqlDbType.BigInt, sr)
                .AddParams("@userid", SqlDbType.BigInt, userid)
                .GetTable();
        }

        public string HasBestAnswer(long id, long qid)
        {
            return new SqlQuickBuild(@"select count(1) from Question where QuestionId=@qid and BestAnswerId=@id and IsDelete=0")
                .AddParams("@qid", SqlDbType.BigInt, qid)
                .AddParams("@id", SqlDbType.BigInt, id)
                .GetSingleStr();
        }

        public bool BsetAnswer(long id, long qid)
        {
            return new SqlQuickBuild(@"update Question set BestAnswerId=@id where QuestionId=@qid and IsDelete=0")
                    .AddParams("@qid", SqlDbType.BigInt, qid)
                .AddParams("@id", SqlDbType.BigInt, id)
                .ExecuteSql();
        }
    }

}

