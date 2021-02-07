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
    /// 分享链接表
    /// </summary>
    public class Auto_ShareLinkDAL
    {
		#region add
		/// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(ShareLink model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into ShareLink(");			
			strSql.Append(" UserID,ShareOpenId,ShareToken,ShareMd5Key,ShareAddress )");
			strSql.Append(" values (");
			strSql.Append("@UserID,@ShareOpenId,@ShareToken,@ShareMd5Key,@ShareAddress)");			
			strSql.Append(";select @@IDENTITY");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@UserID", model.UserID),
        new SqlParameter("@ShareOpenId", model.ShareOpenId),
        new SqlParameter("@ShareToken", model.ShareToken),
        new SqlParameter("@ShareMd5Key", model.ShareMd5Key),
        new SqlParameter("@ShareAddress", model.ShareAddress),
 
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
		public bool Update(ShareLink model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update ShareLink set ");
			strSql.Append("UserID=@UserID,ShareOpenId=@ShareOpenId,ShareToken=@ShareToken,ShareMd5Key=@ShareMd5Key,ShareAddress=@ShareAddress");
		
			strSql.Append(" where ShareLinkID=@ShareLinkID");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@UserID", model.UserID),
        new SqlParameter("@ShareOpenId", model.ShareOpenId),
        new SqlParameter("@ShareToken", model.ShareToken),
        new SqlParameter("@ShareMd5Key", model.ShareMd5Key),
        new SqlParameter("@ShareAddress", model.ShareAddress),


				new SqlParameter("@ShareLinkID", model.ShareLinkID)
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
			string sql="delete from ShareLink where ShareLinkID=@ShareLinkID";
			SqlParameter[] parameters = { new SqlParameter("@ShareLinkID", Id) };			
			return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
		}
		#endregion
		
		#region getmodel
		/// <summary>
		/// 查询单条数据
		/// </summary>
        public DataTable GetModel(long Id)
        {
            string sql = "SELECT ShareLinkID,UserID,ShareOpenId,ShareToken,ShareMd5Key,ShareAddress FROM ShareLink WHERE ShareLinkID=@ShareLinkID";
			SqlParameter[] parameters = { new SqlParameter("@ShareLinkID", Id) };
			return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }
		#endregion

		#region query
		/// <summary>
		/// 记录总数
		/// </summary>
		public int Count()
        {
            string sql="select count(*) from ShareLink ";
            return (int)SqlHelper.GetSingle(sql);
        }

		/// <summary>
		/// 查询所有数据
		/// </summary>
		public DataTable GetList()
        {
            string sql = "SELECT ShareLinkID,UserID,ShareOpenId,ShareToken,ShareMd5Key,ShareAddress FROM ShareLink ORDER BY ShareLinkID desc ";
            return SqlHelper.GetTable(sql);
        }

		/// <summary>
        /// 查询最新number 条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " ShareLinkID,UserID,ShareOpenId,ShareToken,ShareMd5Key,ShareAddress FROM ShareLink ORDER BY ShareLinkID desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY ShareLinkID desc  ) as rowid ,ShareLinkID,UserID,ShareOpenId,ShareToken,ShareMd5Key,ShareAddress FROM ShareLink   ) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY ShareLinkID desc  ) as rowid ,ShareLinkID,UserID,ShareOpenId,ShareToken,ShareMd5Key,ShareAddress FROM ShareLink   ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
		#endregion
    }
}
