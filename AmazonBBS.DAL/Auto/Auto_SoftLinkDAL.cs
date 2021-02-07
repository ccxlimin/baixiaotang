﻿using System;
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
    /// 软件导航
    /// </summary>
    public class Auto_SoftLinkDAL
    {

        #region add
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(SoftLink model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into SoftLink(");
            strSql.Append(" SoftLinkType,LinkName,LinkAddress,LinkMemo,CreateTime,IsDelete )");
            strSql.Append(" values (");
            strSql.Append("@SoftLinkType,@LinkName,@LinkAddress,@LinkMemo,@CreateTime,@IsDelete)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                                    new SqlParameter("@SoftLinkType", model.SoftLinkType),
        new SqlParameter("@LinkName", model.LinkName),
        new SqlParameter("@LinkAddress", model.LinkAddress),
        new SqlParameter("@LinkMemo", model.LinkMemo),
        new SqlParameter("@CreateTime", model.CreateTime),
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
        public bool Update(SoftLink model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update SoftLink set ");
            strSql.Append("SoftLinkType=@SoftLinkType,LinkName=@LinkName,LinkAddress=@LinkAddress,LinkMemo=@LinkMemo,CreateTime=@CreateTime,IsDelete=@IsDelete");

            strSql.Append(" where SoftLinkID=@SoftLinkID");
            SqlParameter[] parameters = {
                                        new SqlParameter("@SoftLinkType", model.SoftLinkType),
        new SqlParameter("@LinkName", model.LinkName),
        new SqlParameter("@LinkAddress", model.LinkAddress),
        new SqlParameter("@LinkMemo", model.LinkMemo),
        new SqlParameter("@CreateTime", model.CreateTime),
        new SqlParameter("@IsDelete", model.IsDelete),

                                new SqlParameter("@SoftLinkID", model.SoftLinkID)
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
            string sql = "delete from SoftLink where SoftLinkID=@SoftLinkID";

            SqlParameter[] parameters = { new SqlParameter("@SoftLinkID", Id) };

            return SqlHelper.ExecuteSql(sql, CommandType.Text, parameters) > 0;
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        public DataTable GetModel(long Id)
        {
            string sql = "SELECT SoftLinkID,SoftLinkType,LinkName,LinkAddress,LinkMemo,CreateTime,IsDelete FROM SoftLink WHERE SoftLinkID=@SoftLinkID and IsDelete=0";
            SqlParameter[] parameters = { new SqlParameter("@SoftLinkID", Id) };
            return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }
        #endregion

        #region query

        /// <summary>
        /// 记录总数
        /// </summary>
        public int Count()
        {
            string sql = "select count(*) from SoftLink ";
            return (int)SqlHelper.GetSingle(sql);
        }


        /// <summary>
        /// 查询所有数据
        /// </summary>
        public DataTable GetList()
        {
            string sql = "SELECT SoftLinkID,SoftLinkType,LinkName,LinkAddress,LinkMemo,CreateTime,IsDelete FROM SoftLink where IsDelete=0 ORDER BY SoftLinkID desc ";
            return SqlHelper.GetTable(sql);
        }

        /// <summary>
        /// 查询最新number 条数据
        /// </summary>
        /// <param name="number">最新N条</param>
        /// <returns></returns>
        public DataTable GetList(int number)
        {
            string sql = "SELECT top " + number + " SoftLinkID,SoftLinkType,LinkName,LinkAddress,LinkMemo,CreateTime,IsDelete FROM SoftLink ORDER BY SoftLinkID desc ";
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

            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY SoftLinkID desc  ) as rowid ,SoftLinkID,SoftLinkType,LinkName,LinkAddress,LinkMemo,CreateTime,IsDelete FROM SoftLink   ) ");

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
            sb.Append(" select * from  (SELECT ROW_NUMBER() OVER(ORDER BY SoftLinkID desc  ) as rowid ,SoftLinkID,SoftLinkType,LinkName,LinkAddress,LinkMemo,CreateTime,IsDelete FROM SoftLink   ) ");
            sb.Append("  t where t.rowid between " + startIndex + " and " + endIndex);
            return SqlHelper.GetTable(sb.ToString());
        }

        #endregion

    }

}
