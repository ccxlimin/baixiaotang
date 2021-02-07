using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class PartyCreateViewModel
    {
        /// <summary>
        /// 活动费用列表
        /// </summary>
        public List<ActivityFee> PartyFee { get; set; }

        /// <summary>
        /// 活动报名填写项
        /// </summary>
        public List<JoinItemQuestionExt> JoinItemQues { get; set; }

        /// <summary>
        /// 活动表
        /// </summary>
        public Activity Party { get; set; }
    }
}
