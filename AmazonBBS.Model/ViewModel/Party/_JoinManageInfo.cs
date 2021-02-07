using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class _JoinManageInfo : ActivityJoin
    {
        /// <summary>
        /// 票种类型
        /// </summary>
        public string FeeName { get; set; }

        /// <summary>
        /// 商品原始具体费用
        /// </summary>
        public int? ItemSourceFee
        {
            get;
            set;
        }
    }
}
