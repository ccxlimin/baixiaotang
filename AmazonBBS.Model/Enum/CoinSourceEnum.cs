using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 积分和金钱 的 变动渠道来源(如  1充值，2 回答赠送，3 签到赠送）
    /// </summary>
    public enum CoinSourceEnum
    {
        /// <summary>
        /// 充值
        /// </summary>
        Pay = 1,

        /// <summary>
        /// 评论获得
        /// </summary>
        Comment = 2,

        /// <summary>
        /// 签到赠送
        /// </summary>
        Sign = 3,

        /// <summary>
        /// 提问问题扣除
        /// </summary>
        AskQuestion = 4,

        /// <summary>
        /// 管理员授予
        /// </summary>
        MasterGive = 5,

        /// <summary>
        /// 参加报名活动
        /// </summary>
        JoinActivity = 6,

        /// <summary>
        /// 查看答案
        /// </summary>
        SeeAnswer = 7,

        /// <summary>
        /// 被查看答案
        /// </summary>
        BeSeeAnswer = 8,

        /// <summary>
        /// 兑换礼物
        /// </summary>
        BuyGift = 9,

        /// <summary>
        /// 积分兑换VIP分
        /// </summary>
        Score2VipScore = 10,

        /// <summary>
        /// VIP分充值
        /// </summary>
        VipScorePay = 11,

        /// <summary>
        /// VIP分兑换积分
        /// </summary>
        VipScore2Score = 12,

        /// <summary>
        /// 主动点赞获得
        /// </summary>
        Prise = 13,

        /// <summary>
        /// 被点赞获得
        /// </summary>
        PriseFor = 14,

        /// <summary>
        /// 发布招聘
        /// </summary>
        PublishZhaoPin = 15,

        /// <summary>
        /// 发布求职
        /// </summary>
        PublishQiuZhi = 16,

        /// <summary>
        /// 发布产品
        /// </summary>
        PublishProduct = 17,

        /// <summary>
        /// 查看招聘信息相关联系方式
        /// </summary>
        SeeZhaoPinInfo = 18,

        /// <summary>
        /// 查看求职信息相关联系方式
        /// </summary>
        SeeQiuZhiInfo = 19,
        /// <summary>
        /// 购买兑换VIP专属
        /// </summary>
        BuyVIP = 20,

        /// <summary>
        /// 优秀回答获得
        /// </summary>
        NiceAnswer = 21,

        /// <summary>
        /// 分享链接注册领取奖励
        /// </summary>
        ShareCoin = 22,

        /// <summary>
        /// 发布新帖子 得分
        /// </summary>
        NewBBS = 23,

        /// <summary>
        /// 发布新文章 得分
        /// </summary>
        NewArticle = 24,

        /// <summary>
        /// 购买内容主体
        /// </summary>
        BuyContent = 25,

        /// <summary>
        /// 用户购买内容主体
        /// </summary>
        UserBuyContent = 26,

        /// <summary>
        /// 下载附件 扣积分
        /// </summary>
        DownAttachMent = 27,

        /// <summary>
        /// 用户下载附件 ，作者得积分
        /// </summary>
        UserDownAttachMent = 28,

        /// <summary>
        /// 用户评论 帖子   获得
        /// </summary>
        UserComment_BBS = 29,

        /// <summary>
        /// 用户评论  文章  获得
        /// </summary>
        UserComment_Article = 30,

        /// <summary>
        /// 查看协会会员版块消费积分
        /// </summary>
        FeeBBS_Orher = 31,

        /// <summary>
        /// 新用户 连续签到3天赠送
        /// </summary>
        NewUserSignCount3 = 900003,
        /// <summary>
        /// 新用户 连续签到10天赠送
        /// </summary>
        NewUserSignCount10 = 900010,

        /// <summary>
        /// 老用户 累计签到3天赠送
        /// </summary>
        OldUserSignCount3 = 900103,
        /// <summary>
        /// 老用户 累计签到3天赠送
        /// </summary>
        OldUserSignCount10 = 900110,
        /// <summary>
        /// 老用户 累计签到3天赠送
        /// </summary>
        OldUserSignCount20 = 900120,
    }

    /// <summary>
    /// 类型(积分/金钱 )
    /// </summary>
    public enum CoinTypeEnum
    {
        Score = 1,
        Money = 2
    }
}
