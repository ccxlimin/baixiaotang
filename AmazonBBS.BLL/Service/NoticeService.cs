using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.BLL
{
    public class NoticeService : INoticeService
    {
        private readonly AmazonBBSDBContext _amazonBBSDBContext;

        public NoticeService(AmazonBBSDBContext amazonBBSDBContext)
        {
            _amazonBBSDBContext = amazonBBSDBContext;
        }

        public void OnLeaveWord_Notice_Master(long masterId, LeaveWord leaveWord)
        {
            NoticeBLL.Instance.OnLeaveWord_Notice_Master(masterId, leaveWord, NoticeTypeEnum.OnLeaveWord);
        }

        public void OnBuySuccess_Notice_AutoReply_Buyer(long buyerUserId, string msg)
        {
            NoticeBLL.Instance.OnBuySuccess_Notice_AutoReply_Buyer(buyerUserId, msg, NoticeTypeEnum.OnAutoReply);
        }

        /// <summary>
        /// 待发货通知管理员
        /// </summary>
        /// <param name="buyerUserId"></param>
        /// <param name="ordertype"></param>
        /// <param name="mainId"></param>
        public void OnUserBuySuccess_Notice_Master(long buyerUserId, int ordertype, long mainId)
        {
            string mainName = string.Empty;
            string url = string.Empty;
            if (ordertype == OrderEnumType.Gift.GetHashCode() || ordertype == OrderEnumType.Data.GetHashCode() || ordertype == OrderEnumType.KeCheng.GetHashCode())
            {
                mainName = _amazonBBSDBContext.Gift.FirstOrDefault(a => a.GiftID == mainId)?.GiftName;
                url = "/gift/detail/" + mainId.ToString();
            }

            NoticeBLL.Instance.OnUserBuySuccess_Notice_Master(_amazonBBSDBContext.UserBase.FirstOrDefault(a => a.UserID == buyerUserId && a.IsDelete == 0), url, mainName, NoticeTypeEnum.ToSend);
        }

        /// <summary>
        /// 用户购买问题或文章里的积分内容时提醒 用户 和作者
        /// </summary>
        /// <param name="buyerId"></param>
        /// <param name="authorId"></param>
        /// <param name="mainId"></param>
        /// <param name="bbsOrArticle"></param>
        /// <param name="coin"></param>
        /// <param name="coinType"></param>
        public void OnUserBuy_Content_Success_For_BBS_Arcitle_Notice_BuyerAndAuthor(UserBase buyer, long authorId, long mainId, string mainTitle, ContentFeeMainEnumType bbsOrArticle, int coin, int coinType, DateTime time)
        {
            string url = (bbsOrArticle == ContentFeeMainEnumType.BBS ? "/bbs/detail/" : "article/detail/") + mainId;
            string coinMsg = coin + (coinType == 1 ? "积分" : "金钱");
            //消耗积分购买帖子内容通知|尊敬的用户您好，您在 {0} 花费 10积分/金钱 成功购买《XXX》内容，已成功扣除 10积分/金钱。
            //消耗积分购买帖子内容通知|尊敬的用户您好，您在 {0} 花费 {1} 成功购买《&lt;a href='{2}' target='_blank' style='color:red;'&gt;{3}&lt;/a&gt;》内容，已成功扣除 {1}。
            NoticeBLL.Instance.OnUserBuy_Content_Success_For_BBS_Arcitle_Notice_Buyer(buyer.UserID, coinMsg, url, mainTitle, time);

            //用户购买帖子内容通知|尊敬的作者您好， XXX 在 2019年 购买您的《XXX》内容，10积分/金钱 已到账，请注意查看。
            //用户购买帖子内容通知|尊敬的作者您好， &lt;a href='/user/detail/{0}' target='_blank' style='color:red;'&gt;{1}&lt;/a&gt; 在 {2} 购买您的《&lt;a href='{3}' target='_blank' style='color:red;'&gt;{4}&lt;/a&gt;》内容，{5} 已到账，请注意查看。
            NoticeBLL.Instance.OnUserBuy_Content_Success_For_BBS_Arcitle_Notice_Author(buyer, authorId, coinMsg, url, mainTitle, time);
        }

        /// <summary>
        /// 发布帖子或文章需要审核时，通知网站管理员 和 作者本人
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="url"></param>
        /// <param name="mainTitle"></param>
        /// <param name="contentFeeMainEnumType">1帖子 2文章  3活动</param>
        public void On_BBS_Article_Publish_Success_Notice(UserBase publisher, string url, string mainTitle, int bbsOrArticleOrParty)
        {
            string msg = bbsOrArticleOrParty == 1 ? "发表的帖子" : bbsOrArticleOrParty == 2 ? "发表的文章" : "发布的活动";
            var noticetype = bbsOrArticleOrParty == 1 ? NoticeTypeEnum.BBS_To_Check :
                bbsOrArticleOrParty == 2 ? NoticeTypeEnum.Article_To_Check : NoticeTypeEnum.Party_To_Check;
            NoticeBLL.Instance.On_BBS_Article_Publish_Success_Notice_Publisher(publisher, msg, mainTitle, url, noticetype);
            NoticeBLL.Instance.On_BBS_Article_Publish_Success_Notice_Master(publisher, msg, mainTitle, url, noticetype);
        }

        /// <summary>
        /// 当审核 帖子、文章、活动 结束时，通知发布用户
        /// </summary>
        /// <param name="publishUserId">作者</param>
        /// <param name="mainMsg">帖子/文章/活动</param>
        /// <param name="uri">路径</param>
        /// <param name="mainTitle">标题</param>
        /// <param name="passStatusMsg">通过/不通过</param>
        /// <param name="time">审核时间</param>
        public void On_Check_Handled_Notice_Publisher(long publishUserId, string mainMsg, string uri, string mainTitle, string passStatusMsg, DateTime time)
        {
            NoticeBLL.Instance.On_Check_Handled_Notice_Publisher(publishUserId, mainMsg, uri, mainTitle, passStatusMsg, time);
        }

        public void OnGiveScoreSuccess_Notice_Root(UserBase masterUser, UserBase giveToUser, int coin, int coinType, DateTime time)
        {
            NoticeBLL.Instance.OnGiveScoreSuccess_Notice_Root(masterUser, giveToUser, coin, coinType, time);
        }

        public void OnGiveScoreSuccess_Notice_User(long giveToUserId, int coin, int coinType, DateTime time)
        {
            NoticeBLL.Instance.OnGiveScoreSuccess_Notice_User(giveToUserId, coin, coinType, time);
        }

        public void OnSign_Enough_Notice_User(long userId, int enouthSignCount, int coin, bool isNewUser, DateTime time)
        {
            NoticeBLL.Instance.OnSign_Enough_Notice_User(userId, enouthSignCount, coin, isNewUser, time);
        }
    }
}
