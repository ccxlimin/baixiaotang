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
    /// 工作评论
    /// </summary>
    public class JobCommentDAL : Auto_JobCommentDAL
    {
        public DataSet GetJobComment(long id, int type, int startindex, int endindex)
        {
            return new SqlQuickBuild(@"
select count(*) from JobComment where MainID=@id and IsDelete=0 and CommentType=@type;
select T.* from (
select row_number() over (order by CreateTime desc) row,* from JobComment where MainID=@id and IsDelete=0 and CommentType=@type)
T where T.row between @startindex and @endindex;
")
.AddParams("@id", SqlDbType.BigInt, id)
.AddParams("@type", SqlDbType.Int, type)
.AddParams("@startindex", SqlDbType.Int, startindex)
.AddParams("@endindex", SqlDbType.Int, endindex)
.Query();
        }
    }

}

