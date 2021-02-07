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
    /// 公司新闻
    /// </summary>
    public class NewsDAL : Auto_NewsDAL
    {
        public DataTable Top5(int count)
        {
            return new SqlQuickBuild("select top (@top) * from News where IsDelete=0 order by CreateTime desc")
                                    .AddParams("@top", SqlDbType.Int, count)
                                    .GetTable();
        }
    }

}

