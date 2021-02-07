using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AmazonBBS.Common
{
    public class SqlHelper
    {
        public static readonly string DefaultConnectionString = ConfigurationManager.ConnectionStrings["AmazonBBSConnection"].ConnectionString;
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        #region ExecuteSql

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static int ExecuteSql(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, cmdType, cmdText, cmdParms);
                int val = ExecuteNonQuery(cmd);
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static int ExecuteSql(SqlConnection sqlConnection, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, sqlConnection, null, cmdType, cmdText, cmdParms);
            int val = ExecuteNonQuery(cmd);
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static int ExecuteSql(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            int val = ExecuteNonQuery(cmd);
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static int ExecuteSql(string cmdText, CommandType commandType = CommandType.Text)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection sqlConnection = new SqlConnection(DefaultConnectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, commandType, cmdText, null);
                return ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandType"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static int ExecuteSql(string cmdText, CommandType commandType = CommandType.Text, params SqlParameter[] pars)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection sqlConnection = new SqlConnection(DefaultConnectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, commandType, cmdText, pars);
                return ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static int ExecuteSql(string cmdText, params SqlParameter[] pars)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection sqlConnection = new SqlConnection(DefaultConnectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, CommandType.Text, cmdText, pars);
                return ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="conStr"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static int ExecuteSql(string connectionString, string cmdText, params SqlParameter[] pars)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, CommandType.Text, cmdText, pars);
                return ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 批量执行SQL语句（带参数）--对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static bool ExecuteSql_Batch(SqlTransaction tran, IEnumerable<KeyValuePair<string, SqlParameter[]>> pairs)
        {
            bool result = true;
            SqlConnection conn = new SqlConnection(DefaultConnectionString);
            using (conn)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = tran.Connection;
                conn.Open();
                //SqlTransaction trans = conn.BeginTransaction();
                cmd.Transaction = tran;
                try
                {
                    foreach (var item in pairs)
                    {
                        cmd.CommandText = item.Key;
                        cmd.Parameters.AddRange(item.Value);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }
        /// <summary>
        /// 批量执行SQL语句（带参数）--对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static bool ExecuteSql_Batch(IEnumerable<KeyValuePair<string, SqlParameter[]>> pairs)
        {
            bool result = true;
            SqlConnection conn = new SqlConnection(DefaultConnectionString);
            using (conn)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                cmd.Transaction = trans;
                try
                {
                    foreach (var item in pairs)
                    {
                        cmd.CommandText = item.Key;
                        cmd.Parameters.AddRange(item.Value);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    result = false;
                }
                finally
                {
                    trans.Dispose();
                }
            }
            return result;
        }
        #endregion

        #region GetSingle

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object GetSingle(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                object val = ExecuteScalar(cmd);
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object GetSingle(SqlConnection sqlConnection, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, sqlConnection, null, cmdType, cmdText, cmdParms);
            object val = ExecuteScalar(cmd);
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="myTran"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object GetSingle(SqlTransaction myTran, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, myTran.Connection, myTran, cmdType, cmdText, cmdParms);
            object val = ExecuteScalar(cmd);
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object GetSingle(string connectionString, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }
                cmd.Connection = sqlConnection;
                if (cmdParms != null)
                {
                    foreach (SqlParameter item in cmdParms)
                    {
                        if (item.Value == null)
                        {
                            item.Value = DBNull.Value;
                        }
                    }
                    cmd.Parameters.AddRange(cmdParms);
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                }
                return ExecuteScalar(cmd);
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static object GetSingle(string connectionString, string cmdText)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, null);
                return ExecuteScalar(cmd);
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static object GetSingle(string cmdText, CommandType commandType = CommandType.Text)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection sqlConnection = new SqlConnection(DefaultConnectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, commandType, cmdText, null);
                return ExecuteScalar(cmd);
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object GetSingle(string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection sqlConnection = new SqlConnection(DefaultConnectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, CommandType.Text, cmdText, cmdParms);
                return ExecuteScalar(cmd);
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object GetSingle(string cmdText, CommandType cmdType = CommandType.Text, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection sqlConnection = new SqlConnection(DefaultConnectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, cmdType, cmdText, cmdParms);
                return ExecuteScalar(cmd);
            }
        }

        #region GetStrSingle
        private static string ObjectToString(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return string.Empty;
            }
            else
            {
                return obj.ToString();
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static string GetSingleStr(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            return ObjectToString(GetSingle(connectionString, cmdType, cmdText, cmdParms));
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static string GetSingleStr(SqlConnection sqlConnection, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            return ObjectToString(GetSingle(sqlConnection, cmdType, cmdText, cmdParms));
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="myTran"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static string GetSingleStr(SqlTransaction myTran, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            return ObjectToString(GetSingle(myTran, cmdType, cmdText, cmdParms));
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static string GetSingleStr(string connectionString, string cmdText, params SqlParameter[] cmdParms)
        {
            return ObjectToString(GetSingle(connectionString, cmdText, cmdParms));
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public static string GetSingleStr(string connectionString, string cmdText)
        {
            return ObjectToString(GetSingle(connectionString, cmdText));
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static string GetSingleStr(string cmdText, CommandType commandType = CommandType.Text)
        {
            return ObjectToString(GetSingle(cmdText, commandType));
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static string GetSingleStr(string cmdText, params SqlParameter[] cmdParms)
        {
            return ObjectToString(GetSingle(cmdText, cmdParms));
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static string GetSingleStr(string cmdText, CommandType cmdType = CommandType.Text, params SqlParameter[] cmdParms)
        {
            return ObjectToString(GetSingle(cmdText, cmdType, cmdParms));
        }
        #endregion
        #endregion

        #region GetTable - DataTable
        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataTable GetTable(SqlTransaction tran, string cmdText, CommandType commandType = CommandType.Text)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, tran.Connection, tran, commandType, cmdText, null);
            SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }
        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataTable GetTable(SqlTransaction tran, string cmdText, CommandType commandType = CommandType.Text, params SqlParameter[] pars)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, tran.Connection, tran, commandType, cmdText, pars);
            SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataTable GetTable(string cmdText, CommandType commandType = CommandType.Text)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(DefaultConnectionString))
            {
                PrepareCommand(cmd, conn, null, commandType, cmdText, null);
                SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataTable GetTable(SqlConnection sqlConnection, string cmdText, CommandType commandType = CommandType.Text)
        {
            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, sqlConnection, null, commandType, cmdText, null);
            SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;

        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandType"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static DataTable GetTable(string cmdText, CommandType commandType = CommandType.Text, params SqlParameter[] pars)
        {
            SqlCommand cmd = new SqlCommand();
            if (pars != null)
            {
                cmd.Parameters.AddRange(pars);
            }
            using (SqlConnection sqlConnection = new SqlConnection(DefaultConnectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, commandType, cmdText, null);
                SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public static DataTable GetTable(string connectionString, string cmdText)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, CommandType.Text, cmdText, null);
                SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandType"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static DataTable GetTable(string connectionString, string cmdText, CommandType commandType = CommandType.Text, params SqlParameter[] pars)
        {
            SqlCommand cmd = new SqlCommand();
            if (pars != null)
            {
                cmd.Parameters.AddRange(pars);
            }
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, commandType, cmdText, null);
                SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (cmd.Parameters.Count > 0)
                {
                    cmd.Parameters.Clear();
                }
                return dt;
            }
        }

        #endregion

        #region Query - DataSet
        /// <summary>
        /// 查询数据填充到数据集DataSet中
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataSet Query(string cmdText, CommandType commandType = CommandType.Text)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection sqlConnection = new SqlConnection(DefaultConnectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, commandType, cmdText, null);
                SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds;
            }
        }
        /// <summary>
        /// 查询数据填充到数据集DataSet中
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="connectionString"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataSet Query(string connectionString, string cmdText, CommandType commandType = CommandType.Text)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, commandType, cmdText, null);
                SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds;
            }
        }

        /// <summary>
        /// 查询数据填充到数据集DataSet中
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataSet Query(SqlConnection sqlConnection, string cmdText, CommandType commandType = CommandType.Text)
        {

            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, sqlConnection, null, commandType, cmdText, null);
            SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            return ds;

        }

        /// <summary>
        /// 查询数据填充到数据集DataSet中
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmdText"></param>
        /// <param name="trans"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataSet Query(SqlConnection sqlConnection, string cmdText, SqlTransaction trans, CommandType commandType = CommandType.Text)
        {

            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, sqlConnection, trans, commandType, cmdText, null);
            SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            return ds;

        }

        /// <summary>
        /// 查询数据填充到数据集DataSet中
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandType"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static DataSet Query(string cmdText, CommandType commandType = CommandType.Text, params SqlParameter[] pars)
        {
            SqlCommand cmd = new SqlCommand();
            if (pars != null)
            {
                cmd.Parameters.AddRange(pars);
            }
            using (SqlConnection sqlConnection = new SqlConnection(DefaultConnectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, commandType, cmdText, null);
                SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds;
            }
        }

        /// <summary>
        /// 查询数据填充到数据集DataSet中
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="conStr"></param>
        /// <param name="commandType"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static DataSet Query(string connectionString, string cmdText, CommandType commandType = CommandType.Text, params SqlParameter[] pars)
        {
            SqlCommand cmd = new SqlCommand();
            if (pars != null)
            {
                cmd.Parameters.AddRange(pars);
            }
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, commandType, cmdText, null);
                SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds;
            }
        }

        /// <summary>
        /// 查询数据填充到数据集DataSet中
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static DataSet Query(string cmdText, params SqlParameter[] pars)
        {
            SqlCommand cmd = new SqlCommand();
            if (pars != null)
            {
                cmd.Parameters.AddRange(pars);
            }
            using (SqlConnection sqlConnection = new SqlConnection(DefaultConnectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, CommandType.Text, cmdText, null);
                SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds;
            }
        }
        /// <summary>
        /// 查询数据填充到数据集DataSet中
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="constr"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static DataSet Query(string connectionString, string cmdText, params SqlParameter[] pars)
        {
            SqlCommand cmd = new SqlCommand();
            if (pars != null)
            {
                cmd.Parameters.AddRange(pars);
            }
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, CommandType.Text, cmdText, null);
                SqlDataAdapter sda = CreateSqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds;
            }
        }

        #endregion

        #region GetTableByPage

        /// <summary>
        /// 获取排序sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>带SortId排序的新的sql</returns>
        public static DataTable GetTableByPage(string sql, int index, int size, string orderby, out int countall)
        {
            int beginindex = ((index - 1) * size) + 1;
            int endindex = index * size;
            string rebuiltSql = "select * from (select row_number() over(" + orderby + ") SortId,*  from (" + sql + ") T) T2 where T2.SortId between " + beginindex + " and " + endindex;
            countall = Convert.ToInt32(GetSingle("select count(1) from (" + sql + ") T"));
            return GetTable(rebuiltSql);
        }

        /// <summary>
        /// 获取排序sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>带SortId排序的新的sql</returns>
        public static DataTable GetTableByPage(string sql, int index, int size, string orderby, out int countall, string connStr)
        {
            int beginindex = ((index - 1) * size) + 1;
            int endindex = index * size;
            string rebuiltSql = "select * from (select row_number() over(" + orderby + ") SortId,*  from (" + sql + ") T) T2 where T2.SortId between " + beginindex + " and " + endindex;
            countall = Convert.ToInt32(GetSingle(connStr, CommandType.Text, "select count(1) from (" + sql + ") T"));
            return GetTable(connStr, rebuiltSql);
        }

        /// <summary>
        /// 获取排序sql(带参数)
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>带SortId排序的新的sql</returns>
        public static DataTable GetTableByPage(string sql, int index, int size, string orderby, out int countall, string connStr, SqlParameter[] parms)
        {
            int beginindex = ((index - 1) * size) + 1;
            int endindex = index * size;
            string rebuiltSql = "select * from (select row_number() over(" + orderby + ") SortId,*  from (" + sql + ") T) T2 where T2.SortId between " + beginindex + " and " + endindex;
            countall = Convert.ToInt32(GetSingle(connStr, CommandType.Text, "select count(1) from (" + sql + ") T", parms));
            return GetTable(connStr, rebuiltSql, CommandType.Text, parms);
        }

        /// <summary>
        /// 获取排序sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>带SortId排序的新的sql</returns>
        public static DataTable GetTableByPage(string sql, int index, int size, string orderby, string connStr)
        {
            int beginindex = ((index - 1) * size) + 1;
            int endindex = index * size;
            string rebuiltSql = "select * from (select row_number() over(" + orderby + ") SortId,*  from (" + sql + ") T) T2 where T2.SortId between " + beginindex + " and " + endindex;
            return GetTable(connStr, rebuiltSql);
        }

        #endregion

        #region Other DB Helpers

        //获取自增型ID
        public static int GetInsertID(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {

            SqlCommand cmd = new SqlCommand();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, sqlConnection, null, cmdType, cmdText + "SELECT CAST(scope_identity() AS int)", cmdParms);
                int val = (Int32)ExecuteScalar(cmd);
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static object ExecuteSqlReturn(string connectionString, string name, string output, System.Data.SqlClient.SqlParameter[] pars)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlComm = sqlConnection.CreateCommand())
                {
                    //设置要调用的存储过程的名称
                    sqlComm.CommandText = name;
                    //指定SqlCommand对象传给数据库的是存储过程的名称而不是sql语句
                    sqlComm.CommandType = CommandType.StoredProcedure;

                    foreach (SqlParameter par in pars)
                    {
                        if (par.Value == null)
                        {
                            par.Value = DBNull.Value;
                        }
                        sqlComm.Parameters.Add(par);
                    }

                    SqlParameter RetuanValue = sqlComm.Parameters.Add(new SqlParameter(output, SqlDbType.BigInt));
                    //指定为输出参数
                    RetuanValue.Direction = ParameterDirection.Output;
                    //执行
                    sqlComm.ExecuteNonQuery();
                    //得到输出参数的值,把赋值给name,注意,这里得到的是object类型的,要进行相应的类型轮换
                    return sqlComm.Parameters[output].Value;
                }
            }
        }


        /// <summary>
        /// Execute a SqlCommand that returns a resultset against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  SqlDataReader r = ExecuteReader(connectionString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>A SqlDataReader containing the results</returns> 
        /// 
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, sqlConnection, null, cmdType, cmdText, cmdParms);
                //SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                SqlDataReader rdr = CreateSqlDataReader(cmd);
                //cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                sqlConnection.Close();
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString">目标连接字符</param>
        /// <param name="TableName">目标表</param>
        /// <param name="dt">源数据</param>
        public static void SqlBulkCopyByDatatable(string connectionString, string TableName, DataTable dt)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    try
                    {
                        sqlbulkcopy.DestinationTableName = TableName;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }
                        sqlbulkcopy.WriteToServer(dt);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public static bool SqlBulkCopyByDatatable(string TableName, DataTable dt, SqlTransaction tran)
        {
            bool ok = false;
            using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(tran.Connection, SqlBulkCopyOptions.CheckConstraints, tran))
            {
                try
                {
                    sqlbulkcopy.DestinationTableName = TableName;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                    }
                    sqlbulkcopy.WriteToServer(dt);
                    ok = true;
                }
                catch (Exception ex)
                {
                }
            }
            return ok;
        }

        public static object IsDBNull(string v)
        {
            if (string.IsNullOrEmpty(v))
                return DBNull.Value;
            else
                return v;
        }

        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="cacheKey">Key to the parameter cache</param>
        /// <param name="cmdParms">an array of SqlParamters to be cached</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] cmdParms)
        {
            parmCache[cacheKey] = cmdParms;
        }

        /// <summary>
        /// Retrieve cached parameters
        /// </summary>
        /// <param name="cacheKey">key used to lookup parameters</param>
        /// <returns>Cached SqlParamters array</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">SqlCommand object</param>
        /// <param name="sqlConnection">SqlConnection object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection sqlConnection, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {

            if (sqlConnection.State != ConnectionState.Open)
                sqlConnection.Open();

            cmd.Connection = sqlConnection;
            cmd.CommandText = cmdText;
            //设置查询超时时间
            if (!string.IsNullOrEmpty(ConfigHelper.AppSettings("CommandTimeout")))
                cmd.CommandTimeout = Convert.ToInt32(ConfigHelper.AppSettings("CommandTimeout"));

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    if (parm.Value == null)
                    {
                        parm.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parm);
                }
            }
        }

        #endregion

        #region 监控sql语句执行时间，时间大于1秒的记录日志

        /// <summary>
        /// 插入sql执行日志
        /// </summary>
        /// <param name="sql">执行的sql语句</param>
        /// <param name="time"></param>
        /// <param name="connstr"></param>
        private static void InsertExecuteSQLLog(string sql, long time, string fulltime, string connstr = "")
        {
            //try
            //{
            //    logservice.LogWebService1asmx log = null;
            //    logservice200.LogWebService1asmx log200 = null;
            //    logread.LogRead logread = null;
            //    if (connstr.Contains("192.168.1.254") || connstr.Contains("192.168.1.6"))
            //    {
            //        log = new logservice.LogWebService1asmx();
            //        if (GetIsWebForm() == "0")
            //        {
            //            log.AddNewExecuteSQLLogAsync(sql, time, fulltime, connstr, "0", ClientIP, OpEmpCode, LoginAccount);
            //        }
            //        else
            //        {
            //            log.AddExecuteSQLLogAsync(sql, time, fulltime, connstr, GetIsWebForm());
            //        }
            //    }
            //    else if (connstr.Contains("192.168.1.8"))
            //    {
            //        logread = new logread.LogRead();
            //        if (GetIsWebForm() == "0")
            //        {
            //            logread.AddNewExecuteSQLLogAsync(sql, time, fulltime, connstr, "0", ClientIP, OpEmpCode, LoginAccount);
            //        }
            //        else
            //        {
            //            logread.AddExecuteSQLLogAsync(sql, time, fulltime, connstr, GetIsWebForm());
            //        }
            //    }
            //    //else
            //    //{
            //    //    log200 = new logservice200.LogWebService1asmx();
            //    //    if (GetIsWebForm() == "0")
            //    //    {
            //    //        log200.AddNewExecuteSQLLogCompleted += new logservice200.AddNewExecuteSQLLogCompletedEventHandler(log200_AddNewExecuteSQLLogCompleted);
            //    //        log200.AddNewExecuteSQLLogAsync(sql, time, fulltime, connstr, "0", ClientIP, OpEmpCode, LoginAccount);
            //    //    }
            //    //    else
            //    //    {
            //    //        log200.AddExecuteSQLLogCompleted += new logservice200.AddExecuteSQLLogCompletedEventHandler(log200_AddExecuteSQLLogCompleted);
            //    //        log200.AddExecuteSQLLogAsync(sql, time, fulltime, connstr, GetIsWebForm());
            //    //    }
            //    //}
            //}
            //catch (Exception e)
            //{
            //    string logpath = "D:\\融资管理系统异常日志\\融资管理系统写入日志异常" + DateTime.Today.ToString("yyyy-MM") + ".txt";
            //    if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(logpath)))
            //    {
            //        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logpath));
            //    }
            //    if (!System.IO.File.Exists(logpath))
            //    {
            //        System.IO.File.Create(logpath).Close();
            //    }
            //    System.IO.StreamWriter sw = new System.IO.StreamWriter(logpath, true);
            //    sw.WriteLine(DateTime.Now.ToString() + ":" + e.Message + "-" + e.StackTrace + "\r\n");
            //    sw.Close();
            //}
        }

        private static SqlDataAdapter CreateSqlDataAdapter(SqlCommand cmd)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sw.Stop();
            long time = sw.ElapsedMilliseconds;
            string fulltime = sw.Elapsed.ToString();
            if (time > GetExecuteSqlTime() && GetIsWriteSQLHelperExcuteLog())
                InsertExecuteSQLLog(cmd.CommandText, time, fulltime, cmd.Connection.ConnectionString);
            return sda;
        }

        private static SqlDataReader CreateSqlDataReader(SqlCommand cmd)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string sql = cmd.CommandText;
            string connstr = cmd.Connection.ConnectionString;
            SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            sw.Stop();
            long time = sw.ElapsedMilliseconds;
            string fulltime = sw.Elapsed.ToString();
            if (time > GetExecuteSqlTime() && GetIsWriteSQLHelperExcuteLog())
                InsertExecuteSQLLog(sql, time, fulltime, connstr);
            return rdr;
        }

        private static int ExecuteNonQuery(SqlCommand cmd)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int count = cmd.ExecuteNonQuery();
            sw.Stop();
            long time = sw.ElapsedMilliseconds;
            string fulltime = sw.Elapsed.ToString();
            if (time > GetExecuteSqlTime() && GetIsWriteSQLHelperExcuteLog())
                InsertExecuteSQLLog(cmd.CommandText, time, fulltime, cmd.Connection.ConnectionString);
            return count;
        }

        private static object ExecuteScalar(SqlCommand cmd)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            object val = cmd.ExecuteScalar();
            sw.Stop();
            long time = sw.ElapsedMilliseconds;
            string fulltime = sw.Elapsed.ToString();
            if (time > GetExecuteSqlTime() && GetIsWriteSQLHelperExcuteLog())
                InsertExecuteSQLLog(cmd.CommandText, time, fulltime, cmd.Connection.ConnectionString);
            return val;
        }

        private static int GetExecuteSqlTime()
        {
            int time = 1000;
            if (!string.IsNullOrEmpty(ConfigHelper.AppSettings("ExecuteSqlTime")))
            {
                time = Convert.ToInt32(ConfigHelper.AppSettings("ExecuteSqlTime"));
            }
            return time;
        }

        /// <summary>
        /// 是否记录日志
        /// </summary>
        /// <returns></returns>
        private static bool GetIsWriteSQLHelperExcuteLog()
        {
            bool IsWriteSQLHelperExcuteLog = true;
            if (!string.IsNullOrEmpty(ConfigHelper.AppSettings("IsWriteSQLHelperExcuteLog")))
            {
                IsWriteSQLHelperExcuteLog = (ConfigHelper.AppSettings("IsWriteSQLHelperExcuteLog") == "1");
            }
            return IsWriteSQLHelperExcuteLog;
        }

        #endregion

        #region 创建 Sql 参数


        #endregion
    }
}
