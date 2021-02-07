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
    /// 枚举
    /// </summary>
    public class BBSEnumDAL : Auto_BBSEnumDAL
    {
        public DataTable Query(int enumType, bool sort)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@" select * from BBSEnum where EnumType=@enumtype and IsDelete=0 ");
            if (sort)
            {
                sql.Append(" order by SortIndex ");
            }

            return new SqlQuickBuild(sql.ToString()).AddParams("@enumtype", SqlDbType.Int, enumType).GetTable();
        }

        public DataTable GetBBSMenus()
        {
            return new SqlQuickBuild("select * from BBSEnum where IsBBS=1 and IsDelete=0 and EnumType=1 order by SortIndex")
                .GetTable();
        }

        public DataTable GetInfo(string id)
        {
            return new SqlQuickBuild("select * from  BBSEnum where IsBBS=1 and IsDelete=0 and EnumType=1 and EnumCode=@code;")
                 .AddParams("@code", SqlDbType.VarChar, id)
                 .GetTable();
        }

        /// <summary>
        /// 获取能够发表文章的信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetSetArticleRol()
        {
            return new SqlQuickBuild(@"select * from BBSEnum where IsDelete=0 and CanArticle=1").GetTable();
        }

        public string Exist(string cn, int type, int id)
        {
            return new SqlQuickBuild("select count(1) from BBSEnum where BBSEnumId!=@id and EnumDesc=@cn and EnumType=@type and IsDelete=0")
                .AddParams("@cn", SqlDbType.NVarChar, cn)
                .AddParams("@type", SqlDbType.Int, type)
                .AddParams("@id", SqlDbType.Int, id)
                .GetSingleStr();

        }

        public DataSet GetShareCoinList(long userID)
        {
            SqlQuickBuild sql = new SqlQuickBuild();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                        select count(1) from ShareRegistLog where ShareUserID=@uid;
                        select a.EnumDesc ShareCoin,a.BBSEnumId BBSID,a.SortIndex ShareCount,ISNULL(b.ScoreCoinLogId,0)                 IsPickCoin from BBSEnum  a 
                        left join ScoreCoinLog b on b.CoinSource=22 and b.CreateUser=a.BBSEnumId and b.UserID=@uid 
                        where a.EnumType=5 ;");
            sql.AddParams("@uid", SqlDbType.BigInt, userID);
            sql.Cmd = sb.ToString();
            return sql.Query();
        }
    }

}

