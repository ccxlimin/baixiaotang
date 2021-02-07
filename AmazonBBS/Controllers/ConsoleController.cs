using AmazonBBS.BLL;
using AmazonBBS.Common;
using AmazonBBS.Model;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace AmazonBBS.Controllers
{
    [IsMaster]
    public class ConsoleController : BaseController
    {
        public INoticeService noticeService { get; set; }
        public AmazonBBSDBContext amazonBBSDBContext { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        #region 审核帖子/文章/活动
        [IsMaster]
        public ActionResult CheckNote()
        {
            //BBSListViewModel model = new BBSListViewModel();
            //model.QuestionPage = InitPage(20);

            //model.QuestionList = QuestionBLL.Instance.GetCheckList(model.QuestionPage);
            //return View(model);
            return View();
        }

        [IsMaster]
        [HttpGet]
        public ActionResult LoadQuestionCheck()
        {
            ResultInfo<BBSListViewModel> ri = new ResultInfo<BBSListViewModel>();
            ri.Data = new BBSListViewModel();
            ri.Data.QuestionList = QuestionBLL.Instance.GetCheckList();
            ri.Ok = true;
            return Result(ri);
        }

        [IsMaster]
        [HttpGet]
        public ActionResult LoadArticleCheck()
        {
            ResultInfo<ArticleViewModel> ri = new ResultInfo<ArticleViewModel>();
            ri.Data = new ArticleViewModel();
            ri.Data.Articles = ArticleBLL.Instance.GetCheckList();
            ri.Ok = true;
            return Result(ri);
        }

        [IsMaster]
        [HttpGet]
        public ActionResult LoadPartyCheck()
        {
            ResultInfo<ActiveListViewModel> ri = new ResultInfo<ActiveListViewModel>();
            ri.Data = new ActiveListViewModel();
            ri.Data.Activis = ActivityBLL.Instance.GetCheckList();
            ri.Ok = true;
            return Result(ri);
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">0 不存在 1贴吧 2文章 3活动</param>
        /// <param name="check">2通过  3不通过</param>
        /// <returns></returns>
        [IsMaster]
        [HttpPost]
        public ActionResult Checked(int id = 0, int type = 0, int check = 2)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                if (type == 0)
                {
                    ri.Msg = "异常";
                }
                else
                {
                    string checkStatusMsg = check == 2 ? "通过" : "未通过";
                    if (type == 1)
                    {
                        Question model = QuestionBLL.Instance.GetModel(id);
                        if (model == null || model.IsDelete == 1)
                        {
                            ri.Msg = "帖子已被删除";
                        }
                        else
                        {
                            model.IsChecked = check;
                            if (check == 3)
                            {
                                model.IsDelete = 1;
                            }
                            ri = QuestionBLL.Instance.Update(model);
                            if (ri.Ok)
                            {
                                noticeService.On_Check_Handled_Notice_Publisher(model.UserID.Value, "帖子", ConfigHelper.AppSettings("BBSDetail").FormatWith(model.QuestionId), model.Title, checkStatusMsg, DateTime.Now);
                            }
                        }
                    }
                    else if (type == 2)
                    {
                        Article model = ArticleBLL.Instance.GetModel(id);
                        if (model == null || model.IsDelete == 1)
                        {
                            ri.Msg = "文章已被删除";
                        }
                        else
                        {
                            model.IsChecked = check;
                            if (check == 3)
                            {
                                model.IsDelete = 1;
                            }
                            ri = ArticleBLL.Instance.Update(model);
                            if (ri.Ok)
                            {
                                noticeService.On_Check_Handled_Notice_Publisher(model.UserID.Value, "文章", ConfigHelper.AppSettings("ArticleDetail").FormatWith(model.ArticleId), model.Title, checkStatusMsg, DateTime.Now);
                            }
                        }
                    }
                    else if (type == 3)
                    {
                        Activity model = ActivityBLL.Instance.GetModel(id);
                        if (model == null || model.IsDelete == 1)
                        {
                            ri.Msg = "活动已被删除";
                        }
                        else
                        {
                            model.IsChecked = check;
                            if (check == 3)
                            {
                                model.IsDelete = 1;
                            }
                            ri = ActivityBLL.Instance.Update(model);
                            if (ri.Ok)
                            {
                                noticeService.On_Check_Handled_Notice_Publisher(model.UserID.Value, "活动", ConfigHelper.AppSettings("PartyDetail").FormatWith(model.ActivityId), model.Title, checkStatusMsg, DateTime.Now);
                            }
                        }
                    }
                    else
                    {
                        ri.Msg = "不存在";
                    }
                }
            }
            else
            {
                ri.Msg = "ID异常";
            }
            //if (id > 0)
            //{
            //Question model = QuestionBLL.Instance.GetModel(id);
            //if (model.IsDelete == 1)
            //{
            //    ri.Msg = "帖子已被删除";
            //}
            //else
            //{
            //    model.IsChecked = check;
            //    if (check == 3)
            //    {
            //        model.IsDelete = 1;
            //    }
            //    ri = QuestionBLL.Instance.Update(model);
            //}
            //}
            //else
            //{
            //    ri.Msg = "帖子不存在";
            //}
            if (ri.Ok)
            {

            }
            return Result(ri);
        }

        #endregion

        #region 积分等级、头衔、专属头衔各Logo设置
        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="type">1:头衔 2:专属头衔 3:积分等级</param>
        /// <param name="id"></param>
        /// <param name="imgIndex"></param>
        /// <returns></returns>
        [HttpPost]
        [IsRoot]
        public ActionResult SetLevelLogo(string desc, int type, long id = 0, int imgIndex = 0)
        {
            ResultInfo ri = new ResultInfo();
            string logoname = type == 1 ? "levelname_" :
                type == 2 ? "onlylevelname_" : "scorelevelname_";
            string logoPath = "/Content/img/LevelLogo";

            var bBSEnumType = (type == 1 ? BBSEnumType.LevelName : type == 2 ? BBSEnumType.OnlyLevelName : BBSEnumType.ScoreLevel).GetHashCode();
            var info = DB.BBSEnum.FirstOrDefault(a => a.EnumType == bBSEnumType && a.IsDelete == 0 && a.BBSEnumId == id);
            if (info != null)
            {
                string fileName = bBSEnumType == BBSEnumType.LevelName.GetHashCode() ? "{0}{1}_{2}.png".FormatWith(logoname, desc, imgIndex) : "{0}{1}.png".FormatWith(logoname, desc);
                ri = UpLoadImg("file", logoPath, filename: fileName);
                //目前只有头衔 的 logo 路径是叠加的
                if (ri.Ok)
                {
                    if (bBSEnumType == BBSEnumType.LevelName.GetHashCode())
                    {
                        List<String> urls;
                        if (info.Url.IsNullOrEmpty())
                        {
                            //string empty = string.Empty;
                            //urls = new string[9] { empty, empty, empty, empty, empty, empty, empty, empty, empty };
                            urls = new List<string>();
                        }
                        else
                        {
                            urls = info.Url.Split(new string[] { "#BXT#" }, StringSplitOptions.None).ToList();
                        }
                        if (urls.Count == 0)
                        {
                            urls.Add(ri.Url);
                        }
                        else if (urls.Count < imgIndex)
                        {
                            urls.Add(ri.Url);
                        }
                        else
                        {
                            urls[imgIndex - 1] = ri.Url;
                        }
                        info.Url = string.Join("#BXT#", urls);
                    }
                    else
                    {
                        info.Url = ri.Url;
                    }
                    if (DB.SaveChanges() > 0)
                    {
                        ri.Ok = true;
                        string key = "MenuCache_{0}".FormatWith(info.EnumType);
                        CSharpCacheHelper.Remove(key);
                    }
                }
            }
            else
            {
                ri.Msg = "上传对象信息错误，本次上传失败";
            }
            return Result(ri);
        }
        #endregion

        #region 主页轮播管理
        [IsMaster]
        public ActionResult Slider()
        {
            ViewBag.slideChangeTime = ConfigHelper.AppSettings("slideChangeTime");
            List<Slide> lists = SlideBLL.Instance.GetALLSlider().OrderBy(a => a.IsDelete).ThenByDescending(a => a.SlideType).ThenByDescending(a => a.CreateTime).ToList();
            return View(lists);
        }

        #region 添加
        [IsMaster]
        [HttpPost]
        public ActionResult AddSlide(string name, string address, string color, int slideType)
        {
            ResultInfo ri = new ResultInfo();
            if (slideType > 0)
            {
                if (name.IsNotNullOrEmpty())
                {
                    if (address.IsNotNullOrEmpty())
                    {
                        address = address.StartsWith("http") ? address : "http://{0}".FormatWith(address);
                        address = address.Contains("www.") ? address : address.Contains("https") ? address.Replace("https://", "https://www.") : address.Replace("http://", "http://www.");
                    }
                    ri = UpLoadImg("file", "/Content/Slide", beforeSaveFile: (save, _ri) =>
                     {
                         _ri = save();
                         if (_ri.Ok)
                         {
                             Slide model = new Slide()
                             {
                                 Url = address,
                                 Title = name,
                                 IsDelete = 0,
                                 Img = _ri.Url,
                                 CreateUser = UserID.ToString(),
                                 CreateTime = DateTime.Now,
                                 FontColor = color,
                                 SlideType = slideType,
                             };
                             int rid = SlideBLL.Instance.Add(model);
                             if (rid > 0)
                             {
                                 model.SlideId = rid;
                                 _ri.Ok = true;
                                 var sliders = SlideBLL.Instance.GetALLSlider();
                                 sliders.Add(model);
                                 CSharpCacheHelper.Set(APPConst.Slider, sliders.OrderBy(a => a.IsDelete).ToList(), APPConst.ExpriseTime.Day2);
                             }
                         }
                         else
                         {
                             _ri.Msg = "上传图片失败";
                         }
                     });
                }
                else
                {
                    ri.Msg = "描述不能为空";
                }
            }
            else
            {
                ri.Msg = "轮播位置错误";
            }
            return Result(ri);
        }
        #endregion

        #region 删除/恢复 轮播图
        /// <summary>
        /// 删除/恢复 轮播图
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateType"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        [IsMaster]
        public ActionResult UpdateSlider(int id, int updateType, string desc)
        {
            ResultInfo ri = new ResultInfo();

            if (id > 0)
            {
                BeginTran();
                var sliders = SlideBLL.Instance.GetALLSlider();
                var model = sliders.FirstOrDefault(a => { return a.SlideId == id; });
                if (model != null)
                {
                    if (model.IsDelete == updateType)
                    {
                        ri.Msg = "该条数据状态已经是{0}状态！".FormatWith(desc);
                    }
                    else
                    {
                        ri.Ok = SlideBLL.Instance.SetDeleteOrNot(id, updateType, Tran);
                        if (ri.Ok)
                        {
                            Commit();
                            sliders.Where(a => a.SlideId == id).ToList().ForEach(a =>
                           {
                               a.IsDelete = updateType;
                           });
                            CSharpCacheHelper.Set(APPConst.Slider, sliders.OrderBy(a => a.IsDelete).ToList(), APPConst.ExpriseTime.Day2);
                        }
                        else
                        {
                            RollBack();
                        }
                    }
                }
                else
                {
                    ri.Msg = "没有此数据！";
                }
            }
            else
            {
                ri.Msg = "异常！";
            }

            return Result(ri);
        }
        #endregion

        /// <summary>
        /// 编辑轮播图
        /// </summary>
        /// <returns></returns>
        [IsMaster]
        [HttpPost]
        public ActionResult EditSlider(int id, string name, string address, string color, bool isChangeIMG, int slideType)
        {
            ResultInfo ri = new ResultInfo();
            var sliders = SlideBLL.Instance.GetALLSlider();
            var model = sliders.FirstOrDefault(a => a.SlideId == id);
            if (model != null)
            {
                if (isChangeIMG)
                {
                    ri = UpLoadImg("file", "/Content/Slide", beforeSaveFile: (save, ri_) =>
                         {
                             ri_ = save();
                             if (ri_.Ok)
                             {
                                 model.Img = ri_.Url;
                             }
                         });
                    if (!ri.Ok) { return Result(ri); }
                }
                if (address.IsNotNullOrEmpty())
                {
                    address = address.StartsWith("http") ? address : "http://{0}".FormatWith(address);
                    address = address.Contains("www.") ? address : address.Contains("https") ? address.Replace("https://", "https://www.") : address.Replace("http://", "http://www.");
                }
                model.IsDelete = 0;
                model.Title = name;
                model.Url = address;
                model.FontColor = color;
                model.SlideType = slideType;
                ri = SlideBLL.Instance.Update(model);
                if (ri.Ok)
                {
                    sliders.ForEach(a =>
                    {
                        if (a.SlideId == id)
                        {
                            a = model;
                        }
                    });
                    CSharpCacheHelper.Set(APPConst.Slider, sliders.OrderBy(a => a.IsDelete).ToList(), APPConst.ExpriseTime.Day2);
                }
            }
            else
            {
                ri.Msg = "轮播不存在";
            }
            return Result(ri);
        }
        #endregion

        #region 标签管理
        [IsRoot]
        public ActionResult TagManage()
        {
            //var menuList = BBSEnumBLL.Instance.Query(1, true);
            //menuList = menuList.Where(a => { return a.IsBBS == 1 || a.IsBBS == 4; }).ToList();
            //ViewBag.MenuType = menuList;
            //return View(GetTagsByType(menuList[0].BBSEnumId));
            return View(GetTagsByType());
        }

        [IsRoot]
        [HttpGet]
        public ActionResult GetTags(int type)
        {
            var model = GetTagsByType(type);
            return PartialView("/Views/Console/_LoadTagsList.cshtml", model);
        }

        /// <summary>
        /// 根据页面分类 获取标签
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private TagViewModel GetTagsByType(int type = 0)
        {
            var model = new TagViewModel()
            {
                TagPage = InitPage()
            };
            model.Tags = TagBLL.Instance.GetTags(type, model.TagPage);
            return model;
        }

        /// <summary>
        /// 删除或恢复标签
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        [IsRoot]
        public ActionResult UpdateTag(long id, int flag)
        {
            ResultInfo ri = new ResultInfo();
            var model = TagBLL.Instance.GetModel(id);
            if (model != null)
            {
                if (model.IsDelete != flag)
                {
                    model.IsDelete = flag;
                    ri = TagBLL.Instance.Update(model);
                }
                else
                {
                    ri.Ok = true;
                    ri.Msg = "该标签状态已置换！";
                }
            }
            else
            {
                ri.Msg = "标签数据不存在！";
            }
            return Result(ri);
        }
        #endregion

        #region 行业岗位管理
        [IsRoot]
        public ActionResult JobManage()
        {
            List<JobTrade> list = JobTradeBLL.Instance.SearchAll();
            ViewBag.JobTypeSelect = JobBLL.Instance.GetJobType();

            return View(list);
        }

        [IsRoot]
        public ActionResult GetTrade()
        {
            List<JobTrade> list = JobTradeBLL.Instance.SearchAll();
            return PartialView("_LoadJobTradeData", list);
        }

        [IsRoot]
        public ActionResult GetJobs()
        {
            var list = JobBLL.Instance.GetJobs();
            return PartialView("_LoadJobData", list);
        }

        [IsRoot]
        [HttpPost]
        public ActionResult DeleteTrade(Guid id, int type)
        {
            ResultInfo ri = new ResultInfo();
            var model = JobTradeBLL.Instance.GetModel(id);
            if (model != null)
            {
                model.IsDelete = type;
                ri = JobTradeBLL.Instance.Update(model);
            }
            else
            {
                ri.Msg = "该条数据不存在！";
            }
            return Result(ri);
        }

        [IsRoot]
        [HttpPost]
        public ActionResult DeleteJob(Guid id, int type)
        {
            ResultInfo ri = new ResultInfo();
            var model = JobBLL.Instance.GetModel(id);
            if (model != null)
            {
                model.IsDelete = type;
                ri = JobBLL.Instance.Update(model);
            }
            else
            {
                ri.Msg = "该条数据不存在！";
            }
            return Result(ri);
        }
        #endregion

        #region 成员帐号管理
        [IsMaster]
        [HttpGet]
        [ActionName("GetUserByKey")]
        public ActionResult GetUserByKey(string id)
        {
            ResultInfo ri = new ResultInfo();
            UserManageViewModel user = UserBaseBLL.Instance.GetUserByKey(id);
            if (user.UserInfoList != null && user.UserInfoList.Count > 0)
            {
                return PartialView("_UserInfoList", user);
            }
            else
            {
                ri.Msg = "无";
            }
            return Result(ri);
        }

        [IsMaster]
        [HttpGet]
        public ActionResult SearchUser(string search_login_condition, string search_login_count)
        {
            ResultInfo ri = new ResultInfo();

            var page = InitPage(defaultMaxPageSize: 200);
            if (search_login_condition.IsNotNullOrEmpty())
            {
                if (search_login_condition.ToInt32() < 3)
                {
                    if (search_login_count.IsNullOrEmpty())
                    {
                        ri.Msg = "登录次数不能为空";
                        return Result(ri);
                    }
                }
                int condition_login_type = search_login_condition.ToInt32();
                int condition_login_count = search_login_count.ToInt32();
                var condition = DB.UserBase.Where(a => 1 == 1 && a.IsDelete == 0);
                DateTime time = DateTime.Now;
                if (condition_login_type <= 3)
                {
                    time = condition_login_type == 1 ? time.Date.AddDays(-time.Day + 1) : condition_login_type == 2 ? time.AddMonths(-3) : time.AddMonths(-6);
                    condition = condition.Where(a => a.LoginTime >= time)
                            .Join(DB.UserLoginLog.Where(a => a.CreateTime >= time && !a.IsDelete).GroupBy(a => a.UserId).Select(a => new { UserId = a.Key, count = a.Count() }).Where(a => a.count >= condition_login_count), a => a.UserID, b => b.UserId, (a, b) => a);
                }
                else
                {
                    time = condition_login_type == 4 ? time.AddDays(-100) :
                        condition_login_type == 5 ? time.AddYears(-1) :
                        condition_login_type == 6 ? time.AddYears(-2) : time.AddYears(-3);
                    condition = condition.Where(a => a.LoginTime <= time);
                }
                int skipCount = page.StartIndex - 1;
                page.RecordCount = condition.Count();
                var list = skipCount == 0 ?
                    condition.OrderByDescending(a => a.LoginTime).Take(page.PageSize).ToList()
                    : condition.OrderByDescending(a => a.LoginTime).Skip(page.StartIndex - 1).Take(page.PageSize).ToList();
                UserManageViewModel userManageViewModel = new UserManageViewModel();
                if (list.Count > 0)
                {
                    userManageViewModel.UserInfoList = ModelConvertHelper<UserInfoViewModel>.ConvertToList(list.ToDataTable());
                }
                userManageViewModel.Page = page;
                ViewBag.Page = "_Page";
                return PartialView("_UserInfoList", userManageViewModel);
            }
            else
            {
                ri.Msg = "登录条件不能为空";
            }
            return Result(ri);
        }

        [IsMaster]
        public ActionResult AccountManage()
        {
            Paging page = InitPage(20, defaultMaxPageSize: 200);
            UserManageViewModel user = UserBaseBLL.Instance.QueryAllUserInfo(page, UserID);
            user.Page = page;
            ViewBag.PS = page.PageSize;
            return View(user);
        }

        [IsMaster]
        [HttpPost]
        public ActionResult SetCheckForUser(long id, int type = 0)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                UserBase user = UserBaseBLL.Instance.GetUserInfo(id);
                if (user == null)
                {
                    ri.Msg = "用户不存在";
                }
                if (user.IsDelete != 0)
                {
                    ri.Msg = "用户已被删除";
                }
                else
                {
                    UserExt uext = UserExtBLL.Instance.GetExtInfo(id);
                    if (uext.CheckBBS == type)
                    {
                        ri.Ok = true;
                    }
                    else
                    {
                        uext.CheckBBS = type;
                        ri.Ok = UserExtBLL.Instance.CheckBBS(id, type);
                    }
                }
            }
            else
            {
                ri.Msg = "异常";
            }
            return Result(ri);
        }

        /// <summary>
        /// 获取成员详细信息(仅限管理员有权限查看)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [IsMaster]
        [HttpGet]
        public ActionResult UserDetailInfo(long id)
        {
            ResultInfo<UserDetailInfoViewModel> ri = new ResultInfo<UserDetailInfoViewModel>();
            var user = UserBaseBLL.Instance.GetModel(id);
            //屏蔽密码
            user.Password = string.Empty;
            if (user != null)
            {
                ri.Data = new UserDetailInfoViewModel();
                ri.Ok = true;
                ri.Data.UserBase = user;
                ri.Data.UserExt = UserExtBLL.Instance.GetExtInfo(id);
            }
            else
            {
                ri.Msg = "用户不存在";
            }
            return Result(ri);
        }

        /// <summary>
        /// 删除帐号
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [IsMaster]
        [HttpPost]
        public ActionResult DeleteAccount(long uid)
        {
            ResultInfo ri = new ResultInfo();
            if (uid > 0)
            {
                var user = UserBaseBLL.Instance.GetUserInfo(uid);
                if (user != null)
                {
                    user.IsDelete = 1;
                    ri = UserBaseBLL.Instance.Update(user);
                }
                else
                {
                    ri.Msg = "该用户已被删除";
                }
            }
            else
            {
                ri.Msg = "异常";
            }
            return Result(ri);
        }

        /// <summary>
        /// 赠vip分
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        [IsMaster]
        [HttpPost]
        public ActionResult GiveVipScore(int score, long uid, int type)
        {
            return Result(AddScore(score, type, uid));
        }

        private ResultInfo AddScore(int coin, int coinType, long uid)
        {
            ResultInfo ri = new ResultInfo();
            if (coin > 0)
            {
                var user = UserBaseBLL.Instance.GetUserInfo(uid);
                if (user == null)
                {
                    ri.Msg = "用户不存在";
                }
                else
                {
                    //判断普通管理员本月是否已经赠送过一次了
                    bool normalMaster = UserBaseBLL.Instance.IsNormalMaster;
                    if (normalMaster)
                    {
                        var startDate = DateTime.Now.Date;
                        var enddate = DateTime.Now;
                        if (amazonBBSDBContext.ScoreCoinLog.FirstOrDefault(a => a.CreateUser == UserID.ToString() && a.UserID == user.UserID && a.CoinTime >= startDate && a.CoinTime <= enddate) != null)
                        {
                            ri.Msg = "本月你已经赠送过一次了";
                            return ri;
                        }
                    }
                    if (UserExtBLL.Instance.AddScore(user.UserID, coin, coinType))
                    {
                        if (ScoreCoinLogBLL.Instance.Log(coin, coinType, CoinSourceEnum.MasterGive, Convert.ToInt64(user.UserID), user.UserName, null, UserID))
                        {
                            var now = DateTime.Now;
                            //如果是普通管理员赠送积分，则通知超级管理员
                            if (UserBaseBLL.Instance.IsNormalMaster)
                            {
                                noticeService.OnGiveScoreSuccess_Notice_Root(UserInfo, user, coin, coinType, now);
                            }
                            //通知被赠人
                            noticeService.OnGiveScoreSuccess_Notice_User(user.UserID, coin, coinType, now);
                            ri.Ok = true;
                        }
                    }
                }
            }
            else
            {
                ri.Msg = coinType == 1 ? "赠送积分不能为0" : "赠送VIP分不能为0";
            }
            return ri;
        }
        #endregion

        #region 首页排序规则设置
        [IsRoot]
        public ActionResult HomeSortConfig()
        {
            return View();
        }

        [IsRoot]
        [HttpPost]
        public ActionResult SetHomeSortConfit(string sort, string value)
        {
            ResultInfo ri = new ResultInfo();
            return SetConfig(sort, value);
        }
        #endregion

        #region 签到随机设置
        [IsRoot]
        public ActionResult SignRandom()
        {
            return View();
        }

        [IsRoot]
        [HttpPost]
        public ActionResult AddSignRandom(int signScore, string signTip)
        {
            string signConfig = ConfigHelper.AppSettings("signconfig");
            int index = 1;
            if (signConfig.IsNotNullOrEmpty())
            {
                var chars = new[] { "|sign|" };
                index = signConfig.Split(chars, StringSplitOptions.RemoveEmptyEntries).Length + 1;
            }
            ConfigHelper.SetValue("signconfig", signConfig + "{3}{0}$sign${1}$sign${2}".FormatWith(index, signScore, HttpUtility.UrlDecode(signTip), index > 1 ? "|sign|" : string.Empty), "/ZConfig/system.config");
            ResultInfo ri = new ResultInfo() { Ok = true };
            return Result(ri);
        }

        [IsRoot]
        [HttpPost]
        public ActionResult EditSignRandom(int id, int signScore, string signTip)
        {
            ResultInfo ri = new ResultInfo();
            string signConfig = ConfigHelper.AppSettings("signconfig");
            if (signConfig.IsNullOrEmpty())
            {
                ri.Msg = "尚无配置！";
            }
            else
            {
                string[] signs = signConfig.Split(new[] { "|sign|" }, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder sb = new StringBuilder();
                foreach (string sign in signs)
                {
                    string[] signInfo = sign.Split(new[] { "$sign$" }, StringSplitOptions.RemoveEmptyEntries);
                    if (signInfo[0].ToInt32() == id)
                    {
                        signConfig = signConfig.Replace("{3}{0}$sign${1}$sign${2}".FormatWith(signInfo[0], signInfo[1], signInfo[2], id == 1 ? string.Empty : "|sign|"), "{3}{0}$sign${1}$sign${2}".FormatWith(id, signScore, HttpUtility.UrlDecode(signTip), id == 1 ? string.Empty : "|sign|"));
                        ConfigHelper.SetValue("signconfig", signConfig, "/ZConfig/system.config");
                        ri.Ok = true;
                        break;
                    }
                }
            }
            return Result(ri);
        }

        [IsRoot]
        [HttpPost]
        public ActionResult DeleteSignRandom(int id)
        {
            ResultInfo ri = new ResultInfo();
            string signConfig = ConfigHelper.AppSettings("signconfig");
            if (signConfig.IsNullOrEmpty())
            {
                ri.Msg = "尚无配置！";
            }
            else
            {
                string[] signs = signConfig.Split(new[] { "|sign|" }, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder sb = new StringBuilder();
                signs.ForEach(sign =>
                {
                    string[] signInfo = sign.Split(new[] { "$sign$" }, StringSplitOptions.RemoveEmptyEntries);
                    if (signInfo[0].ToInt32() == id)
                    {
                        signConfig = signConfig.Replace("{3}{0}$sign${1}$sign${2}".FormatWith(signInfo[0], signInfo[1], signInfo[2], id == 1 ? string.Empty : "|sign|"), string.Empty);
                        ConfigHelper.SetValue("signconfig", signConfig, "/ZConfig/system.config");
                        ri.Ok = true;
                    }
                });
            }

            return Result(ri);
        }
        #endregion

        #region 点击显示随机信息
        [IsRoot]
        public ActionResult TapShowInfo()
        {
            var list = ClickMsgBLL.Instance.FindALL().OrderByDescending(a => a.CreateTime).ToList();
            return View(list);
        }

        [IsRoot]
        [HttpPost]
        public ActionResult AddTapShowInfo(string clickTip, string clickTip_en, string color)
        {
            var ri = new ResultInfo();
            if (clickTip.IsNotNullOrEmpty())
            {
                if (clickTip_en.IsNotNullOrEmpty())
                {
                    ClickMsg model = new ClickMsg()
                    {
                        Msg = HttpUtility.UrlDecode(clickTip),
                        Msg_en = HttpUtility.UrlDecode(clickTip_en)
                        ,
                        Color = HttpUtility.UrlDecode(color)
                        ,
                        CreateTime = DateTime.Now
                        ,
                        CreateUser = UserID.ToString()
                        ,
                        UpdateTime = DateTime.Now
                        ,
                        UpdateUser = UserID.ToString()
                        ,
                        IsDelete = 0,
                        ClickMsgId = Guid.NewGuid()
                    };
                    ri.Ok = ClickMsgBLL.Instance.Add(model) > 0;
                    if (ri.Ok)
                    {
                        CacheBLL.Instance.Add_ClickMsg(model);
                    }
                }
                else
                {
                    ri.Msg = "英文提示语不能为空！";
                }
            }
            else
            {
                ri.Msg = "提示语不能为空！";
            }
            return Result(ri);
        }

        [IsRoot]
        [HttpPost]
        public ActionResult EditTapShowInfo(Guid id, string clickTip, string clickTip_en, string color)
        {
            ResultInfo ri = new ResultInfo();
            var model = ClickMsgBLL.Instance.FindALL().First(a => { return a.ClickMsgId == id; });
            if (model != null)
            {
                if (model.IsDelete == 0)
                {
                    model.Msg = HttpUtility.UrlDecode(clickTip);
                    model.Msg_en = HttpUtility.UrlDecode(clickTip_en);
                    model.Color = HttpUtility.UrlDecode(color);
                    if (ClickMsgBLL.Instance.Update(model).Ok)
                    {
                        ri.Ok = true;
                        CacheBLL.Instance.Update_ClickMsg(model);
                    }
                }
                else
                {
                    ri.Msg = "该条提示信息已被删除！";
                }
            }
            else
            {
                ri.Msg = "该条提示信息不存在！";
            }

            return Result(ri);
        }

        [IsRoot]
        [HttpPost]
        public ActionResult DeleteTapShowInfo(Guid id, int deleteOrReDelete)
        {
            ResultInfo ri = new ResultInfo();
            var model = ClickMsgBLL.Instance.FindALL().First(a => { return a.ClickMsgId == id; });
            if (model != null)
            {
                if (deleteOrReDelete == 1 && model.IsDelete == 1)
                {
                    ri.Msg = "该条提示信息已被删除！";
                }
                else if (deleteOrReDelete == 0 && model.IsDelete == 0)
                {
                    ri.Msg = "该条提示信息已恢复！";
                }
                else
                {
                    model.IsDelete = deleteOrReDelete;
                    if (ClickMsgBLL.Instance.Update(model).Ok)
                    {
                        ri.Ok = true;
                        CacheBLL.Instance.Update_ClickMsg(model);
                    }
                }
            }
            else
            {
                ri.Msg = "该条提示信息不存在！";
            }
            return Result(ri);
        }
        #endregion

        #region 用户法人认证审核
        [IsMaster]
        public ActionResult CheckUserAuth()
        {
            Paging page = InitPage(30);
            List<UserInfoViewModel> list = UserExtBLL.Instance.GetFaRenRenZheng(page);
            ViewBag.page = page;
            return View(list);
        }

        /// <summary>
        /// 用户请求法人认证通过
        /// </summary>
        /// <param name="id"></param>
        /// <param name="passType"></param>
        /// <returns></returns>
        [IsMaster]
        public ActionResult UserAuthPass(long id, int passType)
        {
            ResultInfo ri = new ResultInfo();

            if (id > 0)
            {
                UserExt model = UserExtBLL.Instance.GetExtInfo(id);
                if (model != null)
                {
                    if (passType == 1)
                    {
                        int userv = model.UserV.Value;
                        /*
                         1 红人            2 大牛        4 法人
                         3 红人 大牛       6 大牛 法人   5 红人 法人
                         7 红人 法人 大牛
                        */
                        if (userv >= 4)
                        {
                            ri.Msg = "该用户已被审核通过";
                            ri.Ok = true;
                        }
                        else
                        {
                            model.UserV += 4;
                            ri = UserExtBLL.Instance.Update(model);
                            if (ri.Ok) { ri.Msg = "审核成功"; }
                        }
                    }
                    else if (passType == 0)
                    {
                        ri.Ok = UserExtBLL.Instance.RejectUserAuth(id);
                        //删除营业执照和身份证照
                        UploadHelper.DeleteUpFile(model.FaRenPic);
                        UploadHelper.DeleteUpFile(model.CardPic);
                    }
                }
                else
                {
                    ri.Msg = "异常";
                }

            }
            else
            {
                ri.Msg = "异常";
            }
            return Result(ri);
        }

        #endregion

        #region 课程留言管理
        [IsRoot]
        public ActionResult LeaveWord()
        {
            var list = LeaveWordBLL.Instance.SearchAll();
            return View(list);
        }


        [IsRoot]
        public void ExportLeaveWord()
        {
            var list = LeaveWordBLL.Instance.SearchAll();
            if (list.Count > 0)
            {
                string leaveDiy1 = ConfigHelper.AppSettings("DIY1");
                string leaveDiy2 = ConfigHelper.AppSettings("DIY2");
                string leaveDiy3 = ConfigHelper.AppSettings("DIY3");

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("称呼"));
                dt.Columns.Add(new DataColumn("电话"));
                dt.Columns.Add(new DataColumn("年龄"));
                dt.Columns.Add(new DataColumn(leaveDiy1));
                dt.Columns.Add(new DataColumn(leaveDiy2));
                dt.Columns.Add(new DataColumn(leaveDiy3));
                dt.Columns.Add(new DataColumn("留言时间"));

                list.ForEach(item =>
                {
                    DataRow dr = dt.NewRow();
                    dr["称呼"] = item.Name;
                    dr["电话"] = item.Telephone;
                    dr["年龄"] = item.Age;
                    dr[leaveDiy1] = item.DIY1;
                    dr[leaveDiy2] = item.DIY2;
                    dr[leaveDiy3] = item.DIY3;
                    dr["留言时间"] = item.CreateTime;
                    dt.Rows.Add(dr);
                });
                NPOIExcelHelper.ExportByWeb(dt, "百晓堂课程留言信息", "百晓堂课程留言信息.xls");
            }
        }
        #endregion

        #region 软件设置-导航
        [IsRoot]
        public ActionResult SoftLink(long id = 0)
        {
            if (id > 0)
            {
                var model = SoftLinkBLL.Instance.GetModel(id);
                return View(model);
            }
            else
            {
                ViewBag.SoftLinkTypeList = SoftLinkTypeBLL.Instance.GetSoftLinkTypes();
                return View();
            }
        }

        /// <summary>
        /// 编辑软件导航分类
        /// </summary>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult EditSoftLinkType(int id, string softname, string softlogo, string softcolor)
        {
            ResultInfo ri = new ResultInfo();
            HttpPostedFileBase upfile = Request.Files["SoftLogoImg"];
            var model = SoftLinkTypeBLL.Instance.GetModel(id);
            string oldLogo = model.SoftLinkLogo;
            if (upfile == null)
            {
                //判断logo是否一致
                if (oldLogo != softlogo)
                {
                    ri.Msg = "请上传LOGO";
                    return Result(ri);
                }
            }
            else
            {
                string ext = Path.GetExtension(upfile.FileName);
                if (!CheckFileExt(ext))
                {
                    ri.Msg = $"不允许上传{ext}类型的文件！";
                }
                else
                {
                    string absolutePath = "/Content/SoftLogo";
                    string localPath = UploadHelper.GetMapPath(absolutePath);
                    if (!Directory.Exists(localPath))
                    {
                        Directory.CreateDirectory(localPath);
                    }

                    string filename = DateTimeHelper.GetToday(9) + "_" + UserID + ext;
                    string fullFilePath = localPath + "/" + filename;
                    upfile.SaveAs(fullFilePath);
                    model.SoftLinkLogo = "{0}/{1}".FormatWith(absolutePath, filename);
                }
            }
            model.SoftLinkTypeName = softname;
            model.SoftLinkColor = softcolor;
            ri = SoftLinkTypeBLL.Instance.Update(model);
            if (ri.Ok)
            {
                UploadHelper.DeleteUpFile(oldLogo);
            }
            return Result(ri);
        }

        /// <summary>
        /// 删除导航分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult DeleteSoftLinkType(int id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var softtype = SoftLinkTypeBLL.Instance.GetModel(id);
                if (softtype != null)
                {
                    if (softtype.IsDelete == 0)
                    {
                        softtype.IsDelete = 1;
                        if (SoftLinkTypeBLL.Instance.Update(softtype).Ok)
                        {
                            //将该导航分类下的所有超链接全置失效
                            SoftLinkBLL.Instance.DeleteALLByID(softtype.SoftLinkTypeID);
                            ri.Ok = true;
                            ri.Msg = "删除成功";
                            UploadHelper.DeleteUpFile(softtype.SoftLinkLogo);
                        }
                    }
                    else
                    {
                        ri.Msg = "该导航分类已被删除";
                        ri.Ok = true;
                    }
                }
                else
                {
                    ri.Msg = "未找到该导航分类，请刷新页面重试";
                }
            }
            else
            {
                ri.Msg = "异常";
            }
            return Result(ri);
        }

        /// <summary>
        /// 新增软件导航分类
        /// </summary>
        /// <returns></returns>
        [IsRoot]
        public ActionResult AddSoftType(string name, string color)
        {
            ResultInfo ri = new ResultInfo();
            if (name.IsNotNullOrEmpty())
            {
                HttpPostedFileBase upfile = Request.Files["logo"];
                if (upfile == null)
                {
                    ri.Msg = "请上传LOGO";
                }
                else
                {
                    string ext = Path.GetExtension(upfile.FileName);
                    if (!CheckFileExt(ext))
                    {
                        ri.Msg = $"不允许上传{ext}类型的文件！";
                    }
                    else
                    {
                        string absolutePath = "/Content/SoftLogo";
                        string localPath = UploadHelper.GetMapPath(absolutePath);
                        if (!Directory.Exists(localPath))
                        {
                            Directory.CreateDirectory(localPath);
                        }

                        string filename = DateTimeHelper.GetToday(9) + "_" + UserID + ext;
                        string fullFilePath = localPath + "/" + filename;
                        upfile.SaveAs(fullFilePath);

                        SoftLinkType model = new SoftLinkType();
                        model.SoftLinkTypeName = name;
                        model.SoftLinkColor = color;
                        model.IsDelete = 0;
                        model.CreateTime = DateTime.Now;
                        model.CreateUser = UserID.ToString();
                        model.SoftLinkLogo = "{0}/{1}".FormatWith(absolutePath, filename);
                        int result = SoftLinkTypeBLL.Instance.Add(model);
                        ri.Ok = result > 0;
                    }
                }
            }
            else
            {
                ri.Msg = "导航分类名称不能为空";
            }
            return Result(ri);
        }

        [IsRoot]
        public ActionResult AddSoftLink(string name, string href, int typeid, string desc, int otype, long id = 0)
        {
            ResultInfo ri = new ResultInfo();
            if (name.IsNotNullOrEmpty())
            {
                if (href.IsNotNullOrEmpty())
                {
                    if (desc.IsNotNullOrEmpty())
                    {
                        if (otype == 0)
                        {
                            SoftLink model = new SoftLink();
                            model.LinkName = name;
                            model.LinkAddress = href.StartsWith("http") ? href : "http://{0}".FormatWith(href);
                            model.SoftLinkType = typeid;
                            model.LinkMemo = desc;
                            model.CreateTime = DateTime.Now;
                            model.IsDelete = 0;

                            ri.Ok = SoftLinkBLL.Instance.Add(model) > 0;
                        }
                        else if (otype == 1)
                        {
                            SoftLink model = SoftLinkBLL.Instance.GetModel(id);
                            model.LinkName = name;
                            model.LinkAddress = href.StartsWith("http") ? href : "http://{0}".FormatWith(href);
                            model.SoftLinkType = typeid;
                            model.LinkMemo = desc;
                            ri = SoftLinkBLL.Instance.Update(model);
                        }
                    }
                    else
                    {
                        ri.Msg = "链接描述不能为空";
                    }
                }
                else
                {
                    ri.Msg = "链接地址不能为空";
                }
            }
            else
            {
                ri.Msg = "链接名称不能为空";
            }
            return Result(ri);
        }
        #endregion

        #region 网站基本配置

        [IsRoot]
        public ActionResult BaseConfig()
        {
            ViewBag.TagFixedNumber = ConfigHelper.AppSettings(APPConst.TagFixedNumber);
            ViewBag.TagRandomNumber = ConfigHelper.AppSettings(APPConst.TagRandomNumber);
            ViewBag.NewShowCount = ConfigHelper.AppSettings(APPConst.NewShowCount);
            ViewBag.ADCount = ConfigHelper.AppSettings("ADCount");
            ViewBag.PUBLISH_ZHAOPIN = ConfigHelper.AppSettings("PUBLISH_ZHAOPIN");
            ViewBag.PUBLISH_QIUZHI = ConfigHelper.AppSettings("PUBLISH_QIUZHI");
            ViewBag.PUBLISH_PRODUCT = ConfigHelper.AppSettings("PUBLISH_PRODUCT");
            ViewBag.vipScorePayvalue = ConfigHelper.AppSettings("vipScorePayByRMB");
            ViewBag.score2VipScorevalue = ConfigHelper.AppSettings("scoreChangeToVipScore");
            ViewBag.Prise = ConfigHelper.AppSettings("Prise");
            ViewBag.PriseFor = ConfigHelper.AppSettings("PriseFor");
            ViewBag.AutoCheck = ConfigHelper.AppSettings("AutoCheck_NeedScore");
            ViewBag.MustCheckByScore = ConfigHelper.AppSettings("MustCheckByScore");
            //ViewBag.kefuqq = ConfigHelper.AppSettings("KeFu_QQ");
            var type1 = CustomerEnumType.QQ.GetHashCode();
            var type2 = CustomerEnumType.WeChar.GetHashCode();
            var type3 = CustomerEnumType.WeChar_GZH.GetHashCode();
            ViewBag.kefuqq = DB.Customer.Where(a => a.Type == type1 && a.IsDelete == 0).OrderByDescending(a => a.CreateTime).ToList();
            ViewBag.wechat = DB.Customer.Where(a => a.Type == type2 && a.IsDelete == 0).OrderByDescending(a => a.CreateTime).ToList();
            ViewBag.wechat_gzh = DB.Customer.Where(a => a.Type == type3 && a.IsDelete == 0).OrderByDescending(a => a.CreateTime).ToList();

            ViewBag.ZhaoPinCONTACT = ConfigHelper.AppSettings("FeeHRZhaoPinValue");
            ViewBag.QiuZhiCONTACT = ConfigHelper.AppSettings("FeeHRQiuZhiValue");
            ViewBag.ProductCONTACT = ConfigHelper.AppSettings("FeeHRProductValue");

            ViewBag.BuyVIPFeeCount = ConfigHelper.AppSettings("BuyVIPFeeCount");
            ViewBag.HotUsersCount = ConfigHelper.AppSettings("HotUsersCount");
            ViewBag.NewUsersDays = ConfigHelper.AppSettings("NewUsersDays");
            ViewBag.UserAuth1 = ConfigHelper.AppSettings("UserAuth1");
            ViewBag.UserAuth2 = ConfigHelper.AppSettings("UserAuth2");
            ViewBag.UserAuth3 = ConfigHelper.AppSettings("UserAuth3");
            ViewBag.MustWriteLog = ConfigHelper.AppSettings("MustWriteLog");
            ViewBag.DIY1 = ConfigHelper.AppSettings("DIY1");
            ViewBag.DIY2 = ConfigHelper.AppSettings("DIY2");
            ViewBag.DIY3 = ConfigHelper.AppSettings("DIY3");

            ViewBag.UserAddTagCount = ConfigHelper.AppSettings("UserAddTagCount");
            ViewBag.UserEditCount = ConfigHelper.AppSettings("UserEditCount");
            ViewBag.normalUserSee_Score = ConfigHelper.AppSettings("normalUserSee_Score");
            ViewBag.normalMasterGiveUserMAXVIPScore = ConfigHelper.AppSettings("normalMasterGiveUserMAXVIPScore");
            ViewBag.normalMasterGiveUserMAXScore = ConfigHelper.AppSettings("normalMasterGiveUserMAXScore");
            ViewBag.scoreName_levelName_desc_url = ConfigHelper.AppSettings("scoreName_levelName_desc_url");

            ViewBag.new_user_continu_SignCount3 = ConfigHelper.AppSettings("new_user_continu_SignCount3");
            ViewBag.new_user_continu_SignCount10 = ConfigHelper.AppSettings("new_user_continu_SignCount10");
            ViewBag.old_user_total_SignCount3 = ConfigHelper.AppSettings("old_user_total_SignCount3");
            ViewBag.old_user_total_SignCount10 = ConfigHelper.AppSettings("old_user_total_SignCount10");
            ViewBag.old_user_total_SignCount20 = ConfigHelper.AppSettings("old_user_total_SignCount20");
            return View();
        }

        /// <summary>
        /// 设置QQ客服
        /// </summary>
        /// <param name="qq"></param>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult SetQQKefu(string qq, string name)
        {
            //string qqkefu = ConfigHelper.AppSettings("KeFu_QQ") + ",{0}".FormatWith(qq);
            //return SetConfig("KeFu_QQ", qqkefu);
            var customer = new Customer
            {
                CreateTime = DateTime.Now,
                Id = Guid.NewGuid(),
                Img = string.Empty,
                IsDelete = 0,
                Name = name,
                QQ = qq,
                Type = CustomerEnumType.QQ.GetHashCode(),
                UpdadteTime = DateTime.Now
            };
            DB.Customer.Add(customer);
            DB.SaveChanges();

            //加缓存
            var list = (CustomerVM)UserBaseBLL.Instance.GetCustomers();
            if (list.QQs == null)
            {
                list.QQs = new List<Customer>();
            }
            list.QQs.Add(customer);

            CSharpCacheHelper.Set(APPConst.Customoer, list);

            return Result(new ResultInfo()
            {
                Ok = true,
                Msg = "设置成功"
            });
        }

        /// <summary>
        /// 添加微信客服
        /// </summary>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult AddWeChatKefu(string name)
        {
            ResultInfo ri = new ResultInfo();
            HttpPostedFileBase upfile = Request.Files["wechat"];
            if (upfile == null)
            {
                ri.Msg = "请选择要上传的文件";
            }
            else
            {
                string ext = Path.GetExtension(upfile.FileName);
                if (!CheckFileExt(ext))
                {
                    ri.Msg = $"不允许上传{ext}类型的文件！";
                }
                else
                {
                    string absolutePath = "/Content/KeFu";
                    string localPath = UploadHelper.GetMapPath(absolutePath);
                    if (!Directory.Exists(localPath))
                    {
                        Directory.CreateDirectory(localPath);
                    }

                    string filename = DateTimeHelper.GetToday(9) + "_" + UserID + ext;
                    string fullFilePath = localPath + "/" + filename;
                    upfile.SaveAs(fullFilePath);

                    //return SetConfig("KeFu_QRCode", wechatkefu);

                    var customer = new Customer
                    {
                        CreateTime = DateTime.Now,
                        Id = Guid.NewGuid(),
                        Img = "{0}/{1}".FormatWith(absolutePath, filename),
                        IsDelete = 0,
                        Name = name,
                        QQ = string.Empty,
                        Type = CustomerEnumType.WeChar.GetHashCode(),
                        UpdadteTime = DateTime.Now
                    };

                    DB.Customer.Add(customer);
                    DB.SaveChanges();

                    //加缓存
                    var list = (CustomerVM)UserBaseBLL.Instance.GetCustomers();
                    if (list.WXs == null)
                    {
                        list.WXs = new List<Customer>();
                    }
                    list.WXs.Add(customer);

                    CSharpCacheHelper.Set(APPConst.Customoer, list);

                    return Result(new ResultInfo()
                    {
                        Ok = true,
                        Msg = "设置成功"
                    });
                }
            }

            return Result(ri);
        }

        /// <summary>
        /// 添加微信公众号
        /// </summary>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult AddWeChatGZH(string name)
        {
            ResultInfo ri = new ResultInfo();
            HttpPostedFileBase upfile = Request.Files["wechatgzh"];
            if (upfile == null)
            {
                ri.Msg = "请选择要上传的文件";
            }
            else
            {
                string ext = Path.GetExtension(upfile.FileName);
                if (!CheckFileExt(ext))
                {
                    ri.Msg = $"不允许上传{ext}类型的文件！";
                }
                else
                {
                    string absolutePath = "/Content/WXGZH";
                    string localPath = UploadHelper.GetMapPath(absolutePath);
                    if (!Directory.Exists(localPath))
                    {
                        Directory.CreateDirectory(localPath);
                    }

                    string filename = DateTimeHelper.GetToday(9) + "_" + UserID + ext;
                    string fullFilePath = localPath + "/" + filename;
                    upfile.SaveAs(fullFilePath);

                    var customer = new Customer
                    {
                        CreateTime = DateTime.Now,
                        Id = Guid.NewGuid(),
                        Img = "{0}/{1}".FormatWith(absolutePath, filename),
                        IsDelete = 0,
                        Name = name,
                        QQ = string.Empty,
                        Type = CustomerEnumType.WeChar_GZH.GetHashCode(),
                        UpdadteTime = DateTime.Now
                    };

                    DB.Customer.Add(customer);
                    DB.SaveChanges();

                    //加缓存
                    var list = (CustomerVM)UserBaseBLL.Instance.GetCustomers();
                    if (list.GZHs == null)
                    {
                        list.GZHs = new List<Customer>();
                    }
                    list.GZHs.Add(customer);

                    CSharpCacheHelper.Set(APPConst.Customoer, list);

                    //return SetConfig("WeChat_GZH", wechatkefu);
                    return Result(new ResultInfo()
                    {
                        Ok = true,
                        Msg = "设置成功"
                    });
                }
            }

            return Result(ri);
        }

        [IsRoot]
        [HttpPost]
        public ActionResult customerdelete(Guid id)
        {
            ResultInfo ri = new ResultInfo();

            if (id != Guid.Empty)
            {
                var model = DB.Customer.FirstOrDefault(a => a.Id == id);
                if (model != null)
                {
                    model.IsDelete = 1;
                    DB.SaveChanges();
                    ri.Ok = true; ri.Msg = "删除成功";
                    CSharpCacheHelper.Remove(APPConst.Customoer);
                }
                else
                {
                    ri.Msg = "删除对象不存在";
                }
            }
            else
            {
                ri.Msg = "删除失败";
            }

            return Result(ri);
        }

        /// <summary>
        /// 删除QQ客服
        /// </summary>
        /// <param name="qq"></param>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult DeleteQQ(string qq)
        {
            ResultInfo ri = new ResultInfo();
            List<string> qqs = ConfigHelper.AppSettings("KeFu_QQ").Split(',').ToList();
            qqs.Remove(qq);
            return SetConfig("KeFu_QQ", string.Join(",", qqs.ToArray()));
        }


        /// <summary>
        /// 删除微信客服
        /// </summary>
        /// <param name="wechat"></param>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult DeleteWeChat(string wechat)
        {
            ResultInfo ri = new ResultInfo();
            List<string> wechats = ConfigHelper.AppSettings("KeFu_QRCode").Split(',').ToList();
            wechats.Remove(wechat);
            UploadHelper.DeleteUpFile(wechat);

            return SetConfig("KeFu_QRCode", string.Join(",", wechats.ToArray()));
        }

        /// <summary>
        /// 删除公众号
        /// </summary>
        /// <param name="wechat"></param>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult DeleteWeChatGZH(string wechat)
        {
            ResultInfo ri = new ResultInfo();
            List<string> wechats = ConfigHelper.AppSettings("WeChat_GZH").Split(',').ToList();
            wechats.Remove(wechat);
            UploadHelper.DeleteUpFile(wechat);

            return SetConfig("WeChat_GZH", string.Join(",", wechats.ToArray()));
        }

        #region 对配置文件作值修改

        [IsMaster]
        [HttpPost]
        public ActionResult SetConfig(string id, string value, bool cache = false)
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                ConfigHelper.SetValue(id, value.ToString(), "/ZConfig/system.config");
                ri.Ok = true;
                //设置缓存
                if (cache)
                {
                    CSharpCacheHelper.Set(id, value);
                }
            }
            catch (Exception e)
            {
                ri.Msg = "设置失败，稍后重试";
            }
            return Result(ri);
        }
        #endregion
        #endregion

        #region 菜单名维护
        /// <summary>
        /// 菜单名维护
        /// </summary>
        /// <returns></returns>
        [IsRoot]
        public ActionResult EditMenu()
        {
            List<BBSEnum> MenuList = BBSEnumBLL.Instance.Query(BBSEnumType.Menu.GetHashCode(), true);
            return View(MenuList);
        }

        /// <summary>
        /// 编辑菜单
        /// </summary>
        /// <param name="sort">排序号</param>
        /// <param name="cn">枚举中文名</param>
        /// <param name="id">主键</param>
        /// <param name="type">0 编辑 3新增头衔 4新增专属头衔</param>
        /// <param name="code">枚举英文Code</param>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult EditMenu(string sort, string cn, int id, int type, string code = "")
        {
            ResultInfo ri = new ResultInfo();
            if (!string.IsNullOrEmpty(sort))
            {
                if (MatchHelper.IsNum.IsMatch(sort))
                {
                    if (!string.IsNullOrEmpty(cn))
                    {
                        if (type == 5 && (!MatchHelper.IsNum.IsMatch(cn)))
                        {
                            ri.Msg = "奖励积分必须为数字！";
                        }
                        else
                        {
                            bool needUpdate = false;//更新头衔后，更新所有用户的等级

                            //判断是否已重复
                            if (type == 0)
                            {
                                BBSEnum model = BBSEnumBLL.Instance.GetModel(id);
                                if (model.EnumType == 5 && (!MatchHelper.IsNum.IsMatch(cn)))
                                {
                                    ri.Msg = "奖励积分必须为数字！";
                                }
                                else
                                {
                                    int sort_ = Convert.ToInt32(sort);
                                    needUpdate = model.EnumType == 3 && model.SortIndex != sort_;

                                    //判断是否重复
                                    if (!BBSEnumBLL.Instance.Exist(cn, Convert.ToInt32(model.EnumType), id))
                                    {
                                        string oldName = model.EnumDesc;

                                        model.EnumCode = code;
                                        model.EnumDesc = cn;
                                        model.SortIndex = sort_;
                                        model.UpdateUser = UserID.ToString();
                                        model.UpdateTime = DateTime.Now;
                                        if (BBSEnumBLL.Instance.Edit(model))
                                        {
                                            if (model.EnumType != 5)
                                            {
                                                string logoname = model.EnumType == 2 ? "scorelevelname_" : model.EnumType == 4 ? "onlylevelname_" : "levelname_";
                                                string oldFullName = UploadHelper.GetMapPath("/Content/img/LevelLogo/{0}{1}.png".FormatWith(logoname, oldName));
                                                if (System.IO.File.Exists(oldFullName))
                                                {
                                                    string newFullName = oldFullName.Replace(oldName, cn);
                                                    System.IO.File.Move(oldFullName, newFullName);
                                                }
                                            }
                                            ri.Ok = true;
                                            ri.Msg = "修改成功";
                                        }
                                        else
                                        {
                                            ri.Msg = "修改失败";
                                        }
                                        string key = "MenuCache_{0}".FormatWith(model.EnumType);
                                        CSharpCacheHelper.Remove(key);
                                    }
                                    else
                                    {
                                        ri.Msg = "名称已存在，不能重复！";
                                    }
                                }
                            }
                            else
                            {
                                needUpdate = type == 3;
                                if (!BBSEnumBLL.Instance.Exist(cn, type, 0))
                                {
                                    //新增
                                    BBSEnum model = new BBSEnum()
                                    {
                                        CreateTime = DateTime.Now,
                                        CreateUser = UserID.ToString(),
                                        EnumCode = code,
                                        EnumDesc = cn,
                                        EnumType = type,
                                        IsDelete = 0,
                                        SortIndex = Convert.ToInt32(sort),
                                        UpdateTime = DateTime.Now,
                                        UpdateUser = UserID.ToString()
                                    };
                                    if (BBSEnumBLL.Instance.Add(model) > 0)
                                    {
                                        ri.Ok = true;
                                        ri.Msg = "新增成功";
                                        string key = "MenuCache_{0}".FormatWith(type);
                                        CSharpCacheHelper.Remove(key);
                                    }
                                    else
                                    {
                                        ri.Msg = "新增失败";
                                    }
                                }
                                else
                                {
                                    ri.Msg = "名称已存在，不能重复！";
                                }
                            }
                            if (needUpdate)
                            {
                                try
                                {
                                    BeginTran();
                                    //更新每个用户相应等级
                                    List<SignCountAndLevel> users = UserExtBLL.Instance.GetALLSignAndLevel(Tran);
                                    List<BBSEnum> levels = BBSEnumBLL.Instance.Query(3, true);
                                    string sqlTemp = "update UserExt set LevelName={0} where UserID={1};\r\n";
                                    string empty = "null";
                                    StringBuilder sb = new StringBuilder();
                                    users.ForEach(user =>
                                    {
                                        var tempLevel = levels.LastOrDefault(item => { return item.SortIndex <= user.SignCount; });
                                        if (tempLevel == null)
                                        {
                                            if (user.LevelName.IsNotNullOrEmpty())
                                            {
                                                sb.Append(sqlTemp.FormatWith(empty, user.UserID.ToString()));
                                            }
                                        }
                                        else
                                        {
                                            if (tempLevel.BBSEnumId.ToString() != user.LevelName)
                                            {
                                                sb.Append(sqlTemp.FormatWith(tempLevel.BBSEnumId.ToString(), user.UserID.ToString()));
                                            }
                                        }
                                    });
                                    //更新
                                    if (SqlHelper.ExecuteSql(Tran, System.Data.CommandType.Text, sb.ToString(), null) > 0)
                                    {
                                        Commit();
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
                            }
                        }
                    }
                    else
                    {
                        ri.Msg = "菜单中文为能为空";
                    }
                }
                else
                {
                    ri.Msg = "第一列必须为数字！";
                }
            }
            else
            {
                ri.Msg = "显示次序不能为空";
            }
            return Result(ri);
        }

        [IsRoot]
        [HttpPost]
        public ActionResult SetMenuColor(long id, string fontcolor, string fontbgcolor, string pagebgcolor)
        {
            ResultInfo ri = new ResultInfo();
            var model = BBSEnumBLL.Instance.GetModel(id);
            if (model == null)
            {
                ri.Msg = "菜单不存在";
            }
            else
            {
                if (model.IsDelete == 0)
                {
                    model.FontColor = fontcolor;
                    model.FontBGColor = fontbgcolor;
                    model.PageBGColor = pagebgcolor;
                    model.UpdateTime = DateTime.Now;
                    model.UpdateUser = UserID.ToString();

                    ri = BBSEnumBLL.Instance.Update(model);
                    string key = "MenuCache_{0}".FormatWith(model.EnumType);
                    CSharpCacheHelper.Remove(key);
                }
                else
                {
                    ri.Msg = "菜单已被删除，不可再进行操作";
                }
            }

            return Result(ri);
        }
        #endregion

        #region 积分等级维护
        [IsRoot]
        public ActionResult EditScoreLevel()
        {
            return View();
        }

        #endregion

        #region 头衔维护
        /// <summary>
        /// 头衔维护
        /// </summary>
        /// <returns></returns>
        [IsRoot]
        public ActionResult EditLevelName()
        {
            return View();
        }
        #endregion

        #region 分享奖励设置
        [IsRoot]
        public ActionResult EditShareCoin()
        {
            return View();
        }
        #endregion

        #region 用户认证
        [IsRoot]
        public ActionResult UserV()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="type">1 红人 2 牛人 3 红人 牛人</param>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult UserV(string user, int type)
        {
            ResultInfo ri = new ResultInfo();
            long userid = UserBaseBLL.Instance.ExistUserName(user);
            if (userid > 0)
            {
                UserExt userext = UserExtBLL.Instance.GetExtInfo(userid);
                var userv = userext.UserV;
                if (userv == type)
                {
                    ri.Msg = "该用户已认证该类型！";
                }
                else
                {
                    if ((type == 1 && (userv == 1 || userv == 3 || userv == 5 || userv == 7)) || (type == 2 && (userv == 3 || userv == 6 || userv == 7)) || (type == 3 && (userv == 3 || userv == 7)))
                    {
                        ri.Msg = "该用户已认证该类型！";
                    }
                    //if (userv - 2 == type || userv - 1 == type || userv - 4 == type)
                    //{
                    //}
                    else
                    {
                        /*
                          1 红人            2 大牛        4 法人
                          3 红人 大牛       6 大牛 法人   5 红人 法人
                          7 红人 法人 大牛
                         */
                        if (userv == 4)
                        {
                            //userext.UserV = type == 1 ? 5 : type == 2 ? 6 : 7;
                            userext.UserV = userv + type;
                        }
                        else if (userv == 5)
                        {
                            userext.UserV = type == 1 ? 5 : 7;
                        }
                        else if (userv == 7)
                        {
                            ri.Msg = "已经认证了 红人 牛人 法人了，无需再操作";
                        }
                        else
                        {
                            userext.UserV = type;
                        }
                        ri = UserExtBLL.Instance.Update(userext);
                        if (ri.Ok)
                        {
                            ri.Msg = "认证成功";
                        }
                    }
                }
            }
            return Result(ri);
        }

        #endregion

        #region 用户头衔授予
        ///// <summary>
        ///// 用户头衔授予
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult GiveName()
        //{
        //    return View();
        //}

        /// <summary>
        /// 授予头衔
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="levelName">（专属）头衔的ID</param>
        /// <param name="only">是否专属头衔</param>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult GiveName(long uid, int levelName, bool only)
        {
            ResultInfo ri = new ResultInfo();
            if (uid > 0)
            {
                UserBase user = UserBaseBLL.Instance.GetUserInfo(uid);
                if (user != null)
                {
                    //更新用户头衔
                    if (UserExtBLL.Instance.UpdateLevelName(uid, levelName, only))
                    {
                        ri.Ok = true;
                        ri.Msg = "头衔授予成功";
                    }
                    else
                    {
                        ri.Msg = "头衔授予失败";
                    }
                }
                else
                {
                    ri.Msg = "用户不存在";
                }
            }
            else
            {
                ri.Msg = "授予用户不能为空";
            }
            return Result(ri);
        }
        #endregion

        #region 用户头像审核
        [IsMaster]
        public ActionResult CheckVIPHead()
        {
            var list = VIPHeadBLL.Instance.GetALLVIPHead();
            return View(list);
        }

        [IsMaster]
        [HttpPost]
        public ActionResult CheckVIPHead(int id, int passOrDeny)
        {
            ResultInfo ri = new ResultInfo();
            var model = VIPHeadBLL.Instance.GetModel(id);
            if (model != null & model.IsDelete == 0)
            {
                BeginTran();
                model.IsChecked = passOrDeny;
                ri = VIPHeadBLL.Instance.Update(model, Tran);
                if (ri.Ok)
                {
                    if (UserBaseBLL.Instance.UploadHeadUrl(model.HeadUrl, model.UserID.Value, Tran))
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
                ri.Msg = "信息不存在！";
            }
            return Result(ri);
        }
        #endregion

        #region 设置推荐网站
        [IsRoot]
        public ActionResult SuggestSite()
        {
            List<SuggestSite> list = SuggestSiteBLL.Instance.SearchAll();
            return View(list);
        }

        /// <summary>
        /// 新增推荐网站
        /// </summary>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult AddSuggestSite(string name, string address)
        {
            ResultInfo ri = new ResultInfo();
            var model = new SuggestSite()
            {
                CreateTime = DateTime.Now,
                IsDelete = 0,
                CreateUser = UserID.ToString(),
                SiteAddress = address.StartsWith("http") ? address : "http://{0}".FormatWith(address),
                SiteName = name
            };
            ri.Ok = SuggestSiteBLL.Instance.Add(model) > 0;
            return Result(ri);
        }

        /// <summary>
        /// 编辑推荐网站
        /// </summary>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult SuggestEdit(int id, string name, string address)
        {
            ResultInfo ri = new ResultInfo();
            if (name.IsNotNullOrEmpty())
            {
                if (address.IsNotNullOrEmpty())
                {
                    var model = SuggestSiteBLL.Instance.GetModel(id);
                    if (model != null)
                    {
                        if (model.IsDelete == 0)
                        {
                            model.SiteName = name;
                            model.SiteAddress = address.StartsWith("http") ? address : "http://{0}".FormatWith(address);
                            ri = SuggestSiteBLL.Instance.Update(model);
                        }
                        else
                        {
                            ri.Msg = "该推荐网站已被删除";
                        }
                    }
                    else
                    {
                        ri.Msg = "该推荐网站信息不存在";
                    }
                }
                else
                {
                    ri.Msg = "网站地址不能为空";
                }
            }
            else
            {
                ri.Msg = "网站名称不能为空";
            }



            return Result(ri);
        }

        /// <summary>
        /// 删除推荐网站
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult DeleteSuggestSite(int id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var model = SuggestSiteBLL.Instance.GetModel(id);
                if (model != null)
                {
                    if (model.IsDelete == 0)
                    {
                        model.IsDelete = 1;
                        ri = SuggestSiteBLL.Instance.Update(model);
                    }
                    else
                    {
                        ri.Msg = "该推荐网站已被删除";
                    }
                }
                else
                {
                    ri.Msg = "该推荐网站信息不存在";
                }
            }
            else
            {
                ri.Msg = "异常";
            }
            return Result(ri);
        }
        #endregion

        #region 设置管理员
        /// <summary>
        /// 设置管理员
        /// </summary>
        /// <returns></returns>
        [IsRoot]
        public ActionResult Master()
        {
            //获取当前所有管理员
            var masters = DB.Master.Where(a => a.UserID != UserID).ToList();
            ViewBag.masters = masters;
            ViewBag.menus = DB.BBSEnum.Where(a => a.IsBBS == 1 && a.IsDelete == 0).ToList();

            var userids = masters.Select(a => a.UserID).ToList();
            ViewBag.Users = DB.UserBase.Where(a => userids.Contains(a.UserID)).ToList();
            return View();
        }

        [IsRoot]
        [HttpPost]
        public ActionResult deletemaster(int id)
        {
            ResultInfo ri = new ResultInfo();
            var model = DB.Master.FirstOrDefault(a => a.MasterId == id);
            if (model != null)
            {
                DB.Master.Remove(model);
                DB.SaveChanges();

                //更新缓存
                CSharpCacheHelper.Remove("masterCache" + model.UserID);
                CSharpCacheHelper.Remove("rootCache" + model.UserID);
                CSharpCacheHelper.Remove("normalMasterCache" + model.UserID);

                ri.Ok = true;
                ri.Msg = "删除成功";
            }
            return Result(ri);
        }

        [IsRoot]
        [HttpPost]
        public ActionResult Master(string id, int bbsMenuID)
        {
            ResultInfo ri = new ResultInfo();
            if (!string.IsNullOrEmpty(id))
            {
                long userid = UserBaseBLL.Instance.ExistUserName(id);
                if (MasterBLL.Instance.Add(new Master()
                {
                    CreateTime = DateTime.Now,
                    IsDelete = 0,
                    IsRoot = 0,
                    BBSMenuId = bbsMenuID,
                    UserID = userid
                }) > 0)
                {
                    ri.Ok = true;
                    ri.Msg = "管理员分配成功";
                }
                else
                {
                    ri.Msg = "管理员分配失败";
                }
            }
            else
            {
                ri.Msg = "用户名不存在";
            }
            return Result(ri);
        }
        #endregion

        #region 兑换礼品编辑
        /// <summary>
        /// 兑换礼品编辑
        /// </summary>
        /// <returns></returns>
        [IsRoot]
        public ActionResult Gift()
        {
            return View();
        }
        #endregion

        #region 发布新活动
        /// <summary>
        /// 发布新活动
        /// </summary>
        /// <returns></returns>
        [IsRoot]
        public ActionResult Activity()
        {
            return View();
        }
        #endregion

        #region 用户匹配
        [HttpPost]
        public ActionResult CheckUser(string id)
        {
            ResultInfo ri = new ResultInfo();
            if (string.IsNullOrEmpty(id))
            {
                ri.Msg = "查找用户名不能为空";
            }
            else
            {
                if (UserBaseBLL.Instance.ExistUserName(id) > 0)
                {
                    ri.Ok = true;
                    ri.Msg = "匹配成功";
                }
                else
                {
                    ri.Msg = "查无此人";
                }
            }
            return Result(ri);
        }
        #endregion

        #region 广告匹配
        [IsRoot]
        [HttpPost]
        public ActionResult CheckAD(string id)
        {
            ResultInfo ri = new ResultInfo();
            if (string.IsNullOrEmpty(id))
            {
                ri.Msg = "查找广告标题不能为空";
            }
            else
            {
                var adlist = ADBLL.Instance.GetALLAD();
                if (adlist.Exists(a => { return a.ADID == id.ToInt64(); }))
                {
                    ri.Ok = true;
                    ri.Msg = "匹配成功";
                }
                else
                {
                    ri.Msg = "查无广告";
                }

                var adcache = (List<AD>)CSharpCacheHelper.Get(APPConst.AD);
                adcache.Add(new AD() { });
                //更新缓存
                CSharpCacheHelper.Set(APPConst.AD, adcache, APPConst.ExpriseTime.Day2);
            }
            return Result(ri);
        }
        #endregion

        #region 新增广告
        [IsRoot]
        [HttpPost]
        public ActionResult ADDAD(string ADTitle,
          string ADMSG,
          string ADContact,
          string ADWeChat)
        {
            ResultInfo ri = new ResultInfo();
            HttpPostedFileBase upfile = Request.Files["ADIMG"];
            if (upfile == null)
            {
                ri.Msg = "请选择要上传的文件";
            }
            else
            {
                string ext = Path.GetExtension(upfile.FileName);
                if (!CheckFileExt(ext))
                {
                    ri.Msg = $"不允许上传{ext}类型的文件！";
                }
                else
                {
                    string localPath = UploadHelper.GetMapPath("/Content/AD");
                    if (!Directory.Exists(localPath))
                    {
                        Directory.CreateDirectory(localPath);
                    }

                    string filename = DateTimeHelper.GetToday(9) + "_" + UserID + ext;
                    string fullFilePath = localPath + "/" + filename;
                    upfile.SaveAs(fullFilePath);

                    AD model = new AD()
                    {
                        ADIMG = "/Content/AD/" + filename,
                        ADMsg = ADMSG,
                        ADTitle = ADTitle,
                        ADViewCount = 0,
                        ADWeChat = ADWeChat,
                        CreateTime = DateTime.Now,
                        CreateUser = UserID.ToString(),
                        IsDelete = 0,
                        ADContact = ADContact,
                    };
                    ri.Ok = ADBLL.Instance.Add(model) > 0;

                    var adcache = (List<AD>)CSharpCacheHelper.Get(APPConst.AD);
                    if (adcache == null)
                    {
                        adcache = amazonBBSDBContext.AD.Where(a => a.IsDelete == 0).ToList();
                    }
                    adcache.Add(model);
                    //更新缓存
                    CSharpCacheHelper.Set(APPConst.AD, adcache, APPConst.ExpriseTime.Day2);
                }
            }
            return Result(ri);
        }

        /// <summary>
        /// 检查文件扩展名是否合法
        /// </summary>
        /// <param name="fileExt"></param>
        /// <returns></returns>
        private bool CheckFileExt(string fileExt)
        {
            //检查危险文件
            string[] excExt = { "asp", "aspx", "ashx", "asa", "asmx", "asax", "php", "jsp", "htm", "html" };
            for (int i = 0; i < excExt.Length; i++)
            {
                if (excExt[i].ToLower() == fileExt.ToLower())
                {
                    return false;
                }
            }
            //检查合法文件
            return CheckUploadFileType(fileExt);
        }

        private static bool CheckUploadFileType(string fileName)
        {
            //允许的文件类型，如"rar,txt"
            var allowUploadFileType = ConfigHelper.AppSettings("AllowUploadFileType");

            //得到后缀，如rar
            var fileType = fileName.Substring(fileName.LastIndexOf(".") + 1);
            var typeList = allowUploadFileType.Split(',');

            return typeList.Contains(fileType) ? true : false;
        }
        #endregion

        #region 设置用户能够发表文章的等级
        [IsRoot]
        [HttpPost]
        [ActionName("SETARTICLE")]
        public ActionResult setarticle(int id)
        {
            ResultInfo ri = new ResultInfo();
            if (id == 0)
            {
                ri.Msg = "选择ID错误，请刷新页面重新设置";
            }
            else
            {
                BBSEnum model = BBSEnumBLL.Instance.GetModel(id);
                if (model != null)
                {
                    //取消之前的设置
                    BBSEnum old = BBSEnumBLL.Instance.GetSetArticleRol();
                    if (old != null)
                    {
                        old.CanArticle = null;
                        BBSEnumBLL.Instance.Edit(old);
                    }
                    model.CanArticle = 1;
                    if (BBSEnumBLL.Instance.Edit(model))
                    {
                        ri.Ok = true;
                        ri.Msg = "设置成功";
                    }
                }
                else
                {
                    ri.Msg = "所设置的条例不存在";
                }
            }
            return Result(ri);
        }
        #endregion

        #region 设置微信号
        [IsRoot]
        [HttpPost]
        public ActionResult SetWeChat(string title, string wechat)
        {
            ResultInfo ri = new ResultInfo();
            AD model = ADBLL.Instance.Find(title);
            if (model != null)
            {
                model.ADWeChat = wechat;
                ri = ADBLL.Instance.Update(model);
            }
            return Result(ri);
        }
        #endregion

        #region 定义网站默认头像
        [IsRoot]
        public ActionResult Head()
        {
            List<SiteHead> list = SiteHeadBLL.Instance.SearchAll();
            return View(list);
        }

        /// <summary>
        /// 添加头像
        /// </summary>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult AddHeadIMG()
        {
            ResultInfo ri = UpLoadImg("file", "/Content/img/Head", filename: DateTimeHelper.GetToday(9), onlyThumbImg: true);
            if (ri.Ok)
            {
                SiteHead model = new SiteHead()
                {
                    CreateTime = DateTime.Now,
                    HeadImg = ri.Url,
                    IsDefault = 0
                };
                ri.Ok = SiteHeadBLL.Instance.Add(model) > 0;
                if (!ri.Ok)
                {
                    UploadHelper.DeleteUpFile(ri.Url);
                }
            }
            return Result(ri);
        }

        /// <summary>
        /// 删除头像
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult DeleteHead(long id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var model = SiteHeadBLL.Instance.GetModel(id);
                if (model != null)
                {
                    ri = SiteHeadBLL.Instance.Delete(id);
                    if (ri.Ok)
                    {
                        UploadHelper.DeleteUpFile(model.HeadImg);
                    }
                }
                else
                {
                    ri.Msg = "头像数据不存在，可能已被删除";
                }
            }
            else
            {
                ri.Msg = "ID异常";
            }
            return Result(ri);
        }
        #endregion

        #region 新增学习课程
        [IsRoot]
        public ActionResult AddStudyChapter()
        {
            var tt = DB.UserBase.FirstOrDefault();
            var list = DB.StudyUnit.Where(a => a.IsDelete == 0).OrderBy(a => a.SortIndex).ToList();
            return View(list);
        }

        [IsRoot]
        public ActionResult GetUnit()
        {
            return PartialView("_LoadStudyUnitData", DB.StudyUnit.Where(a => a.IsDelete == 0).OrderBy(a => a.SortIndex).ToList());
        }

        [IsRoot]
        public ActionResult GetUnitClass()
        {
            return PartialView("_LoadStudyClassData", _GetUnitClass());
        }

        [IsRoot]
        public List<StudyInfoVM> _GetUnitClass()
        {
            var studys = DB.StudyUnit.Where(a => a.IsDelete == 0).OrderBy(a => a.SortIndex).Select(a => new StudyInfoVM
            {
                Index = a.SortIndex,
                UnitId = a.StudyUnitId,
                UnitName = a.Name,
                ClassInfoVMs = DB.StudyClass.Where(temp => temp.IsDelete == 0 && temp.StudyUnitId == a.StudyUnitId).OrderBy(tep => tep.SortIndex).Select(temp => new ClassInfoVM
                {
                    ClassId = temp.StudyClassId,
                    ClassName = temp.Name,
                    Index = temp.SortIndex
                }).ToList()
            }).ToList();
            return studys;
        }

        [IsRoot]
        [HttpPost]
        public ActionResult ClassInfo(Guid id)
        {
            ResultInfo ri = new ResultInfo();
            ri.Data = DB.StudyClass.FirstOrDefault(a => a.StudyClassId == id).Content;
            ri.Ok = true;
            return Json(ri);
        }

        [IsRoot]
        [HttpPost]
        public ActionResult NewUnit(string unitName)
        {
            ResultInfo ri = new ResultInfo();
            if (unitName.IsNotNullOrEmpty())
            {
                DB.StudyUnit.Add(new StudyUnit()
                {
                    CreateUser = UserID,
                    IsDelete = 0,
                    Name = unitName,
                    SortIndex = DB.StudyUnit.Count(a => a.IsDelete == 0),
                    StudyUnitId = Guid.NewGuid()
                });
                DB.SaveChanges();
                ri.Ok = true;
            }
            else
            {
                ri.Msg = "学习单元名称不能为空！";
            }
            return Json(ri);
        }

        /// <summary>
        /// 新增课时
        /// </summary>
        /// <param name="className"></param>
        /// <param name="unitId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult NewClass(string className, Guid unitId, string content)
        {
            ResultInfo ri = new ResultInfo();

            if (className.IsNotNullOrEmpty())
            {
                if (unitId != Guid.Empty)
                {
                    if (content.IsNotNullOrEmpty())
                    {
                        if (DB.StudyUnit.FirstOrDefault(a => a.StudyUnitId == unitId) != null)
                        {
                            if (DB.StudyClass.FirstOrDefault(a => a.Name == className) == null)
                            {
                                DB.StudyClass.Add(new StudyClass
                                {
                                    Content = HttpUtility.UrlDecode(content),
                                    CreateUser = UserID,
                                    IsDelete = 0,
                                    Name = className,
                                    SortIndex = DB.StudyClass.Count(a => a.StudyUnitId == unitId) + 1,
                                    StudyUnitId = unitId,
                                    StudyClassId = Guid.NewGuid(),
                                    CreateTime = DateTime.Now
                                });

                                DB.StudyClass.FutureCount();
                                DB.SaveChanges();
                                ri.Ok = true;
                                ri.Msg = "添加成功";
                            }
                            else
                            {
                                ri.Msg = "课时标题已存在";
                            }
                        }
                        else
                        {
                            ri.Msg = "课程章节不存在！";
                        }
                    }
                    else
                    {
                        ri.Msg = "课时内容不能为空";
                    }
                }
                else
                {
                    ri.Msg = "课时对应的章节不能为空";
                }
            }
            else
            {
                ri.Msg = "课时标题不能为空";
            }

            return Json(ri);
        }

        #region 编辑学习章节
        [IsRoot]
        [HttpPost]
        public ActionResult EditUnitStudyUnit(Guid id, string unitName)
        {
            ResultInfo ri = new ResultInfo();
            if (id != Guid.Empty)
            {
                if (unitName.IsNotNullOrEmpty())
                {
                    if (DB.StudyUnit.Where(a => a.StudyUnitId == id).Update(a => new StudyUnit { Name = unitName }) > 0)
                    {
                        ri.Ok = true;
                        ri.Msg = "修改成功";
                    }
                }
                else
                {
                    ri.Msg = "章节名称不能为空！";
                }
            }
            else
            {
                ri.Msg = "修改失败，请刷新页面重试！";
            }
            return Json(ri);
        }
        #endregion

        #region 删除学习章节
        [IsRoot]
        [HttpPost]
        public ActionResult DeleteUnit(Guid id)
        {
            ResultInfo ri = new ResultInfo();
            var old = DB.StudyUnit.First(a => a.StudyUnitId == id);
            if (old != null)
            {
                var tran = DB.Database.BeginTransaction();
                DB.StudyUnit.Remove(old);
                var classes = DB.StudyClass.Where(a => a.StudyUnitId == id);
                DB.StudyClass.RemoveRange(classes);
                var studyclasses = DB.UserStudy.Where(a => a.StudyUnitId == id);
                DB.UserStudy.RemoveRange(studyclasses);
                DB.SaveChanges();
                tran.Commit();
                ri.Ok = true;
                ri.Msg = "删除成功";
            }
            else
            {
                ri.Msg = "删除对象不存在！";
            }

            return Json(ri);
        }
        #endregion

        #region 编辑学习课程
        [IsRoot]
        [HttpPost]
        public ActionResult EditStudyClass(Guid id, string className, string content)
        {
            ResultInfo ri = new ResultInfo();
            if (id != Guid.Empty)
            {
                if (className.IsNotNullOrEmpty())
                {
                    if (content.IsNotNullOrEmpty())
                    {
                        if (DB.StudyClass.Where(a => a.StudyClassId == id).Update(a => new StudyClass { Name = className, Content = HttpUtility.UrlDecode(content) }) > 0)
                        {
                            ri.Ok = true;
                            ri.Msg = "修改成功";
                        }
                    }
                    else
                    {
                        ri.Msg = "课时内容不能为空！";
                    }
                }
                else
                {
                    ri.Msg = "课时名称不能为空！";
                }
            }
            else
            {
                ri.Msg = "修改失败，请刷新页面重试！";
            }
            return Json(ri);
        }
        #endregion

        #region 删除学习课程
        [IsRoot]
        [HttpPost]
        public ActionResult DeleteClass(Guid id)
        {
            ResultInfo ri = new ResultInfo();
            var old = DB.StudyClass.First(a => a.StudyClassId == id);
            if (old != null)
            {
                var tran = DB.Database.BeginTransaction();
                DB.StudyClass.Remove(old);
                var studyclasses = DB.UserStudy.Where(a => a.StudyClassId == id);
                DB.UserStudy.RemoveRange(studyclasses);
                DB.SaveChanges();
                tran.Commit();
                ri.Ok = true;
                ri.Msg = "删除成功";
            }
            else
            {
                ri.Msg = "删除对象不存在！";
            }

            return Json(ri);
        }
        #endregion

        #region 查看设置的图片
        [IsRoot]
        [HttpGet]
        public ActionResult GetStudyShareImg()
        {
            ResultInfo ri = new ResultInfo();

            ri.Data = DB.StudyShareImgSet.OrderByDescending(a => a.IsUse).ThenByDescending(a => a.CreateTime).ToList().Select(a => new
            {
                id = a.StudyShareImgSetId,
                isuse = a.IsUse,
                url = a.Url,
                time = a.CreateTime.ToString(2)
            }).ToList();
            ri.Ok = true;

            return Result(ri);
        }
        #endregion

        #region 更新学习分享图片
        [IsRoot]
        [HttpPost]
        public ActionResult updatestudyshareimg(int id = 0)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var model = DB.StudyShareImgSet.FirstOrDefault(a => a.StudyShareImgSetId == id);
                if (model != null)
                {
                    var updateTO = model.IsUse ? false : true;
                    //将其余的置为禁用
                    DB.StudyShareImgSet.Where(a => a.IsUse).Update(a => new StudyShareImgSet { IsUse = false });
                    model.IsUse = updateTO;
                    DB.SaveChanges();
                    ri.Ok = true;
                }
                else
                {
                    ri.Msg = "更新对象不存在";
                }
            }
            return Result(ri);
        }
        #endregion

        #endregion

        #region 批量标签调整

        [IsRoot]
        [HttpGet]
        public ActionResult GetAllTags()
        {
            ResultInfo ri = new ResultInfo();

            ri.Data = DB.Tag.Where(a => a.IsDelete == 0).Select(a => new { id = a.TagId, name = a.TagName });
            ri.Ok = true;

            return Result(ri);
        }

        [IsRoot]
        public ActionResult BatchTag()
        {
            return View();
        }

        [IsRoot]
        [HttpGet]
        public ActionResult LoadQuestionForSetTag()
        {
            ResultInfo ri = new ResultInfo();

            ri.Data = DB.Question.Where(a => a.IsDelete == 0 && a.IsChecked == 2).OrderByDescending(a => a.CreateTime)
                .Select(a => new
                {
                    title = a.Title,
                    mid = a.QuestionId,
                    tags = DB.MenuBelongTag.Where(t => t.MainId == a.QuestionId && t.MainType == 1)
                    .Join(DB.Tag.Where(tag => tag.IsDelete == 0), ta => ta.TagId, tb => tb.TagId, (ta, tb) => tb)
                    .Select(tb => new { name = tb.TagName, id = tb.TagId }).ToList()
                });

            ri.Ok = true;
            return Result(ri);
        }

        [IsRoot]
        [HttpGet]
        public ActionResult LoadArticleForSetTag()
        {
            ResultInfo ri = new ResultInfo();

            ri.Data = DB.Article.Where(a => a.IsDelete == 0 && a.IsChecked == 2).OrderByDescending(a => a.CreateTime)
                .Select(a => new
                {
                    title = a.Title,
                    mid = a.ArticleId,
                    tags = DB.MenuBelongTag.Where(t => t.MainId == a.ArticleId && t.MainType == 2)
                    .Join(DB.Tag.Where(tag => tag.IsDelete == 0), ta => ta.TagId, tb => tb.TagId, (ta, tb) => tb)
                    .Select(tb => new { name = tb.TagName, id = tb.TagId }).ToList()
                });
            ri.Ok = true;
            return Result(ri);
        }

        [IsRoot]
        public ActionResult BatchSetTags(int id, List<long> mids, List<long> tagids)
        {
            ResultInfo ri = new ResultInfo();

            if (mids.Count > 0)
            {
                if (tagids.Count > 0)
                {
                    var tran = DB.Database.BeginTransaction();
                    try
                    {
                        //简单点，先删除已有的，再全部新增吧
                        //开启事务
                        DB.MenuBelongTag.Where(a => mids.Contains(a.MainId.Value) && a.MainType == id).Delete();
                        List<MenuBelongTag> menuBelongTags = new List<MenuBelongTag>();

                        mids.ForEach(mid =>
                        {
                            tagids.ForEach(tag =>
                            {
                                menuBelongTags.Add(new MenuBelongTag
                                {
                                    MainId = mid,
                                    MainType = id,
                                    TagId = tag
                                });
                            });
                        });
                        DB.MenuBelongTag.AddRange(menuBelongTags);
                        DB.SaveChanges();
                        tran.Commit();
                        ri.Ok = true;
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
                else
                {
                    ri.Msg = "选择的标签为空";
                }
            }
            else
            {
                ri.Msg = "需要更改的对象不能为空";
            }

            return Result(ri);
        }
        #endregion

        #region 设置产品自动回复

        #region 加载数据
        [IsRoot]
        public ActionResult AutoReply()
        {
            return View();
        }

        [IsRoot]
        public ActionResult LoadGiftForSet()
        {
            ResultInfo ri = new ResultInfo();
            var type = OrderEnumType.Gift.GetHashCode();
            var gifttype = 1;
            ri.Data = DB.Gift.Where(a => a.IsDelete == 0 && a.GType == gifttype).Select(a => new
            {
                name = a.GiftName,
                id = a.GiftID,
                type = a.GType,
                auto = DB.AutoReply.FirstOrDefault(auto => auto.OrderType == type && auto.ItemID == a.GiftID) != null,
                autoId = DB.AutoReply.Where(auto => auto.OrderType == type && auto.ItemID == a.GiftID).Select(auto => auto.AutoReplyId).FirstOrDefault(),
                content = DB.AutoReply.Where(auto => auto.OrderType == type && auto.ItemID == a.GiftID).Select(auto => auto.Content).FirstOrDefault(),
            });
            ri.Ok = true;

            return Result(ri);
        }

        [IsRoot]
        public ActionResult LoadDataForSet()
        {
            ResultInfo ri = new ResultInfo();
            var type = OrderEnumType.Data.GetHashCode();
            var gifttype = 2;
            ri.Data = DB.Gift.Where(a => a.IsDelete == 0 && a.GType == gifttype).Select(a => new
            {
                name = a.GiftName,
                id = a.GiftID,
                type = a.GType,
                auto = DB.AutoReply.FirstOrDefault(auto => auto.OrderType == type && auto.ItemID == a.GiftID) != null,
                autoId = DB.AutoReply.Where(auto => auto.OrderType == type && auto.ItemID == a.GiftID).Select(auto => auto.AutoReplyId).FirstOrDefault(),
                content = DB.AutoReply.Where(auto => auto.OrderType == type && auto.ItemID == a.GiftID).Select(auto => auto.Content).FirstOrDefault(),
            });
            ri.Ok = true;
            return Result(ri);
        }

        [IsRoot]
        public ActionResult LoadClassForSet()
        {
            ResultInfo ri = new ResultInfo();
            var type = OrderEnumType.KeCheng.GetHashCode();
            var gifttype = 3;
            ri.Data = DB.Gift.Where(a => a.IsDelete == 0 && a.GType == gifttype).Select(a => new
            {
                name = a.GiftName,
                id = a.GiftID,
                type = a.GType,
                auto = DB.AutoReply.FirstOrDefault(auto => auto.OrderType == type && auto.ItemID == a.GiftID) != null,
                autoId = DB.AutoReply.Where(auto => auto.OrderType == type && auto.ItemID == a.GiftID).Select(auto => auto.AutoReplyId).FirstOrDefault(),
                content = DB.AutoReply.Where(auto => auto.OrderType == type && auto.ItemID == a.GiftID).Select(auto => auto.Content).FirstOrDefault(),
            });
            ri.Ok = true;
            return Result(ri);
        }
        #endregion

        #region 导入自动回复内容数据
        /// <summary>
        /// 导入自动回复内容数据
        /// </summary>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult ImportAutoReplyContent()
        {
            ResultInfo ri = new ResultInfo();

            var content = GetRequest("content");
            if (content.IsNotNullOrEmpty())
            {
                bool can = true;
                var file = Request.Files["file"];
                if (content.Contains("#"))
                {
                    if (file == null)
                    {
                        can = false;
                        ri.Msg = "请导入数据";
                    }
                }
                if (can)
                {
                    var tran = DB.Database.BeginTransaction();
                    try
                    {
                        //如果是追加
                        long mainId = GetRequest<long>("id");
                        int type = GetRequest<int>("type");
                        var autoreplyid = Guid.Empty;
                        var autoReply = DB.AutoReply.FirstOrDefault(a => a.ItemID == mainId && a.OrderType == type);
                        if (autoReply == null)
                        {
                            autoreplyid = Guid.NewGuid();
                            DB.AutoReply.Add(new AutoReply()
                            {
                                AutoReplyId = autoreplyid,
                                CreateTime = DateTime.Now,
                                CreateUser = UserID.ToString(),
                                IsDelete = 0,
                                ItemID = mainId,
                                OrderType = type,
                                Content = content
                            });
                        }
                        else
                        {
                            autoreplyid = autoReply.AutoReplyId;
                        }
                        if (file != null)
                        {
                            var result = UpLoadFile(file, "/content/autoreply/uploadfile");
                            var path = UploadHelper.GetMapPath(result.Url);
                            var dt = NPOIExcelHelper.Import(path);
                            bool ok = true;
                            List<string> columns = new List<string>();
                            foreach (DataColumn column in dt.Columns)
                            {
                                if (!content.Contains(column.ColumnName))
                                {
                                    ok = false;
                                    break;
                                }
                                else
                                {
                                    columns.Add(column.ColumnName);
                                }
                            }
                            if (ok)
                            {
                                int colCount = columns.Count;
                                List<AutoReplyItem> autoReplyItems = new List<AutoReplyItem>();
                                int rowIndex = 1;
                                if (DB.AutoReplyItem.Count() > 0)
                                {
                                    rowIndex = DB.AutoReplyItem.Max(a => a.GroupId) + 1;
                                }
                                foreach (DataRow dr in dt.Rows)
                                {
                                    foreach (string column in columns)
                                    {
                                        var col = dr[column];
                                        if (col != null && !col.Equals(DBNull.Value))
                                        {
                                            autoReplyItems.Add(new AutoReplyItem
                                            {
                                                AutoReplyId = autoreplyid,
                                                CreateTime = DateTime.Now,
                                                CreateUser = UserID,
                                                GroupId = rowIndex,
                                                ReplaceKey = column,
                                                ReplaceValue = col.ToString(),
                                                IsDelete = 0,
                                                IsUsed = false,
                                            });
                                        }
                                        else
                                        {
                                            ok = false;
                                            break;
                                        }
                                    }
                                    if (!ok)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        rowIndex++;
                                    }
                                }
                                if (ok)
                                {
                                    DB.AutoReplyItem.AddRange(autoReplyItems);
                                    DB.SaveChanges();
                                    tran.Commit();
                                    ri.Ok = true;
                                    ri.Msg = "设置成功";
                                }
                                else
                                {
                                    tran.Rollback();
                                    ri.Msg = "行数据不能有空值！";
                                }
                            }
                            else
                            {
                                tran.Rollback();
                                ri.Msg = "替换关键字数量不一样！";
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        ErrorBLL.Instance.Log(e.ToString());
                    }
                }
            }
            else
            {
                ri.Msg = "回复内容不能为空！";
            }
            return Result(ri);
        }
        #endregion

        #region 获取已配置的自动回复
        /// <summary>
        /// 获取已配置的自动回复
        /// </summary>
        /// <returns></returns>
        [IsRoot]
        [HttpGet]
        public ActionResult Getautoreply(int type, long id)
        {
            ResultInfo ri = new ResultInfo();

            ri.Data = DB.AutoReply.Where(a => a.ItemID == id && a.IsDelete == 0 && a.OrderType == type).Select(a => new
            {
                content = a.Content,
                auto = DB.AutoReplyItem.Where(auto => auto.AutoReplyId == a.AutoReplyId && auto.IsDelete == 0).GroupBy(auto => auto.GroupId)
                .Select(auto => new
                {
                    item = auto.Select(reply => new
                    {
                        kid = reply.AutoReplyItemId,
                        key = reply.ReplaceKey,
                        value = reply.ReplaceValue,
                        used = reply.IsUsed
                    })
                }).ToList()
            }).ToList();
            ri.Ok = true;

            return Result(ri);
        }
        #endregion

        #region 删除指定
        /// <summary>
        /// 删除指定
        /// </summary>
        /// <param name="kids"></param>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult Deleteautoreplyitem(List<long> kids)
        {
            ResultInfo ri = new ResultInfo();

            if (kids != null && kids.Count > 0)
            {
                var tran = DB.Database.BeginTransaction();
                try
                {
                    DB.AutoReplyItem.Where(a => kids.Contains(a.AutoReplyItemId)).Delete();
                    DB.SaveChanges();
                    tran.Commit();
                    ri.Ok = true;
                    ri.Msg = "删除成功";
                }
                catch
                {
                    tran.Rollback();
                    ri.Msg = "删除失败";
                }
            }
            else
            {
                ri.Msg = "没有可删除的项";
            }

            return Result(ri);
        }
        #endregion

        #region 删除该项目下的所有自动回复
        [IsRoot]
        public ActionResult DeleteAllReply(Guid id)
        {
            ResultInfo ri = new ResultInfo();

            if (id != Guid.Empty)
            {
                var tran = DB.Database.BeginTransaction();
                var reply = DB.AutoReply.FirstOrDefault(a => a.AutoReplyId == id);
                if (reply != null)
                {
                    var replyitems = DB.AutoReplyItem.Where(a => a.AutoReplyId == id);
                    DB.AutoReply.Remove(reply);
                    replyitems.Delete();
                    DB.SaveChanges();
                    tran.Commit();
                    ri.Msg = "清空成功";
                    ri.Ok = true;
                }
                else
                {
                    ri.Msg = "删除源不存在！";
                }
            }
            else
            {
                ri.Msg = "删除源不存在！";
            }

            return Result(ri);
        }
        #endregion

        #endregion

        #region 待发货订单列表
        [IsRoot]
        public ActionResult ToSend()
        {
            var list = DB.OrderSend.Where(a => a.SendStatus == 0)
                .Join(
                DB.UserGift.Where(a => a.IsPay == 1),
                a => a.UserGiftId,
                b => b.UserGiftId,
                (a, b) => new ToSendOrder
                {
                    OrderSend = a,
                    BuyerInfo = DB.UserBase.FirstOrDefault(user => user.UserID == b.BuyUserID),
                    GiftInfo = DB.Gift.FirstOrDefault(gift => gift.GiftID == b.GiftID),
                    UserGiftInfo = b,
                }).ToList();
            return View(list.OrderByDescending(m => m.UserGiftInfo.BuyTime).ToList());
        }
        #endregion

        #region 已发货订单列表
        [IsRoot]
        public ActionResult ToSended()
        {
            var auto1 = OrderEnumType.Gift.GetHashCode();
            var auto2 = OrderEnumType.Data.GetHashCode();
            var auto3 = OrderEnumType.KeCheng.GetHashCode();


            var list = DB.OrderSend.Where(a => a.SendStatus == 1)
                .Join(DB.UserGift.Where(a => a.IsPay == 1), a => a.UserGiftId, b => b.UserGiftId, (a, b) => new ToSendOrder
                {
                    OrderSend = a,
                    BuyerInfo = DB.UserBase.FirstOrDefault(user => user.UserID == b.BuyUserID),
                    GiftInfo = DB.Gift.FirstOrDefault(gift => gift.GiftID == b.GiftID),
                    UserGiftInfo = b,
                    Express = DB.Express.FirstOrDefault(e => e.UserGiftId == a.UserGiftId),
                    IsAutoSend = DB.AutoReply.FirstOrDefault(auto => auto.ItemID == b.GiftID && (auto.OrderType == auto1 || auto.OrderType == auto2 || auto.OrderType == auto3)) != null
                }).ToList();
            return View(list.OrderByDescending(x => x.UserGiftInfo.BuyTime).ToList());
        }
        #endregion

        #region 获取待发货（新订单数量）
        [IsRoot]
        [HttpPost]
        public ActionResult orderToSend()
        {
            ResultInfo ri = new ResultInfo();

            var list = DB.OrderSend.Where(a => a.SendStatus == 0)
                .Join(DB.UserGift.Where(a => a.IsPay == 1), a => a.UserGiftId, b => b.UserGiftId, (a, b) => a).Count();
            ri.Data = list;
            ri.Ok = true;
            return Result(ri);
        }
        #endregion

        #region 保存发货单号
        [IsRoot]
        [HttpPost]
        public ActionResult Saveexpress(string no, string name, int orderSendId, long buyerId)
        {
            ResultInfo ri = new ResultInfo();

            if (no.IsNotNullOrEmpty())
            {
                if (name.IsNotNullOrEmpty())
                {
                    var model = DB.OrderSend.FirstOrDefault(a => a.OrderSendId == orderSendId && a.CreateUser == buyerId);
                    if (model != null)
                    {
                        var express = DB.Express.FirstOrDefault(a => a.UserGiftId == model.UserGiftId);
                        if (express == null)
                        {
                            //新增
                            DB.Express.Add(new Express
                            {
                                UserGiftId = model.UserGiftId,
                                CreateTime = DateTime.Now,
                                CreateUser = UserID,
                                ExpressName = name,
                                ExpressNo = no,
                                ExpressType = 0,
                                IsDelete = 0
                            });
                        }
                        else
                        {
                            express.ExpressNo = no;
                            express.ExpressName = name;
                            express.ExpressType = 0;
                        }
                        model.SendStatus = 1;
                        DB.SaveChanges();
                        ri.Ok = true;
                        ri.Msg = "发货成功";
                    }
                    else
                    {
                        ri.Msg = "订单信息不存在";
                    }
                }
                else
                {
                    ri.Msg = "物流名称不能为空";
                }
            }
            else
            {
                ri.Msg = "运单号不能为空";
            }

            return Result(ri);
        }
        #endregion

        #region 礼物 数据  课程 页面排序
        [IsRoot]
        public ActionResult GiftSortConfig()
        {
            return View();
        }

        [IsRoot]
        [HttpPost]
        public ActionResult SetGiftSortConfig(string sort, string value)
        {
            ResultInfo ri = new ResultInfo();
            return SetConfig(sort, value);
        }
        #endregion

        #region 广告设置
        [IsRoot]
        public ActionResult AD()
        {
            var ads = amazonBBSDBContext.AD.Where(a => a.IsDelete == 0).OrderByDescending(a => a.CreateTime).ToList();
            return View(ads);
        }

        #region 广告删除
        [IsRoot]
        [HttpPost]
        public ActionResult Addelete(long id)
        {
            ResultInfo ri = new ResultInfo();

            var info = amazonBBSDBContext.AD.FirstOrDefault(a => a.ADID == id);
            if (info != null)
            {
                if (info.IsDelete == 1)
                {
                    ri.Msg = "广告已被删除";
                }
                else
                {
                    info.IsDelete = 1;
                    amazonBBSDBContext.SaveChanges();
                    ri.Ok = true;
                    //更新缓存
                    CSharpCacheHelper.Set(APPConst.AD, amazonBBSDBContext.AD.Where(a => a.IsDelete == 0).ToList(), APPConst.ExpriseTime.Day2);
                }
            }
            else
            {
                ri.Msg = "广告不存在";
            }

            return Result(ri);
        }
        #endregion

        #region 广告编辑
        [IsRoot]
        [HttpPost]
        public ActionResult ADEdit(string ADTitle,
          string ADMSG,
          string ADContact,
          string ADWeChat, long id, bool ischanged)
        {
            ResultInfo ri = new ResultInfo();

            var ad = amazonBBSDBContext.AD.FirstOrDefault(a => a.ADID == id);
            if (ad != null)
            {
                if (ischanged)
                {
                    HttpPostedFileBase upfile = Request.Files["ADIMG"];
                    if (upfile == null)
                    {
                        ri.Msg = "请选择要上传的文件";
                        return Result(ri);
                    }
                    ri = UpLoadImg("ADIMG", "/Content/AD", ImgExtTypeEnum.jpg);
                    ad.ADIMG = ri.Url;
                }
                ad.ADTitle = ADTitle;
                ad.ADMsg = ADMSG;
                ad.ADContact = ADContact;
                ad.ADWeChat = ADWeChat;
                amazonBBSDBContext.SaveChanges();
                ri.Ok = true;
                CSharpCacheHelper.Set(APPConst.AD, amazonBBSDBContext.AD.Where(a => a.IsDelete == 0).ToList(), APPConst.ExpriseTime.Day2);
            }
            else
            {
                ri.Msg = "广告不存在";
            }
            return Result(ri);
        }
        #endregion

        #endregion

        #region 滚屏公告设置
        [IsRoot]
        public ActionResult ScrollNotice()
        {
            ViewBag.scrollNoticeChangeTime = ConfigHelper.AppSettings("scrollNoticeChangeTime");
            return View(DB.SiteNotice.Where(a => !a.IsDelete).OrderByDescending(a => a.CreateTime).ToList());
        }

        /// <summary>
        /// 添加滚屏公告
        /// </summary>
        /// <param name="notice_short_title"></param>
        /// <param name="notice_short_title_bgcolor"></param>
        /// <returns></returns>
        [IsRoot]
        [HttpPost]
        public ActionResult AddScrollNotice(string notice_short_title, string notice_short_title_bgcolor, string notice_short_title_fontcolor, string notice_long_title,
            string notice_long_title_bgcolor, string notice_long_title_fontcolor, string notice_url)
        {
            ResultInfo ri = new ResultInfo();
            DB.SiteNotice.Add(new SiteNotice
            {
                CreateTime = DateTime.Now,
                IsDelete = false,
                NoticeType = 1,
                ShortTitle = notice_short_title,
                ShortTitleBGColor = notice_short_title_bgcolor,
                ShortTitleFontColor = notice_short_title_fontcolor,
                Title = notice_long_title,
                TitleBGColor = notice_long_title_bgcolor,
                TitleFontColor = notice_long_title_fontcolor,
                Url = notice_url,
                UserId = UserID,
                UpdateTime = DateTime.Now,
            });
            DB.SaveChanges();
            ri.Ok = true;
            return Result(ri);
        }

        [IsRoot]
        [HttpPost]
        public ActionResult UpdateScrollNotice(long id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var model = DB.SiteNotice.FirstOrDefault(a => a.Id == id);
                if (model != null)
                {
                    if (model.IsDelete)
                    {
                        ri.Msg = "该滚屏公告已经被删除了";
                    }
                    else
                    {
                        model.IsDelete = true;
                        DB.SaveChanges();
                        ri.Ok = true;
                    }
                }
                else
                {
                    ri.Msg = "该滚屏公告不存在";
                }
            }
            else
            {
                ri.Msg = "id不能为空";
            }

            return Result(ri);
        }

        [IsRoot]
        [HttpPost]
        public ActionResult EditScrollNotice(long id, string notice_short_title, string notice_short_title_bgcolor, string notice_short_title_fontcolor, string notice_long_title, string notice_long_title_bgcolor, string notice_long_title_fontcolor, string notice_url)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var model = DB.SiteNotice.FirstOrDefault(a => a.Id == id);
                if (model != null)
                {
                    model.ShortTitle = notice_short_title;
                    model.ShortTitleBGColor = notice_short_title_bgcolor;
                    model.ShortTitleFontColor = notice_short_title_fontcolor;
                    model.Title = notice_long_title;
                    model.TitleBGColor = notice_long_title_bgcolor;
                    model.TitleFontColor = notice_long_title_fontcolor;
                    model.Url = notice_url;
                    model.UpdateTime = DateTime.Now;
                    DB.SaveChanges();
                    ri.Ok = true;
                }
                else
                {
                    ri.Msg = "该滚屏公告不存在";
                }
            }
            else
            {
                ri.Msg = "id不能为空";
            }
            return Result(ri);
        }
        #endregion
    }
}