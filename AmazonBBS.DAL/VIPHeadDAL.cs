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
    /// 用户动态头像
    /// </summary>
    public class VIPHeadDAL : Auto_VIPHeadDAL
    {
        public DataTable GetALLVIPHead()
        {
            return new SqlQuickBuild(@"select a.*,b.UserName from VIPHead a 
                                left join UserBase b on b.UserID=a.UserID
                                where a.IsDelete=0 and IsChecked=0;")
                                .GetTable();
        }
    }

}
