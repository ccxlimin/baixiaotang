using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.BLL
{
    public interface INoticeService
    {
        void OnLeaveWord_Notice_Master(long leaveUserId, LeaveWord leaveWord);
        void OnBuySuccess_Notice_AutoReply_Buyer(long buyerUserId, string msg);
        void OnUserBuySuccess_Notice_Master(long buyerUserId, int ordertype, long mainId);
        void OnUserBuy_Content_Success_For_BBS_Arcitle_Notice_BuyerAndAuthor(UserBase buyer, long authorId, long mainId, string title, ContentFeeMainEnumType bbsOrArticle, int coin, int coinType, DateTime time);
        void On_BBS_Article_Publish_Success_Notice(UserBase publisher, string url, string mainTitle, int bbsOrArticle);
        void On_Check_Handled_Notice_Publisher(long publishUserId, string mainMsg, string uri, string mainTitle, string passStatusMsg, DateTime time);

        void OnGiveScoreSuccess_Notice_Root(UserBase masterUser, UserBase giveToUser, int coin, int coinType, DateTime time);
        void OnGiveScoreSuccess_Notice_User(long giveToUserId, int coin, int coinType, DateTime time);

        /// <summary>
        /// 连续签到赠送积分 通知用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enouthSignCount"></param>
        /// <param name="coin"></param>
        /// <param name="isNewUser">是否新用户</param>
        /// <param name="time"></param>
        void OnSign_Enough_Notice_User(long userId, int enouthSignCount, int coin, bool isNewUser, DateTime time);
    }
}
