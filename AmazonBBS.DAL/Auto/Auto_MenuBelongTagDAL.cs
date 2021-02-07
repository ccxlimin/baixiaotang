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
    /// 页面模块对应标签
    /// </summary>
    public class Auto_MenuBelongTagDAL
    {
		#region add
		/// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(MenuBelongTag model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into MenuBelongTag(");			
			strSql.Append(" MainId,MainType,TagId )");
			strSql.Append(" values (");
			strSql.Append("@MainId,@MainType,@TagId)");			
			strSql.Append(";select @@IDENTITY");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@MainId", model.MainId),
        new SqlParameter("@MainType", model.MainType),
        new SqlParameter("@TagId", model.TagId),
 
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
		public bool Update(MenuBelongTag model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update MenuBelongTag set ");
			strSql.Append("MainId=@MainId,MainType=@MainType,TagId=@TagId");
		
			strSql.Append(" where MenuBelongTagId=@MenuBelongTagId");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@MainId", model.MainId),
        new SqlParameter("@MainType", model.MainType),
        new SqlParameter("@TagId", model.TagId),


				new SqlParameter("@MenuBelongTagId", model.MenuBelongTagId)
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
			string sql="delete from MenuBelongTag where MenuBelongTagId=@MenuBelongTagId";
			SqlParameter[] parameters = { new SqlParameter("@MenuBelongTagId", Id) };			
			return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
		}
		#endregion
		
		#region getmodel
		/// <summary>
		/// 查询单条数据
		/// </summary>
        public DataTable GetModel(long Id)
        {
            string sql = "SELECT MenuBelongTagId,MainId,MainType,TagId FROM MenuBelongTag WHERE MenuBelongTagId=@MenuBelongTagId";
			SqlParameter[] parameters = { new SqlParameter("@MenuBelongTagId", Id) };
			return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }
		#endregion

		#region query
		/// <summary>
		/// 记录总数
		/// </summary>
		public int Count()
        {
            string sql="select count(*) from MenuBelongTag ";
            return (int)SqlHelper.GetSingle(sql);
        }

		/// <summary>
		/// 查询所有数据
		/// </summary>
		public DataTable GetList()
        {
            string sql = "SELECT MenuBelongTagId,MainId,MainType,TagId FROM MenuBelongTag ORDER BY MenuBelongTagId desc ";
            return SqlHelper.GetTable(sql);
        }

		/// <summary>
        /// 查询最新number 条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " MenuBelongTagId,MainId,MainType,TagId FROM MenuBelongTag ORDER BY MenuBelongTagId desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY MenuBelongTagId desc  ) as rowid ,MenuBelongTagId,MainId,MainType,TagId FROM MenuBelongTag   ) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY MenuBelongTagId desc  ) as rowid ,MenuBelongTagId,MainId,MainType,TagId FROM MenuBelongTag   ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
		#endregion
    }
}
