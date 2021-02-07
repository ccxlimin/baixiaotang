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
    public class Auto_EmailNoticeDAL
    {
		#region add
		/// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(EmailNotice model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into EmailNotice(");			
			strSql.Append(" EmailNoticeAuthor,AuthorID,MainID,MainType,MD5Key,MD5Sign,CreateTime )");
			strSql.Append(" values (");
			strSql.Append("@EmailNoticeAuthor,@AuthorID,@MainID,@MainType,@MD5Key,@MD5Sign,@CreateTime)");			
			strSql.Append(";select @@IDENTITY");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@EmailNoticeAuthor", model.EmailNoticeAuthor),
        new SqlParameter("@AuthorID", model.AuthorID),
        new SqlParameter("@MainID", model.MainID),
        new SqlParameter("@MainType", model.MainType),
        new SqlParameter("@MD5Key", model.MD5Key),
        new SqlParameter("@MD5Sign", model.MD5Sign),
        new SqlParameter("@CreateTime", model.CreateTime),

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
		public bool Update(EmailNotice model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update EmailNotice set ");
			strSql.Append("EmailNoticeAuthor=@EmailNoticeAuthor,AuthorID=@AuthorID,MainID=@MainID,MainType=@MainType,MD5Key=@MD5Key,MD5Sign=@MD5Sign,CreateTime=@CreateTime");
		
			strSql.Append(" where EmailNoticeId=@EmailNoticeId");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@EmailNoticeAuthor", model.EmailNoticeAuthor),
        new SqlParameter("@AuthorID", model.AuthorID),
        new SqlParameter("@MainID", model.MainID),
        new SqlParameter("@MainType", model.MainType),
        new SqlParameter("@MD5Key", model.MD5Key),
        new SqlParameter("@MD5Sign", model.MD5Sign),
        new SqlParameter("@CreateTime", model.CreateTime),

                new SqlParameter("@EmailNoticeId", model.EmailNoticeId)
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
			string sql="delete from EmailNotice where EmailNoticeId=@EmailNoticeId";
			SqlParameter[] parameters = { new SqlParameter("@EmailNoticeId", Id) };			
			return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
		}
		#endregion
		
		#region getmodel
		/// <summary>
		/// 查询单条数据
		/// </summary>
        public DataTable GetModel(long Id)
        {
            string sql = "SELECT EmailNoticeId,EmailNoticeAuthor,AuthorID,MainID,MainType,MD5Key,MD5Sign,CreateTime FROM EmailNotice WHERE EmailNoticeId=@EmailNoticeId";
			SqlParameter[] parameters = { new SqlParameter("@EmailNoticeId", Id) };
			return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }
		#endregion

		#region query
		/// <summary>
		/// 记录总数
		/// </summary>
		public int Count()
        {
            string sql="select count(*) from EmailNotice ";
            return (int)SqlHelper.GetSingle(sql);
        }

		/// <summary>
		/// 查询所有数据
		/// </summary>
		public DataTable GetList()
        {
            string sql = "SELECT EmailNoticeId,EmailNoticeAuthor,AuthorID,MainID,MainType,MD5Key,MD5Sign,CreateTime FROM EmailNotice ORDER BY EmailNoticeId desc ";
            return SqlHelper.GetTable(sql);
        }

		/// <summary>
        /// 查询最新number 条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " EmailNoticeId,EmailNoticeAuthor,AuthorID,MainID,MainType,MD5Key,MD5Sign,CreateTime FROM EmailNotice ORDER BY EmailNoticeId desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY EmailNoticeId desc  ) as rowid ,EmailNoticeId,EmailNoticeAuthor,AuthorID,MainID,MainType,MD5Key,MD5Sign,CreateTime FROM EmailNotice   ) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY EmailNoticeId desc  ) as rowid ,EmailNoticeId,EmailNoticeAuthor,AuthorID,MainID,MainType,MD5Key,MD5Sign,CreateTime FROM EmailNotice   ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
		#endregion
    }
}
