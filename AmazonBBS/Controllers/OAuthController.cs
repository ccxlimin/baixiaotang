using AmazonBBS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmazonBBS.Model;
using AmazonBBS.BLL;
using System.Security.Cryptography;
using System.Text;

namespace AmazonBBS.Controllers
{
    public class OAuthController : BaseController
    {
        public INoticeService noticeService { get; set; }
        /// <summary>
        /// 公钥
        /// </summary>
        private readonly string publicKey = "kSi1.9#Y";

        /// <summary>
        /// 请求授权密钥
        /// </summary>

        public readonly string oauthkey = "bxtCJQ.4";

        #region CS请求获取私钥
        /// <summary>
        /// CS请求获取私钥
        /// </summary>
        /// <param name="timespan"></param>
        /// <param name="oauthkey"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OauthKey(string timespan, string oauthkey, string userID)
        {
            bool ok = false;
            string msg = string.Empty;
            if (timespan.IsNullOrEmpty() || oauthkey.IsNullOrEmpty() || userID.IsNullOrEmpty())
            {
                msg = "请求参数为空！";
            }
            else
            {
                if (this.oauthkey.Equals(oauthkey))
                {
                    //验证时间戳是不是今天的
                    DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                    long lTime = long.Parse(timespan + "0000");
                    TimeSpan toNow = new TimeSpan(lTime);
                    var time = dtStart.Add(toNow);
                    if (DateTime.Now.Date <= time && time <= DateTime.Now)
                    {

                        //判断当前timespan是否存在数据库
                        var exist = DB.PrivateKey.FirstOrDefault(a => a.timespan == timespan && a.userID == userID);
                        if (exist == null)
                        {
                            var privateKey = new CodeHelper(16).GenerateCode();
                            //生成校验码
                            DB.PrivateKey.Add(new Model.PrivateKey
                            {
                                CreateTime = DateTime.Now,
                                IsDelete = 0,
                                ProtectKey = privateKey,
                                timespan = timespan,
                                userID = userID
                            });
                            DB.SaveChanges();
                            ok = true;
                            msg = privateKey;
                        }
                        else
                        {
                            msg = "生成失败";
                        }
                    }
                    else
                    {
                        msg = "请求失败";
                    }
                }
                else
                {
                    msg = "授权密钥不正确！";
                }
            }

            return Json(new { Ok = ok, Data = msg }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 登录
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sign"></param>
        /// <param name="timespan"></param>
        /// <param name="userName"></param>
        /// <param name="userID">采集器内部用户唯一ID号</param>
        /// <returns></returns>
        public ActionResult Amsharer(string sign, string timespan, string userName, string userID)
        {
            bool ok = false;
            if (sign.IsNotNullOrEmpty() && timespan.IsNotNullOrEmpty() && userName.IsNotNullOrEmpty() && userID.IsNotNullOrEmpty())
            {
                //校验PrivateKey
                var now = DateTime.Now;
                var start = now.Date;
                var exist = DB.PrivateKey.FirstOrDefault(a => a.timespan == timespan && a.userID == userID && a.CreateTime >= start && a.CreateTime <= now);
                if (exist != null)
                {
                    //加密校验
                    var tosign = $"{exist.ProtectKey}{publicKey}{userID}{userName}";
                    string pwd = "";
                    MD5 md5 = MD5.Create(); //实例化一个md5对像
                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                    byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(tosign));
                    // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
                    for (int i = 0; i < s.Length; i++)
                    {
                        // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                        var temp = s[i].ToString("X");
                        pwd = pwd + (temp.Length == 1 ? $"0{temp}" : temp);
                    }
                    var sign_ = pwd;
                    if (sign_ == sign)
                    {
                        #region 校验成功，判断该用户是否首次访问，进行数据库创建用户操作
                        var tran = DB.Database.BeginTransaction();
                        try
                        {
                            if (!IsLogin)
                            {
                                var source = RegistEnumType.AmzSharer.GetHashCode();
                                var user = DB.UserBase.FirstOrDefault(a => a.Source == source && a.IsDelete == 0 && a.OpenId == userID);
                                if (user == null)
                                {
                                    #region 创建用户
                                    user = new UserBase()
                                    {
                                        UserName = userName,
                                        HeadUrl = "/content/img/head_default.gif",
                                        OpenId = userID,
                                        Source = source,

                                        IsDelete = 0,
                                        CreateTime = DateTime.Now,
                                        UpdateTime = DateTime.Now,
                                        CreateUser = userName,
                                        UpdateUser = userName,

                                        LoginTime = DateTime.Now,
                                    };
                                    try
                                    {
                                        user.LoginIP = NetHelper.GetInternetIP();
                                    }
                                    catch
                                    {
                                        user.LoginIP = NetHelper.GetIPAddress2();
                                    }
                                    DB.UserBase.Add(user);

                                    DB.SaveChanges();
                                    UserExt userExt = new UserExt
                                    {
                                        UserID = user.UserID,
                                        TotalScore = ConfigHelper.AppSettings("RegistScore").ToInt32(),
                                        TotalCoin = 0,
                                        UserV = 0,
                                        VIP = 0,
                                        HeadNameShowType = 1
                                    };
                                    IsSign = false;
                                    DB.UserExt.Add(userExt);
                                    DB.SaveChanges();
                                    tran.Commit();

                                    //登录
                                    SetLogin(user);
                                    ok = true;
                                    #endregion
                                }
                                else
                                {
                                    if (user.IsDelete == 0)
                                    {
                                        //登录
                                        SetLogin(user);

                                        //用户当日是否已签到
                                        IsSign = ScoreCoinLogBLL.Instance.GetSignStatus(user.UserID);

                                        //用户登录计算VIP等级
                                        var userext = DB.UserExt.FirstOrDefault(a => a.UserID == user.UserID);
                                        if (userext.VIP > 0)
                                        {
                                            if (userext.VIPExpiryTime < DateTime.Now)
                                            {
                                                //过期
                                                userext.VIP = -userext.VIP;
                                                DB.SaveChanges();
                                                tran.Commit();
                                            }
                                        }
                                        ok = true;
                                    }
                                }
                            }
                            else
                            {
                                return Redirect("/");
                            }
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                        #endregion
                    }
                }
            }
            if (ok)
            {
                return Redirect("/");
            }
            else
            {
                return Content("登录失败，请重新尝试从客户端登录");
            }
        }
        #endregion

        #region 签到
        [HttpPost]
        public ActionResult Amzsharersign(string sign, string timespan, string userName, string userID)
        {
            ResultInfo ri = new ResultInfo();
            if (sign.IsNotNullOrEmpty() && timespan.IsNotNullOrEmpty() && userName.IsNotNullOrEmpty() && userID.IsNotNullOrEmpty())
            {
                //校验PrivateKey
                var now = DateTime.Now;
                var start = now.Date;
                var exist = DB.PrivateKey.FirstOrDefault(a => a.timespan == timespan && a.userID == userID && a.CreateTime >= start && a.CreateTime <= now);
                if (exist != null)
                {
                    //加密校验
                    var tosign = $"{exist.ProtectKey}{publicKey}{userID}{userName}";
                    string pwd = "";
                    MD5 md5 = MD5.Create(); //实例化一个md5对像
                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                    byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(tosign));
                    // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
                    for (int i = 0; i < s.Length; i++)
                    {
                        // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                        var temp = s[i].ToString("X");
                        pwd = pwd + (temp.Length == 1 ? $"0{temp}" : temp);
                    }
                    var sign_ = pwd;
                    if (sign_ == sign)
                    {
                        #region 校验成功，判断该用户是否首次访问，进行数据库创建用户操作
                        var tran = DB.Database.BeginTransaction();
                        try
                        {
                            var source = RegistEnumType.AmzSharer.GetHashCode();
                            var user = DB.UserBase.FirstOrDefault(a => a.Source == source && a.IsDelete == 0 && a.OpenId == userID);
                            if (user == null)
                            {
                                #region 创建用户
                                user = new UserBase()
                                {
                                    UserName = userName,
                                    HeadUrl = "/content/img/head_default.gif",
                                    OpenId = userID,
                                    Source = source,

                                    IsDelete = 0,
                                    CreateTime = DateTime.Now,
                                    UpdateTime = DateTime.Now,
                                    CreateUser = userName,
                                    UpdateUser = userName,

                                    LoginTime = DateTime.Now,
                                };
                                try
                                {
                                    user.LoginIP = NetHelper.GetInternetIP();
                                }
                                catch
                                {
                                    user.LoginIP = NetHelper.GetIPAddress2();
                                }
                                DB.UserBase.Add(user);

                                DB.SaveChanges();
                                UserExt userExt = new UserExt
                                {
                                    UserID = user.UserID,
                                    TotalScore = ConfigHelper.AppSettings("RegistScore").ToInt32(),
                                    TotalCoin = 0,
                                    UserV = 0,
                                    VIP = 0,
                                    HeadNameShowType = 1
                                };
                                DB.UserExt.Add(userExt);
                                DB.SaveChanges();
                                tran.Commit();
                                IsSign = false;
                                ri = Sign(user.UserID);
                                #endregion
                            }
                            else
                            {
                                if (user.IsDelete == 0)
                                {
                                    ri = Sign(user.UserID);
                                }
                            }
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                        #endregion
                    }
                }
            }
            return Json(new { Ok = ri.Ok, Data = ri.Msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 签到
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        private ResultInfo Sign(long userid)
        {
            ResultInfo ri = new ResultInfo();
            //判断今日是否已签到
            if (!ScoreCoinLogBLL.Instance.GetSignStatus(userid))
            {
                UserBase currentUserInfo = UserInfo;

                BeginTran();
                int coinSource = CoinSourceEnum.Sign.GetHashCode();
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

                //记录积分明细
                addModels.Add(new ScoreCoinLog()
                {
                    Coin = num,
                    CoinSource = CoinSourceEnum.Sign.GetHashCode(),
                    CoinType = CoinTypeEnum.Score.GetHashCode(),
                    CoinTime = DateTime.Now,
                    CreateUser = userid.ToString(),
                    UserID = userid,
                    UserName = currentUserInfo.UserName,
                });
                addModels.ForEach(n =>
                {
                    ri.Ok = ScoreCoinLogBLL.Instance.Add(n, Tran) > 0;
                });
                if (ri.Ok)
                {
                    //更新积分数值
                    if (UserExtBLL.Instance.AddScore(userid, num, 1, Tran))
                    {
                        //计算积分等级
                        string sqlTemp = "update UserExt set LevelName={0} where UserID={1};";
                        string empty = "null";
                        StringBuilder sb = new StringBuilder();
                        List<BBSEnum> levels = BBSEnumBLL.Instance.Query(3, true);
                        SignCountAndLevel userext = UserExtBLL.Instance.GetSignAndLevelByUserID(Tran, userid);
                        var tempLevel = levels.LastOrDefault(item => { return item.SortIndex <= userext.SignCount; });
                        if (tempLevel == null)
                        {
                            if (userext.LevelName.IsNotNullOrEmpty())
                            {
                                sb.Append(sqlTemp.FormatWith(empty, userext.UserID.ToString()));
                            }
                        }
                        else
                        {
                            if (tempLevel.BBSEnumId.ToString() != userext.LevelName)
                            {
                                sb.Append(sqlTemp.FormatWith(tempLevel.BBSEnumId.ToString(), userext.UserID.ToString()));
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
                        if (ri.Ok)
                        {
                            Commit();
                            ri.ID = DB.UserExt.FirstOrDefault(a => a.UserID == userid).TotalScore.Value;
                            ri.Msg = "签到成功";
                            IsSign = true;

                            //签到通知
                            NoticeBLL.Instance.OnSign_Notice_User(num, UserInfo, NoticeTypeEnum.Sign);
                            //连续签到赠送积分 通知用户
                            if (isSignEnough)
                            {
                                noticeService.OnSign_Enough_Notice_User(userid, userSignCountCurrentMonth, userSignEnoughCoin, isNewUser, now);
                            }
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
            return ri;
        }
        #endregion
    }
}