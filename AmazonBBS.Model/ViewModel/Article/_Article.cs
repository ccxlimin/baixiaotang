using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class _Article : Article
    {



        /// <summary>
        /// 文章作者名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 作者头像
        /// </summary>
        public string HeadUrl { get; set; }

        /// <summary>
        /// 封面
        /// </summary>
        public string DefaultIMg { get; set; }

        /// <summary>
        /// 最后回复时间
        /// </summary>
        public DateTime? LastCommentTime { get; set; }

        /// <summary>
        /// 帖子点赞人数
        /// </summary>
        public int PriseCount { get; set; }

        /// <summary>
        /// 关注人数
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 当前登录人是否已关注此文章
        /// </summary>
        public bool IsLiked { get; set; }

        /// <summary>
        /// 当前登录人是否已点赞此文章
        /// </summary>
        public bool IsPrised { get; set; }

        /// <summary>
        /// 评论人数
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 评论列表
        /// </summary>
        public _Comments Comments { get; set; }

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
        public int HeadNameShowType { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public List<Tag> Tags { get; set; }










        ///////////////////////////////////////////2018.9.16 Add 




        /// <summary>
        /// 作者签到次数
        /// </summary>
        public int SignCount { get; set; }

        /// <summary>
        /// 用户剩余VIP分（金钱）
        /// </summary>
        public int TotalCoin { get; set; }

        /// <summary>
        /// 作者发表帖子数
        /// </summary>
        public int User_BBS_Count { get; set; }

        /// <summary>
        /// 作者发表文章数
        /// </summary>
        public int User_Article_Count { get; set; }

        /// <summary>
        /// 作者粉丝数量
        /// </summary>
        public int User_Fans_Count { get; set; }

        /// <summary>
        /// 主题购买人数
        /// </summary>
        public int ContentBuyCount { get; set; }

        /// <summary>
        /// 是否已购买主题
        /// </summary>
        public bool IsBuyContent { get; set; }

        public DiscuzLeftUserInfo DiscuzLeftUserInfo { get; set; }

        /// <summary>
        /// 附件 
        /// </summary>
        public List<AttachMentWithBuyInfo> AttachMents { get; set; }


        /// <summary>
        /// 是否显示当前发布增加的积分
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

        //////////////////////////////////////////////////////
    }
}
