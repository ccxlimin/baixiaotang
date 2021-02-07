using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class UserViewModel
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string HeadUrl { get; set; }
        public string Sign { get; set; }

        /// <summary>
        /// 关注人数 
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 是否已关注
        /// </summary>
        public bool IsLiked { get; set; }

        /// <summary>
        /// 总积分（用于积分等级计算）
        /// </summary>
        public int TotalScore { get; set; }

        /// <summary>
        /// 总金钱（总 VIP分）
        /// </summary>
        public int TotalCoin { get; set; }

        /// <summary>
        /// 问题提问数
        /// </summary>
        public int QuestionCount { get; set; }

        /// <summary>
        /// 最佳解答次数
        /// </summary>
        public int BsetAnswerCount { get; set; }

        /// <summary>
        /// 优秀解答次数
        /// </summary>
        public int NiceAnswerCount { get; set; }

        /// <summary>
        /// 发表文章数
        /// </summary>
        public int ArticltCount { get; set; }

        /// <summary>
        /// 头衔
        /// </summary>
        public string LevelName { get; set; }

        /// <summary>
        /// 头衔 图片地址
        /// </summary>
        public string LevelNameUrls { get; set; }

        /// <summary>
        /// 专属头衔
        /// </summary>
        public string OnlyLevelName { get; set; }

        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool IsMaster { get; set; }

        /// <summary>
        /// 用户认证(1:1级认证，2:2级认证 3：3级认证 4：1级和2级认证 5：2级和3级认证 6：1级和3级认证)
        /// </summary>
        public int UserV { get; set; }


        /// <summary>
        /// 总签到次数
        /// </summary>
        public int SignCount { get; set; }

        /// <summary>
        /// 总评论次数
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 性别(1男2女0
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public string Birth { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 头衔显示类型(1头衔(默认)   2专属头衔)
        /// </summary>
        public int HeadNameShowType { get; set; }
    }
}
