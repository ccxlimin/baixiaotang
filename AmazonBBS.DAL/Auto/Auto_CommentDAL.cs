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
    /// 评论表
    /// </summary>
    public class Auto_CommentDAL
    {
        #region add
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(Comment model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Comment(");
            strSql.Append(" MainID,MainType,CommentUserID,CommentContent,IsHideOrFeeToSee,FeeCoinType,NeedCoin,CommentOrReplyType,ReplyTopCommentId,ReplyToUserID,ReplyToCommentID,CreateTime,CreateUser,UpdateTime,UpdateUser,IsDelete,IsAnonymous )");
            strSql.Append(" values (");
            strSql.Append("@MainID,@MainType,@CommentUserID,@CommentContent,@IsHideOrFeeToSee,@FeeCoinType,@NeedCoin,@CommentOrReplyType,@ReplyTopCommentId,@ReplyToUserID,@ReplyToCommentID,@CreateTime,@CreateUser,@UpdateTime,@UpdateUser,@IsDelete,@IsAnonymous)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                                    new SqlParameter("@MainID", model.MainID),
                                    new SqlParameter("@MainType", model.MainType),
        new SqlParameter("@CommentUserID", model.CommentUserID),
        new SqlParameter("@CommentContent", model.CommentContent),
        new SqlParameter("@IsHideOrFeeToSee", model.IsHideOrFeeToSee),
        new SqlParameter("@FeeCoinType", model.FeeCoinType),
        new SqlParameter("@NeedCoin", model.NeedCoin),
        new SqlParameter("@CommentOrReplyType", model.CommentOrReplyType),
        new SqlParameter("@ReplyTopCommentId", model.ReplyTopCommentId),
        new SqlParameter("@ReplyToUserID", model.ReplyToUserID),
        new SqlParameter("@ReplyToCommentID", model.ReplyToCommentID),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@UpdateUser", model.UpdateUser),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@IsAnonymous", model.IsAnonymous),

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
        public bool Update(Comment model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Comment set ");
            strSql.Append("MainID=@MainID,MainType=@MainType,CommentUserID=@CommentUserID,CommentContent=@CommentContent,IsHideOrFeeToSee=@IsHideOrFeeToSee,FeeCoinType=@FeeCoinType,NeedCoin=@NeedCoin,CommentOrReplyType=@CommentOrReplyType,ReplyTopCommentId=@ReplyTopCommentId,ReplyToUserID=@ReplyToUserID,ReplyToCommentID=@ReplyToCommentID,CreateTime=@CreateTime,CreateUser=@CreateUser,UpdateTime=@UpdateTime,UpdateUser=@UpdateUser,IsDelete=@IsDelete,IsAnonymous=@IsAnonymous");

            strSql.Append(" where CommentId=@CommentId");
            SqlParameter[] parameters = {
                                        new SqlParameter("@MainID", model.MainID),
                                        new SqlParameter("@MainType", model.MainType),
        new SqlParameter("@CommentUserID", model.CommentUserID),
        new SqlParameter("@CommentContent", model.CommentContent),
        new SqlParameter("@IsHideOrFeeToSee", model.IsHideOrFeeToSee),
        new SqlParameter("@FeeCoinType", model.FeeCoinType),
        new SqlParameter("@NeedCoin", model.NeedCoin),
        new SqlParameter("@CommentOrReplyType", model.CommentOrReplyType),
        new SqlParameter("@ReplyTopCommentId", model.ReplyTopCommentId),
        new SqlParameter("@ReplyToUserID", model.ReplyToUserID),
        new SqlParameter("@ReplyToCommentID", model.ReplyToCommentID),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@UpdateUser", model.UpdateUser),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@IsAnonymous", model.IsAnonymous),

                                new SqlParameter("@CommentId", model.CommentId)
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
            string sql = "delete from Comment where CommentId=@CommentId";
            SqlParameter[] parameters = { new SqlParameter("@CommentId", Id) };
            return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        public DataTable GetModel(long Id)
        {
            string sql = "SELECT * FROM Comment WHERE CommentId=@CommentId";
            SqlParameter[] parameters = { new SqlParameter("@CommentId", Id) };
            return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }
        #endregion

        #region query
        /// <summary>
        /// 记录总数
        /// </summary>
        public int Count()
        {
            string sql = "select count(*) from Comment ";
            return (int)SqlHelper.GetSingle(sql);
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        public DataTable GetList()
        {
            string sql = "SELECT * FROM Comment ORDER BY CommentId desc ";
            return SqlHelper.GetTable(sql);
        }

        /// <summary>
        /// 查询最新number 条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " * FROM Comment ORDER BY CommentId desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CommentId desc  ) as rowid ,* FROM Comment   ) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CommentId desc  ) as rowid ,* FROM Comment   ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
        #endregion
    }
}