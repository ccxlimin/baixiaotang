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
    /// 支付回调日志
    /// </summary>
    public class Auto_PayCBLogDAL
    {

		#region add
		/// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(PayCBLog model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into PayCBLog(");			
			strSql.Append(" CallBackTime,TradeNo,TradeStatus,Memo,IsDelete,IsPay )");
			strSql.Append(" values (");
			strSql.Append("@CallBackTime,@TradeNo,@TradeStatus,@Memo,@IsDelete,@IsPay)");			
			strSql.Append(";select @@IDENTITY");
			SqlParameter[] parameters = {
							        new SqlParameter("@CallBackTime", model.CallBackTime),
        new SqlParameter("@TradeNo", model.TradeNo),
        new SqlParameter("@TradeStatus", model.TradeStatus),
        new SqlParameter("@Memo", model.Memo),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@IsPay", model.IsPay),
                        };

			object obj = SqlHelper.GetSingle(strSql.ToString(), CommandType.Text, parameters);
            return obj == null ? 0 : Convert.ToInt32(obj);
		}
		#endregion

		
		#region update
		/// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		public bool Update(PayCBLog model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update PayCBLog set ");
			strSql.Append("CallBackTime=@CallBackTime,TradeNo=@TradeNo,TradeStatus=@TradeStatus,Memo=@Memo,IsDelete=@IsDelete,IsPay=@IsPay");
		
			strSql.Append(" where PayCBLogID=@PayCBLogID");
			SqlParameter[] parameters = {
								        new SqlParameter("@CallBackTime", model.CallBackTime),
        new SqlParameter("@TradeNo", model.TradeNo),
        new SqlParameter("@TradeStatus", model.TradeStatus),
        new SqlParameter("@Memo", model.Memo),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@IsPay", model.IsPay),

                                new SqlParameter("@PayCBLogID", model.PayCBLogID)
							};
			
			return SqlHelper.ExecuteSql(strSql.ToString(), CommandType.Text, parameters) > 0;
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
			string sql="delete from PayCBLog where PayCBLogID=@PayCBLogID";
			
			SqlParameter[] parameters = { new SqlParameter("@PayCBLogID", Id) };			

			return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
		}
		#endregion
		
		#region getmodel
		/// <summary>
		/// 查询单条数据
		/// </summary>
        public DataTable GetModel(long Id)
        {
            string sql = "SELECT PayCBLogID,CallBackTime,TradeNo,TradeStatus,Memo,IsDelete,IsPay FROM PayCBLog WHERE PayCBLogID=@PayCBLogID";
			SqlParameter[] parameters = { new SqlParameter("@PayCBLogID", Id) };
			return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }
		#endregion

		#region query

		/// <summary>
		/// 记录总数
		/// </summary>
		public int Count()
        {
            string sql="select count(*) from PayCBLog ";
            return (int)SqlHelper.GetSingle(sql);
        }


		/// <summary>
		/// 查询所有数据
		/// </summary>
		public DataTable GetList()
        {
            string sql = "SELECT PayCBLogID,CallBackTime,TradeNo,TradeStatus,Memo,IsDelete,IsPay FROM PayCBLog ORDER BY PayCBLogID desc ";
            return SqlHelper.GetTable(sql);
        }

		/// <summary>
        /// 查询最新number 条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " PayCBLogID,CallBackTime,TradeNo,TradeStatus,Memo,IsDelete,IsPay FROM PayCBLog ORDER BY PayCBLogID desc ";
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

            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY PayCBLogID desc  ) as rowid ,PayCBLogID,CallBackTime,TradeNo,TradeStatus,Memo,IsDelete,IsPay FROM PayCBLog   ) ");

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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY PayCBLogID desc  ) as rowid ,PayCBLogID,CallBackTime,TradeNo,TradeStatus,Memo,IsDelete,IsPay FROM PayCBLog   ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }

		#endregion

    }

}

