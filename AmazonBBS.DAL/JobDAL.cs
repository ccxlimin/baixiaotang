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
    /// 职位
    /// </summary>
    public class JobDAL : Auto_JobDAL
    {
        public DataTable GetJobType()
        {
            return new SqlQuickBuild("select * from Job where IsJobType=1 and IsDelete=0;").GetTable();
        }
    }

}
