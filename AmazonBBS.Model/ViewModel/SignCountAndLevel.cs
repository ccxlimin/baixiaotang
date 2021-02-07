using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 签到次数与头衔等级
    /// </summary>
    public class SignCountAndLevel
    {
        /// <summary>
        /// 签到次数
        /// </summary>
        public int SignCount { get; set; }

        public long UserID { get; set; }

        public string LevelName { get; set; }
    }
}
