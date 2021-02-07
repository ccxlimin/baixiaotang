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
    /// 用户扩展表
    /// </summary>
    public class Auto_UserExtDAL
    {
		#region add
		/// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(UserExt model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into UserExt(");			
			strSql.Append(" UserID,TotalScore,TotalCoin,LevelName,OnlyLevelName,CloseTime,UserV,RealName,CardID,CompanyName,CompanyTel,CardPic,FaRenPic,CheckBBS,VIP,VIPExpiryTime,HeadNameShowType,UserCenterSkin )");
			strSql.Append(" values (");
			strSql.Append("@UserID,@TotalScore,@TotalCoin,@LevelName,@OnlyLevelName,@CloseTime,@UserV,@RealName,@CardID,@CompanyName,@CompanyTel,@CardPic,@FaRenPic,@CheckBBS,@VIP,@VIPExpiryTime,@HeadNameShowType,@UserCenterSkin);select @@IDENTITY");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@UserID", model.UserID),
        new SqlParameter("@TotalScore", model.TotalScore),
        new SqlParameter("@TotalCoin", model.TotalCoin),
        new SqlParameter("@LevelName", model.LevelName),
        new SqlParameter("@OnlyLevelName", model.OnlyLevelName),
        new SqlParameter("@CloseTime", model.CloseTime),
        new SqlParameter("@UserV", model.UserV),
        new SqlParameter("@RealName", model.RealName),
        new SqlParameter("@CardID", model.CardID),
        new SqlParameter("@CompanyName", model.CompanyName),
        new SqlParameter("@CompanyTel", model.CompanyTel),
        new SqlParameter("@CardPic", model.CardPic),
        new SqlParameter("@FaRenPic", model.FaRenPic),
        new SqlParameter("@CheckBBS", model.CheckBBS),
        new SqlParameter("@VIP", model.VIP),
        new SqlParameter("@VIPExpiryTime", model.VIPExpiryTime),
        new SqlParameter("@HeadNameShowType", model.HeadNameShowType),
        new SqlParameter("@UserCenterSkin", model.UserCenterSkin),
 
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
		public bool Update(UserExt model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update UserExt set ");
			strSql.Append("UserID=@UserID,TotalScore=@TotalScore,TotalCoin=@TotalCoin,LevelName=@LevelName,OnlyLevelName=@OnlyLevelName,CloseTime=@CloseTime,UserV=@UserV,RealName=@RealName,CardID=@CardID,CompanyName=@CompanyName,CompanyTel=@CompanyTel,CardPic=@CardPic,FaRenPic=@FaRenPic,CheckBBS=@CheckBBS,VIP=@VIP,VIPExpiryTime=@VIPExpiryTime,HeadNameShowType=@HeadNameShowType,UserCenterSkin=@UserCenterSkin");
		
			strSql.Append(" where UserExtId=@UserExtId");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@UserID", model.UserID),
        new SqlParameter("@TotalScore", model.TotalScore),
        new SqlParameter("@TotalCoin", model.TotalCoin),
        new SqlParameter("@LevelName", model.LevelName),
        new SqlParameter("@OnlyLevelName", model.OnlyLevelName),
        new SqlParameter("@CloseTime", model.CloseTime),
        new SqlParameter("@UserV", model.UserV),
        new SqlParameter("@RealName", model.RealName),
        new SqlParameter("@CardID", model.CardID),
        new SqlParameter("@CompanyName", model.CompanyName),
        new SqlParameter("@CompanyTel", model.CompanyTel),
        new SqlParameter("@CardPic", model.CardPic),
        new SqlParameter("@FaRenPic", model.FaRenPic),
        new SqlParameter("@CheckBBS", model.CheckBBS),
        new SqlParameter("@VIP", model.VIP),
        new SqlParameter("@VIPExpiryTime", model.VIPExpiryTime),
        new SqlParameter("@HeadNameShowType", model.HeadNameShowType),
        new SqlParameter("@UserCenterSkin", model.UserCenterSkin),


				new SqlParameter("@UserExtId", model.UserExtId)
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
			string sql="delete from UserExt where UserExtId=@UserExtId";
			SqlParameter[] parameters = { new SqlParameter("@UserExtId", Id) };			
			return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
		}
		#endregion
		
		#region getmodel
		/// <summary>
		/// 查询单条数据
		/// </summary>
        public DataTable GetModel<T>(T Id, SqlTransaction tran = null)
        {
            string sql = "SELECT * FROM UserExt WHERE UserExtId=@UserExtId";
			SqlParameter[] parameters = { new SqlParameter("@UserExtId", Id) };
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
            string sql="select count(*) from UserExt";
            return (int)SqlHelper.GetSingle(sql);
        }

		/// <summary>
		/// 查询所有数据
		/// </summary>
		public DataTable GetList()
        {
            string sql = "SELECT * FROM UserExt ORDER BY UserExtId desc ";
            return SqlHelper.GetTable(sql);
        }

		/// <summary>
        /// 查询最新的N条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " * FROM UserExt ORDER BY UserExtId desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY UserExtId desc) as rowid ,* FROM UserExt) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY UserExtId desc) as rowid ,* FROM UserExt) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
		#endregion
    }
}
