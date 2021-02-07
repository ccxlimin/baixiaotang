using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class _Gift : Gift
    {
        /// <summary>
        /// 创建人 
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// 礼物已购买人数
        /// </summary>
        public int AllBuyCount { get; set; }


        /// <summary>
        /// 当前用户是否已购买过
        /// </summary>
        public int CurrentUserBuyCount { get; set; }

        /// <summary>
        /// 评论人数
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 评论列表
        /// </summary>
        public _Comments Comments { get; set; }

        /// <summary>
        /// 费用类型
        /// </summary>
        public List<GiftFee> FeeList { get; set; }

        /// <summary>
        /// 报名填写项
        /// </summary>
        public List<JoinItemQuestionExt> JoinQuestions { get; set; }

        /// <summary>
        /// 用户报名购买的选项列表
        /// </summary>
        public List<UserBuyGiftListInfo> JoinFeeList { get; set; }

        /// <summary>
        /// 费用类型(0免费 10 积分付费 20 金钱付费 30 RMB付费
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
    }
}
