using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.BLL
{
    public interface IScoreService
    {
        /// <summary>
        /// 判断有没有足够的钱
        /// </summary>
        /// <param name="type"></param>
        /// <param name="coin"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool HasEnoughCoin(int type, int coin, long userID);

        /// <summary>
        /// 判断用户是否拥有足够的积分/VIP分 ,并扣除相应金钱 并记录消费流水
        /// </summary>
        /// <param name="type">1积分 2金钱/VIP分</param>
        /// <param name="coin">数量</param>
        /// <param name="userID">用户ID</param>
        /// <param name="coinSourceEnum">coinSourceEnum</param>
        /// <param name="needSave">是否保存DBContext</param>
        /// <returns></returns>
        Tuple<bool, string> HasEnoughCoinAndSubCoin(int type, int coin, long userID, CoinSourceEnum coinSourceEnum, bool needSave = false);

        /// <summary>
        /// 给用户添加积分-并记录流水
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="feeType"></param>
        /// <param name="fee"></param>
        /// <param name="coinSourceEnum"></param>
        /// <param name="needSave"></param>
        /// <param name="scoreBeloneMainEnumType"></param>
        /// <param name="mainId"></param>
        /// <returns></returns>
        bool AddScoreOrCoin(long userId, int feeType, int fee, CoinSourceEnum coinSourceEnum, bool needSave = false, ScoreBeloneMainEnumType scoreBeloneMainEnumType = ScoreBeloneMainEnumType.None, long mainId = 0);

        /// <summary>
        /// 发布帖子、文章后 触发 加分 及通知动作
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mainId"></param>
        /// <param name="scoreBeloneMainEnumType"></param>
        /// <param name="coinSourceEnum"></param>
        void AddScoreOnPublish_BBS_Article(long userId, long mainId, ScoreBeloneMainEnumType scoreBeloneMainEnumType, CoinSourceEnum coinSourceEnum);

        /// <summary>
        /// 用户评论 帖子 文章  触发 加分 及通知  动作
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mainId"></param>
        /// <param name="scoreBeloneMainEnumType"></param>
        /// <param name="coinSourceEnum"></param>
        void AddScoreOnComment_BBS_Article(long userId, long mainId, ScoreBeloneMainEnumType scoreBeloneMainEnumType, CoinSourceEnum coinSourceEnum);
    }
}
