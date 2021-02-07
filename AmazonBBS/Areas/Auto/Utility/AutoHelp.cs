using AmazonBBS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AmazonBBS.Areas.Auto.Utility
{
    public class AutoHelp
    {

        /// <summary>
        /// 获取表中所有字段
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public DataTable GetField(string tableName)
        {
            string sql = "SELECT COLUMN_NAME,IS_NULLABLE,DATA_TYPE FROM INFORMATION_SCHEMA.columns WHERE TABLE_NAME=@table";

            SqlParameter[] parameters =
            {
                new SqlParameter("@table",tableName)
            };

            return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 获取表中的主键
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public DataTable GetPk(string tableName)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@table_name",tableName),
                 new SqlParameter("@table_owner", null),
                  new SqlParameter("@table_qualifier", null),
            };

            return SqlHelper.GetTable("sp_pkeys", CommandType.StoredProcedure, parameters);
        }

        /// <summary>
        /// 判断某表中的某列是否为标识列(自增)
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool IsIdentityField(string tableName, string fieldName)
        {
            return new SqlQuickBuild("SELECT COLUMNPROPERTY( OBJECT_ID(@tableName),@fieldName,'IsIdentity')")
                .AddParams("@tableName", SqlDbType.VarChar, tableName)
                .AddParams("@fieldName", SqlDbType.VarChar, fieldName)
                .GetSingleStr().ToInt32() > 0;
        }

        /// <summary>
        /// 查询某表中的标识列
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetIdentityField(string tableName)
        {
            return new SqlQuickBuild(@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.columns
   WHERE TABLE_NAME=@tableName AND  COLUMNPROPERTY(      
      OBJECT_ID(@tableName),COLUMN_NAME,'IsIdentity')=1").AddParams("@tableName", SqlDbType.VarChar, tableName).GetSingleStr();
        }

        /// <summary>
        /// 获取所有表名称
        /// </summary>
        /// <returns></returns>
        public DataTable GetTable()
        {
            string sql = "SELECT Name FROM SysObjects Where XType='U' ORDER BY Name";
            return SqlHelper.GetTable(sql, CommandType.Text);
        }

        /// <summary>
        /// 获取字段备注
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public DataTable GetDescriptions(string tableName)
        {
            string sql = @"SELECT     [Table_Name] = OBJECT_NAME(c.object_id),     [Column_Name] = c.name,     [Descriptions] = ex.value FROM 
                                sys.columns c 
                            LEFT OUTER JOIN 
                                sys.extended_properties ex 
                            ON 
                                ex.major_id = c.object_id 
                                AND ex.minor_id = c.column_id 
                                AND ex.name = 'MS_Description' 
                            WHERE 
                                OBJECTPROPERTY(c.object_id, 'IsMsShipped')=0 
                                 AND OBJECT_NAME(c.object_id) = @table
                            ORDER 
                                BY OBJECT_NAME(c.object_id), c.column_id";

            SqlParameter[] parameters =
            {
                new SqlParameter("@table",tableName)
            };

            return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 获取表的说明
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public DataTable GetTableDesc(string tableName)
        {
            string sql = @"SELECT tbs.name as tableName,ds.value AS contents 
                            FROM sys.extended_properties ds  
                            LEFT JOIN sysobjects tbs ON ds.major_id=tbs.id  
                            WHERE  ds.minor_id=0 AND tbs.name=@table";

            SqlParameter[] parameters =
            {
                new SqlParameter("@table",tableName)
            };
            return SqlHelper.GetTable(sql, CommandType.Text, parameters);
        }

    }
}