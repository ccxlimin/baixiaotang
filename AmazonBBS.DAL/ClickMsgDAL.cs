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
    /// 鼠标点击提示语
    /// </summary>
    public class ClickMsgDAL : Auto_ClickMsgDAL
    {
        public DataTable FindALL()
        {
            return new SqlQuickBuild("select * from ClickMsg;").GetTable();
        }
    }

}
