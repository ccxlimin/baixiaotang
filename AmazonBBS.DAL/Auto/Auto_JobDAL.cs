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
    /// 职位
    /// </summary>
    public class Auto_JobDAL
    {
		#region add
		/// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(Job model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into Job(");			
			strSql.Append(" JobId,JobType,JobName,ParentJobId,IsJobType,IsJob,CreateType,CreateUser,CreateTime,IsDelete )");
			strSql.Append(" values (");
			strSql.Append("@JobId,@JobType,@JobName,@ParentJobId,@IsJobType,@IsJob,@CreateType,@CreateUser,@CreateTime,@IsDelete);");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@JobType", model.JobType),
        new SqlParameter("@JobName", model.JobName),
        new SqlParameter("@ParentJobId", model.ParentJobId),
        new SqlParameter("@IsJobType", model.IsJobType),
        new SqlParameter("@IsJob", model.IsJob),
        new SqlParameter("@CreateType", model.CreateType),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@IsDelete", model.IsDelete),
 
        new SqlParameter("@JobId", model.JobId)
			};

			int obj;
            if (tran == null)
            {
                obj = SqlHelper.ExecuteSql(strSql.ToString(), CommandType.Text, parameters);
            }
            else
            {
                obj = SqlHelper.ExecuteSql(tran, CommandType.Text, strSql.ToString(), parameters);
            }
            return obj;
		}
		#endregion
		
		#region update
		/// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		public bool Update(Job model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update Job set ");
			strSql.Append("JobId=@JobId,JobType=@JobType,JobName=@JobName,ParentJobId=@ParentJobId,IsJobType=@IsJobType,IsJob=@IsJob,CreateType=@CreateType,CreateUser=@CreateUser,CreateTime=@CreateTime,IsDelete=@IsDelete");
		
			strSql.Append(" where JobId=@JobId");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@JobType", model.JobType),
        new SqlParameter("@JobName", model.JobName),
        new SqlParameter("@ParentJobId", model.ParentJobId),
        new SqlParameter("@IsJobType", model.IsJobType),
        new SqlParameter("@IsJob", model.IsJob),
        new SqlParameter("@CreateType", model.CreateType),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@IsDelete", model.IsDelete),


				new SqlParameter("@JobId", model.JobId)
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
			string sql="delete from Job where JobId=@JobId";
			SqlParameter[] parameters = { new SqlParameter("@JobId", Id) };			
			return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
		}
		#endregion
		
		#region getmodel
		/// <summary>
		/// 查询单条数据
		/// </summary>
        public DataTable GetModel<T>(T Id)
        {
            string sql = "SELECT * FROM Job WHERE JobId=@JobId and IsDelete=0 ";
			SqlParameter[] parameters = { new SqlParameter("@JobId", Id) };
			return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }
		#endregion

		#region query
		/// <summary>
		/// 记录总数
		/// </summary>
		public int Count()
        {
            string sql="select count(*) from Job where IsDelete=0 ";
            return (int)SqlHelper.GetSingle(sql);
        }

		/// <summary>
		/// 查询所有数据
		/// </summary>
		public DataTable GetList()
        {
            string sql = "SELECT * FROM Job where IsDelete=0  ORDER BY CreateTime desc ";
            return SqlHelper.GetTable(sql);
        }

		/// <summary>
        /// 查询最新的N条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " * FROM Job where IsDelete=0  ORDER BY CreateTime desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CreateTime desc) as rowid ,* FROM Job where IsDelete=0 ) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CreateTime desc) as rowid ,* FROM Job where IsDelete=0 ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
		#endregion
    }
}
