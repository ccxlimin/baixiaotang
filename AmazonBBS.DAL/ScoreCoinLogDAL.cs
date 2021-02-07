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
    /// 积分金钱流水表
    /// </summary>
    public class ScoreCoinLogDAL : Auto_ScoreCoinLogDAL
    {
        public DataTable IsSignToday(long userid)
        {
            return new SqlQuickBuild(@"select top 1 CoinTime from ScoreCoinLog where UserID=@userid and CoinSource=3 order by CoinTime Desc")
                .AddParams("@userid", SqlDbType.BigInt, userid)
                .GetTable();
        }

        public string HasGiveScore(long? commentUserID, long mainID, int coinSource, int coinType, SqlTransaction tran)
        {
            return new SqlQuickBuild("select count(1) from ScoreCoinLog where CreateUser=@mainID and CoinSource=@coinsource and UserID=@uid and CoinType=@coinType")
                .AddParams("@uid", SqlDbType.BigInt, commentUserID)
                .AddParams("@mainID", SqlDbType.BigInt, mainID)
                .AddParams("@coinsource", SqlDbType.Int, coinSource)
                .AddParams("@coinType", SqlDbType.Int, coinType)
                .GetSingleStr();
        }
    }

}

