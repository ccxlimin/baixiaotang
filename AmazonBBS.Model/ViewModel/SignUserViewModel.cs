using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 签到用户页面视图表
    /// </summary>
    public class SignUserViewModel
    {
        public string UserName { get; set; }
        public long UserID { get; set; }

        /// <summary>
        /// 今日签到时间
        /// </summary>
        public DateTime SignTime { get; set; }

        /// <summary>
        /// 今日是否签到
        /// </summary>
        public bool SignToday { get; set; }

        /// <summary>
        /// 本月总签到次数
        /// </summary>
        public string SignCount { get; set; }

        /// <summary>
        /// 本月连续签到次数
        /// </summary>
        public string MonthContinueSignCount { get; set; }

        /// <summary>
        /// 总签到次数
        /// </summary>
        public string SignTotalCount { get; set; }
    }
}
