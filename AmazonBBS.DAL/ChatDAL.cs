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
    /// 聊天记录
    /// </summary>
    public class ChatDAL : Auto_ChatDAL
    {
        public DataSet GetAllUnReadMsg(long userID)
        {
            return new SqlQuickBuild(@"select count(*) UnReadCount,FromID,FromUserName from Chat where ToID=@userID and IsRead=0 group by FromID,FromUserName;
                                        select count(1) from Notice where IsDelete=0 and IsRead=0 and ToUserID=@userID ")
                .AddParams("@userID", SqlDbType.BigInt, userID)
                .Query();
        }

        public DataTable GetALlUnReadMessageByFromID(long id, long currentUserid)
        {
            return new SqlQuickBuild(@"
                                    select a.*,b.HeadUrl Head,b.Sign from Chat a
                                    inner join UserBase b on b.UserID=a.FromID 
                                    where a.FromID=@fromid and a.ToID=@currentUserid and a.IsRead=0;")
                .AddParams("@fromid", SqlDbType.BigInt, id)
                .AddParams("@currentUserid", SqlDbType.BigInt, currentUserid)
                .GetTable();
        }

        public bool Read(string msgid)
        {
            if (msgid.IndexOf("delete") > -1 || msgid.IndexOf("update") > -1)
            {
                return false;
            }
            return new SqlQuickBuild("update Chat set ReadTime=getdate(),IsRead=1 where ChatID in ({0}) and (IsRead=0 or IsRead is null);".FormatWith(msgid))
                .ExecuteSql();
        }

        public bool Read(long fromuserid, long touserid)
        {
            return new SqlQuickBuild("update Chat set ReadTime=getdate(),IsRead=1 where FromID={0} and ToID={1} and IsRead=0;".FormatWith(fromuserid, touserid))
                .ExecuteSql();
        }

        public DataSet GetMyMessage(int startIndex, int endIndex, long userId)
        {
            string searchCondition = @"
                                        from Chat W 
								        inner join UserBase b on b.UserID=w.FromID and b.IsDelete=0
								        inner join UserBase c on c.UserID=w.ToID and c.IsDelete=0
                                        where (ToID=@uid or FromID=@uid) and ChatID= (select MAX(ChatID) from Chat
                                        where (FromID=W.FromID and ToID=W.ToID) or (FromID=W.ToID and ToID=W.FromID))";
            return new SqlQuickBuild(@"
                                    select count(1) {0};
                                    select T.* from (
                                    select 
                                     row_number() over(order by w.SendTime desc)rid,
                                     w.FromID,w.ToID,w.[Message],w.SendTime,w.IsRead,
                                     b.UserName FromUserName,c.UserName ToUserName 
                                     {0}
                                    ) T where T.rid between @startIndex and @endIndex;
                                    ".FormatWith(searchCondition))
                                    .AddParams("@uid", SqlDbType.BigInt, userId)
                                    .AddParams("@startIndex", SqlDbType.Int, startIndex)
                                    .AddParams("@endIndex", SqlDbType.Int, endIndex)
                                    .Query();
        }

        public DataSet GetDialogByUserID(int startIndex, int endIndex, long userID, long toUserID)
        {
            string searchCondition = @"
                                    from Chat W
                                    inner join UserBase b on W.FromID = b.UserID and b.IsDelete=0
                                    inner join UserBase c on W.ToID = c.UserID and c.IsDelete=0
                                    where (W.FromID=@uid and W.ToID=@toUserID) or (W.FromID=@toUserID and W.ToID=@uid)";
            return new SqlQuickBuild(@"
                                    select count(1) {0};
                                    select T.* from (
                                     select 
                                     row_number() over(order by w.SendTime desc)rid,
                                     w.FromID,w.ToID,w.[Message],w.SendTime,w.IsRead,
                                     b.UserName FromUserName,c.UserName ToUserName 
                                     {0}
                                    ) T where T.rid between @startIndex and @endIndex;".FormatWith(searchCondition))
                                    .AddParams("@uid", SqlDbType.BigInt, userID)
                                    .AddParams("@toUserID", SqlDbType.BigInt, toUserID)
                                    .AddParams("@startIndex", SqlDbType.Int, startIndex)
                                    .AddParams("@endIndex", SqlDbType.Int, endIndex)
                                    .Query();
        }
    }

}

