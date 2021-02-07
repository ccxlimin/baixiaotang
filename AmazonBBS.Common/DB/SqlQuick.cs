using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Common
{
    public class SqlQuickBuild
    {
        public SqlQuickBuild() { }

        /// <summary>
        /// 构建SQL执行命令
        /// </summary>
        /// <param name="cmd"></param>
        public SqlQuickBuild(string cmd)
        {
            Cmd = cmd;
        }

        public string Cmd { get; set; }

        private List<SqlParameter> _SqlParameterList = new List<SqlParameter>();

        public SqlParameter[] SqlParameters
        {
            get
            {
                return _SqlParameterList.ToArray();
            }
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="sqlParams"></param>
        /// <param name="sqlDbType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlQuickBuild AddParams(string sqlParams, SqlDbType sqlDbType, object value)
        {
            _SqlParameterList.Add(new SqlParameter(sqlParams, sqlDbType));
            _SqlParameterList[_SqlParameterList.Count - 1].Value = value;
            return this;
        }

        public DataTable GetTable(SqlTransaction tran = null)
        {
            if (tran == null)
            {
                return SqlHelper.GetTable(Cmd, CommandType.Text, SqlParameters);
            }
            else
            {
                return SqlHelper.GetTable(tran, Cmd, CommandType.Text, SqlParameters);
            }
        }

        public DataSet Query()
        {
            return SqlHelper.Query(Cmd, CommandType.Text, SqlParameters);
        }

        public object GetSingle(SqlTransaction tran = null)
        {
            if (tran == null)
            {
                return SqlHelper.GetSingle(tran, CommandType.Text, Cmd, SqlParameters);
            }
            else
            {
                return SqlHelper.GetSingle(Cmd, CommandType.Text, SqlParameters);
            }
        }

        public string GetSingleStr(SqlTransaction tran = null)
        {
            if (tran == null)
            {
                return SqlHelper.GetSingleStr(Cmd, CommandType.Text, SqlParameters);
            }
            else
            {
                return SqlHelper.GetSingleStr(tran, CommandType.Text, Cmd, SqlParameters);
            }
        }

        public bool ExecuteSql(SqlTransaction tran = null)
        {
            if (tran == null)
            {
                return SqlHelper.ExecuteSql(Cmd, CommandType.Text, SqlParameters) > 0;
            }
            return SqlHelper.ExecuteSql(tran, CommandType.Text, Cmd, SqlParameters) > 0;
        }
    }
}
