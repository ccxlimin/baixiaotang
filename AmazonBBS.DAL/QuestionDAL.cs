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
        public DataSet GetQuestionList(int id, int startIndex, int endIndex, string keyWords = null, long tagId = 0)
        {
            StringBuilder sb = new StringBuilder();
            var sql = new SqlQuickBuild();
            if (keyWords.IsNotNullOrEmpty() || tagId > 0)
            {
                string baseSql = @"
                                    --计算总量
                                    select count(1) from Question a 
                                    left join UserBase b on b.UserID=a.UserID 
                                    {0}
                                    {1};

                            select * from (
                             select ROW_NUMBER() over(order by a.createtime desc) rid,
                             b.UserName,
                            isnull(b.HeadUrl,'/Content/img/head_default.gif') HeadUrl,
                             (select count(1) from UserLike ul where ul.LikeTargetId=a.QuestionId and ul.LikeType=1 and ul.IsDelete=0) LikeCount,
                            (select count(1) from Comment aa where aa.MainID=a.QuestionId and aa.MainType=1 and aa.CommentOrReplyType=1 and aa.IsDelete=0) CommentCount,
                             (select count(*) from Prise where [Type]=1 and TargetID=a.QuestionId) PriseCount,
                            (select max(CreateTime) from Comment where mainid=a.questionid and MainType=1) LastCommentTime,
                             (select count(1) from [Master] _master where _master.UserID=a.UserID and _master.IsDelete=0) IsMaster,--是否管理员
                             a.*,
                             c.VIP,c.VIPExpiryTime,c.TotalScore,
                             (select EnumDesc from BBSEnum where c.LevelName=BBSEnumId and EnumType=3) LevelName,
                             (select EnumDesc from BBSEnum where c.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,
                             c.HeadNameShowType,c.UserV  
                             from Question a 
                             left join UserBase b on a.UserID=b.UserID 
                             left join UserExt c on c.UserID=b.UserID
                             {0}
                             {1}
                             ) T where T.rid BETWEEN @startindex and @endindex";

                if (tagId > 0)
                {
                    //搜标签
                    sb.Append(baseSql.FormatWith(@"
                                left join MenuBelongTag mbt on mbt.MainId=a.QuestionId and mbt.MainType=1
                                left join Tag tag on tag.TagId=mbt.TagId",
                                " where tag.TagId=@tagid and b.IsDelete=0 and a.IsDelete=0 "));
                    sql.AddParams("@tagid", SqlDbType.BigInt, tagId);
                }
                else if (keyWords.IsNotNullOrEmpty())
                {
                    //搜关键词
                    sb.Append(baseSql.FormatWith(string.Empty, " where a.TopicID=@id and a.IsDelete=0 and a.IsChecked=2 and b.IsDelete=0 and (a.Title like @key or b.UserName like @key)"));
                    sql.AddParams("@id", SqlDbType.Int, id);
                    sql.AddParams("@key", SqlDbType.VarChar, "%{0}%".FormatWith(keyWords));
                }
            }
            else
            {
                sb.Append(@"
                        --选择置顶贴
                        select a.*,b.UserName,
                        isnull(b.HeadUrl,'/Content/img/head_default.gif') HeadUrl,
                        (select count(1) from UserLike ul where ul.LikeTargetId=a.QuestionId and ul.LikeType=1 and ul.IsDelete=0) LikeCount,
                        (select count(1) from Comment aa where aa.MainID=a.QuestionId and aa.MainType=1 and aa.CommentOrReplyType=1 and aa.IsDelete=0) CommentCount,
                        c.VIP,c.VIPExpiryTime,c.TotalScore,
                        (select count(*) from Prise where [Type]=1 and TargetID=a.QuestionId) PriseCount,
                        (select max(CreateTime) from Comment where mainid=a.questionid and MainType=1) LastCommentTime,
                        (select count(1) from [Master] _master where _master.UserID=a.UserID and _master.IsDelete=0) IsMaster,--是否管理员
                        (select EnumDesc from BBSEnum where c.LevelName=BBSEnumId and EnumType=3) LevelName,
                        (select EnumDesc from BBSEnum where c.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,
                        c.HeadNameShowType,c.UserV  
                        from Question a
                        left join UserBase b on a.UserID=b.UserID
                        left join UserExt c on c.UserID=b.UserID
                        where a.TopicID=@id and a.IsTop=1 and a.IsDelete=0 and b.IsDelete=0 ;

                                    --计算总量
                                    select count(1) from Question where TopicID=@id and IsDelete=0 and IsChecked=2;

                                    select * from (
                                        select ROW_NUMBER() over(order by a.createtime desc) rid,
                        b.UserName,
                        isnull(b.HeadUrl,'/Content/img/head_default.gif') HeadUrl,
                        (select count(1) from UserLike ul where ul.LikeTargetId=a.QuestionId and ul.LikeType=1 and ul.IsDelete=0) LikeCount,
                        (select count(1) from Comment aa where aa.MainID=a.QuestionId and aa.MainType=1 and aa.CommentOrReplyType=1 and aa.IsDelete=0) CommentCount,
                        (select count(*) from Prise where [Type]=1 and TargetID=a.QuestionId) PriseCount,
                        (select max(CreateTime) from Comment where mainid=a.questionid and MainType=1) LastCommentTime,
                        (select count(1) from [Master] _master where _master.UserID=a.UserID and _master.IsDelete=0) IsMaster,--是否管理员
                        a.*,
                        c.VIP,c.VIPExpiryTime,c.TotalScore,
                        --(select EnumDesc from BBSEnum where c.LevelName=BBSEnumId and EnumType=3) LevelName,
                        d.EnumDesc LevelName ,
                        d.Url LevelNameUrls,
                        --(select EnumDesc from BBSEnum where c.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,
                        e.EnumDesc OnlyLevelName ,
                        c.HeadNameShowType,c.UserV 
                        from Question a 
                        left join UserBase b on a.UserID=b.UserID 
                        left join UserExt c on c.UserID=b.UserID
                        left join BBSEnum d on c.LevelName = d.BBSENumId and d.EnumType = @LevelName 
                        left join BBSEnum e on c.OnlyLevelName = e.BBSENumId and e.EnumType = @OnlyLevelName 
                        where a.TopicID=@id and a.IsTop=0 and a.IsDelete=0 and a.IsChecked=2 and b.IsDelete=0) T where T.rid BETWEEN @startindex and @endindex");
                sql.AddParams("@id", SqlDbType.BigInt, id);
            }
            sql.Cmd = sb.ToString();
            return sql.AddParams("@startindex", SqlDbType.Int, startIndex)
                    .AddParams("@endindex", SqlDbType.Int, endIndex)
                .AddParams("@LevelName", SqlDbType.Int, BBSEnumType.LevelName.GetHashCode())
                .AddParams("@OnlyLevelName", SqlDbType.Int, BBSEnumType.OnlyLevelName.GetHashCode())
                    .Query();
        }

        public DataSet GetListBySortType(string orderBy, string sortCondition, int totalCount, int startIndex, int endIndex)
        {
            return new SqlQuickBuild(@"
                            --计算总量
                            select count(1) from (
                            select top (@totalCount) a.QuestionId from Question a
                               left join UserBase b on a.UserID=b.UserID and b.IsDelete=0
                               where a.IsDelete=0 and a.IsChecked=2 and b.IsDelete=0
                               {0}
                               ) T where 1=1;
                            
                            select * from (
                               select top 100 ROW_NUMBER() over(order by {1}) rid,a.*,
                                b.UserName,
                                isnull(b.HeadUrl,'/Content/img/head_default.gif') HeadUrl,
                                (select count(1) from UserLike ul where ul.LikeTargetId=a.QuestionId and ul.LikeType=1 and ul.IsDelete=0) LikeCount,
                                (select count(1) from Comment aa where aa.MainID=a.QuestionId and MainType=1 and CommentOrReplyType=1 and aa.IsDelete=0) CommentCount,
                                (select count(*) from Prise where [Type]=1 and TargetID=a.QuestionId) PriseCount,
                                (select max(CreateTime) from Comment where mainid=a.questionid and MainType=1) LastCommentTime,
                                (select count(1) from [Master] _master where _master.UserID=a.UserID and _master.IsDelete=0) IsMaster,--是否管理员
                                c.VIP,c.VIPExpiryTime,c.TotalScore,
                                --(select EnumDesc from BBSEnum where c.LevelName=BBSEnumId and EnumType=3) LevelName,
                                d.EnumDesc LevelName ,
                                d.Url LevelNameUrls,
                                --(select EnumDesc from BBSEnum where c.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,
                                e.EnumDesc OnlyLevelName ,
                                c.HeadNameShowType,c.UserV 
                                from Question a
                               left join UserBase b on a.UserID=b.UserID and b.IsDelete=0
                               left join UserExt c on c.UserID=b.UserID
                            left join BBSEnum d on c.LevelName = d.BBSENumId and d.EnumType = @LevelName 
                            left join BBSEnum e on c.OnlyLevelName = e.BBSENumId and e.EnumType = @OnlyLevelName 
                               where a.IsDelete=0 and a.IsChecked=2 and b.IsDelete=0
                               {0}
                               ) T where T.rid between @startIndex and @endIndex
                            ".FormatWith(sortCondition, orderBy))
                            .AddParams("@totalCount", SqlDbType.Int, totalCount)
                            //.AddParams("@sortType", SqlDbType.Text, sortCondition)
                            .AddParams("@startIndex", SqlDbType.Int, startIndex)
                            .AddParams("@endIndex", SqlDbType.Int, endIndex)
                            .AddParams("@LevelName", SqlDbType.Int, BBSEnumType.LevelName.GetHashCode())
                            .AddParams("@OnlyLevelName", SqlDbType.Int, BBSEnumType.OnlyLevelName.GetHashCode())
                            .Query();
        }

        public DataSet GetAllBBSList(string orderBy, int startIndex, int endIndex)
        {
            return new SqlQuickBuild(@"
                            --计算总量
                            select count(1) from (
                            select a.QuestionId from Question a
                               left join UserBase b on a.UserID=b.UserID and b.IsDelete=0
                               where a.IsDelete=0 and a.IsChecked=2 and b.IsDelete=0
                               ) T where 1=1;
                            
                            select * from (
                               select ROW_NUMBER() over(order by a.CreateTime {0}) rid,a.*,
                                b.UserName,
                                isnull(b.HeadUrl,'/Content/img/head_default.gif') HeadUrl,
                                (select count(1) from UserLike ul where ul.LikeTargetId=a.QuestionId and ul.LikeType=1 and ul.IsDelete=0) LikeCount,
                                (select count(1) from Comment aa where aa.MainID=a.QuestionId and MainType=1 and CommentOrReplyType=1 and aa.IsDelete=0) CommentCount,
                                (select count(*) from Prise where [Type]=1 and TargetID=a.QuestionId) PriseCount,
                                (select max(CreateTime) from Comment where mainid=a.questionid and MainType=1) LastCommentTime,
                                (select count(1) from [Master] _master where _master.UserID=a.UserID and _master.IsDelete=0) IsMaster,--是否管理员
                                c.VIP,c.VIPExpiryTime,c.TotalScore,
                                --(select EnumDesc from BBSEnum where c.LevelName=BBSEnumId and EnumType=3) LevelName,
                                d.EnumDesc LevelName ,
                                d.Url LevelNameUrls,
                                --(select EnumDesc from BBSEnum where c.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,
                                e.EnumDesc OnlyLevelName ,
                                c.HeadNameShowType,c.UserV 
                                from Question a
                               left join UserBase b on a.UserID=b.UserID and b.IsDelete=0
                               left join UserExt c on c.UserID=b.UserID
                                left join BBSEnum d on c.LevelName = d.BBSENumId and d.EnumType = @LevelName 
                                left join BBSEnum e on c.OnlyLevelName = e.BBSENumId and e.EnumType = @OnlyLevelName 
                               where a.IsDelete=0 and a.IsChecked=2 and b.IsDelete=0
                               ) T where T.rid between @startIndex and @endIndex
                            ".FormatWith(orderBy))
                            .AddParams("@startIndex", SqlDbType.Int, startIndex)
                            .AddParams("@endIndex", SqlDbType.Int, endIndex)
                            .AddParams("@LevelName", SqlDbType.Int, BBSEnumType.LevelName.GetHashCode())
                            .AddParams("@OnlyLevelName", SqlDbType.Int, BBSEnumType.OnlyLevelName.GetHashCode())
                            .Query();
        }

        public DataTable GetNiceComment(long questionId, long? bestanswerid)
        {
            return new SqlQuickBuild(@"select top 1 
                (select count(1) from prise where [type]=2 and TargetID=CommentId ) PrisedCount,
                (select UserName from UserBase where UserID=CommentUserID)UserName,
                CommentId,CommentUserID
                from Comment where maintype=1 and mainid=@qid and CommentId!=@best and isdelete=0 and IsAnonymous=0 
                order by PrisedCount desc,createtime")
                .AddParams("@qid", SqlDbType.BigInt, questionId)
                .AddParams("@best", SqlDbType.BigInt, bestanswerid)
                .GetTable();
        }

        public DataTable GetALLQuestion()
        {
            return new SqlQuickBuild(@"select QuestionId,NiceAnswerId,BestAnswerId,Coin,CoinType,
(select count(1) from Comment aa where aa.MainID=a.QuestionId and aa.IsDelete=0) CommentCount 
from Question a where a.IsDelete=0;")
                .GetTable();
        }

        public DataTable GetListForIndex(int top)
        {
            return new SqlQuickBuild(@"
                                    select top (@top) * from Question a
                                    left join UserBase b on a.UserID=b.UserID
                                    where a.IsDelete=0 and a.IsChecked=2 
                                    order by a.IsTop desc, a.IsJinghua desc,a.PVCount desc")
                .AddParams("@top", SqlDbType.Int, top)
                .GetTable();
        }

        public DataTable GetCheckList()
        {
            return new SqlQuickBuild(@"
                                        select 
                        b.UserName,
                        isnull(b.HeadUrl,'/Content/img/head_default.gif') HeadUrl,
                        (select count(1) from UserLike ul where ul.LikeTargetId=a.QuestionId and ul.LikeType=1 and ul.IsDelete=0) LikeCount,
                        (select count(1) from Comment aa where aa.MainID=a.QuestionId and aa.IsDelete=0) CommentCount,
                        a.* from Question a 
                        left join UserBase b on a.UserID=b.UserID 
                        where a.TopicID in (select BBSEnumID from BBSEnum where EnumType=1) and a.IsTop=0 and a.IsDelete=0 and IsChecked=1")
               .GetTable();
        }

        public DataTable GetTop5QuesForIndex(int top)
        {
            return new SqlQuickBuild(@"
                        select 
                        (select count(1) from Comment aa where aa.MainID=tq.QuestionId and aa.IsDelete=0) CommentCount,
                        u.UserName,tq.*
                        from (
                        select top (@count)
                        --(select count(1) from Comment a where a.MainID=q.QuestionId and a.CommentOrReplyType=1 and a.IsDelete=0) Commentcount   
                        (select count(1) from Comment a where a.MainID=q.QuestionId and a.IsDelete=0) Commentcount
                        ,q.*
                         from Question q where IsChecked=2 order by Commentcount desc,q.CreateTime desc
                         ) tq
                         left join userbase u on u.userid=tq.UserID
                        ")
                        .AddParams("@count", SqlDbType.Int, top)
                        .GetTable();
        }

        public bool AddAttachMent(List<AttachMent> attachMents, SqlTransaction tran)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"insert into AttachMent values");
            for (int i = 0, count = attachMents.Count; i < count; i++)
            {
                var attach = attachMents[i];
                sb.Append("(");
                sb.Append("\'" + attach.AttachMentId + "\',");
                sb.Append("\'" + attach.MainId + "\',");
                sb.Append("\'" + attach.MainType + "\',");
                sb.Append("\'" + attach.FileName + "\',");
                sb.Append("\'" + attach.FilePath + "\',");
                sb.Append("\'" + attach.FileSize + "\',");
                sb.Append("\'" + attach.DownCount + "\',");
                sb.Append("\'" + attach.IsFee + "\',");
                sb.Append("\'" + attach.Fee + "\',");
                sb.Append("\'" + attach.FeeType + "\',");
                sb.Append("\'" + attach.CreateTime + "\',");
                sb.Append("\'" + attach.CreateUser + "\',");
                sb.Append("\'" + attach.IsDelete + "\'");
                sb.Append(")");
                if (i != count - 1)
                {
                    sb.Append(",");
                }
            }
            return new SqlQuickBuild(sb.ToString()).ExecuteSql(tran);
        }

        public DataSet GetQuestionListByUserid(long userID, int startIndex, int endIndex, bool currentLoginUserIsMaster)
        {
            string anonymous = currentLoginUserIsMaster ? string.Empty : " and a.IsAnonymous=0 ";

            return new SqlQuickBuild(@"
                                    select count(1) from Question where UserID=@userid and IsDelete=0;
                                    select * from (
                                        select ROW_NUMBER() over(order by a.createtime desc) rid,
                                    (select count(1) from UserLike ul where ul.LikeTargetId=a.QuestionId and ul.LikeType=1 and ul.IsDelete=0) LikeCount,
                                    (select count(1) from Comment aa where aa.MainID=a.QuestionId and aa.MainType=1 and aa.IsDelete=0) CommentCount,
                        a.* from Question a 
                        where a.UserID=@userid and a.IsDelete=0 and a.IsChecked=2 {0} ) T where T.rid BETWEEN @startindex and @endindex;
                                    select HeadUrl as QuestionHeadUrl from UserBase where UserID=@userid;".FormatWith(anonymous))
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

        /// <summary>
        /// 获取贴吧问题基本详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public DataTable GetQuestionDetail(int id, long userid)
        {
            return new SqlQuickBuild(@"
            select b.UserName,
            (select isnull(PriseId,0) from Prise _p where _p.type=@priseType and _p.IsDelete=0 and _p.UserID=@userid and _p.TargetID=@qid) IsPrised,
             (select count(1) from Comment _comment where _comment.MainID = a.QuestionId and _comment.MainType=1 and _comment.CommentOrReplyType=1 and _comment.IsDelete=0) CommentCount,
             --(select count(1) from Comment _comment where _comment.MainID = a.QuestionId and _comment.MainType=1 and _comment.IsDelete=0) CommentCount,--含回复
             (select count(1) from UserLike ul where ul.LikeTargetId=a.QuestionId and ul.LikeType=1 and ul.UserID=@userid and ul.IsDelete=0) IsLiked,
             (select count(1) from UserLike ull where ull.LikeTargetId=a.QuestionId and ull.LikeType=1 and ull.IsDelete=0)LikeCount,

             (select count(1) from ContentBuyLog cbl where cbl.BuyerId=@userid and cbl.MainType=1 and cbl.MainID=a.QuestionId) IsBuyContent,
             (select count(1) from ContentBuyLog cbl1 where cbl1.MainType=1 and cbl1.MainID=a.QuestionId) ContentBuyCount,
             c.VIP,
             c.VIPExpiryTime,
             c.TotalScore,
             c.TotalCoin,
             --(select EnumDesc from BBSEnum where c.LevelName=BBSEnumId and EnumType=@LevelName) LevelName,
             d.EnumDesc LevelName ,
             d.Url LevelNameUrls,
             --(select EnumDesc from BBSEnum where c.OnlyLevelName=BBSEnumId and EnumType=@OnlyLevelName) OnlyLevelName,
             e.EnumDesc OnlyLevelName ,

             (select count(1) from ScoreCoinLog where CoinSource=@coinsource and UserID=a.UserID) SignCount,
             (select count(1) from Question ques where ques.UserID=a.UserID and ques.IsDelete=0) User_BBS_Count,
             (select count(1) from Article art where art.UserID=a.UserID and art.IsDelete=0) User_Article_Count,
             (select count(1) from UserLike userlike where userlike.IsDelete=0 and userlike.LikeType=@liketype and userlike.LikeTargetID=a.UserID) User_Fans_Count,
             (case when a.UserID = @userid then (select count(1) from ScoreBeloneItem sbi where sbi.MainId=@qid and sbi.MainType=@sbiMainType and sbi.UserID=@userid and DATEDIFF(day,sbi.CreateTime,getdate())<8) else 0 end) ShowCoinTips,

             (select count(1) from prise where TargetID = @qid and [Type]=@priseType and isdelete=0) PriseCount,
             (select count(1) from against where TargetID = @qid and [Type]=@priseType and isdelete=0) AgainstCount,
             (select count(1) from Against _a where _a.type=@priseType and _a.IsDelete=0 and _a.UserID=@userid and _a.TargetID=@qid) IsAgainst,

             isnull(b.HeadUrl,'/Content/img/head_default.gif') HeadUrl,
             c.HeadNameShowType, 
             a.* from Question a
            left join UserBase b on a.UserID = b.UserID
            left join UserExt c on c.UserID=b.UserID
            left join BBSEnum d on c.LevelName = d.BBSENumId and d.EnumType = @LevelName 
            left join BBSEnum e on c.OnlyLevelName = e.BBSENumId and e.EnumType = @OnlyLevelName 
              where a.QuestionId = @qid and b.IsDelete = 0;
            ")
                .AddParams("@qid", SqlDbType.BigInt, id)
                .AddParams("@userid", SqlDbType.BigInt, userid)
                .AddParams("@LevelName", SqlDbType.Int, BBSEnumType.LevelName.GetHashCode())
                .AddParams("@OnlyLevelName", SqlDbType.Int, BBSEnumType.OnlyLevelName.GetHashCode())
                .AddParams("@coinsource", SqlDbType.Int, CoinSourceEnum.Sign.GetHashCode())
                .AddParams("@liketype", SqlDbType.Int, LikeTypeEnum.Like_User.GetHashCode())
                .AddParams("@sbiMainType", SqlDbType.Int, ScoreBeloneMainEnumType.Publish_BBS.GetHashCode())
                .AddParams("@priseType", SqlDbType.Int, PriseEnumType.BBS.GetHashCode())
                .GetTable();
        }

        public DataSet SearchQuestion(int startIndex, int endIndex, string keywords)
        {
            StringBuilder sb = new StringBuilder();
            var sql = new SqlQuickBuild();
            sb.Append(@" --计算总量
                                    select count(1) from Question a 
                                    left join UserBase b on b.UserID=a.UserID
                                    where a.IsDelete=0 and a.IsChecked=2 and (a.Title like @key or a.Body like @key or b.UserName like @key);
                                    select * from (
                                        select ROW_NUMBER() over(order by a.createtime desc) rid,
                                b.UserName,
                                isnull(b.HeadUrl,'/Content/img/head_default.gif') HeadUrl,
                                (select count(1) from UserLike ul where ul.LikeTargetId=a.QuestionId and ul.LikeType=1 and ul.IsDelete=0) LikeCount,
                                (select count(1) from Comment aa where aa.MainID=a.QuestionId and aa.IsDelete=0) CommentCount,
                                (select count(1) from [Master] _master where _master.UserID=a.UserID and _master.IsDelete=0) IsMaster,--是否管理员
                                a.*,
                                c.VIP,c.VIPExpiryTime,c.TotalScore,
                                (select EnumDesc from BBSEnum where c.LevelName=BBSEnumId and EnumType=3) LevelName,
                                (select EnumDesc from BBSEnum where c.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,
                                c.HeadNameShowType,c.UserV  
                                from Question a 
                                left join UserBase b on a.UserID=b.UserID 
                                left join UserExt c on c.UserID=b.UserID
                        where a.IsDelete=0 and a.IsChecked=2 and (a.Title like @key or a.Body like @key or b.UserName like @key) and b.IsDelete=0) T where T.rid BETWEEN @startindex and @endindex");
            sql.AddParams("@key", SqlDbType.VarChar, "%{0}%".FormatWith(keywords));
            sql.Cmd = sb.ToString();
            return
                    sql.AddParams("@startindex", SqlDbType.Int, startIndex)
                    .AddParams("@endindex", SqlDbType.Int, endIndex)
                    .Query();
        }

        public string HasBestAnswer(long id, long qid)
        {
            return new SqlQuickBuild(@"select count(1) from Question where QuestionId=@qid and BestAnswerId=@id and IsDelete=0")
                .AddParams("@qid", SqlDbType.BigInt, qid)
                .AddParams("@id", SqlDbType.BigInt, id)
                .GetSingleStr();
        }

        public bool BsetAnswer(long id, long qid, SqlTransaction tran)
        {
            return new SqlQuickBuild(@"update Question set BestAnswerId=@id where QuestionId=@qid and IsDelete=0")
                    .AddParams("@qid", SqlDbType.BigInt, qid)
                .AddParams("@id", SqlDbType.BigInt, id)
                .ExecuteSql(tran);
        }
    }

}

