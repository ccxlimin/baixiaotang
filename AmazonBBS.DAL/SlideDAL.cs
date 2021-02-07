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
    /// 首页轮播管理
    /// </summary>
    public class SlideDAL : Auto_SlideDAL
    {
        public DataTable FindALL()
        {
            return new SqlQuickBuild(@"select * from Slide order by CreateTime desc;").GetTable();
        }

        public bool SetDeleteOrNot(int id, int deleteType, SqlTransaction tran)
        {
            return new SqlQuickBuild("update Slide set IsDelete=@isdelete where SlideId=@id")
                .AddParams("@isdelete", SqlDbType.Int, deleteType)
                .AddParams("@id", SqlDbType.Int, id)
                .ExecuteSql(tran);
        }
    }

}
