using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Common
{
    /// <summary>
    /// 事务帮助器
    /// </summary>
    public class TranHelper
    {
        public SqlTransaction Tran { get; set; }
        public SqlConnection Conn { get; set; }

        private bool IsTran { get; set; }
        private string Connection { get; set; }

        /// <summary>
        /// 开启事务
        /// </summary>
        public TranHelper(string connection = null)
        {
            Connection = connection == null ? SqlHelper.DefaultConnectionString : connection;
            BeginTran();
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        private void BeginTran()
        {
            Conn = new SqlConnection(Connection);
            Conn.Open();
            IsTran = true;
            Tran = Conn.BeginTransaction();
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollBack()
        {
            if (IsTran)
            {
                IsTran = false;
                Tran.Rollback();
                Dispose();
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            if (IsTran)
            {
                IsTran = false;
                Tran.Commit();
                Dispose();
            }
        }

        public void Dispose()
        {
            Tran.Dispose();
            Connection.Clone();
        }
    }
}
