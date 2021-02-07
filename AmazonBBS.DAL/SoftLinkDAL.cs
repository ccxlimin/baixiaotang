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
    /// 软件导航
    /// </summary>
    public class SoftLinkDAL : Auto_SoftLinkDAL
    {
        public void DeleteALLByID(int softLinkTypeID)
        {
            new SqlQuickBuild("update SoftLink set IsDelete=1 where SoftLinkType=@id")
                .AddParams("@id", SqlDbType.BigInt, softLinkTypeID)
                .ExecuteSql();
        }
    }

}

