using AmazonBBS.BLL;
using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Controllers
{
    public class ShareController : BaseController
    {
        // GET: Share
        #region 分享链接
        [HttpGet]
        public ActionResult Index()
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                if (IsLogin)
                {
                    ri.Msg = "您已登录，打开此链接无效！将自动关闭！";
                }
                else
                {
                    string uri = Request.Url.ToString();
                    string uidStr = GetRequest("shareID");
                    string token = GetRequest("shareToken");
                    string openId = GetRequest("shareOpenId");
                    if (MatchHelper.IsNum.IsMatch(uidStr))
                    {
                        long _userID = Convert.ToInt64(uidStr);
                        ShareLink share = ShareLinkBLL.Instance.GetLinkByUserID(_userID);
                        if (share == null)
                        {
                            ri.Msg = "您打开的链接不存在！";
                        }
                        else
                        {
                            if (_userID == share.UserID)
                            {
                                if (token == share.ShareToken)
                                {
                                    UserBase sharer = UserBaseBLL.Instance.GetUserInfo(_userID);
                                    if (sharer == null)
                                    {
                                        ri.Msg = "分享人帐号已被删除！";
                                    }
                                    else
                                    {
                                        //记录链接打开次数
                                        ShareLinkBLL.Instance.AddPVCount(share.ShareLinkID);
                                        ri.Ok = true;
                                        ShareViewModel vmodel = new ShareViewModel()
                                        {
                                            ShareLink = share,
                                            UserInfo = sharer,
                                            ShareID = _userID,
                                            ShareToken = token,
                                        };
                                        ViewBag.ShareViewModel = vmodel;
                                        return View();
                                    }
                                }
                                else
                                {
                                    ri.Msg = "分享链接异常！";
                                }
                            }
                            else
                            {
                                ri.Msg = "分享链接异常！";
                            }
                        }
                    }
                    else
                    {
                        ri.Msg = "分享链接异常！";
                    }
                }
            }
            catch (Exception e)
            {
                ri.Msg = "打开分享链接过程中出现了异常，请重新打开分享链接！";
            }
            if (ri.Ok)
            {
                return View();
            }
            else
            {
                return Content("<script>alert('{0}'||'异常');close();</script>".FormatWith(ri.Msg));
            }
        }
        #endregion

        #region 领取分享奖励
        [LOGIN]
        [HttpPost]
        public ActionResult GetShareCoin(int coin = 0, long coinID = 0)
        {
            ResultInfo ri = new ResultInfo();
            if (coin > 0)
            {
                if (coinID > 0)
                {
                    //查找对应奖励
                    var sharecoin = BBSEnumBLL.Instance.GetItem(coinID);
                    if (sharecoin == null)
                    {
                        ri.Msg = "您领取的对应奖励不存在！";
                    }
                    else
                    {
                        if (sharecoin.EnumDesc == coin.ToString())
                        {
                            //判断当前用户分享的注册用户数是否已达标
                            int registCount = ShareRegistLogBLL.Instance.GetRegistCount(UserID);
                            if (registCount >= sharecoin.SortIndex)
                            {
                                BeginTran();
                                if (UserExtBLL.Instance.AddScore(UserID, coin, 1, Tran))
                                {
                                    if (ScoreCoinLogBLL.Instance.Log(coin, 1, CoinSourceEnum.ShareCoin, UserID, UserInfo.UserName, Tran, coinID))
                                    {
                                        ri.Ok = true;
                                        Commit();
                                    }
                                    else
                                    {
                                        RollBack();
                                    }
                                }
                                else
                                {
                                    RollBack();
                                }
                            }
                            else
                            {
                                ri.Msg = "您未达标，无法领取奖励";
                            }
                        }
                        else
                        {
                            ri.Msg = "奖励分数异常";
                        }
                    }
                }
                else
                {
                    ri.Msg = "奖励分数异常";
                }
            }
            else
            {
                ri.Msg = "奖励分数异常";
            }
            return Result(ri);
        }
        #endregion
    }
}