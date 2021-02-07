using AmazonBBS.BLL;
using AmazonBBS.BLL.OAuth2;
using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Controllers
{
    public class AccountController : BaseController
    {
        public INoticeService noticeService { get; set; }
        public IUserService userService { get; set; }

        #region 第三方登录的绑定邮箱帐号
        /// <summary>
        /// 第三方登录的绑定邮箱帐号
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        public ActionResult LoginOauth()
        {
            if (UserInfo.Source != 2)
            {
                ViewBag.Name = UserInfo.UserName;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
            #region MyRegion
            //if (IsLogin)
            //{
            //    return RedirectToAction("/");
            //}
            //else
            //{
            //    int type = GetRequest<int>("from");//1QQ 3微信
            //    string sessionName = string.Empty;
            //    string oauthName = string.Empty;
            //    if (type == 1)
            //    {
            //        sessionName = "QQAuthOpenID";
            //        oauthName = "QQ";
            //    }
            //    else
            //    {
            //        sessionName = "WechatAuthOpenID";
            //        oauthName = "微信";
            //    }
            //    if (SessionHelper.Get(sessionName) != null)
            //    {
            //        ViewBag.ReturnUrl = GetRequest("ReturnUrl");
            //        ViewBag.From = type;
            //        ViewBag.Name = GetRequest("name");
            //        ViewBag.OauthName = oauthName;
            //        return View();
            //    }
            //    else
            //    {
            //        return Content("第三方登录已过期，请重新登录！,<a href='/'>返回首页</a>");
            //    }
            //} 
            #endregion
        }

        #region 登录帐号并绑定第三方
        /// <summary>
        /// 登录帐号并绑定第三方
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoginOauth(UserBase model)
        {
            #region MyRegion
            //if (IsLogin)
            //{
            //    return RedirectToAction("/");
            //}
            //int type = GetRequest<int>("from");//1QQ 3微信
            //string sessionName = string.Empty;
            //string oauthName = string.Empty;
            ////RegistEnumType registtype;
            //if (type == 1)
            //{
            //    sessionName = "QQAuthOpenID";
            //    oauthName = "QQ";
            //    //registtype = RegistEnumType.QQAuth;
            //}
            //else
            //{
            //    sessionName = "WechatAuthOpenID";
            //    oauthName = "微信";
            //    //registtype = RegistEnumType.WeChatAuth;
            //}
            //object sessionOpenID = SessionHelper.Get(sessionName);
            //if (sessionOpenID == null)
            //{
            //    return Content("第三方登录已过期，请重新登录！,<a href='/'>返回首页</a>");
            //}
            //else
            //{ 
            #endregion
            ResultInfo ri = new ResultInfo();
            if (UserInfo.Source != 2)
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.Account))
                    {
                        if (!string.IsNullOrEmpty(model.Password))
                        {
                            //检查用户名是否存在
                            if (UserBaseBLL.Instance.ExistAccount(model.Account))
                            {
                                UserBase user = UserBaseBLL.Instance.GetUserInfo(model.Account, model.Password);
                                if (user != null)
                                {
                                    if (user.IsDelete == 3)
                                    {
                                        ri.Msg = "您的帐号已被管理员暂时冻结！如有疑问，请联系管理员！";
                                    }
                                    else if (user.IsDelete == 1)
                                    {
                                        ri.Msg = "您的帐号已被管理员删除！如有疑问，请联系管理员！";
                                    }
                                    else if (user.IsDelete == 2)
                                    {
                                        ri.Msg = "您的帐号已被管理员永久冻结！如有疑问，请联系管理员！";
                                    }
                                    else
                                    {
                                        #region 绑定第三方帐号并删除已有的第三方帐号登录
                                        //获取第三方登录的信息
                                        OAuth _oauth = OAuthBLL.Instance.GetLogonInfoByOpenId(UserInfo.OpenId, UserInfo.Source.Value);
                                        //证明是第三方登录并绑定邮箱帐号的
                                        if (_oauth != null)
                                        {
                                            ri = BindAndDelete(user, _oauth, UserInfo.OpenId, UserInfo.Source.Value);
                                            if (ri.Ok)
                                            {
                                                AfterLogin(user);
                                                //用户当日是否已签到
                                                IsSign = ScoreCoinLogBLL.Instance.GetSignStatus(user.UserID);
                                                //用户登录计算VIP等级
                                                UserExt userext = UserExtBLL.Instance.GetExtInfo(user.UserID);
                                                if (userext.VIP > 0)
                                                {
                                                    if (userext.VIPExpiryTime < DateTime.Now)
                                                    {
                                                        //过期
                                                        userext.VIP = -userext.VIP;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                ri.Url = Server.MapPath("/User?Error=绑定错误");
                                            }
                                            #region MyRegion
                                            ////删除第三方登录对应的帐号
                                            //if (UserBaseBLL.Instance.DeleteUser(_oauth.UserID.Value, Tran))
                                            //{
                                            //    _oauth.IsBind = 1;
                                            //    _oauth.UserID = user.UserID;
                                            //    if (OAuthBLL.Instance.Update(_oauth, Tran).Ok)
                                            //    {
                                            //        ri.Ok = true;
                                            //        string returnuri = GetRequest("ReturnUrl");
                                            //        ri.Url = returnuri.IsNullOrEmpty() ? "/" : returnuri;
                                            //        Commit();
                                            //    }
                                            //    else
                                            //    {
                                            //        RollBack();
                                            //    }
                                            //}
                                            //else
                                            //{
                                            //    RollBack();
                                            //} 
                                            #endregion
                                        }
                                        #endregion
                                    }
                                }
                                else
                                {
                                    ri.Msg = "登录密码错误";
                                }
                            }
                            else
                            {
                                ri.Msg = "用户名不存在";
                            }
                        }
                        else
                        {
                            ri.Msg = "密码不能为空";
                        }
                    }
                    else
                    {
                        ri.Msg = "用户名不能为空";
                    }
                }
            }
            else
            {
                ri.Msg = "该帐号已是邮箱注册，不需要绑定";
            }
            //}
            return Result(ri);
        }
        #endregion

        #region 注册帐号并绑定第三方
        [LOGIN]
        public ActionResult RegistOauth()
        {
            if (UserInfo.Source != 2)
            {
                ViewBag.UserName = UserInfo.UserName;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }

        /// <summary>
        /// 注册帐号并绑定第三方
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult RegistOauth(UserBase model, string returnUrl)
        {
            ResultInfo ri = new ResultInfo();
            if (UserInfo.Source != 2)
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.Account))
                    {
                        object sessionMail = Session["RegistOauthEmail"];
                        if (sessionMail != null)
                        {
                            if (model.Account.Equals(sessionMail))
                            {
                                if (!string.IsNullOrEmpty(model.Password))
                                {
                                    if (!string.IsNullOrEmpty(model.UserName))
                                    {
                                        object sessionMailCode = Session["SendOauthEmailCode"];
                                        if (sessionMailCode != null && model.ValidateCode.Equals(sessionMailCode.ToString()))
                                        {
                                            Session.Remove("RegistOauthEmail");//移除验证码
                                            Session.Remove("SendOauthEmailCode");//移除验证码

                                            #region 将注册信息写入到当前登录帐号里
                                            var user = UserInfo;
                                            model.CreateTime = DateTime.Now;
                                            model.UpdateTime = DateTime.Now;
                                            model.CreateUser = model.Account;
                                            model.UpdateUser = model.Account;
                                            model.IsDelete = 0;
                                            model.Source = RegistEnumType.EmailRegist.GetHashCode();//邮箱注册=2
                                            model.HeadUrl = user.HeadUrl;
                                            model.UserID = user.UserID;

                                            #region 设置登录时间/IP地址
                                            model.LoginTime = DateTime.Now;
                                            try
                                            {
                                                model.LoginIP = NetHelper.GetInternetIP();
                                            }
                                            catch
                                            {
                                                model.LoginIP = NetHelper.GetIPAddress2();
                                            }
                                            #endregion

                                            BeginTran();
                                            if (UserBaseBLL.Instance.Update(model, Tran).Ok)
                                            {
                                                if (OAuthBLL.Instance.UpdateBindStatusOauthByID(model.UserID, user.Source.Value, 1, Tran))
                                                {
                                                    IsSign = false;
                                                    //UserBaseBLL.Instance.IsBindAccount = true;
                                                    ri.Ok = true;
                                                    ri.Msg = "注册并绑定成功";
                                                    ri.Url = returnUrl.IsNullOrEmpty() ? "/" : returnUrl;

                                                    SetLogin(model);
                                                }
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            ri.Msg = "验证码校验错误，请输入正确的验证码";
                                        }
                                    }
                                    else
                                    {
                                        ri.Msg = "用户昵称不能为空";
                                    }
                                }
                                else
                                {
                                    ri.Msg = "密码不能为空";
                                }
                            }
                            else
                            {
                                ri.Msg = "接收验证码的邮箱和注册邮箱不符";
                            }
                        }
                        else
                        {
                            ri.Msg = "异常错误,请重新注册";
                        }
                    }
                    else
                    {
                        ri.Msg = "帐号不能为空";
                    }
                }
            }
            else
            {
                ri.Msg = "该帐号无需绑定新帐号";
            }
            return Result(ri);
        }
        #endregion
        #endregion

        #region 登录
        public ActionResult Login()
        {
            if (IsLogin)
            {
                return RedirectToAction("/");
            }
            string returnUrl = GetRequest("ReturnUrl");
            ViewBag.ReturnUrl = returnUrl;

            bool isOpenOauth = ConfigHelper.AppSettings("OpenOAuth") == "1";
            if (isOpenOauth)
            {
                //ViewBag.WXCallback = ConfigHelper.AppSettings("WeiXin.CallBackUrl");
                //ViewBag.WXAppID = ConfigHelper.AppSettings("WeiXin.AppID");
                string wechatAppID = ConfigHelper.AppSettings("WeiXin.AppID");
                string wechatCallback = HttpUtility.UrlEncode(ConfigHelper.AppSettings("WeiXin.CallBackUrl") + "?ReturnUrl=" + HttpUtility.UrlEncode(returnUrl));
                string state = new WeChatBLL().GetState();
                ViewBag.WeChatURI = $@"https://open.weixin.qq.com/connect/qrconnect?appid={wechatAppID}&response_type=code&scope=snsapi_login&state={state}&redirect_uri={wechatCallback}#wechat_redirect";
                ViewBag.QQOpenID = ConfigHelper.AppSettings("QQ.AppID");
            }
            ViewBag.OpenOAuth = isOpenOauth;
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserBase model, string ReturnUrl)
        {
            if (IsLogin)
            {
                return RedirectToAction("/");
            }
            ResultInfo ri = new ResultInfo();
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.Account))
                {
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        //检查用户名是否存在
                        if (UserBaseBLL.Instance.ExistAccount(model.Account))
                        {
                            UserBase user = UserBaseBLL.Instance.GetUserInfo(model.Account, model.Password);
                            if (user != null)
                            {
                                if (user.IsDelete == 3)
                                {
                                    ri.Msg = "您的帐号已被管理员暂时冻结！如有疑问，请联系管理员！";
                                }
                                else if (user.IsDelete == 1)
                                {
                                    ri.Msg = "您的帐号已被管理员删除！如有疑问，请联系管理员！";
                                }
                                else if (user.IsDelete == 2)
                                {
                                    ri.Msg = "您的帐号已被管理员永久冻结！如有疑问，请联系管理员！";
                                }
                                else
                                {
                                    AfterLogin(user);

                                    //标记非第三方登录
                                    //UserBaseBLL.Instance.IsBindAccount = true;

                                    ri.Ok = true;
                                    ri.Msg = "登录成功";
                                    if (!string.IsNullOrEmpty(ReturnUrl))
                                    {
                                        ri.Url = ReturnUrl;
                                    }
                                    else
                                    {
                                        ri.Url = "/";
                                    }

                                    //用户当日是否已签到
                                    IsSign = ScoreCoinLogBLL.Instance.GetSignStatus(user.UserID);

                                    //用户登录计算VIP等级
                                    //UserExt userext = UserExtBLL.Instance.GetExtInfo(user.UserID);
                                    var userext = DB.UserExt.FirstOrDefault(a => a.UserID == user.UserID);
                                    if (userext.VIP > 0)
                                    {
                                        if (userext.VIPExpiryTime < DateTime.Now)
                                        {
                                            //过期
                                            userext.VIP = -userext.VIP;
                                            DB.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ri.Msg = "登录密码错误";
                            }
                        }
                        else
                        {
                            ri.Msg = "用户名不存在";
                        }
                    }
                    else
                    {
                        ri.Msg = "密码不能为空";
                    }
                }
                else
                {
                    ri.Msg = "用户名不能为空";
                }
            }
            return Json(ri, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 注册
        public ActionResult Register()
        {
            if (IsLogin)
            {
                return RedirectToAction("/");
            }
            ViewBag.ReturnUrl = GetRequest("ReturnUrl");
            return View();
        }

        #region 普通注册
        /// <summary>
        /// 普通注册
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(UserBase model, string returnUrl)
        {
            if (IsLogin)
            {
                return RedirectToAction("/");
            }
            ResultInfo ri = new ResultInfo();
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.Account))
                {
                    object sessionMail = Session["RegisterEmail"];
                    if (sessionMail != null)
                    {
                        if (model.Account.Equals(sessionMail))
                        {
                            if (!string.IsNullOrEmpty(model.Password))
                            {
                                if (string.IsNullOrEmpty(model.UserName))
                                {
                                    model.UserName = "百晓堂用户" + UserBaseBLL.Instance.Count().ToString();
                                }
                                object sessionMailCode = Session["SendEmailCode"];
                                if (sessionMailCode != null && model.ValidateCode.Equals(sessionMailCode.ToString()))
                                {
                                    Session.Remove("RegisterEmail");//移除验证码
                                    Session.Remove("SendEmailCode");//移除验证码
                                    long result = Regist(model, RegistEnumType.EmailRegist);
                                    if (result > 0)
                                    {
                                        //UserBaseBLL.Instance.IsBindAccount = true;

                                        if (!string.IsNullOrEmpty(returnUrl))
                                        {
                                            ri.Url = returnUrl;
                                        }
                                        else
                                        {
                                            ri.Url = "/";
                                        }
                                        ri.Ok = true;
                                        ri.Msg = "注册成功";
                                    }
                                    else
                                    {
                                        ri.Msg = "注册失败";
                                    }
                                }
                                else
                                {
                                    ri.Msg = "验证码校验错误，请输入正确的验证码";
                                }
                                //}
                                //else
                                //{
                                //    ri.Msg = "用户昵称不能为空";
                                //}
                            }
                            else
                            {
                                ri.Msg = "密码不能为空";
                            }
                        }
                        else
                        {
                            ri.Msg = "接收验证码的邮箱和注册邮箱不符";
                        }
                    }
                    else
                    {
                        ri.Msg = "异常错误,请重新注册";
                    }
                }
                else
                {
                    ri.Msg = "帐号不能为空";
                }
            }
            return Result(ri);
        }
        #endregion

        #region 分享注册
        /// <summary>
        /// 分享注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShareRegister(UserBase model)
        {
            if (IsLogin)
            {
                return RedirectToAction("/");
            }
            ResultInfo ri = new ResultInfo();
            if (ModelState.IsValid)
            {
                string uidStr = GetRequest("shareID");
                string token = GetRequest("shareToken");
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
                                    if (!string.IsNullOrEmpty(model.Account))
                                    {
                                        object sessionMail = Session["RegistShareEmail"];
                                        if (sessionMail != null)
                                        {
                                            if (model.Account.Equals(sessionMail))
                                            {
                                                if (!string.IsNullOrEmpty(model.Password))
                                                {
                                                    if (model.UserName.IsNullOrEmpty())
                                                    {
                                                        model.UserName = "百晓堂用户" + UserBaseBLL.Instance.Count().ToString();
                                                    }
                                                    object sessionMailCode = Session["SendShareEmailCode"];
                                                    if (sessionMailCode != null && model.ValidateCode.Equals(sessionMailCode.ToString()))
                                                    {
                                                        Session.Remove("RegistShareEmail");//移除验证码
                                                        Session.Remove("SendShareEmailCode");//移除验证码
                                                        long result = Regist(model, RegistEnumType.EmailRegist);
                                                        if (result > 0)
                                                        {
                                                            #region 添加注册日志
                                                            ShareRegistLog shareRegistLogModel = new ShareRegistLog()
                                                            {
                                                                CreateTime = DateTime.Now,
                                                                ShareUserID = _userID,
                                                                ShareUserName = sharer.UserName,
                                                                UserID = result,
                                                                UserName = model.UserName,
                                                            };
                                                            ri.Ok = ShareRegistLogBLL.Instance.Add(shareRegistLogModel) > 0;
                                                            #endregion
                                                            ri.Url = "/";
                                                            ri.Msg = "注册成功";

                                                            #region 注册成功后通知分享人
                                                            NoticeBLL.Instance.OnShareRegistSuccess_Notice_Sharer(sharer, model.UserName, DateTime.Now, GetDomainName, NoticeTypeEnum.ShareRegist);
                                                            #endregion
                                                        }
                                                        else
                                                        {
                                                            ri.Msg = "注册失败";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ri.Msg = "验证码校验错误，请输入正确的验证码";
                                                    }
                                                    //}
                                                    //else
                                                    //{
                                                    //    ri.Msg = "用户昵称不能为空";
                                                    //}
                                                }
                                                else
                                                {
                                                    ri.Msg = "密码不能为空";
                                                }
                                            }
                                            else
                                            {
                                                ri.Msg = "接收验证码的邮箱和注册邮箱不符";
                                            }
                                        }
                                        else
                                        {
                                            ri.Msg = "异常错误,请重新注册";
                                        }
                                    }
                                    else
                                    {
                                        ri.Msg = "帐号不能为空";
                                    }
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
            return Result(ri);
        }
        #endregion

        #region 注册写入数据库
        private long Regist(UserBase model, RegistEnumType source, bool login = true)
        {
            long result = 0;
            try
            {
                BeginTran();

                model.CreateTime = DateTime.Now;
                model.UpdateTime = DateTime.Now;
                model.CreateUser = model.Account;
                model.UpdateUser = model.Account;
                model.IsDelete = 0;
                model.Source = source.GetHashCode();//邮箱注册=2

                #region 设置登录时间/IP地址
                model.LoginTime = DateTime.Now;
                try
                {
                    model.LoginIP = NetHelper.GetInternetIP();
                }
                catch
                {
                    model.LoginIP = NetHelper.GetIPAddress2();
                }
                #endregion

                result = UserBaseBLL.Instance.Add(model, Tran);

                if (result > 0)
                {
                    //添加用户扩展表
                    UserExt userext = new UserExt()
                    {
                        UserID = result,
                        TotalScore = Convert.ToInt32(ConfigHelper.AppSettings("RegistScore")),//注册赠送100积分
                        TotalCoin = 0,
                        UserV = 0,
                        VIP = 0,
                        HeadNameShowType = 1
                    };
                    if (UserExtBLL.Instance.Add(userext, Tran) > 0)
                    {
                        model.UserID = result;
                        if (login)
                        {
                            SetLogin(model);//设置登录信息
                        }
                        IsSign = false;
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
            catch (Exception e)
            {
                RollBack();
            }
            return result;
        }
        #endregion

        #endregion

        #region 忘记密码
        public ActionResult Forget()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Find(string account, string code)
        {
            ResultInfo ri = new ResultInfo();
            if (account.IsNotNullOrEmpty())
            {
                if (code.IsNotNullOrEmpty())
                {
                    UserBase userInfo = UserBaseBLL.Instance.GetUserInfo(account);
                    if (userInfo != null)
                    {
                        object sysCode = SessionHelper.Get("ForgetPWD");
                        if (sysCode != null)
                        {
                            if (code.ToLower() != sysCode.ToString().ToLower())
                            {
                                ri.ID = 2;
                                ri.Msg = "验证码不正确";
                            }
                            else
                            {
                                SessionHelper.Remove("ForgetPWD");
                                //发送邮件到邮箱
                                string[] sendEmailAccount = ConfigHelper.AppSettings("Emailor").Split('|');
                                EmailHelper email = new EmailHelper();

                                email.From = sendEmailAccount[0];
                                email.FromName = "百晓堂";

                                //string[] receiveMails = ConfigHelper.AppSettings("ReceiveEmail").Split(',');

                                email.Recipients = new List<EmailHelper.RecipientClass>();
                                email.Recipients.Add(new EmailHelper.RecipientClass()
                                {
                                    Recipient = account,
                                    RecipientName = userInfo.UserName
                                });

                                email.Subject = "密码找回-百晓堂";
                                email.Body = ConfigHelper.AppSettings("FindPwdMsg").FormatWith(userInfo.Password);
                                email.IsBodyHtml = false;
                                //email.ServerHost = "smtp.{0}".FormatWith(sendEmailAccount[0].Split('@')[1]);
                                string emailhost = ConfigHelper.AppSettings("EmailHost");
                                email.ServerHost = emailhost.IsNotNullOrEmpty() ? emailhost : "smtp.{0}".FormatWith(sendEmailAccount[0].Split('@')[1]);
                                email.ServerPort = 465;
                                email.Username = sendEmailAccount[0];
                                email.Password = sendEmailAccount[1];

                                bool ok = email.Send2();
                            }
                        }
                        else
                        {
                            ri.Msg = "验证码错误，请刷新页面";
                        }
                    }
                    else
                    {
                        ri.ID = 1;
                        ri.Msg = "用户名不存在";
                    }
                }
                else
                {
                    ri.Msg = "图形验证码不能为空";
                }
            }
            else
            {
                ri.Msg = "请输入要找回密码的帐号（邮箱）";
            }
            return Result(ri);
        }
        #endregion

        #region 修改密码
        public ActionResult ExChange()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ExChange(string account, string password, string newpwd1, string newpwd2, string code)
        {
            //ExchangePWD
            ResultInfo ri = new ResultInfo();
            if (account.IsNotNullOrEmpty())
            {
                if (password.IsNotNullOrEmpty())
                {
                    if (newpwd1.IsNotNullOrEmpty())
                    {
                        if (code.IsNotNullOrEmpty())
                        {
                            //判断帐号是否存在 
                            if (UserBaseBLL.Instance.ExistAccount(account))
                            {
                                var user = UserBaseBLL.Instance.GetUserInfo(account, password);
                                if (user != null)
                                {
                                    //判断验证码
                                    object sysCode = SessionHelper.Get("ExchangePWD");
                                    if (user.IsDelete == 0)
                                    {
                                        if (sysCode != null && sysCode.ToString().ToLower() == code.ToLower())
                                        {
                                            user.Password = newpwd1;
                                            ri = UserBaseBLL.Instance.Update(user);
                                        }
                                        else
                                        {
                                            ri.Msg = "图形验证码错误";
                                            ri.ID = 3;
                                        }
                                    }
                                    else
                                    {
                                        ri.Msg = "帐号被删除或冻结，暂时无法修改密码";
                                    }
                                }
                                else
                                {
                                    ri.ID = 2;
                                    ri.Msg = "登录密码错误";
                                }
                            }
                            else
                            {
                                ri.ID = 1;
                                ri.Msg = "用户名不存在";
                            }
                        }
                        else
                        {
                            ri.Msg = "图形验证码不能为空";
                        }
                    }
                    else
                    {
                        ri.Msg = "新密码不能为空";
                    }
                }
                else
                {
                    ri.Msg = "密码不能为空";
                }
            }
            else
            {
                ri.Msg = "用户名不能为空";
            }
            return Result(ri);
        }
        #endregion

        #region 退出
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            string returnUrl = GetRequest("ReturnUrl");
            UserInfo = null;
            UserBaseBLL.Instance.UserLogOff();
            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(returnUrl);
        }
        #endregion

        #region 签到
        [HttpPost]
        [LOGIN]
        [ActionName("Signture")]
        public JsonResult Sign()
        {
            ResultInfo<string> ri = new ResultInfo<string>();

            long userid = GetRequest<long>("thunpx");
            if (userid > 0 && userid == UserID)
            {
                UserBase currentUserInfo = UserInfo;
                UserExt userExt = DB.UserExt.FirstOrDefault(a => a.UserID == userid);

                BeginTran();
                //判断今日是否已签到
                if (!ScoreCoinLogBLL.Instance.GetSignStatus(userid))
                {
                    int coinSource = CoinSourceEnum.Sign.GetHashCode();
                    //int num = Convert.ToInt32(ConfigHelper.AppSettings("signScore"));
                    //随机获取积分签到值
                    string signConfig = ConfigHelper.AppSettings("signconfig");
                    int num = 5;
                    if (signConfig.IsNotNullOrEmpty())
                    {
                        Random random = new Random();
                        string[] signs = signConfig.Split(new[] { "|sign|" }, StringSplitOptions.RemoveEmptyEntries);
                        string[] signInfo = signs[random.Next(0, signs.Length)].Split(new[] { "$sign$" }, StringSplitOptions.RemoveEmptyEntries);
                        num = signInfo[1].ToInt32();
                        ri.Data = signInfo[2];
                    }

                    List<ScoreCoinLog> addModels = new List<ScoreCoinLog>();

                    #region 判断是新用户(注册30天内的)或老用户(注册30天外的)，并判断是否满足连续签到条件，赠送相应积分
                    DateTime now = DateTime.Now;
                    DateTime monthFirstDay = now.AddDays(-now.Day + 1).Date;//本月1号
                    //获取用户本月的签到情况
                    List<ScoreCoinLog> userSignLogs = DB.ScoreCoinLog.Where(a => a.UserID == userid && a.CoinTime >= monthFirstDay && a.CoinTime <= now && a.CoinSource.Equals(coinSource)).ToList();

                    bool isNewUser = false;             //[通知用] 是否新用户
                    bool isSignEnough = false;          //[通知用] 标识 签到是否满足条件
                    int userSignCountCurrentMonth = 0;  //[通知用] 用户当月签到多少次
                    int userSignEnoughCoin = 0;         //[通知用] 满足连续签到条件，赠送设定积分，并通知用户，并记录
                    int userSignCoinSource = 0;         //[通知用] 用户签到奖励积分 来源
                    if (now.Subtract(currentUserInfo.CreateTime.Value).Days <= 30)
                    {
                        #region 新用户
                        isNewUser = true;
                        //新用户 - 签到连续天数
                        int[] new_user_signCountsConfig = new int[] { 3, 10, };
                        List<int> signConfigCountSources = new List<int>();
                        signConfigCountSources.Add(CoinSourceEnum.NewUserSignCount3.GetHashCode());
                        signConfigCountSources.Add(CoinSourceEnum.NewUserSignCount10.GetHashCode());
                        //满足连续签到的数次
                        var signGiveCountScore = DB.ScoreCoinLog.Count(a => a.UserID == userid && a.CoinTime >= monthFirstDay && a.CoinTime <= now && signConfigCountSources.Contains(a.CoinSource));
                        if (signGiveCountScore < new_user_signCountsConfig.Length)
                        {
                            userSignCountCurrentMonth = new_user_signCountsConfig[signGiveCountScore];
                            //判断是否满足连续签到N天
                            DateTime loopDate = now.Date;//今天肯定签到了，算1天
                            for (int loopIndex = userSignCountCurrentMonth - 1; loopIndex > 0; loopIndex--)
                            {
                                loopDate = loopDate.AddDays(-1);
                                var signlos = userSignLogs.FirstOrDefault(a => a.CoinTime.Date == loopDate);
                                if (signlos == null)
                                {
                                    isSignEnough = false;
                                    break;
                                }
                                else if (loopIndex == 1)
                                {
                                    isSignEnough = true;
                                }
                            }
                        }
                        if (isSignEnough)
                        {
                            userSignEnoughCoin = ConfigHelper.AppSettings("new_user_continu_SignCount{0}".FormatWith(userSignCountCurrentMonth)).ToInt32();
                            userSignCoinSource = (userSignCountCurrentMonth == 3 ? CoinSourceEnum.NewUserSignCount3 : CoinSourceEnum.NewUserSignCount10).GetHashCode();
                        }
                        #endregion
                    }
                    else
                    {
                        #region 老用户
                        int[] old_user_signCountsConfig = new int[] { 3, 10, 20 };
                        userSignCountCurrentMonth = userSignLogs.Count + 1;//用户本月总签到次数，此次签到也算上一次
                        //满足累计签到次数条件
                        if (old_user_signCountsConfig.Contains(userSignCountCurrentMonth))
                        {
                            isSignEnough = true;
                            userSignEnoughCoin = ConfigHelper.AppSettings("old_user_total_SignCount{0}".FormatWith(userSignCountCurrentMonth)).ToInt32();
                            userSignCoinSource = (userSignCountCurrentMonth == 3 ? CoinSourceEnum.OldUserSignCount3 : userSignCountCurrentMonth == 10 ? CoinSourceEnum.OldUserSignCount10 : CoinSourceEnum.OldUserSignCount20).GetHashCode();
                        }
                        #endregion
                    }

                    #region 创建实体
                    if (isSignEnough && userSignEnoughCoin > 0)
                    {
                        addModels.Add(new ScoreCoinLog
                        {
                            Coin = userSignEnoughCoin,
                            CoinSource = userSignCoinSource,
                            CoinType = CoinTypeEnum.Score.GetHashCode(),
                            CoinTime = now,
                            CreateUser = userid.ToString(),
                            UserID = userid,
                            UserName = currentUserInfo.UserName,
                        });
                    }
                    #endregion

                    #endregion

                    #region 记录积分明细
                    //记录积分明细
                    addModels.Add(new ScoreCoinLog()
                    {
                        Coin = num,
                        CoinSource = coinSource,
                        CoinType = CoinTypeEnum.Score.GetHashCode(),
                        CoinTime = now,
                        CreateUser = userid.ToString(),
                        UserID = userid,
                        UserName = currentUserInfo.UserName,
                    });
                    #endregion

                    addModels.ForEach(n =>
                    {
                        ri.Ok = ScoreCoinLogBLL.Instance.Add(n, Tran) > 0;
                    });
                    if (ri.Ok)
                    //if (ScoreCoinLogBLL.Instance.Add(model, Tran) > 0)
                    {
                        userExt.UserSignTotalCount += 1;//签到次数+1

                        //更新积分数值
                        if (UserExtBLL.Instance.AddScore(userid, num + userSignEnoughCoin, 1, Tran))
                        {
                            #region 计算积分等级
                            //ri.Ok = userService.CalcUserLevel(userid);
                            //计算积分等级
                            string sqlTemp = "update UserExt set LevelName={0} where UserID={1};";
                            string empty = "null";
                            StringBuilder sb = new StringBuilder();
                            List<BBSEnum> levels = BBSEnumBLL.Instance.Query(3, true);
                            //SignCountAndLevel userext = UserExtBLL.Instance.GetSignAndLevelByUserID(Tran, UserID);
                            var tempLevel = levels.LastOrDefault(item => { return item.SortIndex <= userExt.UserSignTotalCount; });
                            if (tempLevel == null)
                            {
                                if (userExt.LevelName.HasValue)
                                {
                                    sb.Append(sqlTemp.FormatWith(empty, userExt.UserID.ToString()));
                                }
                            }
                            else
                            {
                                if (tempLevel.BBSEnumId != userExt.LevelName)
                                {
                                    sb.Append(sqlTemp.FormatWith(tempLevel.BBSEnumId.ToString(), userExt.UserID.ToString()));
                                }
                            }
                            if (sb.ToString().IsNotNullOrEmpty())
                            {
                                if (SqlHelper.ExecuteSql(Tran, System.Data.CommandType.Text, sb.ToString(), null) > 0)
                                {
                                    ri.Ok = true;
                                }
                                else
                                {
                                    RollBack();
                                }
                            }
                            else
                            {
                                ri.Ok = true;
                            }
                            #endregion

                            #region 更新签到记录
                            StringBuilder signSqlBuilder = new StringBuilder("update UserExt set UserSignTotalCount=UserSignTotalCount+1,UserSignTime='{1}' ");
                            //如果上次签到时间不是昨天，则断签，要重置，否则累计加1
                            if (!userExt.UserSignTime.HasValue || now.AddDays(-1).Date != userExt.UserSignTime.Value.Date)
                            {
                                signSqlBuilder.Append(" ,UserSignContinueCount=1,UserMonthSignContinueCount=1 ");
                            }
                            else
                            {
                                //如果是月初
                                if (now.Day == 1)
                                {
                                    signSqlBuilder.Append(" ,UserMonthSignContinueCount=1 ");
                                }
                                else
                                {
                                    signSqlBuilder.Append(" ,UserMonthSignContinueCount=UserMonthSignContinueCount+1 ");
                                }
                                signSqlBuilder.Append(" ,UserSignContinueCount=UserSignContinueCount+1 ");
                            }
                            signSqlBuilder.Append(" where UserID={0}");
                            if (SqlHelper.ExecuteSql(Tran, System.Data.CommandType.Text, signSqlBuilder.ToString().FormatWith(userid, now), null) > 0)
                            {
                                ri.Ok = true;
                            }
                            else
                            {
                                RollBack();
                            }
                            #endregion

                            if (ri.Ok)
                            {
                                Commit();
                                ri.ID = DB.UserExt.FirstOrDefault(a => a.UserID == UserID).TotalScore.Value;
                                ri.Msg = "签到成功";
                                IsSign = true;

                                #region 通知
                                //签到通知
                                NoticeBLL.Instance.OnSign_Notice_User(num, currentUserInfo, NoticeTypeEnum.Sign);
                                //连续签到赠送积分 通知用户
                                if (isSignEnough && userSignEnoughCoin > 0)
                                {
                                    noticeService.OnSign_Enough_Notice_User(userid, userSignCountCurrentMonth, userSignEnoughCoin, isNewUser, now);
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            RollBack();
                        }
                    }
                    else
                    {
                        RollBack();
                        ri.Msg = "签到失败";
                        IsSign = false;
                    }
                }
                else
                {
                    ri.Msg = "今日已签到过了";
                    ri.Type = 1;
                    IsSign = true;
                }
            }
            else
            {
                ri.Msg = "签到异常";
            }

            return Json(ri, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 登录后操作(设置登录状态等)
        private void AfterLogin(UserBase user)
        {
            #region 设置登录时间/IP地址 并更新
            var loginTime = DateTime.Now;
            user.LoginTime = loginTime;
            try
            {
                user.LoginIP = NetHelper.GetIPAddress2();
            }
            catch
            {
                user.LoginIP = NetHelper.GetIPAddress2();
            }

            //更新登录信息
            UserBaseBLL.Instance.Update(user);

            //判断今天是否已记录登录日志，没有记录则添加
            DateTime startDate = loginTime.Date, endDate = startDate.AddDays(1);
            var loginLog = DB.UserLoginLog.FirstOrDefault(a => a.UserId == user.UserID && !a.IsDelete && a.CreateTime >= startDate && a.CreateTime < endDate);
            if (loginLog != null)
            {
                loginLog.CreateTime = loginTime;
                loginLog.UpdateTime = loginTime;
            }
            else
            {
                DB.UserLoginLog.Add(new UserLoginLog
                {
                    CreateTime = loginTime,
                    IsDelete = false,
                    UpdateTime = loginTime,
                    UserId = user.UserID,
                });
            }
            DB.SaveChanges();

            #endregion

            //登录成功
            SetLogin(user);
        }
        #endregion

        #region 检查用户是否登录
        /// <summary>
        /// 检查用户是否登录
        /// </summary>
        /// <returns></returns>
        public ActionResult ChecnLogin()
        {
            ResultInfo ri = new ResultInfo();
            ri.Ok = IsLogin;
            return Result(ri);
        }
        #endregion

        #region 获取用户
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetUser(string id = null)
        {
            ResultInfo<UserViewModel> ri = new ResultInfo<UserViewModel>();
            if (id != null)
            {
                UserViewModel u = null;
                if (MatchHelper.IsNum.IsMatch(id))
                {
                    try
                    {
                        long userId = Convert.ToInt64(id);
                        if (userId > 0)
                        {
                            u = UserBaseBLL.Instance.GetUserDetail(userId, UserID);
                            ri.Ok = true;
                            ri.Data = u;
                        }
                        else
                        {
                            ri.Msg = "异常";
                        }
                    }
                    catch
                    {
                        u = UserBaseBLL.Instance.GetUserDetail(id, UserID);
                        ri.Ok = true;
                        ri.Data = u;
                    }
                }
                else
                {
                    u = UserBaseBLL.Instance.GetUserDetail(id, UserID);
                    ri.Ok = true;
                    ri.Data = u;
                }
            }
            else { ri.Msg = "异常"; }

            return Result(ri);
        }
        #endregion

        #region QQ回调
        public ActionResult Auth2()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QQAuth(string id, string token, string name, string headurl)
        {
            #region 注释
            //ResultInfo ri = new ResultInfo();
            //if (id.IsNotNullOrEmpty())
            //{
            //    //已登录邮箱帐号，绑定QQ登录信息
            //    UserBase userinfo = UserInfo;
            //    if (userinfo == null)
            //    {
            //        //根据openId查找是否已绑定过会员
            //        OAuth oauth = OAuthBLL.Instance.GetLogonInfoByOpenId(id, RegistEnumType.QQAuth);
            //        if (oauth != null)
            //        {
            //            string returnurl = GetRequest("ReturnUrl");
            //            ri.Ok = true;

            //            //判断当前登录是否已绑定邮箱帐号
            //            if (oauth.IsBind == 1)
            //            {
            //                //已存在，直接设置登录状态
            //                var user = UserBaseBLL.Instance.GetUserInfo(Convert.ToInt64(oauth.UserID));
            //                if (user.IsDelete != 0)
            //                {
            //                    ri.Msg = "您的帐号已被管理员删除，无法登录！如有疑问，请联系管理员！";
            //                }
            //                else
            //                {
            //                    AfterLogin(user);
            //                    ri.Url = returnurl;
            //                }
            //            }
            //            else
            //            {
            //                //跳转绑定邮箱
            //                SessionHelper.Set("QQAuthOpenID", oauth.OAuthID, 1 * 15);//设置用户ID
            //                ri.Url = "/Account/LoginOauth?from=1&name={1}&ReturnUrl={0}".FormatWith(returnurl.IsNotNullOrEmpty() ? returnurl : "/", oauth.NickName);
            //            }
            //        }
            //        else
            //        {
            //            //不存在，新用户
            //            OAuth model = new OAuth()
            //            {
            //                HeadUrl = headurl,
            //                IsBind = 0,
            //                NickName = name.IsNotNullOrEmpty() ? name : "QQ用户" + DateTime.Now.ToString("yyyyMMddfff"),
            //                OAuthType = RegistEnumType.QQAuth.GetHashCode(),
            //                OpenId = id,
            //            };
            //            long oauthId = OAuthBLL.Instance.Add(model);
            //            if (oauthId > 0)
            //            {
            //                SessionHelper.Set("QQAuthOpenID", oauthId, 1 * 15);//设置用户ID
            //                ri.Ok = true;
            //                string returnurl = GetRequest("ReturnUrl");
            //                ri.Url = "/Account/LoginOauth?from=1&name={1}&ReturnUrl={0}".FormatWith(returnurl.IsNotNullOrEmpty() ? returnurl : "/", model.NickName);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        //已登录，绑定QQ
            //        OAuth model = new OAuth()
            //        {
            //            HeadUrl = headurl,
            //            IsBind = 1,
            //            UserID = userinfo.UserID,
            //            NickName = name.IsNotNullOrEmpty() ? name : "QQ用户" + DateTime.Now.ToString("yyyyMMddfff"),
            //            OAuthType = RegistEnumType.QQAuth.GetHashCode(),
            //            OpenId = id,
            //        };
            //        long oauthId = OAuthBLL.Instance.Add(model);
            //        if (oauthId > 0)
            //        {
            //            ri.Ok = true;
            //            ri.Url = "/User/BindOauth";
            //        }
            //    }
            //}
            //else
            //{
            //    ri.Msg = "QQ回调异常";
            //}
            //return Result(ri); 
            #endregion
            ResultInfo ri = new ResultInfo();
            if (id.IsNotNullOrEmpty())
            {
                //已登录邮箱帐号，绑定QQ登录信息
                UserBase userinfo = UserInfo;
                if (userinfo == null)
                {
                    //根据openId查找是否首次登录
                    OAuth oauth = OAuthBLL.Instance.GetLogonInfoByOpenId(id, RegistEnumType.QQAuth.GetHashCode());
                    if (oauth != null)
                    {
                        string returnurl = GetRequest("ReturnUrl");
                        //ri.Ok = true;
                        if (!oauth.UserID.HasValue)
                        {
                            //UserBaseBLL.Instance.IsBindAccount = false;
                            //老版本第三方登录时未绑定UserID
                            //注册
                            UserBase u = new UserBase()
                            {
                                OpenId = id,
                                HeadUrl = headurl,
                                UserName = GetUserNameNoExist(name)
                            };
                            long _registUserId = Regist(u, RegistEnumType.QQAuth);
                            if (_registUserId > 0)
                            {
                                oauth.UserID = _registUserId;
                                oauth.NickName = u.UserName;
                                ri.Ok = OAuthBLL.Instance.Update(oauth).Ok;//更新Oauth的Userid
                                ri.Url = returnurl;
                            }
                        }
                        else
                        {
                            //Oauth的UserID存在
                            //判断是否已绑定，则直接登录
                            //if (oauth.IsBind == 1)
                            //{
                            //已存在，直接设置登录状态
                            var user = UserBaseBLL.Instance.GetUserInfo(oauth.UserID.Value);
                            if (user.IsDelete != 0)
                            {
                                ri.Msg = "您的帐号已被管理员删除，无法登录！如有疑问，请联系管理员！";
                            }
                            else
                            {
                                //UserBaseBLL.Instance.IsBindAccount = oauth.IsBind == 1 ? true : false;
                                ri.Ok = true;
                                AfterLogin(user);
                                ri.Url = returnurl;
                            }
                            //}
                        }
                    }
                    else
                    {
                        //不存在，新用户
                        UserBase u = new UserBase()
                        {
                            OpenId = id,
                            UserName = GetUserNameNoExist(name),
                            HeadUrl = headurl
                        };
                        var _userid = Regist(u, RegistEnumType.QQAuth);
                        if (_userid > 0)
                        {
                            OAuth qqOauth = new OAuth()
                            {
                                HeadUrl = headurl,
                                IsBind = 0,
                                NickName = GetUserNameNoExist(name),
                                OAuthType = RegistEnumType.QQAuth.GetHashCode(),
                                OpenId = id,
                                UserID = _userid
                            };
                            if (OAuthBLL.Instance.Add(qqOauth) > 0)
                            {
                                //UserBaseBLL.Instance.IsBindAccount = false;
                                ri.Ok = true;
                            }
                        }

                        #region MyRegion
                        //OAuth model = new OAuth()
                        //{
                        //    HeadUrl = headurl,
                        //    IsBind = 0,
                        //    NickName = name.IsNotNullOrEmpty() ? name : "QQ用户" + DateTime.Now.ToString("yyyyMMddfff"),
                        //    OAuthType = RegistEnumType.QQAuth.GetHashCode(),
                        //    OpenId = id,
                        //};
                        //long oauthId = OAuthBLL.Instance.Add(model);
                        //if (oauthId > 0)
                        //{
                        //    SessionHelper.Set("QQAuthOpenID", oauthId, 1 * 15);//设置用户ID
                        //    ri.Ok = true;
                        //    string returnurl = GetRequest("ReturnUrl");
                        //    ri.Url = "/Account/LoginOauth?from=1&name={1}&ReturnUrl={0}".FormatWith(returnurl.IsNotNullOrEmpty() ? returnurl : "/", model.NickName);
                        //} 
                        #endregion
                    }
                }
                else
                {
                    //已有帐号里绑定第三方
                    OAuth qqOauth = OAuthBLL.Instance.GetLogonInfoByOpenId(id, RegistEnumType.QQAuth.GetHashCode());
                    if (qqOauth != null)
                    {
                        //之前使用QQ登录过
                        //直接更新
                        ri = BindAndDelete(userinfo, qqOauth, id, RegistEnumType.QQAuth.GetHashCode());
                    }
                    else
                    {
                        OAuth model = new OAuth()
                        {
                            HeadUrl = headurl,
                            IsBind = 1,
                            UserID = userinfo.UserID,
                            NickName = GetUserNameNoExist(name),
                            OAuthType = RegistEnumType.QQAuth.GetHashCode(),
                            OpenId = id
                        };
                        if (OAuthBLL.Instance.Add(model) > 0)
                        {
                            ri.Ok = true;
                            ri.Url = "/User/BindOauth";
                        }
                    }

                    #region MyRegion
                    //OAuth model = new OAuth()
                    //{
                    //    HeadUrl = headurl,
                    //    IsBind = 1,
                    //    UserID = userinfo.UserID,
                    //    NickName = name.IsNotNullOrEmpty() ? name : "QQ用户" + DateTime.Now.ToString("yyyyMMddfff"),
                    //    OAuthType = RegistEnumType.QQAuth.GetHashCode(),
                    //    OpenId = id,
                    //};
                    //long oauthId = OAuthBLL.Instance.Add(model);
                    //if (oauthId > 0)
                    //{
                    //    ri.Ok = true;
                    //    ri.Url = "/User/BindOauth";
                    //} 
                    #endregion
                }
            }
            else
            {
                ri.Msg = "QQ回调异常";
            }
            return Result(ri);
        }
        #endregion

        #region WeChat
        //public ActionResult WeChat()
        //{
        //    //WeChatBLL wx = new WeChatBLL();
        //    //string uri = wx.GetCode();
        //    //Response.Redirect(uri, true);
        //    //return Content(string.Empty);
        //    ResultInfo ri = new ResultInfo();
        //    WeChatBLL wx = new WeChatBLL();
        //    ri.Url = wx.GetCode();
        //    ri.Ok = true;
        //    return Result(ri);
        //}

        [HttpGet]
        public string WeChat()
        {
            return new WeChatBLL().GetState();
        }

        public ActionResult WxAuth(string code, string state, string ReturnUrl)
        {
            #region 注释
            //WeChatBLL wx = new WeChatBLL();

            //var ri = wx.GetToken(code, state);
            //if (ri.Ok)
            //{
            //    ResultInfo<WeChatUser> ruser = wx.GetUserInfo(ri.Data.access_token);
            //    if (ruser.Ok)
            //    {
            //        UserBase userinfo = UserInfo;
            //        if (userinfo == null)
            //        {
            //            //登录成功
            //            //查找帐号
            //            //UserBase user = UserBaseBLL.Instance.GetUserInfoByOpenId(ruser.Data.openid, RegistEnumType.WeChatAuth);
            //            OAuth oauth = OAuthBLL.Instance.GetLogonInfoByOpenId(ruser.Data.openid, RegistEnumType.WeChatAuth);
            //            if (oauth != null)
            //            {
            //                ri.Ok = true;
            //                string returnurl = GetRequest("ReturnUrl");

            //                //判断当前登录是否已绑定邮箱帐号
            //                if (oauth.IsBind == 1)
            //                {
            //                    //已存在，直接设置登录状态
            //                    var user = UserBaseBLL.Instance.GetUserInfo(Convert.ToInt64(oauth.UserID));
            //                    if (user.IsDelete != 0)
            //                    {
            //                        ri.Msg = "您的帐号已被管理员删除，无法登录！如有疑问，请联系管理员！";
            //                    }
            //                    else
            //                    {
            //                        AfterLogin(user);
            //                        ri.Url = returnurl;
            //                    }
            //                }
            //                else
            //                {
            //                    //跳转绑定邮箱
            //                    SessionHelper.Set("WechatAuthOpenID", oauth.OAuthID, 1 * 15);//设置用户ID
            //                    ri.Url = "/Account/LoginOauth?from=3&name={1}&ReturnUrl={0}".FormatWith(returnurl.IsNotNullOrEmpty() ? returnurl : "/", oauth.NickName);
            //                }
            //            }
            //            else
            //            {
            //                //不存在，新用户
            //                string name = ruser.Data.nickname;
            //                OAuth model = new OAuth()
            //                {
            //                    HeadUrl = ruser.Data.headimgurl,
            //                    IsBind = 0,
            //                    NickName = name.IsNotNullOrEmpty() ? name : "微信用户" + DateTime.Now.ToString("yyyyMMddfff"),
            //                    OAuthType = RegistEnumType.WeChatAuth.GetHashCode(),
            //                    OpenId = ruser.Data.openid,
            //                };
            //                long oauthId = OAuthBLL.Instance.Add(model);
            //                if (oauthId > 0)
            //                {
            //                    SessionHelper.Set("WechatAuthOpenID", oauthId, 1 * 15);//设置用户ID
            //                    ri.Ok = true;
            //                    string returnurl = GetRequest("ReturnUrl");
            //                    ri.Url = "/Account/LoginOauth?from=3&name={1}&ReturnUrl={0}".FormatWith(returnurl.IsNotNullOrEmpty() ? returnurl : "/", model.NickName);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            string name = ruser.Data.nickname;
            //            OAuth model = new OAuth()
            //            {
            //                HeadUrl = ruser.Data.headimgurl,
            //                IsBind = 1,
            //                UserID = userinfo.UserID,
            //                NickName = name.IsNotNullOrEmpty() ? name : "微信用户" + DateTime.Now.ToString("yyyyMMddfff"),
            //                OAuthType = RegistEnumType.WeChatAuth.GetHashCode(),
            //                OpenId = ruser.Data.openid,
            //            };
            //            long oauthId = OAuthBLL.Instance.Add(model);
            //            if (oauthId > 0)
            //            {
            //                ri.Ok = true;
            //                ri.Url = "/User/BindOauth";
            //            }
            //        }
            //    }
            //    Response.Redirect(ri.Url, true);
            //    return Content(string.Empty);
            //}
            //return Content(ri.Msg);
            #endregion
            WeChatBLL wx = new WeChatBLL();

            var ri = wx.GetToken(code, state);
            if (ri.Ok)
            {
                if (ReturnUrl.IsNullOrEmpty())
                {
                    ReturnUrl = "";
                }
                ReturnUrl = ReturnUrl.IsNullOrEmpty() ? "/" : ReturnUrl;
                ResultInfo<WeChatUser> ruser = wx.GetUserInfo(ri.Data.access_token);
                if (ruser.Ok)
                {
                    string _name = GetUserNameNoExist(ruser.Data.nickname);

                    UserBase userinfo = UserInfo;
                    if (userinfo == null)
                    {
                        //登录成功
                        //查找帐号
                        //根据openId查找是否首次登录
                        OAuth oauth = OAuthBLL.Instance.GetLogonInfoByOpenId(ruser.Data.openid, RegistEnumType.WeChatAuth.GetHashCode());
                        if (oauth != null)
                        {
                            //string returnurl = GetRequest("ReturnUrl");

                            if (!oauth.UserID.HasValue)
                            {
                                //UserBaseBLL.Instance.IsBindAccount = false;
                                //注册
                                UserBase u = new UserBase()
                                {
                                    OpenId = ruser.Data.openid,
                                    HeadUrl = ruser.Data.headimgurl,
                                    UserName = GetUserNameNoExist(ruser.Data.nickname)
                                };
                                long _registUserId = Regist(u, RegistEnumType.WeChatAuth);
                                if (_registUserId > 0)
                                {
                                    oauth.UserID = _registUserId;
                                    oauth.NickName = u.UserName;
                                    ri.Ok = OAuthBLL.Instance.Update(oauth).Ok;//更新Oauth的Userid;
                                    ri.Url = ReturnUrl;
                                }
                            }
                            else
                            {
                                var user = UserBaseBLL.Instance.GetUserInfo(oauth.UserID.Value);
                                if (user.IsDelete != 0)
                                {
                                    ri.Msg = "您的帐号已被管理员删除，无法登录！如有疑问，请联系管理员！";
                                }
                                else
                                {
                                    //UserBaseBLL.Instance.IsBindAccount = oauth.IsBind == 1 ? true : false;
                                    ri.Ok = true;
                                    AfterLogin(user);
                                    ri.Url = ReturnUrl;
                                }
                            }
                            ////判断当前登录是否已绑定邮箱帐号
                            //if (oauth.IsBind == 1)
                            //{
                            //    //已存在，直接设置登录状态
                            //    var user = UserBaseBLL.Instance.GetUserInfo(Convert.ToInt64(oauth.UserID));
                            //    if (user.IsDelete != 0)
                            //    {
                            //        ri.Msg = "您的帐号已被管理员删除，无法登录！如有疑问，请联系管理员！";
                            //    }
                            //    else
                            //    {
                            //        UserBaseBLL.Instance.IsBindAccount = true;
                            //        ri.Ok = true;
                            //        AfterLogin(user);
                            //        ri.Url = returnurl;
                            //    }
                            //}
                            //else
                            //{
                            //    //var oauthUser = UserBaseBLL.Instance.FindUserInfoByOpenID(ruser.Data.openid, RegistEnumType.WeChatAuth);
                            //    UserBaseBLL.Instance.IsBindAccount = false;
                            //    var oauthUser = UserBaseBLL.Instance.GetUserInfo(oauth.UserID.Value);
                            //    if (oauthUser == null)
                            //    {
                            //        //不存在则创建
                            //        UserBase u = new UserBase()
                            //        {
                            //            OpenId = ruser.Data.openid,
                            //            HeadUrl = ruser.Data.headimgurl,
                            //            UserName = _name
                            //        };
                            //        if (Regist(u, RegistEnumType.WeChatAuth) > 0)
                            //        {
                            //            ri.Ok = true;
                            //            ri.Url = returnurl;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (oauthUser.IsDelete != 0)
                            //        {
                            //            ri.Msg = "您的帐号已被管理员删除，无法登录！如有疑问，请联系管理员！";
                            //        }
                            //        else
                            //        {
                            //            ri.Ok = true;
                            //            AfterLogin(oauthUser);
                            //            ri.Url = returnurl;
                            //        }
                            //    }
                            //    ////跳转绑定邮箱
                            //    //SessionHelper.Set("WechatAuthOpenID", oauth.OAuthID, 1 * 15);//设置用户ID
                            //    //ri.Url = "/Account/LoginOauth?from=3&name={1}&ReturnUrl={0}".FormatWith(returnurl.IsNotNullOrEmpty() ? returnurl : "/", oauth.NickName);
                            //}
                        }
                        else
                        {
                            //不存在，新用户
                            //string name = (ruser.Data.nickname.IsNotNullOrEmpty() ? ruser.Data.nickname : "微信用户") + DateTime.Now.ToString("yyyyMMddfff");
                            UserBase u = new UserBase()
                            {
                                UserName = _name,
                                HeadUrl = ruser.Data.headimgurl,
                                OpenId = ruser.Data.openid
                            };

                            var _userid = Regist(u, RegistEnumType.WeChatAuth);
                            if (_userid > 0)
                            {
                                OAuth model = new OAuth()
                                {
                                    HeadUrl = ruser.Data.headimgurl,
                                    IsBind = 0,
                                    NickName = _name,
                                    OAuthType = RegistEnumType.WeChatAuth.GetHashCode(),
                                    OpenId = ruser.Data.openid,
                                    UserID = _userid
                                };
                                long oauthId = OAuthBLL.Instance.Add(model);
                                ri.Ok = oauthId > 0;
                                if (oauthId > 0)
                                {
                                    ri.Ok = true;
                                    ri.Url = ReturnUrl;
                                }
                                //if (oauthId > 0)
                                //{
                                //    SessionHelper.Set("WechatAuthOpenID", oauthId, 1 * 15);//设置用户ID
                                //    ri.Ok = true;
                                //    string returnurl = GetRequest("ReturnUrl");
                                //    ri.Url = "/Account/LoginOauth?from=3&name={1}&ReturnUrl={0}".FormatWith(returnurl.IsNotNullOrEmpty() ? returnurl : "/", model.NickName);
                                //}
                            }
                        }
                    }
                    else
                    {
                        //已有帐号里绑定微信
                        string openid = ruser.Data.openid;

                        OAuth wechatOauth = OAuthBLL.Instance.GetLogonInfoByOpenId(openid, RegistEnumType.WeChatAuth.GetHashCode());
                        if (wechatOauth != null)
                        {
                            //之前使用和微信登录过
                            //直接更新
                            ResultInfo _resultinfo = BindAndDelete(userinfo, wechatOauth, openid, RegistEnumType.WeChatAuth.GetHashCode());
                            ri.Ok = _resultinfo.Ok;
                            ri.Msg = _resultinfo.Msg;
                            ri.Url = _resultinfo.Url;
                            //BeginTran();
                            ////先删除微信登录对应的那条用户信息
                            //if (UserBaseBLL.Instance.DeleteUser(wechatOauth.UserID.Value, Tran))
                            //{
                            //    //再更新Oauth
                            //    wechatOauth.UserID = userinfo.UserID;
                            //    wechatOauth.IsBind = 1;
                            //    if (OAuthBLL.Instance.Update(wechatOauth, Tran).Ok)
                            //    {
                            //        ri.Ok = true;
                            //        ri.Url = "/User/BindOauth";
                            //        Commit();
                            //    }
                            //    else
                            //    {
                            //        RollBack();
                            //    }
                            //}
                            //else
                            //{
                            //    RollBack();
                            //}
                        }
                        else
                        {
                            OAuth model = new OAuth()
                            {
                                HeadUrl = ruser.Data.headimgurl,
                                IsBind = 1,
                                UserID = userinfo.UserID,
                                NickName = _name,
                                OAuthType = RegistEnumType.WeChatAuth.GetHashCode(),
                                OpenId = ruser.Data.openid,
                            };
                            long oauthId = OAuthBLL.Instance.Add(model);
                            if (oauthId > 0)
                            {
                                ri.Ok = true;
                                ri.Url = "/User/BindOauth";
                            }
                        }
                    }
                }
                Response.Redirect(ri.Url, true);
                return Content(string.Empty);
            }
            return Content(ri.Msg);
        }
        #endregion

        #region 绑定和删除帐号
        /// <summary>
        /// 绑定和删除帐号
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oauth"></param>
        /// <param name="openId"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private ResultInfo BindAndDelete(UserBase user, OAuth oauth, string openId, int source)
        {
            ResultInfo ri = new ResultInfo();
            BeginTran();
            //如果社交帐号在本站未注册会员，则跳过用户信息删除
            //先删除第三方登录对应的那条用户信息
            if (!oauth.UserID.HasValue || UserBaseBLL.Instance.DeleteUser(oauth.UserID.Value, Tran))
            {
                //更新邮箱帐号的一些基本数据
                if (user.HeadUrl.IsNullOrEmpty())
                {
                    user.HeadUrl = oauth.HeadUrl;
                }
                if (UserBaseBLL.Instance.Update(user, Tran).Ok)
                {
                    //再更新Oauth 设置邮箱帐号已绑定第三方登录
                    oauth.UserID = user.UserID;
                    oauth.IsBind = 1;
                    if (OAuthBLL.Instance.Update(oauth, Tran).Ok)
                    {
                        ri.Ok = true;
                        ri.Url = "{0}/User/BindOauth".FormatWith(Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, string.Empty));
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
                RollBack();
            }
            return ri;
        }
        #endregion

        #region 第三方帐号解除绑定
        [LOGIN]
        [HttpPost]
        public ActionResult UnBindOAuth(long id, int type)
        {
            ResultInfo ri = new ResultInfo();
            if (IsLogin)
            {
                if (type != 2)
                {
                    if (UserID == id)
                    {
                        //1、先将OAuth表里的绑定状态置为0
                        BeginTran();
                        var oauth = OAuthBLL.Instance.GetInfoByUserIDAndSource(id, type, Tran);
                        if (oauth != null)
                        {
                            if (oauth.IsBind == 1)
                            {
                                //2、再查找UserBase表里是否有注册，如果有注册则恢复
                                var oauthUserBase = UserBaseBLL.Instance.FindUserInfoByOpenID(oauth.OpenId, (RegistEnumType)type);
                                if (oauthUserBase != null)
                                {
                                    //恢复
                                    if (UserBaseBLL.Instance.ReCoverUser(oauthUserBase.UserID, Tran))
                                    {
                                        //将第三方登录信息里的数据的UserID同步为第三方登录注册的用户ID
                                        //解绑
                                        oauth.IsBind = 0;
                                        oauth.UserID = oauthUserBase.UserID;
                                        if (OAuthBLL.Instance.Update(oauth, Tran).Ok)
                                        {
                                            Commit();
                                            ri.Ok = true;
                                            ri.Msg = "解除绑定成功！您可以重新绑定其它帐号！";
                                        }
                                        else
                                        {
                                            RollBack();
                                            ri.Msg = "解除绑定失败！请重试";
                                        }
                                    }
                                    else
                                    {
                                        RollBack();
                                        ri.Msg = "解除绑定失败！请重试";
                                    }
                                }
                                else
                                {
                                    //如果社交帐号未在本站注册会员，则直接解绑 并 将UserID置为null
                                    oauth.IsBind = 0;
                                    oauth.UserID = null;
                                    //if (OAuthBLL.Instance.UpdateBindStatusOauthByID(id, type, 0, Tran))
                                    if (OAuthBLL.Instance.Update(oauth).Ok)
                                    {
                                        Commit();
                                        ri.Ok = true;
                                        ri.Msg = "解除绑定成功！您可以重新绑定其它帐号！";
                                    }
                                    else
                                    {
                                        RollBack();
                                        ri.Msg = "解除绑定失败！请重试";
                                    }
                                }
                            }
                            else
                            {
                                ri.Msg = "您当前已处于未绑定状态，无需解绑！";
                            }
                        }
                        else
                        {
                            ri.Msg = "当前帐号不存在绑定！无需解绑！";
                        }
                    }
                    else
                    {
                        ri.Msg = "登录帐号与解绑帐号不对！";
                    }
                }
                else
                {
                    ri.Msg = "解除绑定状态不对";
                }
            }
            else
            {
                ri.Msg = "未登录";
            }
            return Result(ri);
        }
        #endregion

        #region 用户昵称是否存在

        public bool ExistNickName(string nickname)
        {
            if (nickname.IsNullOrEmpty())
            {
                return false;
            }
            return UserBaseBLL.Instance.ExistUserName(nickname.Trim()) > 0;
        }

        #region 循环获取不重复用户名
        /// <summary>
        /// 循环获取不重复用户名
        /// </summary>
        /// <param name="oldName"></param>
        /// <returns></returns>
        public string GetUserNameNoExist(string oldName)
        {
            string newName = oldName;
            for (int i = 0; ; i++)
            {
                if (ExistNickName(newName))
                {
                    newName = oldName + i.ToString();
                }
                else
                {
                    break;
                }
            }
            return newName;
        }
        #endregion
        #endregion

        #region 注册UserExt
        private bool AddUserExt(long userId, bool openTran)
        {
            //添加用户扩展表
            UserExt userext = new UserExt()
            {
                UserID = userId,
                TotalScore = Convert.ToInt32(ConfigHelper.AppSettings("RegistScore")),//注册赠送100积分
                TotalCoin = 0,
                UserV = 0,
                VIP = 0,
                HeadNameShowType = 1
            };
            var tran = openTran ? Tran : null;
            if (UserExtBLL.Instance.Add(userext, tran) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 测试-批量更新用户信息
        //[HttpGet]
        //public void batchUpdateUserBaseInfo()
        //{
        //    var users = DB.UserBase.ToList();
        //    var userExts = DB.UserExt.ToList();
        //    var source = CoinSourceEnum.Sign.GetHashCode();
        //    var now = DateTime.Now.Date;
        //    var currentMonthFirstDay = new DateTime(now.Year, now.Month, 1);
        //    var day = now.Day;
        //    var userLogs = DB.ScoreCoinLog.Where(a => a.CoinSource == source && a.CoinTime >= currentMonthFirstDay).ToList();
        //    userExts.ForEach(user =>
        //    {
        //        var currentUserLogs = userLogs.Where(a => a.UserID == user.UserID).OrderByDescending(a => a.CoinTime).ToList();
        //        if (currentUserLogs.Count > 0)
        //        {
        //            //判断本月有几次签到
        //            var startTime = currentUserLogs[0].CoinTime.Date;
        //            var continueSignCount = 0;
        //            foreach (var currentUserLog in currentUserLogs)
        //            {
        //                if (startTime == currentUserLog.CoinTime.Date)
        //                {
        //                    continueSignCount++;
        //                    startTime = startTime.AddDays(-1);
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //            user.UserMonthSignContinueCount = continueSignCount;
        //        }
        //    });
        //    DB.SaveChanges();
        //}
        #endregion
    }
}