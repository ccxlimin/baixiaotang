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
    /// 分享链接表
    /// </summary>
    public class ShareLinkDAL : Auto_ShareLinkDAL
    {
        public DataTable GetLinkByUserID(long userID)
        {
            return new SqlQuickBuild("select * from ShareLink where UserID=@uid")
                .AddParams("@uid", SqlDbType.BigInt, userID)
                .GetTable();
        }

        public void AddPVCount(long shareLinkID)
        {
            new SqlQuickBuild("update ShareLink set PVCount=PVCount+1 where ShareLinkID=@id")
                .AddParams("@id", SqlDbType.BigInt, shareLinkID).ExecuteSql();
        }
    }

}
