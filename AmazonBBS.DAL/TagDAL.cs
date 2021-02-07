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
    /// 标签
    /// </summary>
    public class TagDAL : Auto_TagDAL
    {
        public DataSet GetTags(int type, int startIndex, int endIndex)
        {
            return new SqlQuickBuild(@"
                                    select count(1) from Tag a 
                                    left join UserBase b on b.UserID=a.CreateUser
                                    where a.IsDelete=0;

                                    select * from (
                                    select ROW_NUMBER() over(order by a.createTime desc) rid,a.*,b.UserName from Tag a 
                                    left join UserBase b on b.UserID=a.CreateUser
                                    where a.IsDelete=0 
                                    ) T where T.rid between @startIndex and @endIndex ")
                              //.AddParams("@type", SqlDbType.Int, type)
                              .AddParams("@startIndex", SqlDbType.Int, startIndex)
                              .AddParams("@endIndex", SqlDbType.Int, endIndex)
                              .Query();
        }

        public string ExistTag(string name, int pageModel)
        {
            //return new SqlQuickBuild("select count(1) from Tag where TagBelongId=@id and TagName=@name")
            return new SqlQuickBuild("select count(1) from Tag where TagName=@name")
                .AddParams("@name", SqlDbType.NVarChar, name)
                //.AddParams("@id", SqlDbType.Int, pageModel)
                .GetSingleStr();
        }

        public string CanAddTag(long userID, DateTime month)
        {
            return new SqlQuickBuild("select count(1) from Tag where TagCreateType=2 and CreateUser=@user and CreateTime between @month and GETDATE()")
                .AddParams("@user", SqlDbType.NVarChar, userID.ToString())
                .AddParams("@month", SqlDbType.DateTime, month)
                .GetSingleStr();
        }

        public DataTable MatchTags(string matchKey, string queryType)
        {
            //return new SqlQuickBuild(@"select TagName,TagId from Tag where TagBelongId=@type and TagName like @key")
            return new SqlQuickBuild(@"select TagName,TagId from Tag where TagName like @key")
                .AddParams("@key", SqlDbType.VarChar, "%{0}%".FormatWith(matchKey))
                //.AddParams("@type", SqlDbType.BigInt, queryType.ToInt64())
                .GetTable();
        }

        public DataTable GetTop3Tag(int tagType, SqlTransaction tran)
        {
            return new SqlQuickBuild(@"select top 3 (select count(1) from MenuBelongTag where TagId=a.TagId) UseCount,a.TagName,a.TagId from Tag a 
                where a.TagBelongId=@tagtype and a.IsDelete=0 order by UseCount desc,a.CreateTime ")
                .AddParams("@tagtype", SqlDbType.Int, tagType)
                .GetTable(tran);
        }

        public DataSet GetRandomTag(int newcount, int count)
        {
            return new SqlQuickBuild(@"
							select top (@top1) 
							ISNULL(b.ItemCount,0) ItemCount
							,a.* from Tag a 
							left join (
							select _b.TagId,count(*) ItemCount from MenuBelongTag _b 
							left join Question qq on qq.QuestionId=_b.MainId and _b.MainType=1
							left join Article aa on aa.ArticleId=_b.MainId and _b.MainType=2
							where qq.IsDelete=0 or aa.IsDelete=0
							group by _b.TagId
							) b on a.TagId=b.TagId
							where a.IsDelete=0 order by a.CreateTime desc;
						
                            select top (@top2)
							ISNULL(b.ItemCount,0) ItemCount
                            ,* from(
							select * from (
							select ROW_NUMBER() over(order by a.CreateTime desc) rid ,* from Tag a where a.IsDelete=0) T where t.rid>@top1) TT
							left join (
							select _b.TagId,count(*) ItemCount from MenuBelongTag _b 
							left join Question qq on qq.QuestionId=_b.MainId and _b.MainType=1
							left join Article aa on aa.ArticleId=_b.MainId and _b.MainType=2
							where qq.IsDelete=0 or aa.IsDelete=0
							group by _b.TagId
							) b on TT.TagId=b.TagId
							--order by NEWID();
                            order by b.ItemCount desc;
                            ")
                            .AddParams("@top1", SqlDbType.Int, newcount)
                            .AddParams("@top2", SqlDbType.Int, count)
                            .Query();
        }

        public DataTable GetAllTags(TagsSortTypeEnum tagsSortTypeEnum)
        {
            var sql = new SqlQuickBuild();
            string command = @"
							select 
							ISNULL(b.ItemCount,0) ItemCount
							,a.* from Tag a 
							{1} join (
							select _b.TagId,count(*) ItemCount from MenuBelongTag _b 
							left join Question qq on qq.QuestionId=_b.MainId and _b.MainType=1
							left join Article aa on aa.ArticleId=_b.MainId and _b.MainType=2
							where (qq.IsDelete=0 or aa.IsDelete=0) {0}
							group by _b.TagId
							) b on a.TagId=b.TagId
							where a.IsDelete=0 order by a.CreateTime desc;";
            string conditionJoin = " left ";
            string conditionWhere = string.Empty;
            switch (tagsSortTypeEnum)
            {
                case TagsSortTypeEnum.Sort_Hot:
                    //                    conditionJoin = @"
                    //left join MenuBelongTag b on b.TagId=a.TagId and b.MainType=1
                    //left join Question c on c.QuestionId=b.MainId";
                    //                    conditionWhere = " and c.IsRemen=1 and c.IsDelete=0 ";
                    conditionJoin = " right ";
                    conditionWhere = @" and (qq.IsRemen=1)";
                    break;
                case TagsSortTypeEnum.Sort_JingHua:
                    //                    conditionJoin = @"left join MenuBelongTag b on b.TagId=a.TagId and b.MainType=1
                    //left join Question c on c.QuestionId=b.MainId";
                    //                    conditionWhere = " and c.IsJinghua=1 and c.IsDelete=0 ";
                    conditionJoin = " right ";
                    conditionWhere = @" and (qq.IsJinghua=1)";
                    break;
                case TagsSortTypeEnum.Sort_30:
                    //                    conditionJoin = @"left join MenuBelongTag b on b.TagId=a.TagId and b.MainType=1
                    //left join Question c on c.QuestionId=b.MainId";
                    DateTime start = DateTime.Now.AddDays(-DateTime.Now.Day + 1).Date;
                    //                    conditionWhere = " and c.IsDelete=0 and c.CreateTime between @startTime and getdate()";
                    conditionJoin = " right ";
                    conditionWhere = " and (qq.CreateTime between @startTime and getdate() or aa.CreateTime between @startTime and getdate())";
                    sql.AddParams("@startTime", SqlDbType.DateTime, start);
                    break;
            }
            sql.Cmd = command.FormatWith(conditionWhere, conditionJoin);
            return sql.GetTable();
        }

        public DataTable GetTagByMainID(int mainType, long mainId)
        {
            return new SqlQuickBuild(@"select a.* from Tag a 
left join MenuBelongTag b on b.TagId=a.TagId
where b.MainId=@mainid and b.MainType=@maintype and a.IsDelete=0")
.AddParams("@mainid", SqlDbType.BigInt, mainId)
.AddParams("@maintype", SqlDbType.Int, mainType)
.GetTable();
        }
    }

}
