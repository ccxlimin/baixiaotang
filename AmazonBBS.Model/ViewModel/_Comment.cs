using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class _Comment : Comment
    {
        /// <summary>
        /// 是否已购买答案
        /// </summary>
        public bool IsFeeAnswer { get; set; }

        /// <summary>
        /// 是否已赞
        /// </summary>
        public bool IsPrised { get; set; }

        /// <summary>
        /// 赞同数
        /// </summary>
        public int PrisedCount { get; set; }

        /// <summary>
        /// 该评论的回复条数
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 评论者头像
        /// </summary>
        public string HeadUrl { get; set; }

        /// <summary>
        /// 用户个性签名
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 评论者名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 作者VIP
        /// </summary>
        public int VIP { get; set; }

        /// <summary>
        /// VIP有效时间
        /// </summary>
        public DateTime VIPExpiryTime { get; set; }

        /// <summary>
        /// 作者是否管理员
        /// </summary>
        public bool IsMaster { get; set; }

        /// <summary>
        /// 用户认证
        /// </summary>
        public int UserV { get; set; }

        /// <summary>
        /// 用户剩余总积分（用于计算积分等级）
        /// </summary>
        public int? TotalScore { get; set; }

        /// <summary>
        /// 用户头衔
        /// </summary>
        public string LevelName { get; set; }

        /// <summary>
        /// 头衔图标路径
        /// </summary>
        public string LevelNameUrls { get; set; }

        /// <summary>
        /// 用户专属头衔
        /// </summary>
        public string OnlyLevelName { get; set; }

        /// <summary>
        /// 头衔显示类型(1头衔(默认)   2专属头衔)
        /// </summary>
        public int? HeadNameShowType { get; set; }

        /// <summary>
        /// 签到次数
        /// </summary>
        public int? SignCount { get; set; }

        /// <summary>
        /// 发表帖子数
        /// </summary>
        public int? User_BBS_Count { get; set; }

        /// <summary>
        /// 发表文章数
        /// </summary>
        public int? User_Article_Count { get; set; }

        /// <summary>
        /// 粉丝数量
        /// </summary>
        public int? User_Fans_Count { get; set; }

        /// <summary>
        /// 用户剩余VIP分（金钱）
        /// </summary>
        public int? TotalCoin { get; set; }

        /// <summary>
        /// 针对作者本人而言，是否显示加分情况
        /// </summary>
        public bool ShowCoinTips { get; set; }

        /// <summary>
        /// 是否已反对
        /// </summary>
        public bool IsAgainst { get; set; }

        /// <summary>
        /// 反对人数
        /// </summary>
        public int AgainstCount { get; set; }

        public List<_ReplyComment> ReplyList { get; set; }
    }

    public class _ReplyComment : Comment
    {
        /// <summary>
        /// 回复给谁 的名称
        /// </summary>
        public string Reply2UserName { get; set; }

        /// <summary>
        /// 回复给谁 的ID
        /// </summary>
        public long Reply2UserID { get; set; }

        /// <summary>
        /// 是否已赞
        /// </summary>
        public bool IsPrised { get; set; }

        /// <summary>
        /// 赞同数
        /// </summary>
        public int PrisedCount { get; set; }

        /// <summary>
        /// 该回复的回复条数
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 回复者头像
        /// </summary>
        public string HeadUrl { get; set; }

        /// <summary>
        /// 用户个性签名
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 回复者名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 作者VIP
        /// </summary>
        public int VIP { get; set; }

        /// <summary>
        /// VIP有效时间
        /// </summary>
        public DateTime VIPExpiryTime { get; set; }

        /// <summary>
        /// 作者是否管理员
        /// </summary>
        public bool IsMaster { get; set; }

        /// <summary>
        /// 用户认证
        /// </summary>
        public int UserV { get; set; }

        /// <summary>
        /// 用户剩余总积分（用于计算积分等级）
        /// </summary>
        public int TotalScore { get; set; }

        /// <summary>
        /// 用户头衔
        /// </summary>
        public string LevelName { get; set; }

        /// <summary>
        /// 用户专属头衔
        /// </summary>
        public string OnlyLevelName { get; set; }

        /// <summary>
        /// 头衔显示类型(1头衔(默认)   2专属头衔)
        /// </summary>
        public int HeadNameShowType { get; set; }



        public long UserID { get; set; }

        /// <summary>
        /// 签到次数
        /// </summary>
        public int SignCount { get; set; }

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
        /// 用户剩余VIP分（金钱）
        /// </summary>
        public int TotalCoin { get; set; }

        /// <summary>
        /// 是否已反对
        /// </summary>
        public bool IsAgainst { get; set; }

        /// <summary>
        /// 反对人数
        /// </summary>
        public int AgainstCount { get; set; }
    }
}
