using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmazonBBS.Model;
using AmazonBBS.BLL;
using AmazonBBS.Common;
using System.Text;
using AmazonBBS.BLL.OAuth2;
using System.Text.RegularExpressions;
using EntityFramework.Extensions;

namespace AmazonBBS.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IScoreService _scoreService;
        public UserController(IUserService userService, IScoreService scoreService)
        {
            _userService = userService;
            _scoreService = scoreService;
        }

        [LOGIN]
        public ActionResult Index(int id = 0)
        {
            return View();
        }

        #region 个人明细
        /// <summary>
        /// 个人明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(string id = null)
        {
            if (id != null)
            {
                UserViewModel u = null;
                if (MatchHelper.IsNum.IsMatch(id))
                {
                    long userId = Convert.ToInt64(id);
                    if (UserID == userId)
                    {
                        return Redirect("/User");
                    }
                    else
                    {
                        try
                        {
                            if (userId > 0)
                            {
                                u = UserBaseBLL.Instance.GetUserDetail(userId, UserID);
                            }
                            else
                            {
                                return RedirectToAction("Index");
                            }
                        }
                        catch
                        {
                            u = UserBaseBLL.Instance.GetUserDetail(id, UserID);
                        }
                    }
                }
                else
                {
                    if (IsLogin && UserInfo.UserName == id)
                    {
                        return Redirect("/User");
                    }
                    else
                    {
                        u = UserBaseBLL.Instance.GetUserDetail(id, UserID);
                    }
                }
                if (u == null)
                {
                    return Content("您访问的用户不存在！<a href='/'>返回首页</a>");
                }
                else
                {
                    ViewBag.UserSet = _userService.GetUserSet(u.UserID);
                    return View(u);
                }
            }
            return Redirect("/");
        }
        #endregion

        #region 我的

        #region 皮肤设置
        [HttpPost]
        [LOGIN]
        public ActionResult Skin(string skin)
        {
            ResultInfo ri = new ResultInfo();
            if (skin.IsNotNullOrEmpty())
            {
                if (UserExtBLL.Instance.UpdateSkin(UserID, skin))
                {
                    CSharpCacheHelper.Set("skin-{0}".FormatWith(UserID), skin);
                    ri.Ok = true;
                }
                else
                {
                    ri.Msg = "设置皮肤失败！";
                }
            }
            else
            {
                ri.Msg = "设置皮肤失败！";
            }
            return Result(ri);
        }
        #endregion

        #region 对话记录
        /// <summary>
        /// 对话记录
        /// </summary>
        /// <param name="id">聊天对象的USERID</param>
        /// <returns></returns>
        [LOGIN]
        public ActionResult Dialog(long id)
        {
            //判断用户是否存在
            if (UserBaseBLL.Instance.GetUserInfo(id) != null)
            {

                //初始加载20条对话
                MyMessage myMessage = new MyMessage()
                {
                    ChatPage = InitPage(20)
                };

                myMessage.MeID = UserID;
                myMessage.ChatList = ChatBLL.Instance.GetDialogByUserID(myMessage.ChatPage, UserID, id);
                if (myMessage.ChatPage.PageIndex == 1)
                {
                    myMessage.ToUserName = UserBaseBLL.Instance.GetUserNameByUserID(id);
                    myMessage.ToID = id;

                    //置已读
                    ChatBLL.Instance.Read(id, UserID);

                    return View(myMessage);
                }
                else
                {
                    return PartialView("_Dialogs", myMessage);
                }
            }
            else
            {
                return RedirectToAction("Inbox", "User");
            }
        }
        #endregion

        #region 消息
        /// <summary>
        /// 消息
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        public ActionResult InBox()
        {
            ////获取当前所有消息(分页加载20条)
            //MyMessage myMessage = new MyMessage();
            //myMessage.ChatPage = InitPage(20);
            //myMessage.ChatList = ChatBLL.Instance.GetMyMessage(myMessage.ChatPage, UserID);
            //return View(myMessage);

            //获取当前所有消息(分页加载20条)
            MyMessage myMessage = new MyMessage();
            myMessage.ChatPage = InitPage(20);
            myMessage.ChatList = ChatBLL.Instance.GetMyMessage(myMessage.ChatPage, UserID);
            if (myMessage.ChatPage.PageIndex == 1)
            {
                return View(myMessage);
            }
            else
            {
                return PartialView("_InBoxs", myMessage);
            }
        }
        #endregion

        #region 通知
        /// <summary>
        /// 通知
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        public ActionResult Notice()
        {
            //获取当前所有通知
            MyNotice mynotice = new MyNotice();
            mynotice.NoticePage = InitPage(20);
            mynotice.NoticeList = NoticeBLL.Instance.GetMyNotice(mynotice.NoticePage, UserID);
            if (mynotice.NoticePage.PageIndex == 1)
            {
                return View(mynotice);
            }
            else
            {
                return PartialView("_RenderNotice", mynotice);
            }
        }
        #endregion

        #region 分享
        [LOGIN]
        public ActionResult Share()
        {
            //判断是否已存在分享链接
            //若不存在则创建新的
            ShareLink model = ShareLinkBLL.Instance.GetLinkByUserID(UserID);
            if (model == null)
            {
                //创建
                model = new ShareLink()
                {
                    UserID = UserID,
                    ShareMd5Key = StringHelper.GetNickName(8),
                    ShareOpenId = StringHelper.GetNickName(12),
                };
                model.ShareToken = DESEncryptHelper.Encrypt(model.ShareOpenId + UserID.ToString(), model.ShareMd5Key);
                model.ShareAddress = Request.Url.Authority + "?from{0}&OpenId={1}&Token={2}".FormatWith("share", model.ShareOpenId, model.ShareToken);
                ShareLinkBLL.Instance.Add(model);
            }
            return View(model);
        }
        #endregion

        #region 我的分享链接
        /// <summary>
        /// 我的分享链接
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        public ActionResult MyShare()
        {
            MyShareViewModel myshare = new MyShareViewModel();
            ShareLink share = ShareLinkBLL.Instance.GetLinkByUserID(UserID);
            if (share == null)
            {
                //创建链接
                string shareOpenId = StringHelper.GetNickName(18);
                string shareMd5Key = StringHelper.GetNickName(8);
                string shareToken = DESEncryptHelper.Encrypt(UserID + shareOpenId, shareMd5Key);
                share = new ShareLink()
                {
                    ShareMd5Key = shareMd5Key,
                    UserID = UserID,
                    ShareOpenId = shareOpenId,
                    ShareToken = shareToken,
                    ShareAddress = "{0}/Share?shareOpenId={1}&shareToken={2}&shareID={3}".FormatWith(GetDomainName, shareOpenId, shareToken, UserID)
                };
                ShareLinkBLL.Instance.Add(share);
                myshare.IsFirst = true;
            }
            else
            {
                int count = 0;
                myshare.ShareCoinList = BBSEnumBLL.Instance.GetShareCoinList(UserID, ref count);
                myshare.RegistCount = count;
            }
            myshare.ShareLink = share;
            return View(myshare);
        }
        #endregion

        #region 更换头像
        /// <summary>
        /// 更换头像
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult Head(string head)
        {
            ResultInfo ri = new ResultInfo();
            if (head.IsNotNullOrEmpty())
            {
                ri.Ok = UserBaseBLL.Instance.UploadHeadUrl(head, UserID);
                var user = UserInfo;
                user.HeadUrl = head;
                SetLogin(user);
            }
            return Result(ri);
        }
        #endregion

        #region 更换性别
        /// <summary>
        /// 更换性别
        /// </summary>
        /// <param name="gender">1 男 2 女(没有其它选项)</param>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult SetGender(int gender)
        {
            ResultInfo ri = new ResultInfo();
            var user = UserInfo;
            if (user.Gender == gender)
            {
                ri.Msg = "性别没有变化，无需更改";
            }
            else if (gender == 1 || gender == 2)
            {
                ri.Ok = UserBaseBLL.Instance.UpdateGender(user.UserID, gender);
                user.Gender = gender;
                SetLogin(user);
            }
            else
            {
                ri.Msg = "性别不存在，请重新选择更改";
            }
            return Result(ri);
        }
        #endregion

        #region 设置出生年月日
        /// <summary>
        /// 设置出生年月日
        /// </summary>
        /// <param name="birth"></param>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult SetBirth(string birth)
        {
            ResultInfo ri = new ResultInfo();
            var user = UserInfo;
            if (user.Birth == birth)
            {
                ri.Msg = "出生年月日没有变化，无需更改";
            }
            else
            {
                ri.Ok = UserBaseBLL.Instance.UpdateBirth(user.UserID, birth);
                user.Birth = birth;
                SetLogin(user);
            }
            return Result(ri);
        }
        #endregion

        #region 设置所在地
        /// <summary>
        /// 设置所在地
        /// </summary>
        /// <param name="birth"></param>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult SetAreas(string province, string city = null, string county = null)
        {
            ResultInfo ri = new ResultInfo();
            var user = UserInfo;
            if (user.Province == province && user.City == city && user.County == county)
            {
                ri.Msg = "省、市、县都没有变化，无需更改";
            }
            else
            {
                ri.Ok = UserBaseBLL.Instance.UpdateAreas(user.UserID, province, city, county);
                user.Province = province;
                user.City = city;
                user.County = county;
                SetLogin(user);
            }
            return Result(ri);
        }
        #endregion

        #region 设置经营类目
        [LOGIN]
        [HttpPost]
        public ActionResult MyJingYing(string v)
        {
            ResultInfo ri = new ResultInfo();
            if (v.IsNotNullOrEmpty())
            {
                if (DB.UserBase.Update(a => a.UserID == UserID, b => new UserBase { JingYing = v }) > 0)
                {
                    ri.Ok = true;
                    ri.Msg = "设置成功";
                    var user = DB.UserBase.FirstOrDefault(a => a.UserID == UserID);
                    user.JingYing = v;
                    SetLogin(user);
                }
            }
            else
            {
                ri.Msg = "经营类目不能为空";
            }
            return Json(ri);
        }
        #endregion

        #region 更改工作年限
        [LOGIN]
        [HttpPost]
        public ActionResult MyWorkYear(string v)
        {
            ResultInfo ri = new ResultInfo();
            if (v.IsNotNullOrEmpty())
            {
                if (DB.UserBase.Update(a => a.UserID == UserID, b => new UserBase { WorkYear = v }) > 0)
                {
                    ri.Ok = true;
                    ri.Msg = "设置成功";
                    var user = DB.UserBase.FirstOrDefault(a => a.UserID == UserID);
                    user.WorkYear = v;
                    SetLogin(user);
                }
            }
            else
            {
                ri.Msg = "经营类目不能为空";
            }
            return Json(ri);
        }
        #endregion

        #region 设置是否隐藏个人资料
        /// <summary>
        /// 设置是否隐藏个人资料
        /// </summary>
        /// <param name="value">true 展示 false 隐藏 </param>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult SetInfoHideOrShow(bool value)
        {
            ResultInfo ri = new ResultInfo();
            var set = _userService.GetUserSet(UserID);
            set.ShowOrHideBaseInfo = value;
            DB.SaveChanges();
            ri.Ok = true;
            ri.Msg = "设置成功";

            return Json(ri);
        }
        #endregion

        #region 第三方绑定
        /// <summary>
        /// 第三方绑定
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        public ActionResult BindOauth()
        {
            StringBuilder sb = new StringBuilder();
            if (UserInfo.Source == 2)
            {
                List<OAuth> oAuthList = OAuthBLL.Instance.GetALLByUserID(UserID);
                var qqOauth = oAuthList.FirstOrDefault(item => { return item.OAuthType == RegistEnumType.QQAuth.GetHashCode(); });
                if (qqOauth == null || qqOauth.IsBind == 0)
                {
                    string qqopenid = ConfigHelper.AppSettings("QQ.AppID");
                    sb.Append("<script type=\"text/javascript\" src=\"http://qzonestyle.gtimg.cn/qzone/openapi/qc_loader.js\" data-appid=\"{0}\" charset=\"utf-8\"></script>".FormatWith(qqopenid));
                    sb.Append("<span id=\"qqLoginBtn\"></span>");
                }
                else
                {
                    sb.Append("<script>var QC=null;</script>");
                    sb.Append("<div class='well'>已绑定QQ帐号，<a class='btn btn-default' onclick='UnBindOAuthQQ()'>点击解绑</a></div>".FormatWith(UserID));
                    sb.Append(@"<script>
                                    function UnBindOAuthQQ(){
                                        $.post('/Account/UnBindOAuth/" + UserID.ToString() + @"',{type:1},function(data){
                                            if(data.Ok){Leo.msgsuccess(data.Msg||'解绑成功');}else{Leo.msgfail(data.Msg||'解绑失败');}setTimeout(function(){location.reload(!0);},400);
                                        })
                                    }
                                </script>");
                }
                sb.Append("<br />");
                var wechat = oAuthList.FirstOrDefault(item => { return item.OAuthType == RegistEnumType.WeChatAuth.GetHashCode(); });
                if (wechat == null || wechat.IsBind == 0)
                {
                    ViewBag.WXCallback = ConfigHelper.AppSettings("WeiXin.CallBackUrl");
                    ViewBag.WXAppID = ConfigHelper.AppSettings("WeiXin.AppID");
                    string wechatAppID = ConfigHelper.AppSettings("WeiXin.AppID");
                    string wechatCallback = ConfigHelper.AppSettings("WeiXin.CallBackUrl");
                    string state = new WeChatBLL().GetState();
                    var wechatURI = $@"https://open.weixin.qq.com/connect/qrconnect?appid={wechatAppID}&redirect_uri={wechatCallback}?ReturnUrl=http://www.baixiaotangtop.com/User/BindOauth&response_type=code&scope=snsapi_login&state={state}#wechat_redirect";
                    sb.Append("<span id=\"wxLoginBtn\"><a href=\"{0}\"><img src=\"/Content/img/icon24_wx_button.png\" /></a></span>".FormatWith(wechatURI));
                }
                else
                {
                    sb.Append("<div class='well'>已绑定微信帐号，<a class='btn btn-default' onclick='UnBindOAuthWeChat()'>点击解绑</a></div>".FormatWith(UserID));
                    sb.Append(@"<script>
                                    function UnBindOAuthWeChat(){
                                        $.post('/Account/UnBindOAuth/" + UserID.ToString() + @"',{type:3},function(data){
                                            if(data.Ok){Leo.msgsuccess(data.Msg||'解绑成功');}else{Leo.msgfail(data.Msg||'解绑失败');}setTimeout(function(){location.reload(!0);},400);
                                        })
                                    }
                                </script>");
                }
            }
            else if (UserInfo.Source != 4)
            {
                sb.Append("您不是邮箱登录用户，请点击<a href='/Account/LoginOauth?from=userbind'>安全绑定</a>进行邮箱帐号绑定！绑定之后所有数据（积分、帖子、文章等）均以邮箱帐号为准！谢谢！");
            }
            ViewBag.Html = sb;
            return View();
        }
        #endregion

        #region 切换头衔显示
        /// <summary>
        /// 切换头衔显示
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [LOGIN]
        public ActionResult SetHeadNameShow(int id)
        {
            ResultInfo ri = new ResultInfo();
            if (id == 1 || id == 2)
            {
                UserExt user = UserExtBLL.Instance.GetExtInfo(UserID);
                if (user.HeadNameShowType == id)
                {
                    ri.Msg = "您已设置过，无需重复设置";
                }
                else
                {
                    user.HeadNameShowType = id;
                    ri = UserExtBLL.Instance.Update(user);
                }
            }
            else
            {
                ri.Msg = "切换失败";
            }
            return Result(ri);
        }
        #endregion

        #region 购买VIP会员专属
        /// <summary>
        /// 购买VIP会员专属
        /// </summary>
        /// <returns></returns>
        //[LOGIN]
        //[HttpPost]
        //public ActionResult BuyVIP(int id)
        //{
        //    ResultInfo ri = new ResultInfo();
        //    if (id > 0)
        //    {
        //        try
        //        {
        //            BeginTran();
        //            UserExt userext = UserExtBLL.Instance.GetExtInfo(UserID);
        //            //vip等级累计(失效时计为负的，如当前VIP=3，会员过期后，计为VIP=-3，方便以后恢复VIP等级)
        //            userext.VIP += id;
        //            //VIP有效时间往后累计
        //            if (userext.VIPExpiryTime < DateTime.Now || userext.VIPExpiryTime == null)
        //            {
        //                //续费的，当前时间计算
        //                userext.VIPExpiryTime = DateTime.Now.AddDays(id * 30);
        //            }
        //            else
        //            {
        //                userext.VIPExpiryTime = Convert.ToDateTime(userext.VIPExpiryTime).AddDays(id * 30);
        //            }
        //            if (UserExtBLL.Instance.Update(userext, Tran).Ok)
        //            {
        //                //扣费
        //                int fee = Convert.ToInt32(ConfigHelper.AppSettings("BuyVIPFeeCount"));
        //                int totalCoin = fee * id;
        //                //判断是否足够
        //                if (userext.TotalCoin >= totalCoin)
        //                //if (UserExtBLL.Instance.HasEnoughCoin(2, totalCoin, UserID))
        //                {
        //                    if (UserExtBLL.Instance.SubScore(UserID, totalCoin, 2, Tran))
        //                    {
        //                        if (ScoreCoinLogBLL.Instance.Log(-totalCoin, 2, CoinSourceEnum.BuyVIP, UserID, UserInfo.UserName, Tran))
        //                        {
        //                            ri.Ok = true;
        //                            Commit();
        //                        }
        //                        else
        //                        {
        //                            RollBack();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        RollBack();
        //                    }
        //                }
        //                else
        //                {
        //                    RollBack();
        //                }
        //            }
        //            else
        //            {
        //                RollBack();
        //            }
        //        }
        //        catch
        //        {
        //            ri.Msg = "会员充值失败";
        //            RollBack();
        //        }
        //    }
        //    else
        //    {
        //        ri.Msg = "至少买一个月吧？";
        //    }
        //    return Result(ri);
        //}
        #endregion

        #region 购买VIP会员专属
        /// <summary>
        /// 购买VIP会员专属
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult BuyVIP(int id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                try
                {
                    UserExt userext = UserExtBLL.Instance.GetExtInfo(UserID);
                    //计算消费总计
                    int fee = Convert.ToInt32(ConfigHelper.AppSettings("BuyVIPFeeCount"));
                    int totalCoin = fee * id;
                    //判断是否足够
                    if (userext.TotalCoin >= totalCoin)
                    {
                        BeginTran();
                        //if (UserExtBLL.Instance.SubScore(UserID, totalCoin, 2, Tran))
                        //{
                        if (ScoreCoinLogBLL.Instance.Log(-totalCoin, 2, CoinSourceEnum.BuyVIP, UserID, UserInfo.UserName, Tran))
                        {
                            //扣费完成后更新个人信息
                            ////vip等级累计(失效时计为负的，如当前VIP=3，会员过期后，计为VIP=-3，方便以后恢复VIP等级)
                            userext.VIP = Math.Abs(userext.VIP.Value) + id;
                            //VIP有效时间往后累计
                            if (userext.VIPExpiryTime < DateTime.Now || userext.VIPExpiryTime == null)
                            {
                                //续费的，当前时间计算
                                userext.VIPExpiryTime = DateTime.Now.AddDays(id * 30);
                            }
                            else
                            {
                                userext.VIPExpiryTime = Convert.ToDateTime(userext.VIPExpiryTime).AddDays(id * 30);
                            }
                            userext.TotalCoin -= totalCoin;
                            if (UserExtBLL.Instance.Update(userext, Tran).Ok)
                            {
                                ri.Ok = true;
                                Commit();
                                //购买成功，更新缓存
                                CSharpCacheHelper.Remove("isVip_" + UserID.ToString());
                            }
                            else
                            {
                                ri.Msg = "购买会员失败";
                                RollBack();
                            }
                        }
                        else
                        {
                            ri.Msg = "购买会员失败";
                            RollBack();
                        }
                        //}
                        //else
                        //{
                        //    ri.Msg = "购买会员失败";
                        //    RollBack();
                        //}
                    }
                    else
                    {
                        ri.Msg = "您的VIP分不足，请前往个人中心充值";
                        ri.Url = "/User/ReChargeVIPScore";
                        ri.ID = 1;
                        RollBack();
                    }
                }
                catch
                {
                    ri.Msg = "会员充值失败";
                    RollBack();
                }
            }
            else
            {
                ri.Msg = "至少买一个月吧？";
            }
            return Result(ri);
        }
        #endregion

        #region 用户法人认证申请
        [LOGIN]
        public ActionResult UserAuth()
        {
            return View();
        }

        /// <summary>
        /// 用户法人认证申请
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [LOGIN]
        public ActionResult Auth(string truename, string card, string companyname, string companyTel)
        {
            ResultInfo ri = new ResultInfo();
            if (truename.IsNotNullOrEmpty())
            {
                if (card.IsNotNullOrEmpty())
                {
                    if (companyname.IsNotNullOrEmpty())
                    {
                        if (companyTel.IsNotNullOrEmpty())
                        {
                            UserExt userext = UserExtBLL.Instance.GetExtInfo(UserID);
                            ri = UpLoadImg("CardPic", "/Content/U/Auth/Card");
                            if (ri.Ok)
                            {
                                userext.CardPic = ri.Url;
                                ri = UpLoadImg("FaRenPic", "/Content/U/Auth/FaRen");
                                if (ri.Ok)
                                {
                                    userext.FaRenPic = ri.Url;
                                    userext.RealName = truename;
                                    userext.CompanyName = companyname;
                                    userext.CompanyTel = companyTel;
                                    userext.CardID = card;

                                    #region 重置法人认证状态
                                    int userV = userext.UserV.Value;
                                    if (userV != 0)
                                    {
                                        if (userV == 3)
                                        {
                                            userext.UserV = 0;
                                        }
                                        if (userV == 5)
                                        {
                                            userext.UserV = 2;
                                        }
                                        if (userV == 6)
                                        {
                                            userext.UserV = 1;
                                        }
                                    }
                                    #endregion
                                    ri = UserExtBLL.Instance.Update(userext);
                                }
                            }
                        }
                        else
                        {
                            ri.Msg = "公司联系电话不能为空";
                        }
                    }
                    else
                    {
                        ri.Msg = "公司名称不能为空";
                    }
                }
                else
                {
                    ri.Msg = "身份证号不能为空";
                }
            }
            else
            {
                ri.Msg = "姓名不能为空";
            }
            return Result(ri);
        }
        #endregion

        #region 兑换
        [LOGIN]
        public ActionResult ScoreExchange()
        {
            ViewBag.ScoreExchange = ConfigHelper.AppSettings("scoreChangeToVipScore");
            ViewBag.BuyVIPFeeCount = ConfigHelper.AppSettings("BuyVIPFeeCount");

            return View();
        }

        #region 积分兑换为VIP分
        /// <summary>
        /// 积分兑换为VIP分
        /// </summary>
        /// <param name="id">积分数量</param>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult Score2VipScore(int id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                int scoreConfigValue = Convert.ToInt32(ConfigHelper.AppSettings("scoreChangeToVipScore"));
                UserExt ext = UserExtBLL.Instance.GetExtInfo(UserID);
                if (ext.TotalScore < id)
                {
                    ri.Msg = "可供兑换积分不足";
                }
                else
                {
                    BeginTran();
                    //先扣除积分
                    if (UserExtBLL.Instance.SubScore(UserID, id, 1, Tran))
                    {
                        //记录积分明细
                        ScoreCoinLog model = new ScoreCoinLog()
                        {
                            Coin = -id,
                            CoinSource = CoinSourceEnum.Score2VipScore.GetHashCode(),
                            CoinType = CoinTypeEnum.Score.GetHashCode(),
                            CoinTime = DateTime.Now,
                            CreateUser = UserID.ToString(),
                            UserID = UserID,
                            UserName = UserInfo.UserName,
                        };
                        if (ScoreCoinLogBLL.Instance.Add(model, Tran) > 0)
                        {
                            //增加相应金钱
                            int coin_ = id / scoreConfigValue;
                            if (UserExtBLL.Instance.AddScore(UserID, coin_, 2, Tran))
                            {
                                //记录金钱明细
                                ScoreCoinLog model2 = new ScoreCoinLog()
                                {
                                    Coin = coin_,
                                    CoinSource = CoinSourceEnum.Score2VipScore.GetHashCode(),
                                    CoinType = CoinTypeEnum.Money.GetHashCode(),
                                    CoinTime = DateTime.Now,
                                    CreateUser = UserID.ToString(),
                                    UserID = UserID,
                                    UserName = UserInfo.UserName,
                                };
                                if (ScoreCoinLogBLL.Instance.Add(model2, Tran) > 0)
                                {
                                    ri.Ok = true;
                                    Commit();
                                }
                                else
                                {
                                    ri.Msg = "兑换失败";
                                    RollBack();
                                }
                            }
                            else
                            {
                                ri.Msg = "兑换失败";
                                RollBack();
                            }
                        }
                        else
                        {
                            ri.Msg = "兑换失败";
                            RollBack();
                        }
                    }
                    else
                    {
                        ri.Msg = "兑换失败";
                        RollBack();
                    }
                }
            }
            else
            {
                ri.Msg = "兑换个数不能为负";
            }

            return Result(ri);
        }
        #endregion

        #region VIP分兑换为积分
        /// <summary>
        /// VIP分兑换为积分
        /// </summary>
        /// <param name="id">VIP分数量</param>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult VipScore2Score(int id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                try
                {
                    BeginTran();
                    int scoreConfigValue = Convert.ToInt32(ConfigHelper.AppSettings("scoreChangeToVipScore"));
                    UserExt ext = UserExtBLL.Instance.GetExtInfo(UserID);
                    if (ext.TotalCoin < id)
                    {
                        ri.Msg = "可供兑换VIP分不足";
                    }
                    else
                    {
                        //先扣除金钱
                        if (UserExtBLL.Instance.SubScore(UserID, id, 2, Tran))
                        {
                            //记录积分明细
                            ScoreCoinLog model = new ScoreCoinLog()
                            {
                                Coin = -id,
                                CoinSource = CoinSourceEnum.VipScore2Score.GetHashCode(),
                                CoinType = CoinTypeEnum.Money.GetHashCode(),
                                CoinTime = DateTime.Now,
                                CreateUser = UserID.ToString(),
                                UserID = UserID,
                                UserName = UserInfo.UserName,
                            };
                            if (ScoreCoinLogBLL.Instance.Add(model, Tran) > 0)
                            {

                                //增加相应积分
                                int coin_ = id * scoreConfigValue;
                                if (UserExtBLL.Instance.AddScore(UserID, coin_, 1, Tran))
                                {
                                    //记录金钱明细
                                    ScoreCoinLog model2 = new ScoreCoinLog()
                                    {
                                        Coin = coin_,
                                        CoinSource = CoinSourceEnum.Score2VipScore.GetHashCode(),
                                        CoinType = CoinTypeEnum.Score.GetHashCode(),
                                        CoinTime = DateTime.Now,
                                        CreateUser = UserID.ToString(),
                                        UserID = UserID,
                                        UserName = UserInfo.UserName,
                                    };
                                    if (ScoreCoinLogBLL.Instance.Add(model2, Tran) > 0)
                                    {
                                        ri.Ok = true;
                                        Commit();
                                    }
                                    else
                                    {
                                        RollBack();
                                        ri.Msg = "异常";
                                    }
                                }
                            }
                            else
                            {
                                ri.Msg = "异常";
                                RollBack();
                            }
                        }
                        else
                        {
                            ri.Msg = "异常";
                            RollBack();
                        }
                    }
                }
                catch
                {
                    RollBack();
                    ri.Msg = "异常";
                }
            }
            else
            {
                ri.Msg = "兑换个数不能为负";
            }
            return Result(ri);
        }
        #endregion 
        #endregion

        #region VIP分充值
        [LOGIN]
        public ActionResult ReChargeVIPScore()
        {
            return View();
        }
        #endregion

        #region 编辑
        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        public ActionResult Edit()
        {
            ViewBag.UserSet = _userService.GetUserSet(UserID);
            return View();
        }
        #endregion

        #region 我的问题
        /// <summary>
        /// 我的问题
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        public ActionResult MyQuestions()
        {
            var bmodel = GetMyInfoByTargetType<BBSListViewModel>(1);
            return View(bmodel);
        }
        #endregion

        #region 我的回答
        /// <summary>
        /// 我的回答
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        public ActionResult MyAnswers()
        {
            //MyCommentsViewModel amodel = CommentBLL.Instance.GetCommentListByUserid(UserID, CommentEnumType.BBS, InitPage(), UserID);
            var amodel = GetMyInfoByTargetType<MyCommentsViewModel>(2, UserID);
            return View(amodel);
        }
        #endregion

        #region 我的文章
        /// <summary>
        /// 我的文章
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        public ActionResult MyArticle()
        {
            //MyArticleViewModel amodel = ArticleBLL.Instance.GetArticleListByUserid(UserID, InitPage());
            var amodel = GetMyInfoByTargetType<MyArticleViewModel>(6);
            return View(amodel);
        }
        #endregion

        #region 我的招聘
        [LOGIN]
        public ActionResult MyZhaoPin()
        {
            //BaseListViewModel<ZhaoPin> zhaopins = ZhaoPinBLL.Instance.GetZhaoPinList(UserID, InitPage());
            var zhaopins = GetMyInfoByTargetType<BaseListViewModel<ZhaoPin>>(3);
            return View(zhaopins);
        }
        #endregion

        #region 我的求职
        [LOGIN]
        public ActionResult MyQiuZhi()
        {
            //BaseListViewModel<QiuZhi> qiuzhis = QiuZhiBLL.Instance.GetQiuZhiList(UserID, InitPage());
            var qiuzhis = GetMyInfoByTargetType<BaseListViewModel<QiuZhi>>(4);
            return View(qiuzhis);
        }
        #endregion

        #region 我的产品
        [LOGIN]
        public ActionResult MyProduct()
        {
            //BaseListViewModel<Product> products = ProductBLL.Instance.GetProductList(UserID, InitPage());
            var products = GetMyInfoByTargetType<BaseListViewModel<Product>>(5);
            return View(products);
        }
        #endregion

        #region 我的关注
        /// <summary>
        /// 我的关注
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [LOGIN]
        public ActionResult MyLike(string id = "user")
        {
            if (id != null)
            {
                MyLikeViewModel model = null;
                model = GetMyInfoByTargetType<MyLikeViewModel>(7, likeType: id.ToLower());
                return View(model);
            }
            return Redirect("/User");
        }
        #endregion

        #region 我的粉丝
        /// <summary>
        /// 我的粉丝
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        public ActionResult MyFans()
        {
            List<FansViewModel> fans = UserLikeBLL.Instance.GetFansByUserID(UserID, 3);
            return View(fans);
        }
        #endregion

        #region 加载数据
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult LoadMyPublish(int id)
        {
            if (id == 1)
            {
                return PartialView("_LoadMyQuestionList", GetMyInfoByTargetType<BBSListViewModel>(id));
            }
            else if (id == 2)
            {
                return PartialView("_LoadMyAnswerList", GetMyInfoByTargetType<MyCommentsViewModel>(id, UserID));
            }
            else if (id == 3)
            {
                return PartialView("_LoadMyZhaoPinList", GetMyInfoByTargetType<BaseListViewModel<ZhaoPin>>(id));
            }
            else if (id == 4)
            {
                return PartialView("_LoadMyQiuZhiList", GetMyInfoByTargetType<BaseListViewModel<QiuZhi>>(id));
            }
            else if (id == 5)
            {
                return PartialView("_LoadMyProductList", GetMyInfoByTargetType<BaseListViewModel<Product>>(id));
            }
            else if (id == 6)
            {
                return PartialView("_LoadMyArticleList", GetMyInfoByTargetType<MyArticleViewModel>(id));
            }
            else if (id == 7)
            {
                return PartialView("_LoadMyLikeList", GetMyInfoByTargetType<MyLikeViewModel>(id, likeType: GetRequest("tab", "user").ToLower()));
            }
            return PartialView();
        }
        #endregion

        #region 个性签名
        /// <summary>
        /// 个性签名
        /// </summary>
        /// <param name="sign"></param>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult Sign(string sign)
        {
            ResultInfo ri = new ResultInfo();
            if (string.IsNullOrEmpty(sign))
            {
                ri.Msg = "个性签名不能为空"
;
            }
            else
            {
                if (UserBaseBLL.Instance.UpdateSign(UserID, sign))
                {
                    ri.Ok = true;
                    ri.Msg = "个性签名修改成功";
                    var user = UserInfo;
                    user.Sign = sign;
                    SetLogin(user);
                }
                else
                {
                    ri.Msg = "个性签名修改失败";
                }
            }
            return Json(ri, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 判断是否有足够的钱
        [LOGIN]
        [HttpGet]
        public ActionResult EnoughMoney(int fee, int feetype)
        {
            ResultInfo ri = new ResultInfo();
            var ok = _scoreService.HasEnoughCoin(feetype, fee, UserID);
            if (ok)
            {
                ri.Data = true;
            }
            else
            {
                ri.Msg = "余额不足";
            }
            ri.Ok = true;
            return Result(ri);
        }
        #endregion

        #region 网站订单

        public ActionResult Order()
        {
            return View();
        }

        /// <summary>
        /// 获取全部订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LoadAllOrder()
        {
            //区分 人民币购买的 和 积分 购买的， 在 收货表 和 发货表 里字段不一样 
            ResultInfo ri = new ResultInfo();
            int ordertype1 = OrderEnumType.Gift.GetHashCode();
            int ordertype2 = OrderEnumType.Data.GetHashCode();
            int ordertype3 = OrderEnumType.KeCheng.GetHashCode();
            var time = DateTime.Now.AddHours(12).ToString(2);
            //获取购买礼物、数据、课程的订单
            ri.Data = DB.UserGift.Where(a => a.BuyUserID == UserID).OrderByDescending(a => a.BuyTime).Select(a => new
            {
                tel = a.LinkTel,//联系电话
                receiveName = a.LinkMan,//联系人
                buyTime = a.BuyTime,//下单时间
                ispay = a.IsPay.HasValue ? a.IsPay.Value == 1 : false,//是否已付款
                payType = a.FeeType.HasValue ? a.FeeType == 10 ? 1 : a.FeeType == 20 ? 2 : a.FeeType == 30 ? 3 : 0 : 0,//消费类型
                fee = a.Fee,//费用
                sendTime = time,//预计发货时间
                ginfo = DB.Gift.Where(gift => gift.GiftID == a.GiftID).Select(gift => new
                {
                    gid = gift.GiftID,
                    name = gift.GiftName
                }).FirstOrDefault(),
                sendInfo = DB.OrderSend.Where(send => send.UserGiftId == a.UserGiftId).Select(send => new
                {
                    status = send.SendStatus,
                    time = send.UpdateTime,
                }).FirstOrDefault(),
                checkInfo = DB.OrderCheck.Where(check => check.UserGiftId == a.UserGiftId).Select(check => new
                {
                    cid = check.OrderCheckId,
                    status = check.CheckStatus,
                    time = check.UpdateTime
                }).FirstOrDefault(),
                ugid = a.UserGiftId,
            }).ToList();
            ri.Ok = true;
            return Result(ri);
        }

        /// <summary>
        /// 获取待收货订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Loadtocheckorder()
        {
            //区分 人民币购买的 和 积分 购买的， 在 收货表 和 发货表 里字段不一样 
            ResultInfo ri = new ResultInfo();
            int ordertype1 = OrderEnumType.Gift.GetHashCode();
            int ordertype2 = OrderEnumType.Data.GetHashCode();
            int ordertype3 = OrderEnumType.KeCheng.GetHashCode();
            var time = DateTime.Now.AddHours(12).ToString(2);
            //获取购买礼物、数据、课程的订单
            ri.Data = DB.UserGift.Where(a => a.BuyUserID == UserID).OrderByDescending(a => a.BuyTime).Select(a => new
            {
                tel = a.LinkTel,//联系电话
                receiveName = a.LinkMan,//联系人
                buyTime = a.BuyTime,//下单时间
                ispay = a.IsPay.HasValue ? a.IsPay.Value == 1 : false,//是否已付款
                payType = a.FeeType.HasValue ? a.FeeType == 10 ? 1 : a.FeeType == 20 ? 2 : a.FeeType == 30 ? 3 : 0 : 0,//消费类型
                fee = a.Fee,//费用
                sendTime = time,//预计发货时间
                ginfo = DB.Gift.Where(gift => gift.GiftID == a.GiftID).Select(gift => new
                {
                    gid = gift.GiftID,
                    name = gift.GiftName
                }).FirstOrDefault(),
                sendInfo = DB.OrderSend.Where(send => send.UserGiftId == a.UserGiftId).Select(send => new
                {
                    status = send.SendStatus,
                    time = send.UpdateTime,
                }).FirstOrDefault(),
                checkInfo = DB.OrderCheck.Where(check => check.UserGiftId == a.UserGiftId).Select(check => new
                {
                    cid = check.OrderCheckId,
                    status = check.CheckStatus,
                    time = check.UpdateTime
                }).FirstOrDefault(),
                ugid = a.UserGiftId,
            }).Where(a => a.checkInfo != null && a.checkInfo.status == 0).ToList();
            ri.Ok = true;
            return Result(ri);
        }

        /// <summary>
        /// 获取已确认收货订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Loadfinishedorder()
        {
            //区分 人民币购买的 和 积分 购买的， 在 收货表 和 发货表 里字段不一样 
            ResultInfo ri = new ResultInfo();
            int ordertype1 = OrderEnumType.Gift.GetHashCode();
            int ordertype2 = OrderEnumType.Data.GetHashCode();
            int ordertype3 = OrderEnumType.KeCheng.GetHashCode();
            var time = DateTime.Now.AddHours(12).ToString(2);
            //获取购买礼物、数据、课程的订单
            ri.Data = DB.UserGift.Where(a => a.BuyUserID == UserID).OrderByDescending(a => a.BuyTime).Select(a => new
            {
                tel = a.LinkTel,//联系电话
                receiveName = a.LinkMan,//联系人
                buyTime = a.BuyTime,//下单时间
                ispay = a.IsPay.HasValue ? a.IsPay.Value == 1 : false,//是否已付款
                payType = a.FeeType.HasValue ? a.FeeType == 10 ? 1 : a.FeeType == 20 ? 2 : a.FeeType == 30 ? 3 : 0 : 0,//消费类型
                fee = a.Fee,//费用
                sendTime = time,//预计发货时间
                ginfo = DB.Gift.Where(gift => gift.GiftID == a.GiftID).Select(gift => new
                {
                    gid = gift.GiftID,
                    name = gift.GiftName
                }).FirstOrDefault(),
                sendInfo = DB.OrderSend.Where(send => send.UserGiftId == a.UserGiftId).Select(send => new
                {
                    status = send.SendStatus,
                    time = send.UpdateTime,
                }).FirstOrDefault(),
                checkInfo = DB.OrderCheck.Where(check => check.UserGiftId == a.UserGiftId).Select(check => new
                {
                    status = check.CheckStatus,
                    time = check.UpdateTime
                }).FirstOrDefault(),
                customer = DB.Gift.FirstOrDefault(gift => gift.GiftID == a.GiftID).GiftCreateUserID,
            }).Where(a => a.ispay && (a.checkInfo == null || a.checkInfo.status == 1)).ToList();
            ri.Ok = true;
            return Result(ri);
        }

        /// <summary>
        /// 收货
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult confirmreceipt(long id, long gid, long mid)
        {
            var ri = new ResultInfo();
            var checkInfo = DB.OrderCheck.FirstOrDefault(a => a.MainID == mid && a.OrderCheckId == id && a.UserGiftId == gid && a.CreateUser == UserID);
            if (checkInfo != null)
            {
                var usergift = DB.UserGift.FirstOrDefault(a => a.UserGiftId == gid && a.BuyUserID == UserID);
                if (usergift != null)
                {
                    //更新 收货 状态
                    if (checkInfo.CheckStatus == 0)
                    {
                        checkInfo.CheckStatus = OrderCheckEnumType.Checked.GetHashCode();
                        checkInfo.UpdateTime = DateTime.Now;
                        DB.SaveChanges();
                        ri.Ok = true;
                        ri.Msg = "确认收货成功";
                    }
                }
            }
            return Result(ri);
        }
        #endregion

        #region 待收货订单数
        [LOGIN]
        [HttpPost]
        public ActionResult orderToCheck()
        {
            ResultInfo ri = new ResultInfo();

            var list = DB.OrderSend.Where(a => a.SendStatus == 0 && a.CreateUser == UserID)
                .Join(DB.UserGift.Where(a => a.IsPay == 1 && a.BuyUserID == UserID), a => a.UserGiftId, b => b.UserGiftId, (a, b) => a).Count();
            ri.Data = list;
            ri.Ok = true;

            return Result(ri);
        }
        #endregion

        #endregion

        #region 他人
        #region 获取粉丝
        /// <summary>
        /// 获取粉丝
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetFans(long id)
        {
            ResultInfo<List<FansViewModel>> ri = new ResultInfo<List<FansViewModel>>();
            if (id > 0)
            {
                ri.Data = UserLikeBLL.Instance.GetFansByUserID(id, 3);
                ri.Ok = true;
            }
            else
            {
                ri.Msg = "需要查看的用户不存在";
            }
            return Result(ri);
        }
        #endregion

        #region 他的回答

        /// <summary>
        /// 他的回答
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Answer(long id = 0)
        {
            if (id > 0)
            {
                MyCommentsViewModel amodel = CommentBLL.Instance.GetCommentListByUserid(id, CommentEnumType.BBS, InitPage(), UserID);
                return PartialView(amodel);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region 他的提问问题
        /// <summary>
        /// 他的提问问题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Question(long id = 0)
        {
            if (id > 0)
            {
                BBSListViewModel amodel = QuestionBLL.Instance.GetQuestionListByUserid(id, InitPage());
                return PartialView(amodel);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region 他的文章
        /// <summary>
        /// 他的文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Article(long id = 0)
        {
            if (id > 0)
            {
                MyArticleViewModel amodel = ArticleBLL.Instance.GetArticleListByUserid(id, InitPage());
                return PartialView(amodel);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region 他的最佳回答
        /// <summary>
        /// 他的最佳回答
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult BestAnswer(long id = 0)
        {
            if (id > 0)
            {
                MyCommentsViewModel model = CommentBLL.Instance.GetBestAnswersByUserID(id, PriseEnumType.BBSComment, 1, InitPage());
                return PartialView(model);
            }
            return RedirectToAction("/");
        }
        #endregion

        #region 他的优秀回答
        /// <summary>
        /// 他的优秀回答
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NiceAnswer(long id = 0)
        {
            if (id > 0)
            {
                MyCommentsViewModel model = CommentBLL.Instance.GetBestAnswersByUserID(id, PriseEnumType.BBSComment, 2, InitPage());
                return PartialView(model);
            }
            return RedirectToAction("/");
        }
        #endregion
        #endregion

        #region 帐号封禁
        /// <summary>
        /// 帐号封禁
        /// </summary>
        /// <param name="id"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [LOGIN]
        [IsMaster]
        public ActionResult Close(long id, int t)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                UserBaseBLL bll = UserBaseBLL.Instance;
                //long userid = bll.ExistUserName(id);
                if (id > 0)
                {
                    if (t == 0)
                    {
                        ri.Ok = UserBaseBLL.Instance.CloseAccount(id);
                    }
                    else
                    {
                        //判断本月 普通管理员是否已操作过10人 超过则不能再操作，否则可以继续操作
                        var now = DateTime.Now;
                        var startDate = now.Date.AddDays(-now.Day + 1);
                        var endDate = now;
                        if (DB.UserDisabledLog.Count(a => a.CreateTime >= startDate && a.CreateTime <= endDate && !a.IsDelete && a.ActionUserId == UserID) < 10)
                        {
                            DateTime closetime = now.AddMinutes(t);
                            ri.Ok = UserExtBLL.Instance.CloseAccount(id, closetime);
                            //添加日志
                            DB.UserDisabledLog.Add(new UserDisabledLog
                            {
                                ActionUserId = UserID,
                                CreateTime = now,
                                UpdateTime = now,
                                ExpriseTime = closetime,
                                IsDelete = false,
                                UserId = id,
                            });
                            DB.SaveChanges();
                        }
                        else
                        {
                            ri.Msg = "您本月已经封禁10人了，不能再操作了！";
                        }
                    }
                }
                else
                {
                    ri.Msg = "查无此人";
                }
            }
            else
            {
                ri.Msg = "封禁用户不能为空";
            }
            return Result(ri);
        }
        #endregion

        #region 修改昵称
        [LOGIN]
        [HttpPost]
        public ActionResult ChangeNickName(string nickname)
        {
            ResultInfo ri = new ResultInfo();
            if (nickname.IsNotNullOrEmpty())
            {
                //检查当月是否已经修改过
                if (EditLogBLL.Instance.IsChangedThisMonth(UserID, 1))
                {
                    ri.Msg = "您本月已经修改过了。请等到下个月！";
                }
                else
                {
                    //校验昵称是否已存在
                    if (UserBaseBLL.Instance.ExistUserName(nickname.Trim()) > 0)
                    {
                        ri.Msg = "当前昵称已被其他用户注册使用，请更换";
                    }
                    else
                    {
                        ri.Ok = UserBaseBLL.Instance.UpdateUserName(nickname, UserID);
                        //记录变更
                        EditLog editlog = new EditLog()
                        {
                            CreateTime = DateTime.Now,
                            UserID = UserID,
                            NewValue = nickname,
                            OldValue = UserInfo.UserName,
                            Type = 1
                        };
                        ri.Ok = EditLogBLL.Instance.Add(editlog) > 0;
                        var user = UserInfo;
                        user.UserName = nickname;
                        SetLogin(user);//更新登录状态
                    }
                }
            }
            else
            {
                ri.Msg = "昵称不能为空";
            }
            return Result(ri);
        }
        #endregion

        #region 私有

        private T GetMyInfoByTargetType<T>(int type, long targetUserID = 0, CommentEnumType commentEnum = CommentEnumType.BBS, string likeType = "user") where T : new()
        {
            object model = null;
            Paging page = InitPage();
            switch (type)
            {
                //我的问题
                case 1:
                    model = QuestionBLL.Instance.GetQuestionListByUserid(UserID, page);
                    break;
                //我的回答
                case 2:
                    model = CommentBLL.Instance.GetCommentListByUserid(targetUserID, commentEnum, page, UserID);
                    break;
                //我的招聘
                case 3:
                    model = ZhaoPinBLL.Instance.GetZhaoPinList(UserID, page);
                    break;
                //我的求职
                case 4:
                    model = QiuZhiBLL.Instance.GetQiuZhiList(UserID, page);
                    break;
                //我的产品
                case 5:
                    model = ProductBLL.Instance.GetProductList(UserID, page);
                    break;
                //我的文章
                case 6:
                    model = ArticleBLL.Instance.GetArticleListByUserid(UserID, page);
                    break;
                case 7:
                    int _liketype = 0;
                    if (likeType == "user")
                    {
                        ViewBag.TempTitle = "用户";
                        _liketype = 3;
                    }
                    else if (likeType == "question")
                    {
                        ViewBag.TempTitle = "问题";
                        _liketype = 1;
                    }
                    else
                    {
                        ViewBag.TempTitle = "文章";
                        _liketype = 2;
                    }
                    var likes = UserLikeBLL.Instance.GetLikesByUserID(UserID, _liketype, page);
                    model = new MyLikeViewModel()
                    {
                        MyLikePage = page,
                        MyLikes = likes
                    };
                    break;
            }
            return (T)Convert.ChangeType(model, typeof(T));
        }

        #endregion

    }
}