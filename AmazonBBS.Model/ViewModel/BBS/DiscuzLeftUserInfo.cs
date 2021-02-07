using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 帖子/文章版块 左侧用户信息
    /// </summary>
    public class DiscuzLeftUserInfo
    {
        public long? UserID { get; set; }
        public long? CommentUserID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadUrl { get; set; }

        /// <summary>
        /// 签到次数
        /// </summary>
        public int SignCount { get; set; }

        /// <summary>
        /// VIP等级
        /// </summary>
        public int VIP { get; set; }

        /// <summary>
        /// VIP有效时间
        /// </summary>
        public DateTime VIPExpiryTime { get; set; }

        /// <summary>
        /// 头衔显示类型(1头衔(默认)   2专属头衔)
        /// </summary>
        public int HeadNameShowType { get; set; }

        /// 用户头衔
        /// </summary>
        public string LevelName { get; set; }
        /// <summary>
        /// 头衔图片
        /// </summary>
        public string LevelNameUrls { get; set; }

        /// <summary>
        /// 用户专属头衔
        /// </summary>
        public string OnlyLevelName { get; set; }

        /// <summary>
        /// 发表帖子数
        /// </summary>
        public int User_BBS_Count { get; set; }

        /// <summary>
        /// 发表文章数
        /// </summary>
        public int User_Article_Count { get; set; }

        /// <summary>
        /// 粉丝数量
        /// </summary>
        public int User_Fans_Count { get; set; }

        /// <summary>
        /// 用户剩余总积分（用于计算积分等级）
        /// </summary>
        public int TotalScore { get; set; }

        /// <summary>
        /// 用户剩余VIP分（金钱）
        /// </summary>
        public int TotalCoin { get; set; }

        /// <summary>
        /// 用户最近3篇帖子
        /// </summary>
        public List<Question> Questions_3 { get; set; }

        /// <summary>
        /// 用户最近3篇文章
        /// </summary>
        public List<Article> Articles_3 { get; set; }
    }
}
