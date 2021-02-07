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
    /// 点赞表
    /// </summary>
    public class PriseDAL : Auto_PriseDAL
    {
        public string Exist(long id, int type, long userID)
        {
            return new SqlQuickBuild("select count(1) from Prise where UserID=@userid and IsDelete=0 and Type=@type and TargetID=@targetid")
                .AddParams("@userid", SqlDbType.BigInt, userID)
                .AddParams("@type", SqlDbType.Int, type)
                .AddParams("@targetid", SqlDbType.BigInt, id)
                .GetSingleStr();
        }
    }

}

