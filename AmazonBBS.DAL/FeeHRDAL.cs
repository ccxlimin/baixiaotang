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
    /// 消费查看答案记录表
    /// </summary>
    public class FeeHRDAL : Auto_FeeHRDAL
    {
        public string IsPayContact(long userid, long id, int type)
        {
            return new SqlQuickBuild("select count(1) from FeeHR where UserID=@userid and MainID=@mainid and FeeType = @type")
                .AddParams("@userid", SqlDbType.BigInt, userid)
                .AddParams("@mainid", SqlDbType.Int, id)
                .AddParams("@type", SqlDbType.Int, type)
                .GetSingleStr();
        }

    }

}

