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
    /// 邀请面试记录表
    /// </summary>
    public class InviteInterviewDAL : Auto_InviteInterviewDAL
    {
        public string Hasinvited(long qiuZhiID, long userID)
        {
            return new SqlQuickBuild("select count(1) from InviteInterview where QiuZhiID=@qid and CreateUser=@user ")
                .AddParams("@qid", SqlDbType.BigInt, qiuZhiID)
                .AddParams("@user", SqlDbType.BigInt, userID)
                .GetSingleStr();
        }
    }

}
