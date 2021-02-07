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
    /// 文章表
    /// </summary>
    public class ArticleDAL : Auto_ArticleDAL
    {
        public DataTable GetArticleDetail(long articleid, long userid)
        {
            return new SqlQuickBuild(@"select 
                    c.UserName,
                    (select ISNULL(PriseId,0) IsPrised from Prise _p where _p.UserID=@userid and _p.TargetID=@articleid and _p.Type=3 and _p.IsDelete=0) IsPrised,
                    (select count(1) from UserLike ul where ul.LikeTargetID=a.ArticleId and ul.LikeType=2 and ul.IsDelete=0)   LikeCount,
                    (select cul.UserLikeId from UserLike cul where cul.LikeTargetID=a.ArticleId and cul.LikeType=2 and cul.IsDelete=0 and cul.UserID=@userid) IsLiked,
                    a.*,
                    d.VIP,d.VIPExpiryTime
                    ,d.TotalScore,
                    d.TotalCoin,
                    --(select EnumDesc from BBSEnum where d.LevelName=BBSEnumId and EnumType=3) LevelName,
                    e.EnumDesc LevelName ,
                    e.Url LevelNameUrls,
                    --(select EnumDesc from BBSEnum where d.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,
                    f.EnumDesc OnlyLevelName ,

             (select count(1) from ContentBuyLog cbl where cbl.BuyerId=@userid and cbl.MainType=2 and cbl.MainID=a.ArticleId) IsBuyContent,
             (select count(1) from ContentBuyLog cbl1 where cbl1.MainType=2 and cbl1.MainID=a.ArticleId) ContentBuyCount,

             (select count(1) from ScoreCoinLog where CoinSource=@coinsource and UserID=a.UserID) SignCount,
             (select count(1) from Question ques where ques.UserID=a.UserID and ques.IsDelete=0) User_BBS_Count,
             (select count(1) from Article art where art.UserID=a.UserID and art.IsDelete=0) User_Article_Count,
             (select count(1) from UserLike userlike where userlike.IsDelete=0 and userlike.LikeType=@liketype and userlike.LikeTargetID=a.UserID) User_Fans_Count,
             (case when a.UserID = @userid then (select count(1) from ScoreBeloneItem sbi where sbi.MainId=@articleid and sbi.MainType=@sbiMainType and sbi.UserID=@userid and DATEDIFF(day,sbi.CreateTime,getdate())<8) else 0 end) ShowCoinTips,

             (select count(1) from prise where TargetID = @articleid and [Type]=@priseType and isdelete=0) PriseCount,
             (select count(1) from against where TargetID = @articleid and [Type]=@priseType and isdelete=0) AgainstCount,
             (select count(1) from Against _a where _a.type=@priseType and _a.IsDelete=0 and _a.UserID=@userid and _a.TargetID=@articleid) IsAgainst,

                    c.HeadUrl,
                    d.HeadNameShowType 
                    from Article a
                    left join UserLike b on a.ArticleId=b.LikeTargetID and b.IsDelete=0
                    left join UserBase c on a.UserID=c.UserID
                    left join UserExt d on d.UserID=c.UserID
                    left join BBSEnum e on d.LevelName = e.BBSENumId and e.EnumType = @LevelName 
                    left join BBSEnum f on d.OnlyLevelName = f.BBSENumId and f.EnumType = @OnlyLevelName 
                    
                    where a.ArticleId=@articleid and a.IsDelete=0 and a.IsChecked=2")
                    .AddParams("@articleid", SqlDbType.BigInt, articleid)
                    .AddParams("@userid", SqlDbType.BigInt, userid)
                    .AddParams("@LevelName", SqlDbType.Int, BBSEnumType.LevelName.GetHashCode())
                    .AddParams("@OnlyLevelName", SqlDbType.Int, BBSEnumType.OnlyLevelName.GetHashCode())
                    .AddParams("@coinsource", SqlDbType.Int, CoinSourceEnum.Sign.GetHashCode())
                    .AddParams("@liketype", SqlDbType.Int, LikeTypeEnum.Like_User.GetHashCode())
                    .AddParams("@sbiMainType", SqlDbType.Int, ScoreBeloneMainEnumType.Publish_Article.GetHashCode())
                    .AddParams("@priseType", SqlDbType.Int, PriseEnumType.Article.GetHashCode())
                    .GetTable();
        }

        public DataSet GetAllArticleList(string order, int startIndex, int endIndex)
        {
            return new SqlQuickBuild(@"
                            --计算总量
                            select count(1) from (
                            select a.ArticleId from Article a
                               left join UserBase b on a.UserID=b.UserID and b.IsDelete=0
                               where a.IsDelete=0 and a.IsChecked=2 and b.IsDelete=0
                               ) T where 1=1;
                            
                           select * from (
                           select ROW_NUMBER() over(order by a.CreateTime {0}) rid,a.*,
                            b.UserName,b.HeadUrl,
                            (select count(1) from UserLike ul where ul.LikeTargetId=a.ArticleId and ul.LikeType=2 and ul.IsDelete=0) LikeCount,
                            (select count(1) from Comment aa where aa.MainID=a.ArticleId and MainType=2 and CommentOrReplyType=1 and aa.IsDelete=0) CommentCount,
                            (select count(*) from Prise where [Type]=3 and TargetID=a.ArticleId) PriseCount,
                            (select max(CreateTime) from Comment where mainid=a.ArticleId and MainType=2) LastCommentTime,
                            (select count(1) from [Master] _master where _master.UserID=a.UserID and _master.IsDelete=0) IsMaster,--是否管理员
                            c.VIP,c.VIPExpiryTime,c.TotalScore,
                            --(select EnumDesc from BBSEnum where c.LevelName=BBSEnumId and EnumType=3) LevelName,
                            d.EnumDesc LevelName ,
                            d.Url LevelNameUrls,
                            --(select EnumDesc from BBSEnum where c.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,
                            e.EnumDesc OnlyLevelName ,

                            c.HeadNameShowType,c.UserV 
                            from Article a
                           left join UserBase b on a.UserID=b.UserID and b.IsDelete=0
                           left join UserExt c on c.UserID=b.UserID
                            left join BBSEnum d on c.LevelName = d.BBSENumId and d.EnumType = @LevelName 
                            left join BBSEnum e on c.OnlyLevelName = e.BBSENumId and e.EnumType = @OnlyLevelName 
                           where a.IsDelete=0 and a.IsChecked=2 and b.IsDelete=0
                               ) T where T.rid between @startIndex and @endIndex".FormatWith(order))
                              .AddParams("@startIndex", SqlDbType.Int, startIndex)
                              .AddParams("@endIndex", SqlDbType.Int, endIndex)
                            .AddParams("@LevelName", SqlDbType.Int, BBSEnumType.LevelName.GetHashCode())
                            .AddParams("@OnlyLevelName", SqlDbType.Int, BBSEnumType.OnlyLevelName.GetHashCode())
                              .Query();
        }

        public DataSet GetAllArticles(int startIndex, int endIndex, string keyWord = null, long tagId = 0)
        {
            //在提高客户满意度上面，我们需要注意什么
            //在团队工作方面，哪些原则是我们需要着重去考虑的
            //在软件质量方面，哪些点需要考虑
            //项目管理方面，哪些点需要考虑

            StringBuilder sb = new StringBuilder();
            var sql = new SqlQuickBuild();
            if (keyWord.IsNotNullOrEmpty() || tagId > 0)
            {
                string baseSql = @"select count(1) from Article a
left join UserBase b on b.UserID=a.UserID
{0}{1};
                            select * from (select 
                            ROW_NUMBER() over(order by a.CreateTime desc) rid,
                            b.UserName,b.HeadUrl,
                            ( select count(1) from UserLike ul where ul.LikeTargetID=a.ArticleId and ul.LikeType=2 and ul.IsDelete=0) LikeCount,
                            (select count(1) from Comment aa where aa.MainID=a.ArticleId and aa.MainType=2 and aa.CommentOrReplyType=1 and aa.IsDelete=0) CommentCount,
                            (select count(*) from Prise where [Type]=3 and TargetID=a.ArticleId) PriseCount,
                            (select max(CreateTime) from Comment where mainid=a.ArticleId and MainType=2) LastCommentTime,
                            a.*,
                            c.VIP,c.VIPExpiryTime,c.TotalScore,
                            (select EnumDesc from BBSEnum where c.LevelName=BBSEnumId and EnumType=3) LevelName,
                            (select EnumDesc from BBSEnum where c.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,
                            c.HeadNameShowType,c.UserV  
                            from Article a
                            left join UserBase b on a.UserID=b.UserID
                            left join UserExt c on c.UserID=b.UserID
                            {0}{1}
                            )T where T.rid BETWEEN @startindex and @endindex ;";
                if (tagId > 0)
                {
                    sb.Append(baseSql.FormatWith(@" 
                                left join MenuBelongTag mbt on mbt.MainId=a.ArticleId and mbt.MainType=2
                                left join Tag tag on tag.TagId=mbt.TagId",
                                " where tag.TagId=@tagid and b.IsDelete=0 and a.IsDelete=0 "));
                    sql.AddParams("@tagid", SqlDbType.BigInt, tagId);
                }
                else if (keyWord.IsNotNullOrEmpty())
                {
                    //搜关键词
                    sb.Append(baseSql.FormatWith(string.Empty, " where a.IsDelete=0 and a.IsChecked=2 and b.IsDelete=0 and (a.Title like @key or a.Body like @key or b.UserName like @key) "));
                    sql.AddParams("@key", SqlDbType.VarChar, "%{0}%".FormatWith(keyWord));
                }
            }
            else
            {
                sb.Append(@"
                            select count(1) from Article where IsDelete=0 and IsChecked=2;
                            select * from (select 
                            ROW_NUMBER() over(order by a.CreateTime desc) rid,
                            b.UserName,b.HeadUrl,
                            ( select count(1) from UserLike ul where ul.LikeTargetID=a.ArticleId and ul.LikeType=2 and ul.IsDelete=0) LikeCount,
                            (select count(1) from Comment aa where aa.MainID=a.ArticleId and aa.MainType=2 and aa.CommentOrReplyType=1 and aa.IsDelete=0) CommentCount,
                            (select count(*) from Prise where [Type]=3 and TargetID=a.ArticleId) PriseCount,
                            (select max(CreateTime) from Comment where mainid=a.ArticleId and MainType=2) LastCommentTime,
                            a.*,
                            c.VIP,c.VIPExpiryTime,c.TotalScore,
                            (select EnumDesc from BBSEnum where c.LevelName=BBSEnumId and EnumType=3) LevelName,
                            (select EnumDesc from BBSEnum where c.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,
                            c.HeadNameShowType,c.UserV  
                            from Article a
                            left join UserBase b on a.UserID=b.UserID
                            left join UserExt c on c.UserID=b.UserID
                            where a.IsDelete=0 and a.IsChecked=2 and b.IsDelete=0 
                            )T where T.rid BETWEEN @startindex and @endindex ;");
            }
            sql.Cmd = sb.ToString();
            return sql.AddParams("@startindex", SqlDbType.Int, startIndex)
                            .AddParams("@endindex", SqlDbType.Int, endIndex)
                            .Query();
        }

        public DataTable GetCheckList()
        {
            return new SqlQuickBuild(@"
                                        select 
                        b.UserName,b.HeadUrl,
                        a.* from Article a 
                        left join UserBase b on a.UserID=b.UserID 
                        where a.IsChecked=1 and a.IsDelete=0 and b.IsDelete=0;").GetTable();
        }

        public bool UpdatePV(long articleId)
        {
            return new SqlQuickBuild("update  Article set PVCount=ISNULL(PVCount,0)+1 where ArticleId=@id")
                .AddParams("@id", SqlDbType.BigInt, articleId)
                .ExecuteSql();
        }

        public DataSet GetArticleListByUserid(long userID, int startIndex, int endIndex, bool currentLoginUserIsMaster)
        {
            string anonymous = currentLoginUserIsMaster ? string.Empty : " and a.IsAnonymous=0 ";
            return new SqlQuickBuild(@"
                                    select count(1) from Article where UserID=@userid and IsDelete=0;
                                    select * from ( select row_number() over(order by a.CreateTime desc) rid,
                                    (select count(1) from UserLike b where b.LikeType=2 and b.LikeTargetID=a.ArticleId and   
                                        b.IsDelete=0) LikeCount,
                                    a.* from Article a 
                                    where a.UserID=@userid and a.IsDelete=0 and a.IsChecked=2 {0}
                                    ) T where T.rid between @startindex and @endindex;
                                    select HeadUrl as ArticleHeadUrl from UserBase where UserID=@userid and IsDelete=0;".FormatWith(anonymous))
                                    .AddParams("@userid", SqlDbType.BigInt, userID)
                                    .AddParams("@startindex", SqlDbType.Int, startIndex)
                                    .AddParams("@endindex", SqlDbType.Int, endIndex)
                                    .Query();
        }
    }

}

