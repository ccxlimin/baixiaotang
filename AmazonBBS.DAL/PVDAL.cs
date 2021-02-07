using AmazonBBS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.DAL
{
    public class PVDAL
    {
        public bool RecordPVCount(string pvEnum, long id)
        {
            return new SqlQuickBuild("update {0} set PVCount=PVCount+1 where {0}Id=@id".FormatWith(pvEnum))
                .AddParams("@id", System.Data.SqlDbType.BigInt, id)
                .ExecuteSql();
        }

        public bool EditPVCount(string pvEnum, long id, long number)
        {
            return new SqlQuickBuild("update {0} set PVCount=@number where {0}Id=@id".FormatWith(pvEnum))
                .AddParams("@id", System.Data.SqlDbType.BigInt, id)
                .AddParams("@number", System.Data.SqlDbType.BigInt, number)
                .ExecuteSql();
        }
    }
}
