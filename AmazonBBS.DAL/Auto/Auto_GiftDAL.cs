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
    /// 礼物表
    /// </summary>
    public class Auto_GiftDAL
    {
		#region add
		/// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(Gift model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into Gift(");			
			strSql.Append(" GiftName,GiftDesc,GiftInfo,GiftImgs,PVCount,GiftCreateTime,GiftCreateUserID,IsDelete,GType,OpenJoinItem )");
			strSql.Append(" values (");
			strSql.Append("@GiftName,@GiftDesc,@GiftInfo,@GiftImgs,@PVCount,@GiftCreateTime,@GiftCreateUserID,@IsDelete,@GType,@OpenJoinItem);select @@IDENTITY");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@GiftName", model.GiftName),
        new SqlParameter("@GiftDesc", model.GiftDesc),
        new SqlParameter("@GiftInfo", model.GiftInfo),
        new SqlParameter("@GiftImgs", model.GiftImgs),
        new SqlParameter("@PVCount", model.PVCount),
        new SqlParameter("@GiftCreateTime", model.GiftCreateTime),
        new SqlParameter("@GiftCreateUserID", model.GiftCreateUserID),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@GType", model.GType),
        new SqlParameter("@OpenJoinItem", model.OpenJoinItem),
 
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
		public bool Update(Gift model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update Gift set ");
			strSql.Append("GiftName=@GiftName,GiftDesc=@GiftDesc,GiftInfo=@GiftInfo,GiftImgs=@GiftImgs,PVCount=@PVCount,GiftCreateTime=@GiftCreateTime,GiftCreateUserID=@GiftCreateUserID,IsDelete=@IsDelete,GType=@GType,OpenJoinItem=@OpenJoinItem");
		
			strSql.Append(" where GiftID=@GiftID");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@GiftName", model.GiftName),
        new SqlParameter("@GiftDesc", model.GiftDesc),
        new SqlParameter("@GiftInfo", model.GiftInfo),
        new SqlParameter("@GiftImgs", model.GiftImgs),
        new SqlParameter("@PVCount", model.PVCount),
        new SqlParameter("@GiftCreateTime", model.GiftCreateTime),
        new SqlParameter("@GiftCreateUserID", model.GiftCreateUserID),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@GType", model.GType),
        new SqlParameter("@OpenJoinItem", model.OpenJoinItem),


				new SqlParameter("@GiftID", model.GiftID)
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
			string sql="delete from Gift where GiftID=@GiftID";
			SqlParameter[] parameters = { new SqlParameter("@GiftID", Id) };			
			return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
		}
		#endregion
		
		#region getmodel
		/// <summary>
		/// 查询单条数据
		/// </summary>
        public DataTable GetModel<T>(T Id, SqlTransaction tran = null)
        {
            string sql = "SELECT * FROM Gift WHERE GiftID=@GiftID and IsDelete=0 ";
			SqlParameter[] parameters = { new SqlParameter("@GiftID", Id) };
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
            string sql="select count(*) from Gift where IsDelete=0 ";
            return (int)SqlHelper.GetSingle(sql);
        }

		/// <summary>
		/// 查询所有数据
		/// </summary>
		public DataTable GetList()
        {
            string sql = "SELECT * FROM Gift where IsDelete=0  ORDER BY GiftID desc ";
            return SqlHelper.GetTable(sql);
        }

		/// <summary>
        /// 查询最新的N条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " * FROM Gift where IsDelete=0  ORDER BY GiftID desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY GiftID desc) as rowid ,* FROM Gift where IsDelete=0 ) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY GiftID desc) as rowid ,* FROM Gift where IsDelete=0 ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
		#endregion
    }
}
