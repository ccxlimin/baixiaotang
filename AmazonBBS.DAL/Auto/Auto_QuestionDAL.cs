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
    /// 提问表
    /// </summary>
    public class Auto_QuestionDAL
    {

        #region add
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(Question model, SqlTransaction tran)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Question(");
            strSql.Append(" UserID,Title,Body,Coin,CoinType,PVCount,TopicID,CreateTime,CreateUser,UpdateTime,UpdateUser,IsDelete,BestAnswerId,IsJinghua,IsRemen,IsTop,IsChecked,EditCount,FilePath,ContentNeedPay,ContentFee,ContentFeeType,IsAnonymous )");
            strSql.Append(" values (");
            strSql.Append("@UserID,@Title,@Body,@Coin,@CoinType,@PVCount,@TopicID,@CreateTime,@CreateUser,@UpdateTime,@UpdateUser,@IsDelete,@BestAnswerId,@IsJinghua,@IsRemen,@IsTop,@IsChecked,@EditCount,@FilePath,@ContentNeedPay,@ContentFee,@ContentFeeType,@IsAnonymous)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                                    new SqlParameter("@UserID", model.UserID),
        new SqlParameter("@Title", model.Title),
        new SqlParameter("@Body", model.Body),
        new SqlParameter("@Coin", model.Coin),
        new SqlParameter("@CoinType", model.CoinType),
        new SqlParameter("@PVCount", model.PVCount),
        new SqlParameter("@TopicID",model.TopicID),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@UpdateUser", model.UpdateUser),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@BestAnswerId", model.BestAnswerId),
        new SqlParameter("@IsJinghua", model.IsJinghua),
        new SqlParameter("@IsRemen", model.IsRemen),
        new SqlParameter("@IsTop", model.IsTop),
        new SqlParameter("@IsChecked", model.IsChecked),
        new SqlParameter("@EditCount", model.EditCount),
        new SqlParameter("@FilePath", model.FilePath),
        new SqlParameter("@ContentNeedPay", model.ContentNeedPay),
        new SqlParameter("@ContentFee", model.ContentFee),
        new SqlParameter("@ContentFeeType", model.ContentFeeType),
        new SqlParameter("@IsAnonymous", model.IsAnonymous),

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
        public bool Update(Question model, SqlTransaction tran)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Question set ");
            strSql.Append("UserID=@UserID,Title=@Title,Body=@Body,Coin=@Coin,CoinType=@CoinType,PVCount=@PVCount,TopicID=@TopicID,CreateTime=@CreateTime,CreateUser=@CreateUser,UpdateTime=@UpdateTime,UpdateUser=@UpdateUser,IsDelete=@IsDelete,BestAnswerId=@BestAnswerId,IsJinghua=@IsJinghua,IsRemen=@IsRemen,IsTop=@IsTop,IsChecked=@IsChecked,EditCount=@EditCount,FilePath=@FilePath,ContentNeedPay=@ContentNeedPay,ContentFee=@ContentFee,ContentFeeType=@ContentFeeType,IsAnonymous=@IsAnonymous ");

            strSql.Append(" where QuestionId=@QuestionId");
            SqlParameter[] parameters = {
                                        new SqlParameter("@UserID", model.UserID),
        new SqlParameter("@Title", model.Title),
        new SqlParameter("@Body", model.Body),
        new SqlParameter("@Coin", model.Coin),
        new SqlParameter("@CoinType", model.CoinType),
        new SqlParameter("@PVCount", model.PVCount),
        new SqlParameter("@TopicID",model.TopicID),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@UpdateUser", model.UpdateUser),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@BestAnswerId", model.BestAnswerId),
          new SqlParameter("@IsJinghua", model.IsJinghua),
        new SqlParameter("@IsRemen", model.IsRemen),
        new SqlParameter("@IsTop", model.IsTop),
        new SqlParameter("@IsChecked", model.IsChecked),
        new SqlParameter("@EditCount", model.EditCount),
        new SqlParameter("@FilePath", model.FilePath),
        new SqlParameter("@ContentNeedPay", model.ContentNeedPay),
        new SqlParameter("@ContentFee", model.ContentFee),
        new SqlParameter("@ContentFeeType", model.ContentFeeType),
        new SqlParameter("@IsAnonymous", model.IsAnonymous),

                                new SqlParameter("@QuestionId", model.QuestionId)
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
            string sql = "delete from Question where QuestionId=@QuestionId";

            SqlParameter[] parameters = { new SqlParameter("@QuestionId", Id) };

            return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        public DataTable GetModel(long Id)
        {
            string sql = "SELECT * FROM Question WHERE QuestionId=@QuestionId and IsDelete=0";
            SqlParameter[] parameters = { new SqlParameter("@QuestionId", Id) };
            return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }
        #endregion

        #region query

        /// <summary>
        /// 记录总数
        /// </summary>
        public int Count()
        {
            string sql = "select count(*) from Question ";
            return (int)SqlHelper.GetSingle(sql);
        }


        /// <summary>
        /// 查询所有数据
        /// </summary>
        public DataTable GetList()
        {
            string sql = "SELECT * FROM Question ORDER BY QuestionId desc ";
            return SqlHelper.GetTable(sql);
        }

        /// <summary>
        /// 查询最新number 条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " * FROM Question ORDER BY QuestionId desc ";
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

            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY QuestionId desc  ) as rowid ,* FROM Question   ) ");

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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY QuestionId desc  ) as rowid,* FROM Question   ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }

        #endregion

    }

}

