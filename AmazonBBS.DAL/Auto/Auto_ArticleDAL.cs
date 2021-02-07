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
    /// 文章表
    /// </summary>
    public class Auto_ArticleDAL
    {
        #region add
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(Article model, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Article(");
            strSql.Append(" UserID,Title,Body,PVCount,CreateTime,IsDelete,IsChecked,FilePath,EditCount,UpdateTime,UpdateUser,CreateUser,ContentNeedPay,ContentFee,ContentFeeType,IsAnonymous )");
            strSql.Append(" values (");
            strSql.Append("@UserID,@Title,@Body,@PVCount,@CreateTime,@IsDelete,@IsChecked,@FilePath,@EditCount,@UpdateTime,@UpdateUser,@CreateUser,@ContentNeedPay,@ContentFee,@ContentFeeType,@IsAnonymous);select @@IDENTITY");
            SqlParameter[] parameters =
            {
                        new SqlParameter("@UserID", model.UserID),
        new SqlParameter("@Title", model.Title),
        new SqlParameter("@Body", model.Body),
        new SqlParameter("@PVCount", model.PVCount),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@IsChecked", model.IsChecked),
        new SqlParameter("@FilePath", model.FilePath),
        new SqlParameter("@EditCount", model.EditCount),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@UpdateUser", model.UpdateUser),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@ContentNeedPay", model.ContentNeedPay),
        new SqlParameter("@ContentFee", model.ContentFee),
        new SqlParameter("@ContentFeeType", model.ContentFeeType),
        new SqlParameter("@IsAnonymous", model.IsAnonymous),
            };

            object obj;
            if (tran == null)
            {
                obj = SqlHelper.GetSingle(strSql.ToString(), CommandType.Text, parameters);
            }
            else
            {
                obj = SqlHelper.GetSingle(tran, CommandType.Text, strSql.ToString(), parameters);
            }
            return obj == null ? 0 : Convert.ToInt32(obj);
        }
        #endregion

        #region update
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(Article model, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Article set ");
            strSql.Append("UserID=@UserID,Title=@Title,Body=@Body,PVCount=@PVCount,CreateTime=@CreateTime,IsDelete=@IsDelete,IsChecked=@IsChecked,FilePath=@FilePath,EditCount=@EditCount,UpdateTime=@UpdateTime,UpdateUser=@UpdateUser,CreateUser=@CreateUser,ContentNeedPay=@ContentNeedPay,ContentFee=@ContentFee,ContentFeeType=@ContentFeeType,IsAnonymous=@IsAnonymous");

            strSql.Append(" where ArticleId=@ArticleId");
            SqlParameter[] parameters =
            {
                        new SqlParameter("@UserID", model.UserID),
        new SqlParameter("@Title", model.Title),
        new SqlParameter("@Body", model.Body),
        new SqlParameter("@PVCount", model.PVCount),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@IsChecked", model.IsChecked),
        new SqlParameter("@FilePath", model.FilePath),
        new SqlParameter("@EditCount", model.EditCount),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@UpdateUser", model.UpdateUser),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@ContentNeedPay", model.ContentNeedPay),
        new SqlParameter("@ContentFee", model.ContentFee),
        new SqlParameter("@ContentFeeType", model.ContentFeeType),
        new SqlParameter("@IsAnonymous", model.IsAnonymous),

                new SqlParameter("@ArticleId", model.ArticleId)
            };

            if (tran == null)
            {
                return SqlHelper.ExecuteSql(strSql.ToString(), CommandType.Text, parameters) > 0;
            }
            else
            {
                return SqlHelper.ExecuteSql(tran, CommandType.Text, strSql.ToString(), parameters) > 0;
            }
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
            string sql = "delete from Article where ArticleId=@ArticleId";
            SqlParameter[] parameters = { new SqlParameter("@ArticleId", Id) };
            return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        public DataTable GetModel<T>(T Id, SqlTransaction tran)
        {
            string sql = "SELECT * FROM Article WHERE ArticleId=@ArticleId and IsDelete=0 ";
            SqlParameter[] parameters = { new SqlParameter("@ArticleId", Id) };
            if (tran == null)
            {
                return SqlHelper.GetTable(sql, CommandType.Text, parameters);
            }
            else
            {
                return SqlHelper.GetTable(tran, sql, CommandType.Text, parameters);
            }
        }
        #endregion

        #region query
        /// <summary>
        /// 记录总数
        /// </summary>
        public int Count()
        {
            string sql = "select count(*) from Article where IsDelete=0 ";
            return (int)SqlHelper.GetSingle(sql);
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        public DataTable GetList()
        {
            string sql = "SELECT * FROM Article where IsDelete=0  ORDER BY CreateTime desc ";
            return SqlHelper.GetTable(sql);
        }

        /// <summary>
        /// 查询最新的N条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " * FROM Article where IsDelete=0  ORDER BY CreateTime desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CreateTime desc) as rowid ,* FROM Article where IsDelete=0 ) ");
            sb.Append("  t where t.rowid between " + ((pageIndex - 1) * pageSize + 1) + " and " + (pageIndex * pageSize));
            return SqlHelper.GetTable(sb.ToString());
        }

        /// <summary>
        /// 分页显示内容
        /// </summary>
        /// <param name="startIndex">开始码</param>
        /// <param name="endIndex">结束码</param>
        /// <returns></returns>
        public DataTable SearchByRows(int startIndex, int endIndex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CreateTime desc) as rowid ,* FROM Article where IsDelete=0 ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
        #endregion
    }
}
