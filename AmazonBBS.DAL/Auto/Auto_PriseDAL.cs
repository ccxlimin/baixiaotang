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
    /// 点赞表
    /// </summary>
    public class Auto_PriseDAL
    {

        #region add
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(Prise model, SqlTransaction tran)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Prise(");
            strSql.Append(" TargetID,Type,UserID,PriseTime,IsDelete )");
            strSql.Append(" values (");
            strSql.Append("@TargetID,@Type,@UserID,@PriseTime,@IsDelete)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                                    new SqlParameter("@TargetID", model.TargetID),
        new SqlParameter("@Type", model.Type),
        new SqlParameter("@UserID", model.UserID),
        new SqlParameter("@PriseTime", model.PriseTime),
        new SqlParameter("@IsDelete", model.IsDelete),

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
        public bool Update(Prise model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Prise set ");
            strSql.Append("TargetID=@TargetID,Type=@Type,UserID=@UserID,PriseTime=@PriseTime,IsDelete=@IsDelete");

            strSql.Append(" where PriseId=@PriseId");
            SqlParameter[] parameters = {
                                        new SqlParameter("@TargetID", model.TargetID),
        new SqlParameter("@Type", model.Type),
        new SqlParameter("@UserID", model.UserID),
        new SqlParameter("@PriseTime", model.PriseTime),
        new SqlParameter("@IsDelete", model.IsDelete),

                                new SqlParameter("@PriseId", model.PriseId)
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
            string sql = "delete from Prise where PriseId=@PriseId";

            SqlParameter[] parameters = { new SqlParameter("@PriseId", Id) };

            return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        public DataTable GetModel(long Id)
        {
            string sql = "SELECT PriseId,TargetID,Type,UserID,PriseTime,IsDelete FROM Prise WHERE PriseId=@PriseId";
            SqlParameter[] parameters = { new SqlParameter("@PriseId", Id) };
            return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }
        #endregion

        #region query

        /// <summary>
        /// 记录总数
        /// </summary>
        public int Count()
        {
            string sql = "select count(*) from Prise ";
            return (int)SqlHelper.GetSingle(sql);
        }


        /// <summary>
        /// 查询所有数据
        /// </summary>
        public DataTable GetList()
        {
            string sql = "SELECT PriseId,TargetID,Type,UserID,PriseTime,IsDelete FROM Prise ORDER BY PriseId desc ";
            return SqlHelper.GetTable(sql);
        }

        /// <summary>
        /// 查询最新number 条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " PriseId,TargetID,Type,UserID,PriseTime,IsDelete FROM Prise ORDER BY PriseId desc ";
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

            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY PriseId desc  ) as rowid ,PriseId,TargetID,Type,UserID,PriseTime,IsDelete FROM Prise   ) ");

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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY PriseId desc  ) as rowid ,PriseId,TargetID,Type,UserID,PriseTime,IsDelete FROM Prise   ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }

        #endregion

    }

}

