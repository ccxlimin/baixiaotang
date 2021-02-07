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
    /// 导航分类
    /// </summary>
    public class SoftLinkTypeDAL:Auto_SoftLinkTypeDAL
    {
        public DataTable GetSoftLinkTypes()
        {
            return new SqlQuickBuild("select * from SoftLinkType where IsDelete=0").GetTable();
        }

        public DataTable SelectAll()
        {
            return new SqlQuickBuild("select SoftLinkTypeID,SoftLinkTypeName,SoftLinkColor,SoftLinkLogo from softlinktype where isdelete=0").GetTable();
        }
    }

}

