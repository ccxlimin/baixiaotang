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
    /// 首页轮播管理
    /// </summary>
    public class Auto_SlideDAL
    {
		#region add
		/// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(Slide model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into Slide(");			
			strSql.Append(" Title,Img,Url,CreateTime,CreateUser,EndTime,IsDelete,FontColor,SlideType )");
			strSql.Append(" values (");
			strSql.Append("@Title,@Img,@Url,@CreateTime,@CreateUser,@EndTime,@IsDelete,@FontColor,@SlideType);select @@IDENTITY");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@Title", model.Title),
        new SqlParameter("@Img", model.Img),
        new SqlParameter("@Url", model.Url),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@EndTime", model.EndTime),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@FontColor", model.FontColor),
        new SqlParameter("@SlideType", model.SlideType),

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
		public bool Update(Slide model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update Slide set ");
			strSql.Append("Title=@Title,Img=@Img,Url=@Url,CreateTime=@CreateTime,CreateUser=@CreateUser,EndTime=@EndTime,IsDelete=@IsDelete,FontColor=@FontColor,SlideType=@SlideType");
		
			strSql.Append(" where SlideId=@SlideId");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@Title", model.Title),
        new SqlParameter("@Img", model.Img),
        new SqlParameter("@Url", model.Url),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@EndTime", model.EndTime),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@FontColor", model.FontColor),
        new SqlParameter("@SlideType", model.SlideType),
                new SqlParameter("@SlideId", model.SlideId)
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
			string sql="delete from Slide where SlideId=@SlideId";
			SqlParameter[] parameters = { new SqlParameter("@SlideId", Id) };			
			return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
		}
		#endregion
		
		#region getmodel
		/// <summary>
		/// 查询单条数据
		/// </summary>
        public DataTable GetModel<T>(T Id, SqlTransaction tran = null)
        {
            string sql = "SELECT * FROM Slide WHERE SlideId=@SlideId and IsDelete=0 ";
			SqlParameter[] parameters = { new SqlParameter("@SlideId", Id) };
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
            string sql="select count(*) from Slide where IsDelete=0 ";
            return (int)SqlHelper.GetSingle(sql);
        }

		/// <summary>
		/// 查询所有数据
		/// </summary>
		public DataTable GetList()
        {
            string sql = "SELECT * FROM Slide where IsDelete=0  ORDER BY CreateTime desc ";
            return SqlHelper.GetTable(sql);
        }

		/// <summary>
        /// 查询最新的N条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " * FROM Slide where IsDelete=0  ORDER BY CreateTime desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CreateTime desc) as rowid ,* FROM Slide where IsDelete=0 ) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CreateTime desc) as rowid ,* FROM Slide where IsDelete=0 ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
		#endregion
    }
}
