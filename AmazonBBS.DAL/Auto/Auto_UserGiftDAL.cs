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
    /// 用户兑换礼物表
    /// </summary>
    public class Auto_UserGiftDAL
    {
		#region add
		/// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(UserGift model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into UserGift(");			
			strSql.Append(" GiftID,GType,GiftFeeId,Fee,FeeType,BuyUserID,BuyTime,BuyCount,IsPay,LinkMan,LinkTel )");
			strSql.Append(" values (");
			strSql.Append("@GiftID,@GType,@GiftFeeId,@Fee,@FeeType,@BuyUserID,@BuyTime,@BuyCount,@IsPay,@LinkMan,@LinkTel);select @@IDENTITY");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@GiftID", model.GiftID),
        new SqlParameter("@GType", model.GType),
        new SqlParameter("@GiftFeeId", model.GiftFeeId),
        new SqlParameter("@Fee", model.Fee),
        new SqlParameter("@FeeType", model.FeeType),
        new SqlParameter("@BuyUserID", model.BuyUserID),
        new SqlParameter("@BuyTime", model.BuyTime),
        new SqlParameter("@BuyCount", model.BuyCount),
        new SqlParameter("@IsPay", model.IsPay),
        new SqlParameter("@LinkMan", model.LinkMan),
        new SqlParameter("@LinkTel", model.LinkTel),
 
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
		public bool Update(UserGift model, SqlTransaction tran = null)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update UserGift set ");
			strSql.Append("GiftID=@GiftID,GType=@GType,GiftFeeId=@GiftFeeId,Fee=@Fee,FeeType=@FeeType,BuyUserID=@BuyUserID,BuyTime=@BuyTime,BuyCount=@BuyCount,IsPay=@IsPay,LinkMan=@LinkMan,LinkTel=@LinkTel");
		
			strSql.Append(" where UserGiftId=@UserGiftId");
			SqlParameter[] parameters = 
			{
				        new SqlParameter("@GiftID", model.GiftID),
        new SqlParameter("@GType", model.GType),
        new SqlParameter("@GiftFeeId", model.GiftFeeId),
        new SqlParameter("@Fee", model.Fee),
        new SqlParameter("@FeeType", model.FeeType),
        new SqlParameter("@BuyUserID", model.BuyUserID),
        new SqlParameter("@BuyTime", model.BuyTime),
        new SqlParameter("@BuyCount", model.BuyCount),
        new SqlParameter("@IsPay", model.IsPay),
        new SqlParameter("@LinkMan", model.LinkMan),
        new SqlParameter("@LinkTel", model.LinkTel),


				new SqlParameter("@UserGiftId", model.UserGiftId)
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
			string sql="delete from UserGift where UserGiftId=@UserGiftId";
			SqlParameter[] parameters = { new SqlParameter("@UserGiftId", Id) };			
			return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
		}
		#endregion
		
		#region getmodel
		/// <summary>
		/// 查询单条数据
		/// </summary>
        public DataTable GetModel<T>(T Id, SqlTransaction tran = null)
        {
            string sql = "SELECT * FROM UserGift WHERE UserGiftId=@UserGiftId";
			SqlParameter[] parameters = { new SqlParameter("@UserGiftId", Id) };
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
            string sql="select count(*) from UserGift";
            return (int)SqlHelper.GetSingle(sql);
        }

		/// <summary>
		/// 查询所有数据
		/// </summary>
		public DataTable GetList()
        {
            string sql = "SELECT * FROM UserGift ORDER BY UserGiftId desc ";
            return SqlHelper.GetTable(sql);
        }

		/// <summary>
        /// 查询最新的N条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " * FROM UserGift ORDER BY UserGiftId desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY UserGiftId desc) as rowid ,* FROM UserGift) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY UserGiftId desc) as rowid ,* FROM UserGift) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
		#endregion
    }
}
