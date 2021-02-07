﻿using System;
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
    /// 枚举
    /// </summary>
    public class Auto_BBSEnumDAL
    {
		#region add
		/// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(BBSEnum model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into BBSEnum(");			
			strSql.Append(" EnumType,EnumDesc,EnumCode,Url,IsBBS,SortIndex,CreateTime,CreateUser,UpdateTime,UpdateUser,IsDelete,CanArticle,FontBGColor,PageBGColor,FontColor,GroupBy )");
			strSql.Append(" values (");
			strSql.Append("@EnumType,@EnumDesc,@EnumCode,@Url,@IsBBS,@SortIndex,@CreateTime,@CreateUser,@UpdateTime,@UpdateUser,@IsDelete,@CanArticle,@FontBGColor,@PageBGColor,@FontColor,@GroupBy);select @@IDENTITY");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@EnumType", model.EnumType),
        new SqlParameter("@EnumDesc", model.EnumDesc),
        new SqlParameter("@EnumCode", model.EnumCode),
        new SqlParameter("@Url", model.Url),
        new SqlParameter("@IsBBS", model.IsBBS),
        new SqlParameter("@SortIndex", model.SortIndex),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@UpdateUser", model.UpdateUser),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@CanArticle", model.CanArticle),
        new SqlParameter("@FontBGColor", model.FontBGColor),
        new SqlParameter("@PageBGColor", model.PageBGColor),
        new SqlParameter("@FontColor", model.FontColor),
        new SqlParameter("@GroupBy", model.GroupBy),
 
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
		public bool Update(BBSEnum model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update BBSEnum set ");
			strSql.Append("EnumType=@EnumType,EnumDesc=@EnumDesc,EnumCode=@EnumCode,Url=@Url,IsBBS=@IsBBS,SortIndex=@SortIndex,CreateTime=@CreateTime,CreateUser=@CreateUser,UpdateTime=@UpdateTime,UpdateUser=@UpdateUser,IsDelete=@IsDelete,CanArticle=@CanArticle,FontBGColor=@FontBGColor,PageBGColor=@PageBGColor,FontColor=@FontColor,GroupBy=@GroupBy");
		
			strSql.Append(" where BBSEnumId=@BBSEnumId");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@EnumType", model.EnumType),
        new SqlParameter("@EnumDesc", model.EnumDesc),
        new SqlParameter("@EnumCode", model.EnumCode),
        new SqlParameter("@Url", model.Url),
        new SqlParameter("@IsBBS", model.IsBBS),
        new SqlParameter("@SortIndex", model.SortIndex),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@UpdateUser", model.UpdateUser),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@CanArticle", model.CanArticle),
        new SqlParameter("@FontBGColor", model.FontBGColor),
        new SqlParameter("@PageBGColor", model.PageBGColor),
        new SqlParameter("@FontColor", model.FontColor),
        new SqlParameter("@GroupBy", model.GroupBy),


				new SqlParameter("@BBSEnumId", model.BBSEnumId)
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
			string sql="delete from BBSEnum where BBSEnumId=@BBSEnumId";
			SqlParameter[] parameters = { new SqlParameter("@BBSEnumId", Id) };			
			return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
		}
		#endregion
		
		#region getmodel
		/// <summary>
		/// 查询单条数据
		/// </summary>
        public DataTable GetModel<T>(T Id, SqlTransaction tran = null)
        {
            string sql = "SELECT * FROM BBSEnum WHERE BBSEnumId=@BBSEnumId and IsDelete=0 ";
			SqlParameter[] parameters = { new SqlParameter("@BBSEnumId", Id) };
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
            string sql="select count(*) from BBSEnum where IsDelete=0 ";
            return (int)SqlHelper.GetSingle(sql);
        }

		/// <summary>
		/// 查询所有数据
		/// </summary>
		public DataTable GetList()
        {
            string sql = "SELECT * FROM BBSEnum where IsDelete=0  ORDER BY CreateTime desc ";
            return SqlHelper.GetTable(sql);
        }

		/// <summary>
        /// 查询最新的N条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " * FROM BBSEnum where IsDelete=0  ORDER BY CreateTime desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CreateTime desc) as rowid ,* FROM BBSEnum where IsDelete=0 ) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CreateTime desc) as rowid ,* FROM BBSEnum where IsDelete=0 ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
		#endregion
    }
}
