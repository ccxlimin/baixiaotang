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
    public class Auto_NewsDAL
    {

        #region add
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(News model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into News(");
            strSql.Append(" NTitle,NBody,CreateUser,CreateTime,IsTop,IsDelete,PVCount )");
            strSql.Append(" values (");
            strSql.Append("@NTitle,@NBody,@CreateUser,@CreateTime,@IsTop,@IsDelete,@PVCount)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                                    new SqlParameter("@NTitle", model.NTitle),
        new SqlParameter("@NBody", model.NBody),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@IsTop", model.IsTop),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@PVCount", model.PVCount),

                        };

            object obj = SqlHelper.GetSingle(strSql.ToString(), CommandType.Text, parameters);
            return obj == null ? 0 : Convert.ToInt32(obj);
        }
        #endregion


        #region update
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(News model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update News set ");
            strSql.Append("NTitle=@NTitle,NBody=@NBody,CreateUser=@CreateUser,CreateTime=@CreateTime,IsTop=@IsTop,IsDelete=@IsDelete,PVCount=@PVCount");

            strSql.Append(" where NewsID=@NewsID");
            SqlParameter[] parameters = {
                                        new SqlParameter("@NTitle", model.NTitle),
        new SqlParameter("@NBody", model.NBody),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@IsTop", model.IsTop),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@PVCount", model.PVCount),

                                new SqlParameter("@NewsID", model.NewsID)
                            };

            return SqlHelper.ExecuteSql(strSql.ToString(), CommandType.Text, parameters) > 0;
        }
        #endregion

        #region delete
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool Delete(long Id)
        {
            string sql = "delete from News where NewsID=@NewsID";

            SqlParameter[] parameters = { new SqlParameter("@NewsID", Id) };

            return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        public DataTable GetModel(long Id)
        {
            string sql = "SELECT NewsID,NTitle,NBody,CreateUser,CreateTime,IsTop,IsDelete,PVCount FROM News WHERE NewsID=@NewsID";
            SqlParameter[] parameters = { new SqlParameter("@NewsID", Id) };
            return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }
        #endregion

        #region query

        /// <summary>
        /// 记录总数
        /// </summary>
        public int Count()
        {
            string sql = "select count(*) from News ";
            return (int)SqlHelper.GetSingle(sql);
        }


        /// <summary>
        /// 查询所有数据
        /// </summary>
        public DataTable GetList()
        {
            string sql = "SELECT NewsID,NTitle,NBody,CreateUser,CreateTime,IsTop,IsDelete,PVCount FROM News ORDER BY NewsID desc ";
            return SqlHelper.GetTable(sql);
        }

        /// <summary>
        /// 查询最新number 条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " NewsID,NTitle,NBody,CreateUser,CreateTime,IsTop,IsDelete,PVCount FROM News ORDER BY NewsID desc ";
            return SqlHelper.GetTable(sql);
        }

        /// <summary>
        /// 分页显示内容
        /// </summary>
        /// <param name="pageIndex">查询页码</param>
        /// <param name="pageSize">每页显示N条记录</param>
        /// <returns></returns>
        public DataTable Query(int pageIndex, int pageSize)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY NewsID desc  ) as rowid ,NewsID,NTitle,NBody,CreateUser,CreateTime,IsTop,IsDelete,PVCount FROM News   ) ");

            sb.Append("  t where t.rowid between " + ((pageIndex - 1) * pageSize + 1) + " and " + (pageIndex * pageSize));

            return SqlHelper.GetTable(sb.ToString());
        }

        public DataSet SearchByRows(int startIndex, int endIndex)
        {
            return new SqlQuickBuild(@"select count(*) from News where IsDelete=0;
                                      select * from (select row_number() over(order by CreateTime desc) rid,* from News where IsDelete=0)T where T.rid between @startIndex and @endIndex")
                                      .AddParams("@startIndex", SqlDbType.Int, startIndex)
                                      .AddParams("@endIndex", SqlDbType.Int, endIndex)
                                      .Query();
        }

        #endregion

    }

}

