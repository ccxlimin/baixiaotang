using AmazonBBS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.BLL
{
    public class ErrorBLL
    {
        public static ErrorBLL Instance
        {
            get
            {
                return SingleHepler<ErrorBLL>.Instance;
            }
        }

        public void Log(string msg)
        {
            new SqlQuickBuild(@"insert into ErrorLog (ErrorMsg,ErrorTime) values (@msg,getdate())")
         .AddParams("@msg", System.Data.SqlDbType.Text, msg).ExecuteSql();
        }
    }
}
