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
    /// 投递简历记录表
    /// </summary>
    public class SendCVDAL : Auto_SendCVDAL
    {
        public string HasSendCV(long zhaoPinID, long userID)
        {
            return new SqlQuickBuild("select count(1) from SendCV where ZhaoPinID=@zid and CreateUser=@user")
                .AddParams("@zid", SqlDbType.BigInt, zhaoPinID)
                .AddParams("@user", SqlDbType.NVarChar, userID.ToString())
                .GetSingleStr();
        }

        public string GetFilePath(long id, long uid)
        {
            return new SqlQuickBuild(@"select CVPath from SendCV where ZhaoPinID=@zid and CreateUser=@user")
                 .AddParams("@zid", SqlDbType.BigInt, id)
                .AddParams("@user", SqlDbType.NVarChar, uid.ToString())
                .GetSingleStr();
        }
    }

}
