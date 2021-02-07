using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 附件费用
    /// </summary>
    public class FileFee
    {
        public Guid AttachId { get; set; }

        public int Index { get; set; }

        public int FeeType { get; set; }

        public int Fee { get; set; }

        public bool isNew { get; set; }

        /// <summary>
        /// 是否改变了文件
        /// </summary>
        public bool IsChange { get; set; }

        /// <summary>
        /// 费用类型是否改变
        /// </summary>
        public bool FeeTypeChange { get; set; }

        public bool FeeChange { get; set; }
    }
}
