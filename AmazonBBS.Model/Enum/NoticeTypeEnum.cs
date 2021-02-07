using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 通知类型
    /// </summary>
    public enum NoticeTypeEnum
    {
        None = 0,

        /// <summary>
        /// 新增问题 通知
        /// </summary>
        [Description("新帖子")]
        BBS_Add = 1,

        /// <summary>
        /// 自己问题的新回答
        /// </summary>
        BBS_My_Comment = 10,

        /// <summary>
        /// 关注的帖子的新回答
        /// </summary>
        BBS_Like_Comment = 11,

        /// <summary>
        /// 自己的问题被点赞
        /// </summary>
        BBS_My_Prise = 12,

        /// <summary>
        /// 自己的问题回答被回复
        /// </summary>
        BBS_MyComment_Comment = 13,

        /// <summary>
        /// 自己的帖子被关注
        /// </summary>
        BBS_BeLiked = 14,

        /// <summary>
        /// 自己的帖子被反对
        /// </summary>
        BBS_Against = 15,

        /// <summary>
        /// 关注的用户发表新文章
        /// </summary>
        [Description("新文章")]
        Article_Add = 2,

        /// <summary>
        /// 自己文章的新回答
        /// </summary>
        Article_My_Comment = 20,

        /// <summary>
        /// 关注的文章的新回答
        /// </summary>
        Article_Like_Comment = 21,

        /// <summary>
        /// 自己的文章被点赞
        /// </summary>
        Article_My_Prise = 22,

        /// <summary>
        /// 自己的文章回答被回复
        /// </summary>
        Article_MyComment_Comment = 23,

        /// <summary>
        /// 自己的文章被关注
        /// </summary>
        Article_BeLiked = 24,

        /// <summary>
        /// 自己的文章被反对
        /// </summary>
        Article_Against = 25,

        /// <summary>
        /// 关注用户的 发布招聘
        /// </summary>
        [Description("新招聘")]
        ZhaoPin_Add = 3,

        /// <summary>
        /// 自己招聘的新回答
        /// </summary>
        ZhaoPin_My_Comment = 30,

        /// <summary>
        /// 招聘发布支付
        /// </summary>
        ZhaoPin_Pay_Publish = 31,

        /// <summary>
        /// 自己的招聘被关注
        /// </summary>
        ZhaoPin_BeLiked = 34,

        /// <summary>
        /// 关注用户的 发布求职
        /// </summary>
        [Description("新求职")]
        QiuZhi_Add = 4,

        /// <summary>
        /// 自己求职的新回答
        /// </summary>
        QiuZhi_My_Comment = 40,

        /// <summary>
        /// 求职发布支付
        /// </summary>
        QiuZhi_Pay_Publish = 41,

        /// <summary>
        /// 自己的求职被关注
        /// </summary>
        QiuZhi_BeLiked = 44,

        /// <summary>
        /// 关注用户的 发布产品
        /// </summary>
        [Description("新产品")]
        Product_Add = 5,

        /// <summary>
        /// 自己产品的新回答
        /// </summary>
        Product_My_Comment = 50,

        /// <summary>
        /// 产品发布支付
        /// </summary>
        Product_Pay_Publish = 51,

        /// <summary>
        /// 关注用户的 发布活动
        /// </summary>
        [Description("新活动")]
        Party_Add = 6,

        /// <summary>
        /// 自己活动的新回答
        /// </summary>
        Party_My_Comment = 60,

        /// <summary>
        /// 参加活动(含支付)
        /// </summary>
        [Description("活动")]
        Party_Join = 61,

        /// <summary>
        /// 关注用户的 发布礼物
        /// </summary>
        [Description("新礼物")]
        Gift_Add = 7,

        /// <summary>
        /// 自己礼物的新回答
        /// </summary>
        Gift_My_Comment = 70,

        /// <summary>
        /// 购买礼物
        /// </summary>
        [Description("礼物")]
        Gift_Buy = 71,

        /// <summary>
        /// 关注用户的 发布数据
        /// </summary>
        [Description("新数据")]
        DataAnalysis_Add = 8,

        /// <summary>
        /// 自己数据的新回答
        /// </summary>
        DataAnalysis_My_Comment = 80,

        /// <summary>
        /// 购买数据
        /// </summary>
        [Description("数据")]
        DatAnalysis_Buy = 81,

        /// <summary>
        /// 关注用户的 发布课程
        /// </summary>
        [Description("新课程")]
        KeCheng_Add = 9,

        /// <summary>
        /// 自己课程的新回答
        /// </summary>
        KeCheng_My_Comment = 90,

        /// <summary>
        /// 购买课程
        /// </summary>
        [Description("课程")]
        KeCheng_Buy = 91,

        /// <summary>
        /// 用户被关注 通知
        /// </summary>
        UserBeLiked = 999,

        /// <summary>
        /// 关注用户的新回答
        /// </summary>
        User_NewComment = 998,

        /// <summary>
        /// 给评论/回复点赞
        /// </summary>
        Comment_Prise = 997,

        /// <summary>
        /// 面试邀请
        /// </summary>
        InviteInterview = 996,

        /// <summary>
        /// 发送求职简历
        /// </summary>
        SendCV = 995,

        /// <summary>
        /// 预约购买产品
        /// </summary>
        OrderBuyProduct = 994,

        /// <summary>
        /// 签到通知
        /// </summary>
        Sign = 993,

        /// <summary>
        /// 最佳回答
        /// </summary>
        BestAnswer = 992,

        /// <summary>
        /// 优秀回答
        /// </summary>
        NiceAnswer = 991,

        /// <summary>
        /// VIP分 充值
        /// </summary>
        PayVipScore = 990,

        /// <summary>
        /// 分享注册
        /// </summary>
        ShareRegist = 989,

        /// <summary>
        /// 发表新帖子赠积分
        /// </summary>
        OnPublishBBS = 988,

        /// <summary>
        /// 发表新文章赠积分
        /// </summary>
        OnPublishArticle = 987,

        /// <summary>
        /// 有用户留言时
        /// </summary>
        OnLeaveWord = 986,

        /// <summary>
        /// 自动回复
        /// </summary>
        OnAutoReply = 985,

        /// <summary>
        /// 给评论/回复 点 反对
        /// </summary>
        Comment_Against = 984,

        /// <summary>
        /// 举报通知
        /// </summary>
        Report = 983,

        /// <summary>
        /// 待发货通知
        /// </summary>
        ToSend = 982,

        /// <summary>
        /// 用户购买问题或文章里的积分内容时提醒 用户
        /// </summary>
        BuyBBSOrArticle_Notice_Buyer = 981,

        /// <summary>
        /// 用户购买问题或文章里的积分内容时提醒 作者
        /// </summary>
        BuyBBSOrArticle_Notice_Author = 980,

        /// <summary>
        /// 帖子待审核
        /// </summary>
        BBS_To_Check = 979,

        /// <summary>
        /// 文章待审核
        /// </summary>
        Article_To_Check = 978,

        /// <summary>
        /// 活动待审核
        /// </summary>
        Party_To_Check = 977,
        /// <summary>
        /// 赠送积分通知管理员
        /// </summary>
        Give_score_notice_root = 976,
        /// <summary>
        /// 赠送积分通知用户
        /// </summary>
        Give_score_notice_user = 975,

        /// <summary>
        /// 连续签到赠积分
        /// </summary>
        Sign_EnoughNoticeUser = 974,
    }
}
