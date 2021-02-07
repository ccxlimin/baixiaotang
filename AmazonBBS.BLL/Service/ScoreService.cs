using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.BLL
{
    public class ScoreService : IScoreService
    {
        private readonly AmazonBBSDBContext _amazonBBSDBContext;

        public ScoreService(AmazonBBSDBContext amazonBBSDBContext)
        {
            _amazonBBSDBContext = amazonBBSDBContext;
        }

        #region 判断有没有足够的钱
        /// <summary>
        /// 判断有没有足够的钱
        /// </summary>
        /// <param name="type"></param>
        /// <param name="coin"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool HasEnoughCoin(int type, int coin, long userID)
        {
            //bool ok = false;
            var ext = _amazonBBSDBContext.UserExt.FirstOrDefault(a => a.UserID == userID);
            //if (type == 1)
            //{
            //    ok = ext.TotalScore >= coin;
            //}
            //else
            //{
            //    ok = ext.TotalCoin >= coin;
            //}
            //return ok;
            return type == 1 ? ext.TotalScore >= coin : ext.TotalCoin >= coin;
        }
        #endregion

        #region 判断用户是否拥有足够的积分/VIP分 ,并扣除相应金钱 并记录消费流水
        /// <summary>
        /// 判断用户是否拥有足够的积分/VIP分 ,并扣除相应金钱 并记录消费流水
        /// </summary>
        /// <param name="type">1积分 2金钱/VIP分</param>
        /// <param name="coin">数量</param>
        /// <param name="userID">用户ID</param>
        /// <param name="coinSourceEnum">coinSourceEnum</param>
        /// <param name="needSave">是否保存DBContext</param>
        /// <returns></returns>
        public Tuple<bool, string> HasEnoughCoinAndSubCoin(int type, int coin, long userID, CoinSourceEnum coinSourceEnum, bool needSave = false)
        {
            bool ok = false;
            string msg = string.Empty;
            var ext = _amazonBBSDBContext.UserExt.FirstOrDefault(a => a.UserID == userID);
            if (type == 1)
            {
                ok = ext.TotalScore >= coin;
            }
            else
            {
                ok = ext.TotalCoin >= coin;
            }
            if (ok)
            {
                //扣除
                if (type == 1)
                {
                    ext.TotalScore -= coin;
                }
                else
                {
                    ext.TotalCoin -= coin;
                }
                //记录流水
                ScoreCoinLog scorecoinlog = new ScoreCoinLog()
                {
                    UserID = userID,
                    Coin = -coin,
                    CoinSource = coinSourceEnum.GetHashCode(),
                    CoinTime = DateTime.Now,
                    CoinType = type,
                    CreateUser = userID.ToString(),
                    UserName = _amazonBBSDBContext.UserBase.FirstOrDefault(a => a.UserID == userID).UserName,
                };
                _amazonBBSDBContext.ScoreCoinLog.Add(scorecoinlog);
                if (needSave)
                {
                    _amazonBBSDBContext.SaveChanges();
                }
            }
            else
            {
                msg = $"你的{(type == 1 ? "积分" : "金钱")}不足够";
            }
            return new Tuple<bool, string>(ok, msg);
        }
        #endregion

        #region 给用户添加积分-并记录流水
        /// <summary>
        /// 给用户添加积分-并记录流水
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="feeType"></param>
        /// <param name="fee"></param>
        /// <returns></returns>
        public bool AddScoreOrCoin(long userId, int feeType, int fee, CoinSourceEnum coinSourceEnum, bool needSave = false, ScoreBeloneMainEnumType scoreBeloneMainEnumType = ScoreBeloneMainEnumType.None, long mainId = 0)
        {
            DbContextTransaction tran = null;
            if (needSave)
            {
                tran = _amazonBBSDBContext.Database.BeginTransaction();
            }
            //作者增加积分
            var authorExt = _amazonBBSDBContext.UserExt.FirstOrDefault(a => a.UserID == userId);
            if (feeType == 1)
            {
                authorExt.TotalScore += fee;
            }
            else
            {
                authorExt.TotalCoin += fee;
            }
            //添加记录
            var model = _amazonBBSDBContext.ScoreCoinLog.Add(new ScoreCoinLog()
            {
                UserID = userId,
                Coin = fee,
                CoinSource = coinSourceEnum.GetHashCode(),
                CoinTime = DateTime.Now,
                CoinType = feeType,
                CreateUser = userId.ToString(),
                UserName = _amazonBBSDBContext.UserBase.FirstOrDefault(a => a.UserID == userId)?.UserName
            });

            //关联积分对象
            if (mainId > 0 && scoreBeloneMainEnumType != ScoreBeloneMainEnumType.None)
            {
                if (needSave)
                {
                    _amazonBBSDBContext.SaveChanges();
                }
                _amazonBBSDBContext.ScoreBeloneItem.Add(new ScoreBeloneItem
                {
                    CreateTime = DateTime.Now,
                    MainId = mainId,
                    MainType = scoreBeloneMainEnumType.GetHashCode(),
                    ScoreCoinLogId = model.ScoreCoinLogId,
                    UserID = userId
                });
            }

            if (needSave)
            {
                _amazonBBSDBContext.SaveChanges();
                tran.Commit();
            }
            return true;
        }
        #endregion

        #region 发布帖子、文章后 触发 加分 及通知动作
        /// <summary>
        /// 发布帖子、文章后 触发 加分 及通知动作
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="coinSourceEnum"></param>
        /// <returns></returns>
        public void AddScoreOnPublish_BBS_Article(long userId, long mainId, ScoreBeloneMainEnumType scoreBeloneMainEnumType, CoinSourceEnum coinSourceEnum)
        {
            try
            {
                int score = 0;
                string publishTypeName = string.Empty;
                NoticeTypeEnum noticeTypeEnum = NoticeTypeEnum.None;
                if (coinSourceEnum == CoinSourceEnum.NewBBS)
                {
                    publishTypeName = "帖子";
                    score = ConfigHelper.AppSettings("addQuestion").ToInt32();
                    noticeTypeEnum = NoticeTypeEnum.OnPublishBBS;
                }
                else
                {
                    publishTypeName = "文章";
                    score = ConfigHelper.AppSettings("addArticle").ToInt32();
                    noticeTypeEnum = NoticeTypeEnum.OnPublishArticle;
                }

                //int coinScurceType = coinSourceEnum.GetHashCode();
                DateTime start__ = DateTime.Now.Date;
                DateTime end__ = DateTime.Now;
                var coinSource1 = CoinSourceEnum.NewBBS.GetHashCode();
                var coinSource2 = CoinSourceEnum.NewArticle.GetHashCode();

                //bool canAdd = true;
                //判断 今日是否达到 上限
                var count__ = _amazonBBSDBContext.ScoreCoinLog.Count(a => a.UserID == userId && a.CoinType == 1 && (a.CoinSource == coinSource1 || a.CoinSource == coinSource2) && a.CoinTime >= start__ && a.CoinTime <= end__);
                if (count__ < ConfigHelper.AppSettings("DayInAddScoreCount").ToInt32())
                {
                    if (AddScoreOrCoin(userId, 1, score, coinSourceEnum, true, scoreBeloneMainEnumType, mainId))
                    {
                        //站内信通知
                        NoticeBLL.Instance.OnPublish_BBS_Article_AddScore_Notice_Author(userId, DateTime.Now, publishTypeName, score.ToString(), noticeTypeEnum);
                    }
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 用户评论 帖子 文章  触发 加分 及通知  动作
        /// <summary>
        /// 用户评论 帖子 文章  触发 加分 及通知  动作
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="coinSourceEnum"></param>
        public void AddScoreOnComment_BBS_Article(long userId, long mainId, ScoreBeloneMainEnumType scoreBeloneMainEnumType, CoinSourceEnum coinSourceEnum)
        {
            try
            {
                int score = 0;
                string publishTypeName = string.Empty;
                NoticeTypeEnum noticeTypeEnum = NoticeTypeEnum.None;
                if (coinSourceEnum == CoinSourceEnum.UserComment_BBS)
                {
                    publishTypeName = "帖子";
                    score = ConfigHelper.AppSettings("commentQuestion").ToInt32();
                    noticeTypeEnum = NoticeTypeEnum.OnPublishBBS;
                }
                else
                {
                    publishTypeName = "文章";
                    score = ConfigHelper.AppSettings("commentArticle").ToInt32();
                    noticeTypeEnum = NoticeTypeEnum.OnPublishArticle;
                }

                int coinSource1 = CoinSourceEnum.UserComment_BBS.GetHashCode();
                int coinSource2 = CoinSourceEnum.UserComment_Article.GetHashCode();
                //bool canAdd = true;
                //判断 今日是否达到 上限

                var start = DateTime.Now.Date;
                var end = DateTime.Now;

                var count__ = _amazonBBSDBContext.ScoreCoinLog.Count(a => a.UserID == userId && a.CoinType == 1 && (a.CoinSource == coinSource2 || a.CoinSource == coinSource1) && a.CoinTime >= start && a.CoinTime <= end);

                if (count__ < ConfigHelper.AppSettings("DayInAddScoreCount").ToInt32())
                {
                    if (AddScoreOrCoin(userId, 1, score, coinSourceEnum, true, scoreBeloneMainEnumType, mainId))
                    {
                        //站内信通知
                        NoticeBLL.Instance.OnComment_BBS_Article_AddScore_Notice_Commenter(userId, DateTime.Now, publishTypeName, score.ToString(), noticeTypeEnum);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        #endregion
    }
}
