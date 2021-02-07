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
    /// 人才求职
    /// </summary>
    public class QiuZhiDAL : Auto_QiuZhiDAL
    {
        public DataTable GetQiuZhiDetail(long id, long userid)
        {
            return new SqlQuickBuild(@"
                            select a.*,b.HeadUrl,b.UserName,
                            (select count(1) from UserLike ul where ul.LikeTargetId=a.QiuZhiID and ul.LikeType=5 and ul.UserID=@userid and ul.IsDelete=0) IsLiked from QiuZhi a 
                            left join UserBase b on a.Publisher=b.UserID
                            where a.IsDelete=0 and a.QiuZhiID=@id;")
                            .AddParams("@id", SqlDbType.BigInt, id)
                            .AddParams("@userid", SqlDbType.BigInt, userid)
                            .GetTable();
        }

        /// <summary>
        /// 分页显示内容
        /// </summary>
        /// <param name="startIndex">开始码</param>
        /// <param name="endIndex">结束码</param>
        /// <returns></returns>
        public DataSet SearchByRows(int startIndex, int endIndex, string key)
        {
            StringBuilder sb = new StringBuilder();
            var sql = new SqlQuickBuild();
            if (key.IsNotNullOrEmpty())
            {
                sb.Append(@"
select count(*) from QiuZhi a
left join UserBase b on b.UserID=a.Publisher
where a.IsDelete=0 and b.IsDelete=0 and (a.IWant like @key or b.UserName like @key);

select * from  (SELECT ROW_NUMBER() OVER(ORDER BY QiuZhiID desc) as rowid ,
a.*,
(case when a.ValidTime>GETDATE() then 1 else 0 end) Flag,
b.UserName,b.LoginTime LastLogonTime,
c.VIP,c.VIPExpiryTime,c.TotalScore,
(select EnumDesc from BBSEnum where c.LevelName=BBSEnumId and EnumType=3) LevelName,
(select EnumDesc from BBSEnum where c.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,
c.HeadNameShowType,c.UserV  
FROM QiuZhi a
left join UserBase b on b.UserID=a.Publisher
left join UserExt c on c.UserID=b.UserID
where a.IsDelete=0 and b.IsDelete=0 and (a.IWant like @key or b.UserName like @key)) 
               t where t.rowid between @startindex and @endindex order by T.Flag desc,T.CreateTime desc;");
                sql.AddParams("@key", SqlDbType.VarChar, "%{0}%".FormatWith(key));
            }
            else
            {
                sb.Append(@"select count(*) from QiuZhi a where a.IsDelete=0;

select * from  (SELECT ROW_NUMBER() OVER(ORDER BY QiuZhiID desc  ) as rowid,
a.*,
(case when a.ValidTime>GETDATE() then 1 else 0 end) Flag,
b.UserName,b.LoginTime LastLogonTime,
c.VIP,c.VIPExpiryTime,c.TotalScore,
(select EnumDesc from BBSEnum where c.LevelName=BBSEnumId and EnumType=3) LevelName,
(select EnumDesc from BBSEnum where c.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,
c.HeadNameShowType,c.UserV   
FROM QiuZhi a
left join UserBase b on b.UserID=a.Publisher
left join UserExt c on c.UserID=a.Publisher
where a.IsDelete=0 and b.IsDelete=0 ) 
               t where t.rowid between @startindex and @endindex order by T.Flag desc,T.CreateTime desc;");
            }
            sql.Cmd = sb.ToString();
            return sql.AddParams("@startindex", SqlDbType.Int, startIndex)
                            .AddParams("@endindex", SqlDbType.Int, endIndex)
                            .Query();
        }
        public DataSet SelectByCondition(int startIndex, int endIndex, string condition)
        {
            return new SqlQuickBuild(@"select count(*) from QiuZhi a where a.IsDelete=0 {0};

                                select * from  (SELECT ROW_NUMBER() OVER(ORDER BY QiuZhiID desc  ) as rowid ,
                                a.*,
                                (case when a.ValidTime>GETDATE() then 1 else 0 end) Flag,
                                b.UserName,b.LoginTime LastLogonTime,
                                c.VIP,c.VIPExpiryTime,c.TotalScore,
                                (select EnumDesc from BBSEnum where c.LevelName=BBSEnumId and EnumType=3) LevelName,
                                (select EnumDesc from BBSEnum where c.OnlyLevelName=BBSEnumId and EnumType=4) OnlyLevelName,
                                c.HeadNameShowType,c.UserV   
                                FROM QiuZhi a
                                left join UserBase b on b.UserID=a.Publisher
                                left join UserExt c on c.UserID=a.Publisher
                                where a.IsDelete=0 and b.IsDelete=0 {0}) 
                                t where t.rowid between @startIndex and @endIndex order by T.Flag desc,T.CreateTime desc;".FormatWith(condition))
                                .AddParams("@startIndex", SqlDbType.Int, startIndex)
                                .AddParams("@endIndex", SqlDbType.Int, endIndex)
                                .Query();

        }

        public DataSet GetQiuZhiList(long userID, int startIndex, int endIndex)
        {
            return new SqlQuickBuild(@"
                        select count(1) from QiuZhi where Publisher=@userid and isdelete=0;
                        select * from (select row_number() over(order by CreateTime desc) rid,* from QiuZhi where Publisher=@userid and isdelete=0) T where T.rid between @si and @ei")
                .AddParams("@userid", SqlDbType.BigInt, userID)
                .AddParams("@si", SqlDbType.Int, startIndex)
                .AddParams("@ei", SqlDbType.Int, endIndex)
                .Query();
        }

        public DataTable GetQiuZhiBuyPay(long mainid)
        {
            return new SqlQuickBuild("select * from QiuZhi where QiuZhiID=@id")
                .AddParams("@id", SqlDbType.BigInt, mainid)
                .GetTable();
        }
    }
}

