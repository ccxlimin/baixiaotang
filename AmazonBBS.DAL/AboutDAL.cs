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
    /// 公司介绍
    /// </summary>
    public class AboutDAL : Auto_AboutDAL
    {
        public DataTable FindNew()
        {
            return new SqlQuickBuild(@"select top 1 * from [About] where IsDelete = 0 Order by CreateTime desc;")
                .GetTable();
        }

        public bool PV()
        {
            return new SqlQuickBuild("update About set PVCount=PVCount+1 where IsDelete=0").ExecuteSql();
        }

        public bool DeleteALL()
        {
            return new SqlQuickBuild("update [About] set IsDelete=1 where IsDelete=0;").ExecuteSql();
        }
    }

}

