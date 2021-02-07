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
    /// 系统通知
    /// </summary>
    public class NoticeDAL : Auto_NoticeDAL
    {
        public DataSet GetMyNotice(int startIndex, int endIndex, long userID)
        {
            return new SqlQuickBuild(@"
                    select count(1) from Notice where ToUserID=@uid;
                    select T.* from 
                    (select row_number() over(order by CreateTime desc) rid,* from Notice
                    where ToUserID=@uid) T where T.rid between @startIndex and @endIndex;")
                .AddParams("@uid", SqlDbType.BigInt, userID)
                .AddParams("@startIndex", SqlDbType.Int, startIndex)
                .AddParams("@endIndex", SqlDbType.Int, endIndex)
                .Query();
        }

        public void Read(long userID, DateTime now)
        {
            new SqlQuickBuild("update Notice set IsRead=1,ReadTime=getdate() where ToUserID=@userid and IsRead=0 and CreateTime <@now")
                .AddParams("@userid", SqlDbType.BigInt, userID)
                .AddParams("@now", SqlDbType.DateTime, now)
                .ExecuteSql();
        }
    }

}
