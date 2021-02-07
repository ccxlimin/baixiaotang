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
    /// 页面模块对应标签
    /// </summary>
    public class MenuBelongTagDAL : Auto_MenuBelongTagDAL
    {
        public bool DeleteTagForMainID(long mainId, int mainType, SqlTransaction tran)
        {
            return new SqlQuickBuild("delete MenuBelongTag where MainId=@mainid and MainType=@mainType")
                .AddParams("@mainid", SqlDbType.BigInt, mainId)
                .AddParams("@mainType", SqlDbType.Int, mainType)
                .ExecuteSql(tran);
        }

        public bool HasTagByMainId(long mainId, int mainType)
        {
            return new SqlQuickBuild("select count(1) from MenuBelongTag where MainId=@mainid and MainType=@mainType")
                .AddParams("@mainid", SqlDbType.BigInt, mainId)
                .AddParams("@mainType", SqlDbType.Int, mainType)
                .GetSingleStr().ToInt32() > 0;
        }

        public DataSet Count(long tagid)
        {
            return new SqlQuickBuild(@"
                                    --获取标签包含总问题数
                                    select count(1) from MenuBelongTag a
                                    left join Tag b on a.TagId=b.TagId
                                    left join Question c on c.QuestionId=a.MainId
                                    where b.TagId=@tagid and b.IsDelete=0 and c.IsDelete=0 and a.MainType=1;
                                    --获取标签包含总文章数
                                    select count(1) from MenuBelongTag a
                                    left join Tag b on a.TagId=b.TagId
                                    left join Article c on c.ArticleId=a.MainId
                                    where b.TagId=@tagid and b.IsDelete=0 and c.IsDelete=0 and a.MainType=2;")
                                    .AddParams("@tagid", SqlDbType.BigInt, tagid)
                                    .Query();
        }
    }

}
