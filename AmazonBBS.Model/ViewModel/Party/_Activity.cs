using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class _Activity : Activity
    {
        /// <summary>
        /// 费用类型
        /// </summary>
        public List<ActivityFee> FeeList { get; set; }

        /// <summary>
        /// 报名填写项
        /// </summary>
        public List<JoinItemQuestionExt> JoinQuestions { get; set; }

        /// <summary>
        /// 用户报名购买的选项列表
        /// </summary>
        public List<UserBuyPartyListInfo> JoinFeeList { get; set; }

        /// <summary>
        /// 费用类型(0免费 10 积分付费 20 金钱付费 30 RMB付费)
        /// </summary>
        public int? FeeType
        {
            get;
            set;
        }
        /// <summary>
        /// 具体费用
        /// </summary>
        public int? Fee
        {
            get;
            set;
        }

        /// <summary>
        /// 评论人数
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 评论列表
        /// </summary>
        public _Comments Comments { get; set; }

        /// <summary>
        /// 报名人数
        /// </summary>
        public int JoinCount { get; set; }
    }
}
