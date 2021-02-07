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
    /// 管理员表
    /// </summary>
    public class MasterDAL : Auto_MasterDAL
    {
        public DataTable Find(long userID)
        {
            return new SqlQuickBuild(@"select * from [Master] where UserID=@userid and IsDelete=0;")
                  .AddParams("@userid", SqlDbType.BigInt, userID)
                  .GetTable();
        }

        public DataTable GetAllMaster()
        {
            return new SqlQuickBuild(@"select distinct b.UserID,b.UserName,b.HeadUrl,c.UserV from Master a
                                        left join UserBase b on a.UserID=b.UserID
										left join UserExt c on c.UserID=b.UserID
                                        where a.IsDelete=0 and b.IsDelete=0")
                                        .GetTable();
        }
    }

}

