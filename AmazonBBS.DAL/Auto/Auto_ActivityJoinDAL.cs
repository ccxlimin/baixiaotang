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
    /// 活动报名表
    /// </summary>
    public class Auto_ActivityJoinDAL
    {
        #region add
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(ActivityJoin model, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into ActivityJoin(");
            strSql.Append(" JoinUserID,JoinUserName,FeeType,IsFeed,JoinTime,JoinCount,LinkMan,LinkTel,ActivityId,ActivityFeeId,RealPayFee )");
            strSql.Append(" values (");
            strSql.Append("@JoinUserID,@JoinUserName,@FeeType,@IsFeed,@JoinTime,@JoinCount,@LinkMan,@LinkTel,@ActivityId,@ActivityFeeId,@RealPayFee);select @@IDENTITY");
            SqlParameter[] parameters =
            {
                        new SqlParameter("@JoinUserID", model.JoinUserID),
        new SqlParameter("@JoinUserName", model.JoinUserName),
        new SqlParameter("@FeeType", model.FeeType),
        new SqlParameter("@IsFeed", model.IsFeed),
        new SqlParameter("@JoinTime", model.JoinTime),
        new SqlParameter("@JoinCount", model.JoinCount),
        new SqlParameter("@LinkMan", model.LinkMan),
        new SqlParameter("@LinkTel", model.LinkTel),
        new SqlParameter("@ActivityId", model.ActivityId),
        //new SqlParameter("@DIY1", model.DIY1),
        //new SqlParameter("@DIY2", model.DIY2),
        //new SqlParameter("@DIY3", model.DIY3),
        //new SqlParameter("@DIY4", model.DIY4),
        //new SqlParameter("@DIY5", model.DIY5),
        //new SqlParameter("@DIY6", model.DIY6),
        new SqlParameter("@ActivityFeeId", model.ActivityFeeId),
        new SqlParameter("@RealPayFee", model.RealPayFee),

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
        public bool Update(ActivityJoin model, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update ActivityJoin set ");
            strSql.Append("JoinUserID=@JoinUserID,JoinUserName=@JoinUserName,FeeType=@FeeType,IsFeed=@IsFeed,JoinTime=@JoinTime,JoinCount=@JoinCount,LinkMan=@LinkMan,LinkTel=@LinkTel,ActivityId=@ActivityId,ActivityFeeId=@ActivityFeeId,RealPayFee=@RealPayFee");
            strSql.Append(" where ActivityJoinId=@ActivityJoinId");
            SqlParameter[] parameters =
            {
                        new SqlParameter("@JoinUserID", model.JoinUserID),
        new SqlParameter("@JoinUserName", model.JoinUserName),
        new SqlParameter("@FeeType", model.FeeType),
        new SqlParameter("@IsFeed", model.IsFeed),
        new SqlParameter("@JoinTime", model.JoinTime),
        new SqlParameter("@JoinCount", model.JoinCount),
        new SqlParameter("@LinkMan", model.LinkMan),
        new SqlParameter("@LinkTel", model.LinkTel),
        new SqlParameter("@ActivityId", model.ActivityId),
        //new SqlParameter("@DIY1", model.DIY1),
        //new SqlParameter("@DIY2", model.DIY2),
        //new SqlParameter("@DIY3", model.DIY3),
        //new SqlParameter("@DIY4", model.DIY4),
        //new SqlParameter("@DIY5", model.DIY5),
        //new SqlParameter("@DIY6", model.DIY6),
        new SqlParameter("@ActivityFeeId", model.ActivityFeeId),
        new SqlParameter("@RealPayFee", model.RealPayFee),


                new SqlParameter("@ActivityJoinId", model.ActivityJoinId)
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
            string sql = "delete from ActivityJoin where ActivityJoinId=@ActivityJoinId";
            SqlParameter[] parameters = { new SqlParameter("@ActivityJoinId", Id) };
            return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        public DataTable GetModel<T>(T Id, SqlTransaction tran = null)
        {
            string sql = "SELECT * FROM ActivityJoin WHERE ActivityJoinId=@ActivityJoinId";
            SqlParameter[] parameters = { new SqlParameter("@ActivityJoinId", Id) };
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
            string sql = "select count(*) from ActivityJoin";
            return (int)SqlHelper.GetSingle(sql);
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        public DataTable GetList()
        {
            string sql = "SELECT * FROM ActivityJoin ORDER BY ActivityJoinId desc ";
            return SqlHelper.GetTable(sql);
        }

        /// <summary>
        /// 查询最新的N条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " * FROM ActivityJoin ORDER BY ActivityJoinId desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY ActivityJoinId desc) as rowid ,* FROM ActivityJoin) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY ActivityJoinId desc) as rowid ,* FROM ActivityJoin) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
        #endregion
    }
}
