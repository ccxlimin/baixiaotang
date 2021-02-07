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
    /// 聊天记录
    /// </summary>
    public class Auto_ChatDAL
    {

        #region add
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(Chat model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Chat(");
            strSql.Append(" FromID,FromUserName,ToID,ToUserName,Message,SendTime,IsRead,ReadTime )");
            strSql.Append(" values (");
            strSql.Append("@FromID,@FromUserName,@ToID,@ToUserName,@Message,@SendTime,@IsRead,@ReadTime)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                                    new SqlParameter("@FromID", model.FromID),
        new SqlParameter("@FromUserName", model.FromUserName),
        new SqlParameter("@ToID", model.ToID),
        new SqlParameter("@ToUserName", model.ToUserName),
        new SqlParameter("@Message", model.Message),
        new SqlParameter("@SendTime", model.SendTime),
        new SqlParameter("@IsRead", model.IsRead),
        new SqlParameter("@ReadTime", model.ReadTime),

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
        public bool Update(Chat model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Chat set ");
            strSql.Append("FromID=@FromID,FromUserName=@FromUserName,ToID=@ToID,ToUserName=@ToUserName,Message=@Message,SendTime=@SendTime,IsRead=@IsRead,ReadTime=@ReadTime");

            strSql.Append(" where ChatID=@ChatID");
            SqlParameter[] parameters = {
                                        new SqlParameter("@FromID", model.FromID),
        new SqlParameter("@FromUserName", model.FromUserName),
        new SqlParameter("@ToID", model.ToID),
        new SqlParameter("@ToUserName", model.ToUserName),
        new SqlParameter("@Message", model.Message),
        new SqlParameter("@SendTime", model.SendTime),
        new SqlParameter("@IsRead", model.IsRead),
        new SqlParameter("@ReadTime", model.ReadTime),

                                new SqlParameter("@ChatID", model.ChatID)
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
            string sql = "delete from Chat where ChatID=@ChatID";

            SqlParameter[] parameters = { new SqlParameter("@ChatID", Id) };

            return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        public DataTable GetModel(long Id)
        {
            string sql = "SELECT ChatID,FromID,FromUserName,ToID,ToUserName,Message,SendTime,IsRead,ReadTime FROM Chat WHERE ChatID=@ChatID";
            SqlParameter[] parameters = { new SqlParameter("@ChatID", Id) };
            return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }

        public string SendMsgCount(long userID, DateTime startTime, DateTime endTime)
        {
            return new SqlQuickBuild(@"select count(1) from Chat where SendTime between @startTime and @endTime and FromID=@fromID")
                .AddParams("@fromID", SqlDbType.BigInt, userID)
                .AddParams("@startTime", SqlDbType.DateTime, startTime)
                .AddParams("@endTime", SqlDbType.DateTime, endTime)
                .GetSingleStr();
        }
        #endregion

        #region query

        /// <summary>
        /// 记录总数
        /// </summary>
        public int Count()
        {
            string sql = "select count(*) from Chat ";
            return (int)SqlHelper.GetSingle(sql);
        }


        /// <summary>
        /// 查询所有数据
        /// </summary>
        public DataTable GetList()
        {
            string sql = "SELECT ChatID,FromID,FromUserName,ToID,ToUserName,Message,SendTime,IsRead,ReadTime FROM Chat ORDER BY ChatID desc ";
            return SqlHelper.GetTable(sql);
        }

        /// <summary>
        /// 查询最新number 条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " ChatID,FromID,FromUserName,ToID,ToUserName,Message,SendTime,IsRead,ReadTime FROM Chat ORDER BY ChatID desc ";
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

            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY ChatID desc  ) as rowid ,ChatID,FromID,FromUserName,ToID,ToUserName,Message,SendTime,IsRead,ReadTime FROM Chat   ) ");

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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY ChatID desc  ) as rowid ,ChatID,FromID,FromUserName,ToID,ToUserName,Message,SendTime,IsRead,ReadTime FROM Chat   ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }

        #endregion

    }

}

