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
    /// 修改限制记录表
    /// </summary>
    public class EditLogDAL : Auto_EditLogDAL
    {
        public DataTable GetEditLogByNew(long userID, int edittype)
        {
            return new SqlQuickBuild(@"select top 1 * from EditLog where UserID=@uid and [Type]=@type order by CreateTime desc")
                .AddParams("@uid", SqlDbType.BigInt, userID)
                .AddParams("@type", SqlDbType.Int, edittype)
                .GetTable();
        }
    }

}
