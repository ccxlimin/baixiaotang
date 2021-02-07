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
    /// 活动表
    /// </summary>
    public class Auto_ActivityDAL
    {
		#region add
		/// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(Activity model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into Activity(");			
			strSql.Append(" UserID,UserName,Title,Body,ActivityIMG,ActivityType,Address,BeginTime,EndTime,CanJoinOnBegin,JoinBeginTime,JoinEndTime,ActivityCreateTIme,PVCount,IsDelete,IsChecked )");
			strSql.Append(" values (");
			strSql.Append("@UserID,@UserName,@Title,@Body,@ActivityIMG,@ActivityType,@Address,@BeginTime,@EndTime,@CanJoinOnBegin,@JoinBeginTime,@JoinEndTime,@ActivityCreateTIme,@PVCount,@IsDelete,@IsChecked);select @@IDENTITY");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@UserID", model.UserID),
        new SqlParameter("@UserName", model.UserName),
        new SqlParameter("@Title", model.Title),
        new SqlParameter("@Body", model.Body),
        new SqlParameter("@ActivityIMG", model.ActivityIMG),
        new SqlParameter("@ActivityType", model.ActivityType),
        new SqlParameter("@Address", model.Address),
        new SqlParameter("@BeginTime", model.BeginTime),
        new SqlParameter("@EndTime", model.EndTime),
        new SqlParameter("@CanJoinOnBegin", model.CanJoinOnBegin),
        new SqlParameter("@JoinBeginTime", model.JoinBeginTime),
        new SqlParameter("@JoinEndTime", model.JoinEndTime),
        new SqlParameter("@ActivityCreateTIme", model.ActivityCreateTIme),
        new SqlParameter("@PVCount", model.PVCount),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@IsChecked", model.IsChecked),
 
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
		public bool Update(Activity model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update Activity set ");
			strSql.Append("UserID=@UserID,UserName=@UserName,Title=@Title,Body=@Body,ActivityIMG=@ActivityIMG,ActivityType=@ActivityType,Address=@Address,BeginTime=@BeginTime,EndTime=@EndTime,CanJoinOnBegin=@CanJoinOnBegin,JoinBeginTime=@JoinBeginTime,JoinEndTime=@JoinEndTime,ActivityCreateTIme=@ActivityCreateTIme,PVCount=@PVCount,IsDelete=@IsDelete,IsChecked=@IsChecked");
		
			strSql.Append(" where ActivityId=@ActivityId");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@UserID", model.UserID),
        new SqlParameter("@UserName", model.UserName),
        new SqlParameter("@Title", model.Title),
        new SqlParameter("@Body", model.Body),
        new SqlParameter("@ActivityIMG", model.ActivityIMG),
        new SqlParameter("@ActivityType", model.ActivityType),
        new SqlParameter("@Address", model.Address),
        new SqlParameter("@BeginTime", model.BeginTime),
        new SqlParameter("@EndTime", model.EndTime),
        new SqlParameter("@CanJoinOnBegin", model.CanJoinOnBegin),
        new SqlParameter("@JoinBeginTime", model.JoinBeginTime),
        new SqlParameter("@JoinEndTime", model.JoinEndTime),
        new SqlParameter("@ActivityCreateTIme", model.ActivityCreateTIme),
        new SqlParameter("@PVCount", model.PVCount),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@IsChecked", model.IsChecked),


				new SqlParameter("@ActivityId", model.ActivityId)
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
			string sql="delete from Activity where ActivityId=@ActivityId";
			SqlParameter[] parameters = { new SqlParameter("@ActivityId", Id) };			
			return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
		}
		#endregion
		
		#region getmodel
		/// <summary>
		/// 查询单条数据
		/// </summary>
        public DataTable GetModel<T>(T Id, SqlTransaction tran = null)
        {
            string sql = "SELECT * FROM Activity WHERE ActivityId=@ActivityId and IsDelete=0 ";
			SqlParameter[] parameters = { new SqlParameter("@ActivityId", Id) };
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
            string sql="select count(*) from Activity where IsDelete=0 ";
            return (int)SqlHelper.GetSingle(sql);
        }

		/// <summary>
		/// 查询所有数据
		/// </summary>
		public DataTable GetList()
        {
            string sql = "SELECT * FROM Activity where IsDelete=0  ORDER BY ActivityId desc ";
            return SqlHelper.GetTable(sql);
        }

		/// <summary>
        /// 查询最新的N条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " * FROM Activity where IsDelete=0  ORDER BY ActivityId desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY ActivityId desc) as rowid ,* FROM Activity where IsDelete=0 ) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY ActivityId desc) as rowid ,* FROM Activity where IsDelete=0 ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
		#endregion
    }
}
