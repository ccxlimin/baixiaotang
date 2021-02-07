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
    /// 广告
    /// </summary>
    public class ADDAL : Auto_ADDAL
    {
        public string ExistAD(string id)
        {
            return new SqlQuickBuild("select count(1) from AD where IsDelete=0 and ADTitle=@title")
                .AddParams("@title", SqlDbType.NVarChar, id)
                .GetSingleStr();
        }

        public DataTable Find(string title)
        {
            return new SqlQuickBuild("select * from AD where IsDeletee=0 and ADTitle=@title")
                .AddParams("@title", SqlDbType.NVarChar, title)
                .GetTable();
        }
    }

}

