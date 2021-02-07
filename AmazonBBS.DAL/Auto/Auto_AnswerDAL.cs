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
    /// 回答表
    /// </summary>
    public class Auto_AnswerDAL
    {

		#region add
		/// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(Answer model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into Answer(");			
			strSql.Append(" QuestionId,AnswerUserId,AnswerContent,IsCanSee,CoinType,NeedCoin,AType,ReplyToUserID,ReplyTopAnswerId,ReplyToAnswerID,CreateTime,CreateUser,UpdateTime,UpdateUser,IsDelete )");
			strSql.Append(" values (");
			strSql.Append("@QuestionId,@AnswerUserId,@AnswerContent,@IsCanSee,@CoinType,@NeedCoin,@AType,@ReplyToUserID,@ReplyTopAnswerId,@ReplyToAnswerID,@CreateTime,@CreateUser,@UpdateTime,@UpdateUser,@IsDelete)");			
			strSql.Append(";select @@IDENTITY");
			SqlParameter[] parameters = {
							        new SqlParameter("@QuestionId", model.QuestionId),
        new SqlParameter("@AnswerUserId", model.AnswerUserId),
        new SqlParameter("@AnswerContent", model.AnswerContent),
        new SqlParameter("@IsCanSee", model.IsCanSee),
        new SqlParameter("@CoinType", model.CoinType),
        new SqlParameter("@NeedCoin", model.NeedCoin),
        new SqlParameter("@AType", model.AType),
        new SqlParameter("@ReplyToUserID", model.ReplyToUserID),
        new SqlParameter("@ReplyTopAnswerId", model.ReplyTopAnswerId),
        new SqlParameter("@ReplyToAnswerID", model.ReplyToAnswerID),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@UpdateUser", model.UpdateUser),
        new SqlParameter("@IsDelete", model.IsDelete),
 
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
		public bool Update(Answer model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update Answer set ");
			strSql.Append("QuestionId=@QuestionId,AnswerUserId=@AnswerUserId,AnswerContent=@AnswerContent,IsCanSee=@IsCanSee,CoinType=@CoinType,NeedCoin=@NeedCoin,AType=@AType,ReplyToUserID=@ReplyToUserID,ReplyTopAnswerId=@ReplyTopAnswerId,ReplyToAnswerID=@ReplyToAnswerID,CreateTime=@CreateTime,CreateUser=@CreateUser,UpdateTime=@UpdateTime,UpdateUser=@UpdateUser,IsDelete=@IsDelete");
		
			strSql.Append(" where AnswerId=@AnswerId");
			SqlParameter[] parameters = {
								        new SqlParameter("@QuestionId", model.QuestionId),
        new SqlParameter("@AnswerUserId", model.AnswerUserId),
        new SqlParameter("@AnswerContent", model.AnswerContent),
        new SqlParameter("@IsCanSee", model.IsCanSee),
        new SqlParameter("@CoinType", model.CoinType),
        new SqlParameter("@NeedCoin", model.NeedCoin),
        new SqlParameter("@AType", model.AType),
        new SqlParameter("@ReplyToUserID", model.ReplyToUserID),
        new SqlParameter("@ReplyTopAnswerId", model.ReplyTopAnswerId),
        new SqlParameter("@ReplyToAnswerID", model.ReplyToAnswerID),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@UpdateUser", model.UpdateUser),
        new SqlParameter("@IsDelete", model.IsDelete),

								new SqlParameter("@AnswerId", model.AnswerId)
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
			string sql="delete from Answer where AnswerId=@AnswerId";
			
			SqlParameter[] parameters = { new SqlParameter("@AnswerId", Id) };			

			return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
		}
		#endregion
		
		#region getmodel
		/// <summary>
		/// 查询单条数据
		/// </summary>
        public DataTable GetModel(long Id)
        {
            string sql = "SELECT AnswerId,QuestionId,AnswerUserId,AnswerContent,IsCanSee,CoinType,NeedCoin,AType,ReplyToUserID,ReplyTopAnswerId,ReplyToAnswerID,CreateTime,CreateUser,UpdateTime,UpdateUser,IsDelete FROM Answer WHERE AnswerId=@AnswerId and IsDelete=0";
			SqlParameter[] parameters = { new SqlParameter("@AnswerId", Id) };
			return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }
		#endregion

		#region query

		/// <summary>
		/// 记录总数
		/// </summary>
		public int Count()
        {
            string sql="select count(*) from Answer ";
            return (int)SqlHelper.GetSingle(sql);
        }


		/// <summary>
		/// 查询所有数据
		/// </summary>
		public DataTable GetList()
        {
            string sql = "SELECT AnswerId,QuestionId,AnswerUserId,AnswerContent,IsCanSee,CoinType,NeedCoin,AType,ReplyToUserID,ReplyTopAnswerId,ReplyToAnswerID,CreateTime,CreateUser,UpdateTime,UpdateUser,IsDelete FROM Answer ORDER BY AnswerId desc ";
            return SqlHelper.GetTable(sql);
        }

		/// <summary>
        /// 查询最新number 条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " AnswerId,QuestionId,AnswerUserId,AnswerContent,IsCanSee,CoinType,NeedCoin,AType,ReplyToUserID,ReplyTopAnswerId,ReplyToAnswerID,CreateTime,CreateUser,UpdateTime,UpdateUser,IsDelete FROM Answer ORDER BY AnswerId desc ";
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

            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY AnswerId desc  ) as rowid ,AnswerId,QuestionId,AnswerUserId,AnswerContent,IsCanSee,CoinType,NeedCoin,AType,ReplyToUserID,ReplyTopAnswerId,ReplyToAnswerID,CreateTime,CreateUser,UpdateTime,UpdateUser,IsDelete FROM Answer   ) ");

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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY AnswerId desc  ) as rowid ,AnswerId,QuestionId,AnswerUserId,AnswerContent,IsCanSee,CoinType,NeedCoin,AType,ReplyToUserID,ReplyTopAnswerId,ReplyToAnswerID,CreateTime,CreateUser,UpdateTime,UpdateUser,IsDelete FROM Answer   ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }

		#endregion

    }

}

