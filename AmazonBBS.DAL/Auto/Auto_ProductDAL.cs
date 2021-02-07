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
    /// 产品服务
    /// </summary>
    public class Auto_ProductDAL
    {
        #region add
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(Product model, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Product(");
            strSql.Append(" PLogo,PTitle,PLocation,PDesc,PFunction,PPrice,PSize,CompanyName,CreateUser,CreateTime,IsDelete,PVCount,ProductPic,Contact,IsJinghua,IsRemen,IsTop,UpdateUser,UpdateTime,SendDay,PWeight,PUnit,ValidTime )");
            strSql.Append(" values (");
            strSql.Append("@PLogo,@PTitle,@PLocation,@PDesc,@PFunction,@PPrice,@PSize,@CompanyName,@CreateUser,@CreateTime,@IsDelete,@PVCount,@ProductPic,@Contact,@IsJinghua,@IsRemen,@IsTop,@UpdateUser,@UpdateTime,@SendDay,@PWeight,@PUnit,@ValidTime);select @@IDENTITY");
            SqlParameter[] parameters =
            {
                        new SqlParameter("@PLogo", model.PLogo),
        new SqlParameter("@PTitle", model.PTitle),
        new SqlParameter("@PLocation", model.PLocation),
        new SqlParameter("@PDesc", model.PDesc),
        new SqlParameter("@PFunction", model.PFunction),
        new SqlParameter("@PPrice", model.PPrice),
        new SqlParameter("@PSize", model.PSize),
        new SqlParameter("@CompanyName", model.CompanyName),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@PVCount", model.PVCount),
        new SqlParameter("@ProductPic", model.ProductPic),
        new SqlParameter("@Contact", model.Contact),
        new SqlParameter("@IsJinghua", model.IsJinghua),
        new SqlParameter("@IsRemen", model.IsRemen),
        new SqlParameter("@IsTop", model.IsTop),
        new SqlParameter("@UpdateUser", model.UpdateUser),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@SendDay", model.SendDay),
        new SqlParameter("@PWeight", model.PWeight),
        new SqlParameter("@PUnit", model.PUnit),
        new SqlParameter("@ValidTime", model.ValidTime),

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
        public bool Update(Product model, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Product set ");
            strSql.Append("PLogo=@PLogo,PTitle=@PTitle,PLocation=@PLocation,PDesc=@PDesc,PFunction=@PFunction,PPrice=@PPrice,PSize=@PSize,CompanyName=@CompanyName,CreateUser=@CreateUser,CreateTime=@CreateTime,IsDelete=@IsDelete,PVCount=@PVCount,ProductPic=@ProductPic,Contact=@Contact,IsJinghua=@IsJinghua,IsRemen=@IsRemen,IsTop=@IsTop,UpdateUser=@UpdateUser,UpdateTime=@UpdateTime,SendDay=@SendDay,PWeight=@PWeight,PUnit=@PUnit,ValidTime=@ValidTime");

            strSql.Append(" where ProductID=@ProductID");
            SqlParameter[] parameters =
            {
                        new SqlParameter("@PLogo", model.PLogo),
        new SqlParameter("@PTitle", model.PTitle),
        new SqlParameter("@PLocation", model.PLocation),
        new SqlParameter("@PDesc", model.PDesc),
        new SqlParameter("@PFunction", model.PFunction),
        new SqlParameter("@PPrice", model.PPrice),
        new SqlParameter("@PSize", model.PSize),
        new SqlParameter("@CompanyName", model.CompanyName),
        new SqlParameter("@CreateUser", model.CreateUser),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@IsDelete", model.IsDelete),
        new SqlParameter("@PVCount", model.PVCount),
        new SqlParameter("@ProductPic", model.ProductPic),
        new SqlParameter("@Contact", model.Contact),
        new SqlParameter("@IsJinghua", model.IsJinghua),
        new SqlParameter("@IsRemen", model.IsRemen),
        new SqlParameter("@IsTop", model.IsTop),
        new SqlParameter("@UpdateUser", model.UpdateUser),
        new SqlParameter("@UpdateTime", model.UpdateTime),
        new SqlParameter("@SendDay", model.SendDay),
        new SqlParameter("@PWeight", model.PWeight),
        new SqlParameter("@PUnit", model.PUnit),
        new SqlParameter("@ValidTime", model.ValidTime),


                new SqlParameter("@ProductID", model.ProductID)
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
            string sql = "delete from Product where ProductID=@ProductID";
            SqlParameter[] parameters = { new SqlParameter("@ProductID", Id) };
            return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        public DataTable GetModel<T>(T Id, SqlTransaction tran = null)
        {
            string sql = "SELECT * FROM Product WHERE ProductID=@ProductID and IsDelete=0 ";
            SqlParameter[] parameters = { new SqlParameter("@ProductID", Id) };
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
            string sql = "select count(*) from Product where IsDelete=0 ";
            return (int)SqlHelper.GetSingle(sql);
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        public DataTable GetList()
        {
            string sql = "SELECT * FROM Product where IsDelete=0  ORDER BY CreateTime desc ";
            return SqlHelper.GetTable(sql);
        }

        /// <summary>
        /// 查询最新的N条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " * FROM Product where IsDelete=0  ORDER BY CreateTime desc ";
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CreateTime desc) as rowid ,* FROM Product where IsDelete=0 ) ");
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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY CreateTime desc) as rowid ,* FROM Product where IsDelete=0 ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }
        #endregion
    }
}
