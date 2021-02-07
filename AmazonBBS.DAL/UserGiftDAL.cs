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
    /// 用户兑换礼物表
    /// </summary>
    public class UserGiftDAL : Auto_UserGiftDAL
    {
        public string IsJoined(long userId, long aid)
        {
            return new SqlQuickBuild(@"select count(1) from UserGift where GiftID=@aid and BuyUserID=@userId and IsPay=1;")
                .AddParams("@userId", SqlDbType.BigInt, userId)
                .AddParams("@aid", SqlDbType.BigInt, aid)
                .GetSingleStr();
        }

        public DataTable GetBuyUsers(long id)
        {
            return new SqlQuickBuild(@"select b.UserName,b.UserID [Uid],b.HeadUrl from UserGift a
                                        left join UserBase b on a.BuyUserID=b.UserID 
                                        where a.GiftID=@id and a.IsPay=1 order by BuyTime desc")
               .AddParams("@id", SqlDbType.BigInt, id)
               .GetTable();
        }

        public DataSet GetBuyerList(long id, int joinType)
        {
            return new SqlQuickBuild(@"select GiftCreateUserID,GiftID,GiftName,GType from Gift where GiftID=@id and IsDelete=0;
               select a.*,b.FeeName,b.Fee ItemSourceFee,c.UserName BuyerName from UserGift a
               left join GiftFee b on b.GiftFeeId=a.GiftFeeId
			   left join UserBase c on c.UserID=a.BuyUserID
                where a.GiftID=@id;
               select * from JoinItemQuestionExt where IsDelete=0 and MainID=@id and MainType=@jointype;
               select * from JoinItemAnswerExt where JoinMainID=@id and JoinType=@jointype;")
 .AddParams("@id", SqlDbType.BigInt, id)
 .AddParams("@jointype", SqlDbType.Int, joinType)
 .Query();
        }

        public DataTable GetDetailJoinInfo(object userid, object itemID, SqlTransaction tran)
        {
            return new SqlQuickBuild(@"select * from UserGift where GiftID=@id and BuyUserID= @userid ;")
                     .AddParams("@userid", SqlDbType.BigInt, userid)
                     .AddParams("@id", SqlDbType.BigInt, itemID)
                     .GetTable(tran);
        }

        public bool UpdateUserBuyInfo(long userid, long itemID, SqlTransaction tran)
        {
            return new SqlQuickBuild("update UserGift set IsPay=1 where GiftID=@id and BuyUserID= @userid")
                 .AddParams("@userid", SqlDbType.BigInt, userid)
                     .AddParams("@id", SqlDbType.BigInt, itemID)
                     .ExecuteSql(tran);
        }

        public string GetUserBuyItemID(decimal? fee, int? buyCount, string createUser, SqlTransaction tran)
        {
            return new SqlQuickBuild(
                "select top 1 UserGiftId from UserGift where Fee=@fee and BuyCount=@buyCount and BuyUserID=@userId order by BuyTime desc")
                .AddParams("@fee", SqlDbType.Decimal, fee)
                .AddParams("@buyCount", SqlDbType.Int, buyCount)
                .AddParams("@userId", SqlDbType.BigInt, createUser)
                .GetSingleStr(tran);
        }
    }

}

