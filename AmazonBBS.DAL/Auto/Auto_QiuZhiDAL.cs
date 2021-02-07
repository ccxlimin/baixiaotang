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
    /// 人才求职
    /// </summary>
    public class Auto_QiuZhiDAL
    {
        #region add
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(QiuZhi model, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into QiuZhi(");
            strSql.Append(" Publisher,IWant,Money,NowWork,WorkStatus,Study,WorkYear,MyDesc,Contact,CreateTime,IsDelete,SelfAssessment,JianLiPic,PayType,IsPay,JianLiWord,PVCount,IsJinghua,IsRemen,IsTop,WorkType,WorkTime,BelongJobTrade,BelongJob,IWantPlace,ValidTime,UpdateTime,UpdateUser )");
            strSql.Append(" values (");
            strSql.Append("@Publisher,@IWant,@Money,@NowWork,@WorkStatus,@Study,@WorkYear,@MyDesc,@Contact,@CreateTime,@IsDelete,@SelfAssessment,@JianLiPic,@PayType,@IsPay,@JianLiWord,@PVCount,@IsJinghua,@IsRemen,@IsTop,@WorkType,@WorkTime,@BelongJobTrade,@BelongJob,@IWantPlace,@ValidTime,@UpdateTime,@UpdateUser);select @@IDENTITY");
            SqlParameter[] parameters =
            {
                        new SqlParameter("@Publisher", model.Publisher),
        new SqlParameter("@IWant", model.IWant),
        new SqlParameter("@Money", model.Money),
        new SqlParameter("@NowWork", model.NowWork),
        new SqlParameter("@WorkStatus", model.WorkStatus),
        new SqlParameter("@Study", model.Study),
        new SqlParameter("@WorkYear", model.WorkYear),
        new SqlParameter("@MyDesc", model.MyDesc),
        new SqlParameter("@Contact", model.Contact),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@SelfAssessment", model.SelfAssessment),
        new SqlParameter("@JianLiPic", model.JianLiPic),
        new SqlParameter("@PayType", model.PayType),
        new SqlParameter("@IsPay", model.IsPay),
        new SqlParameter("@JianLiWord", model.JianLiWord),
        new SqlParameter("@PVCount", model.PVCount),
        new SqlParameter("@IsJinghua", model.IsJinghua),
        new SqlParameter("@IsRemen", model.IsRemen),
        new SqlParameter("@IsTop", model.IsTop),
        new SqlParameter("@WorkType", model.WorkType),
        new SqlParameter("@WorkTime", model.WorkTime),
        new SqlParameter("@BelongJobTrade", model.BelongJobTrade),
        new SqlParameter("@BelongJob", model.BelongJob),
        new SqlParameter("@IWantPlace", model.IWantPlace),
        new SqlParameter("@ValidTime", model.ValidTime),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@UpdateUser", model.UpdateUser),

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
        public bool Update(QiuZhi model, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update QiuZhi set ");
            strSql.Append("Publisher=@Publisher,IWant=@IWant,Money=@Money,NowWork=@NowWork,WorkStatus=@WorkStatus,Study=@Study,WorkYear=@WorkYear,MyDesc=@MyDesc,Contact=@Contact,CreateTime=@CreateTime,IsDelete=@IsDelete,SelfAssessment=@SelfAssessment,JianLiPic=@JianLiPic,PayType=@PayType,IsPay=@IsPay,JianLiWord=@JianLiWord,PVCount=@PVCount,IsJinghua=@IsJinghua,IsRemen=@IsRemen,IsTop=@IsTop,WorkType=@WorkType,WorkTime=@WorkTime,BelongJobTrade=@BelongJobTrade,BelongJob=@BelongJob,IWantPlace=@IWantPlace,ValidTime=@ValidTime,UpdateTime=@UpdateTime,UpdateUser=@UpdateUser");

            strSql.Append(" where QiuZhiID=@QiuZhiID");
            SqlParameter[] parameters =
            {
                        new SqlParameter("@Publisher", model.Publisher),
        new SqlParameter("@IWant", model.IWant),
        new SqlParameter("@Money", model.Money),
        new SqlParameter("@NowWork", model.NowWork),
        new SqlParameter("@WorkStatus", model.WorkStatus),
        new SqlParameter("@Study", model.Study),
        new SqlParameter("@WorkYear", model.WorkYear),
        new SqlParameter("@MyDesc", model.MyDesc),
        new SqlParameter("@Contact", model.Contact),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@SelfAssessment", model.SelfAssessment),
        new SqlParameter("@JianLiPic", model.JianLiPic),
        new SqlParameter("@PayType", model.PayType),
        new SqlParameter("@IsPay", model.IsPay),
        new SqlParameter("@JianLiWord", model.JianLiWord),
        new SqlParameter("@PVCount", model.PVCount),
        new SqlParameter("@IsJinghua", model.IsJinghua),
        new SqlParameter("@IsRemen", model.IsRemen),
        new SqlParameter("@IsTop", model.IsTop),
        new SqlParameter("@WorkType", model.WorkType),
        new SqlParameter("@WorkTime", model.WorkTime),
        new SqlParameter("@BelongJobTrade", model.BelongJobTrade),
        new SqlParameter("@BelongJob", model.BelongJob),
        new SqlParameter("@IWantPlace", model.IWantPlace),
        new SqlParameter("@ValidTime", model.ValidTime),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@UpdateUser", model.UpdateUser),


                new SqlParameter("@QiuZhiID", model.QiuZhiID)
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
            string sql = "delete from QiuZhi where QiuZhiID=@QiuZhiID";
            SqlParameter[] parameters = { new SqlParameter("@QiuZhiID", Id) };
            return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        public DataTable GetModel<T>(T Id, SqlTransaction tran = null)
        {
            string sql = "SELECT * FROM QiuZhi WHERE QiuZhiID=@QiuZhiID and IsDelete=0 ";
            SqlParameter[] parameters = { new SqlParameter("@QiuZhiID", Id) };
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
            string sql = "select count(*) from QiuZhi where IsDelete=0 ";
            return (int)SqlHelper.GetSingle(sql);
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        public DataTable GetList()
        {
            string sql = "SELECT * FROM QiuZhi where IsDelete=0  ORDER BY CreateTime desc ";
            return SqlHelper.GetTable(sql);
        }

        /// <summary>
        /// 查询最新的N条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " * FROM QiuZhi where IsDelete=0  ORDER BY CreateTime desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CreateTime desc) as rowid ,* FROM QiuZhi where IsDelete=0 ) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CreateTime desc) as rowid ,* FROM QiuZhi where IsDelete=0 ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
        #endregion
    }
}
