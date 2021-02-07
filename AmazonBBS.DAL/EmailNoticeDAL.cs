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
    /// 邮件通知
    /// </summary>
    public class EmailNoticeDAL : Auto_EmailNoticeDAL
    {
        public DataTable GetModelByAuthor(long? userID, int mainType, long mainID)
        {
            return new SqlQuickBuild("select * from EmailNotice where AuthorID=@uid and MainID=@mainId and MainType=@mainType")
                .AddParams("@uid", SqlDbType.BigInt, userID)
                .AddParams("@mainId", SqlDbType.BigInt, mainID)
                .AddParams("@mainType", SqlDbType.Int, mainType)
                .GetTable();
        }
    }

}
