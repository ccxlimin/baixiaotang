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
    /// 礼物表
    /// </summary>
    public class GiftDAL : Auto_GiftDAL
    {
        /// <summary>
        /// 根据类型获取所有兑换数据
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="type">礼物类型(1-礼物 2-数据分析 3-百晓堂课程)</param>
        /// <param name="key">关键词搜索</param>
        /// <param name="sortConfig">排序规则</param>
        /// <returns></returns>
        public DataSet GetALLGifts(int startIndex, int endIndex, int type, string key, string sortConfig)
        {
            StringBuilder sb = new StringBuilder();
            var sql = new SqlQuickBuild();

            StringBuilder orderBy = new StringBuilder();
            string orderType = sortConfig.IsNotNullOrEmpty() && sortConfig.Contains("7") ? "asc" : "desc";
            //排序规则 
            if (sortConfig.IsNotNullOrEmpty())
            {
                var sorts = sortConfig.Split('+');
                sorts.ForEach(item =>
                {
                    switch (item)
                    {
                        //case "1": item.AppendFormat(" a.IsTop {0},", orderType); break;
                        //case "2": item.AppendFormat(" a.IsRemen {0},", orderType); break;
                        //case "3": item.AppendFormat(" a.IsJinghua {0},", orderType); break;
                        case "4": orderBy.AppendFormat(" a.PVCount {0},", orderType); break;
                        case "5": orderBy.AppendFormat(" a.GiftCreateTime {0},", orderType); break;
                    }
                });
                orderBy.Remove(orderBy.Length - 1, 1);
            }
            else
            {
                orderBy.Append(" a.GiftCreateTime {0} ".FormatWith(orderType));
            }

            if (key.IsNotNullOrEmpty())
            {
                sb.Append(@" select count(1) from Gift a
                            left join UserBase b on b.UserID=a.GiftCreateUserID
                            where a.GType=@GType and a.IsDelete=0 and b.IsDelete=0 
                                and (a.GiftName like @key or b.UserName like @key);

                            select * from (
                                select row_number() over(order by a.GiftCreateTime Desc)rid,
                                (select count(1) from UserGift ug where ug.GiftID=a.GiftID) BuyCount,
                                c.Fee,c.FeeType,
                                a.* from Gift a 
                                left join UserBase b on b.UserID=a.GiftCreateUserID
                                left join (select * from (select ROW_NUMBER() over(partition by GiftID order by GiftFeeId desc)rid,* from GiftFee)T where T.rid=1) c on c.GiftID=a.GiftID
                                where a.GType=@GType and a.IsDelete=0 and b.IsDelete=0
                                and (a.GiftName like @key or b.UserName like @key)
                                ) T where T.rid between @startindex and @endindex;");
                sql.AddParams("@key", SqlDbType.VarChar, "%{0}%".FormatWith(key));
            }
            else
            {
                sb.Append(@"
                           select count(1) from Gift where GType=@GType and IsDelete=0;
                                select * from (
                                select row_number() over(order by {0})rid,
                                (select count(1) from UserGift ug where ug.GiftID=a.GiftID) BuyCount,
                                b.Fee,b.FeeType,
                                a.* from Gift a 
                                left join (select * from (select ROW_NUMBER() over(partition by GiftID order by GiftFeeId desc)rid,* from GiftFee)T where T.rid=1) b on b.GiftID=a.GiftID
                                where a.GType=@GType and IsDelete=0) T where T.rid between @startindex and @endindex;".FormatWith(orderBy.ToString()));
            }
            sql.Cmd = sb.ToString();
            return sql.AddParams("@startindex", SqlDbType.Int, startIndex)
                                .AddParams("@endindex", SqlDbType.Int, endIndex)
                                .AddParams("@GType", SqlDbType.Int, type)
                                .Query();
        }

        public DataSet GetGiftDetail(long id, long uesrid, int joinType, bool searchMyJoinItem)
        {
            var sql = new SqlQuickBuild();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
            select (select count(1) from UserGift ug where ug.GiftID=a.GiftID) AllBuyCount,
            --,(select count(1) from UserGift ug2 where ug2.GiftID=a.GiftID and ug2.BuyUserID=@userid) CurrentUserBuyCount,
            a.* 
            from Gift a where a.GiftID=@id;
            select * from GiftFee b where b.GiftID=@id;
            select ItemName,JoinItemQuestionExtId from JoinItemQuestionExt c where c.MainID=@id and c.MainType=@maintype;");
            if (searchMyJoinItem)
            {
                sb.Append(@"select b.*,a.BuyCount from UserGift a 
                            left join GiftFee b on b.GiftFeeId = a.GiftFeeId
                            where a.BuyUserID = @userid and a.IsPay = 1 and a.GiftID = @id");
            }
            sql.AddParams("@id", SqlDbType.BigInt, id)
               .AddParams("@userid", SqlDbType.BigInt, uesrid)
               .AddParams("@maintype", SqlDbType.Int, joinType);
            sql.Cmd = sb.ToString();
            return sql.Query();
        }

        public bool PVCount(long id)
        {
            return new SqlQuickBuild(@"update Gift set PVCount=PVCount+1 where GiftID=@id")
                .AddParams("@id", SqlDbType.BigInt, id)
                .ExecuteSql();
        }

        public bool UpdateCount(int buyCount, long giftid, SqlTransaction tran = null)
        {
            return new SqlQuickBuild(@"update Gift set GiftCount=GiftCount-@count where GiftID=@id")
                .AddParams("@id", SqlDbType.BigInt, giftid)
                .AddParams("@count", SqlDbType.Int, buyCount)
                .ExecuteSql(tran);
        }

        public DataSet GetEditDetail(long id, int jointype)
        {
            return new SqlQuickBuild(@"select * from Gift where IsDelete=0 and GiftID=@id;
                                        select * from GiftFee where GiftID=@id;
                                        select * from JoinItemQuestionExt where IsDelete=0 and MainID=@id and MainType=@joinType;")
                                       .AddParams("@id", SqlDbType.BigInt, id)
                                       .AddParams("@joinType", SqlDbType.Int, jointype)
                                       .Query();
        }
    }

}

