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
    /// 分享用户注册表
    /// </summary>
    public class ShareRegistLogDAL : Auto_ShareRegistLogDAL
    {
        public string GetRegistCount(long userID)
        {
            return new SqlQuickBuild("select count(1) from ShareRegistLog where ShareUserID=@uid")
                .AddParams("@uid", SqlDbType.BigInt, userID)
                .GetSingleStr();
        }
    }

}
