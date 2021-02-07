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
    /// 第三方登录
    /// </summary>
    public class OAuthDAL : Auto_OAuthDAL
    {
        public DataTable GetUserInfoByOpenId(string openid, int sourceType)
        {
            return new SqlQuickBuild(@"select * from OAuth where [OAuthType]=@source and [OpenId] = @openid")
                              .AddParams("@source", SqlDbType.Int, sourceType)
                .AddParams("@openid", SqlDbType.VarChar, openid).GetTable();
        }

        public DataTable GetALLByUserID(long userID)
        {
            return new SqlQuickBuild("select * from OAuth where UserID=@uid")
                .AddParams("@uid", SqlDbType.BigInt, userID)
                .GetTable();
        }

        public DataTable GetInfoByUserIDAndSource(long userID, int source, SqlTransaction tran)
        {
            return new SqlQuickBuild("select * from OAuth where UserID=@uid and OAuthType=@type")
                             .AddParams("@uid", SqlDbType.BigInt, userID)
                            .AddParams("@type", SqlDbType.Int, source)
                            .GetTable(tran);
        }

        public bool UpdateBindStatusOauthByID(long userID, int source, int isBind, SqlTransaction tran)
        {
            return new SqlQuickBuild("update Oauth set IsBind=@isBind where UserID=@uid and OAuthType=@type")
                            .AddParams("@uid", SqlDbType.BigInt, userID)
                            .AddParams("@type", SqlDbType.Int, source)
                            .AddParams("@isBind", SqlDbType.Int, isBind)
                            .ExecuteSql(tran);
        }
    }

}
