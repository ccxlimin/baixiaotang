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
    /// 人才招聘
    /// </summary>
    public class Auto_ZhaoPinDAL
    {
        #region add
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(ZhaoPin model, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into ZhaoPin(");
            strSql.Append(" Publisher,Gangwei,Money,Study,WorkHistory,WorkPlace,CName,CLogo,CPeople,CVip,Contact,CreateTime,IsDelete,JobWord,JobPic,PayType,IsPay,CDesc,PVCount,JobRequire,IsJinghua,IsRemen,IsTop,WorkeType,WorkTime,BelongJobTrade,BelongJob,JobFuLi,NeedCount,ValidTime,UpdateTime,UpdateUser )");
            strSql.Append(" values (");
            strSql.Append("@Publisher,@Gangwei,@Money,@Study,@WorkHistory,@WorkPlace,@CName,@CLogo,@CPeople,@CVip,@Contact,@CreateTime,@IsDelete,@JobWord,@JobPic,@PayType,@IsPay,@CDesc,@PVCount,@JobRequire,@IsJinghua,@IsRemen,@IsTop,@WorkeType,@WorkTime,@BelongJobTrade,@BelongJob,@JobFuLi,@NeedCount,@ValidTime,@UpdateTime,@UpdateUser);select @@IDENTITY");
            SqlParameter[] parameters =
            {
                        new SqlParameter("@Publisher", model.Publisher),
        new SqlParameter("@Gangwei", model.Gangwei),
        new SqlParameter("@Money", model.Money),
        new SqlParameter("@Study", model.Study),
        new SqlParameter("@WorkHistory", model.WorkHistory),
        new SqlParameter("@WorkPlace", model.WorkPlace),
        new SqlParameter("@CName", model.CName),
        new SqlParameter("@CLogo", model.CLogo),
        new SqlParameter("@CPeople", model.CPeople),
        new SqlParameter("@CVip", model.CVip),
        new SqlParameter("@Contact", model.Contact),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@JobWord", model.JobWord),
        new SqlParameter("@JobPic", model.JobPic),
        new SqlParameter("@PayType", model.PayType),
        new SqlParameter("@IsPay", model.IsPay),
        new SqlParameter("@CDesc", model.CDesc),
        new SqlParameter("@PVCount", model.PVCount),
        new SqlParameter("@JobRequire", model.JobRequire),
        new SqlParameter("@IsJinghua", model.IsJinghua),
        new SqlParameter("@IsRemen", model.IsRemen),
        new SqlParameter("@IsTop", model.IsTop),
        new SqlParameter("@WorkeType", model.WorkeType),
        new SqlParameter("@WorkTime", model.WorkTime),
        new SqlParameter("@BelongJobTrade", model.BelongJobTrade),
        new SqlParameter("@BelongJob", model.BelongJob),
        new SqlParameter("@JobFuLi", model.JobFuLi),
        new SqlParameter("@NeedCount", model.NeedCount),
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
        public bool Update(ZhaoPin model, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update ZhaoPin set ");
            strSql.Append("Publisher=@Publisher,Gangwei=@Gangwei,Money=@Money,Study=@Study,WorkHistory=@WorkHistory,WorkPlace=@WorkPlace,CName=@CName,CLogo=@CLogo,CPeople=@CPeople,CVip=@CVip,Contact=@Contact,CreateTime=@CreateTime,IsDelete=@IsDelete,JobWord=@JobWord,JobPic=@JobPic,PayType=@PayType,IsPay=@IsPay,CDesc=@CDesc,PVCount=@PVCount,JobRequire=@JobRequire,IsJinghua=@IsJinghua,IsRemen=@IsRemen,IsTop=@IsTop,WorkeType=@WorkeType,WorkTime=@WorkTime,BelongJobTrade=@BelongJobTrade,BelongJob=@BelongJob,JobFuLi=@JobFuLi,NeedCount=@NeedCount,ValidTime=@ValidTime,UpdateTime=@UpdateTime,UpdateUser=@UpdateUser");

            strSql.Append(" where ZhaoPinID=@ZhaoPinID");
            SqlParameter[] parameters =
            {
                        new SqlParameter("@Publisher", model.Publisher),
        new SqlParameter("@Gangwei", model.Gangwei),
        new SqlParameter("@Money", model.Money),
        new SqlParameter("@Study", model.Study),
        new SqlParameter("@WorkHistory", model.WorkHistory),
        new SqlParameter("@WorkPlace", model.WorkPlace),
        new SqlParameter("@CName", model.CName),
        new SqlParameter("@CLogo", model.CLogo),
        new SqlParameter("@CPeople", model.CPeople),
        new SqlParameter("@CVip", model.CVip),
        new SqlParameter("@Contact", model.Contact),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@JobWord", model.JobWord),
        new SqlParameter("@JobPic", model.JobPic),
        new SqlParameter("@PayType", model.PayType),
        new SqlParameter("@IsPay", model.IsPay),
        new SqlParameter("@CDesc", model.CDesc),
        new SqlParameter("@PVCount", model.PVCount),
        new SqlParameter("@JobRequire", model.JobRequire),
        new SqlParameter("@IsJinghua", model.IsJinghua),
        new SqlParameter("@IsRemen", model.IsRemen),
        new SqlParameter("@IsTop", model.IsTop),
        new SqlParameter("@WorkeType", model.WorkeType),
        new SqlParameter("@WorkTime", model.WorkTime),
        new SqlParameter("@BelongJobTrade", model.BelongJobTrade),
        new SqlParameter("@BelongJob", model.BelongJob),
        new SqlParameter("@JobFuLi", model.JobFuLi),
        new SqlParameter("@NeedCount", model.NeedCount),
        new SqlParameter("@ValidTime", model.ValidTime),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@UpdateUser", model.UpdateUser),


                new SqlParameter("@ZhaoPinID", model.ZhaoPinID)
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
            string sql = "delete from ZhaoPin where ZhaoPinID=@ZhaoPinID";
            SqlParameter[] parameters = { new SqlParameter("@ZhaoPinID", Id) };
            return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        public DataTable GetModel<T>(T Id, SqlTransaction tran = null)
        {
            string sql = "SELECT * FROM ZhaoPin WHERE ZhaoPinID=@ZhaoPinID and IsDelete=0 ";
            SqlParameter[] parameters = { new SqlParameter("@ZhaoPinID", Id) };
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
            string sql = "select count(*) from ZhaoPin where IsDelete=0 ";
            return (int)SqlHelper.GetSingle(sql);
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        public DataTable GetList()
        {
            string sql = "SELECT * FROM ZhaoPin where IsDelete=0  ORDER BY CreateTime desc ";
            return SqlHelper.GetTable(sql);
        }

        /// <summary>
        /// 查询最新的N条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " * FROM ZhaoPin where IsDelete=0  ORDER BY CreateTime desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CreateTime desc) as rowid ,* FROM ZhaoPin where IsDelete=0 ) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CreateTime desc) as rowid ,* FROM ZhaoPin where IsDelete=0 ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
        #endregion
    }
}
