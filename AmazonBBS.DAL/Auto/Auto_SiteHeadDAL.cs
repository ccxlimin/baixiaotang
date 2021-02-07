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
    /// 网站系统头像
    /// </summary>
    public class Auto_SiteHeadDAL
    {
		#region add
		/// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(SiteHead model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into SiteHead(");			
			strSql.Append(" HeadImg,IsDefault,CreateTime )");
			strSql.Append(" values (");
			strSql.Append("@HeadImg,@IsDefault,@CreateTime)");			
			strSql.Append(";select @@IDENTITY");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@HeadImg", model.HeadImg),
        new SqlParameter("@IsDefault", model.IsDefault),
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
		public bool Update(SiteHead model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update SiteHead set ");
			strSql.Append("HeadImg=@HeadImg,IsDefault=@IsDefault,CreateTime=@CreateTime");
		
			strSql.Append(" where SiteHeadId=@SiteHeadId");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@HeadImg", model.HeadImg),
        new SqlParameter("@IsDefault", model.IsDefault),
        new SqlParameter("@CreateTime", model.CreateTime),


				new SqlParameter("@SiteHeadId", model.SiteHeadId)
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
			string sql="delete from SiteHead where SiteHeadId=@SiteHeadId";
			SqlParameter[] parameters = { new SqlParameter("@SiteHeadId", Id) };			
			return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
		}
		#endregion
		
		#region getmodel
		/// <summary>
		/// 查询单条数据
		/// </summary>
        public DataTable GetModel(long Id)
        {
            string sql = "SELECT SiteHeadId,HeadImg,IsDefault,CreateTime FROM SiteHead WHERE SiteHeadId=@SiteHeadId";
			SqlParameter[] parameters = { new SqlParameter("@SiteHeadId", Id) };
			return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }
		#endregion

		#region query
		/// <summary>
		/// 记录总数
		/// </summary>
		public int Count()
        {
            string sql="select count(*) from SiteHead ";
            return (int)SqlHelper.GetSingle(sql);
        }

		/// <summary>
		/// 查询所有数据
		/// </summary>
		public DataTable GetList()
        {
            string sql = "SELECT SiteHeadId,HeadImg,IsDefault,CreateTime FROM SiteHead ORDER BY SiteHeadId desc ";
            return SqlHelper.GetTable(sql);
        }

		/// <summary>
        /// 查询最新number 条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " SiteHeadId,HeadImg,IsDefault,CreateTime FROM SiteHead ORDER BY SiteHeadId desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY SiteHeadId desc  ) as rowid ,SiteHeadId,HeadImg,IsDefault,CreateTime FROM SiteHead   ) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY SiteHeadId desc  ) as rowid ,SiteHeadId,HeadImg,IsDefault,CreateTime FROM SiteHead   ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
		#endregion
    }
}
