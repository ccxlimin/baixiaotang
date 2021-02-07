using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Common
{
    public static class DataTableExt
    {
        public static bool IsNotNullAndRowCount(this DataTable dt)
        {
            return dt != null && dt.Rows.Count > 0;
        }

        public static void ForEach(this DataTable dt, Action<DataRow> action)
        {
            if (dt.IsNotNullAndRowCount())
            {
                foreach (DataRow dr in dt.Rows)
                {
                    action(dr);
                }
            }
        }
    }
}
