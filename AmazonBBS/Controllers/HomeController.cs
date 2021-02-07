using AmazonBBS.BLL;
using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;

namespace AmazonBBS.Controllers
{
    public class HomeController : BaseController
    {
        public IUserService userService { get; set; }

        public ActionResult Index()
        {
            ViewBag.IsVIPUser = IsLogin ? DB.UserExt.FirstOrDefault(a => a.UserID == UserID)?.OnlyLevelName.HasValue : false;//是否协会会员(专属头衔)

            ViewBag.Signed = IsSign;
            ViewBag.IsLogin = IsLogin;
            ViewBag.Uid = UserID;

            ViewBag.VipStudy = userService.GetSignUserStudyInfo(0, 6);

            ViewBag.BBSCount = DB.Question.Where(a => a.IsDelete == 0 && a.IsChecked == 2)
                .Join(DB.UserBase.Where(a => a.IsDelete == 0), a => a.UserID, c => c.UserID, (a, b) => a).Count();
            ViewBag.ArticleCount = DB.Article.Where(a => a.IsDelete == 0)
                .Join(DB.UserBase.Where(a => a.IsDelete == 0), a => a.UserID, b => b.UserID, (a, b) => a).Count();
            ViewBag.ScrollNotice = DB.SiteNotice.Where(a => !a.IsDelete).OrderByDescending(a => a.CreateTime).ToList();

            //热门帖子
            ViewBag.HotBBS = (from a in DB.Question
                              where a.IsRemen == 1
                              orderby a.PVCount descending
                              select a).Take(6).ToList();
            //热门文章
            ViewBag.HotArticle = (from a in DB.Article
                                  orderby a.PVCount descending
                                  select a).Take(6).ToList();
            var model = GetQuestionList(HomeSortTypeEnum.Sort_Default);
            return View(model);
        }

        private BBSListViewModel GetQuestionList(HomeSortTypeEnum sortType)
        {
            string sortConfit = ConfigHelper.AppSettings("HomeSortConfig-{0}".FormatWith(sortType.ToString()));
            int count = ConfigHelper.AppSettings("HomeListCount").ToInt32();
            BBSListViewModel model = new BBSListViewModel();
            model.QuestionPage = InitPage();
            model.QuestionList = QuestionBLL.Instance.GetListBySortType(sortType, count, sortConfit, model.QuestionPage);

            bool isVip = UserBaseBLL.Instance.IsVIP(UserID);//当前用户是否VIP用户
            model.QuestionList?.Where(a => a.TopicID == 1037)?.ToList()?.ForEach(item =>
            {
                if (IsLogin)
                {
                    //是会员 或者 购买过了 则显示
                    item.HideForNoVipUserOrNotBuy = !isVip && DB.BuyOtherLog.FirstOrDefault(a => a.CreateUser == UserID && a.MainID == item.QuestionId) == null;
                }
                else
                {
                    item.HideForNoVipUserOrNotBuy = true;
                }
            });
            return model;
        }

        private BBSListViewModel GetAllBBS(string order)
        {
            BBSListViewModel model = new BBSListViewModel();
            model.QuestionPage = InitPage();
            model.QuestionList = QuestionBLL.Instance.GetAllBBSList(order, model.QuestionPage);

            bool isVip = UserBaseBLL.Instance.IsVIP(UserID);//当前用户是否VIP用户
            model.QuestionList?.Where(a => a.TopicID == 1037)?.ToList()?.ForEach(item =>
            {
                if (IsLogin)
                {
                    //是会员 或者 购买过了 则显示
                    item.HideForNoVipUserOrNotBuy = !isVip && DB.BuyOtherLog.FirstOrDefault(a => a.CreateUser == UserID && a.MainID == item.QuestionId) == null;
                }
                else
                {
                    item.HideForNoVipUserOrNotBuy = true;
                }
            });
            return model;
        }

        private ArticleViewModel GetAllArticle(string order)
        {
            ArticleViewModel model = new ArticleViewModel();
            model.ARticlePage = InitPage();
            model.Articles = ArticleBLL.Instance.GetAllArticleList(order, model.ARticlePage);
            return model;
        }

        /// <summary>
        /// 为首页 根据 选择类型 获取问题列表
        /// </summary>
        /// <returns></returns>
        [ActionName("HOMEBBS")]
        [HttpGet]
        public ActionResult GetQuestionList(string sort, int order = 1)
        {
            var sortType = EnumHelper.ToEnum<HomeSortTypeEnum>(sort);
            if (sortType == HomeSortTypeEnum.Sort_AllBBS)
            {
                var model = GetAllBBS(order == 1 ? "desc" : "asc");
                return PartialView("/Views/BBS/_LoadForHome.cshtml", model);
            }
            else if (sortType == HomeSortTypeEnum.Sort_AllArticle)
            {
                var model = GetAllArticle(order == 1 ? "desc" : "asc");
                return PartialView("/Views/Article/_LoadForHome.cshtml", model);
            }
            else
            {
                var model = GetQuestionList(sortType);
                return PartialView("/Views/BBS/_LoadForHome.cshtml", model);
            }
        }

        #region 获取更多的标签会员学习信息
        [HttpGet]
        public ActionResult StudyUserInfos()
        {
            var studies = userService.GetSignUserStudyInfo(6, 10000);
            ViewBag.More = true;
            ViewBag.Uid = UserID;
            return PartialView("_UserStudy", studies);
        }
        #endregion

        #region 注释
        //public ActionResult Index()
        //{
        //    ViewBag.Signed = IsSign;
        //    ViewBag.IsLogin = IsLogin;
        //    ViewBag.ID = UserID;

        //    //List<_QuestionInfo> list = QuestionBLL.Instance.GetTop5QuesForIndex(20);
        //    //首页排序规则：浏览量+置顶、置顶+精华、最后按浏览量排序
        //    int count = Convert.ToInt32(ConfigHelper.AppSettings("HomeListCount"));
        //    List<_QuestionInfo> list = QuestionBLL.Instance.GetListForIndex(count);
        //    //list.ForEach(a =>
        //    //{
        //    //    string _body = a.Body;
        //    //    string[] imgs = HtmlRegexHelper.GetHtmlImageUrlList(_body, out _body);
        //    //    a.Body = _body;
        //    //    a.Index_Img = imgs.Length > 0 ? imgs[0] : string.Empty;
        //    //});
        //    string[] chars = new string[] { "<br/>" };
        //    for (int i = 0, len = list.Count; i < len; i++)
        //    {
        //        var a = list[i];
        //        string _body = a.Body;
        //        string[] imgs = HtmlRegexHelper.GetHtmlImageUrlList(_body, out _body);
        //        a.Body = _body;
        //        if (i < 2)
        //        {
        //            a.Index_Img = imgs.Length > 0 ? imgs[0] : string.Empty;
        //        }
        //        a.Body = HtmlRegexHelper.ToText(a.Body);
        //        //先选前4行，再判断是否大于150字符进行截取
        //        string[] splits = a.Body.Split(chars, StringSplitOptions.RemoveEmptyEntries);
        //        if (splits.Length > 4)
        //        {
        //            a.Body = "{0}<br/>{1}<br>{2}<br>{3}".FormatWith(splits[0], splits[1], splits[2], splits[3]);
        //        }
        //    }

        //    //拉取推荐网站
        //    ViewBag.SuggestSites = SuggestSiteBLL.Instance.SearchAll();
        //    return View(list);
        //} 
        #endregion

        //[IsRoot]
        [HttpGet]
        public void Test()
        {
            //更新签到次数等信息
            bool updateSignInfo = false;
            if (updateSignInfo)
            {
                var now = DateTime.Now;
                var users = DB.UserBase.Where(a => a.IsDelete == 0).ToList();
                var uids = users.Select(a => a.UserID).ToList();
                var userexts = DB.UserExt.Where(a => uids.Contains(a.UserID.Value)).ToList();
                var signLogs = DB.ScoreCoinLog.Where(a => uids.Contains(a.UserID) && a.CoinSource == 3).OrderByDescending(a => a.CoinTime).ToList();

                userexts.ForEach(userext =>
                {
                    var userSignLogs = signLogs.Where(a => a.UserID == userext.UserID).OrderByDescending(a => a.CoinTime).ToList();
                    if (userSignLogs.Count > 0)
                    {
                        userext.UserSignTotalCount = userSignLogs.Count;
                        userext.UserSignTime = userSignLogs.FirstOrDefault().CoinTime;
                        //本月连续签到天数
                        DateTime monthFirstDay = now.AddDays(-now.Day + 1).Date;//本月1号
                        var loopDate = now.Date;
                        var loopCount = 0;//连续签到几天
                        for (var loopIndex = now.Day; loopIndex >= 1; loopIndex--)
                        {
                            var findItem = userSignLogs.FirstOrDefault(a => a.CoinTime.Date == loopDate);
                            if (findItem != null)
                            {
                                loopCount++;
                            }
                            else
                            {
                                break;
                            }
                            loopDate = loopDate.AddDays(-1).Date;
                        }
                        userext.UserMonthSignContinueCount = loopCount;

                        loopDate = now.Date;
                        loopCount = 0;
                        var lastSignDate = userSignLogs.LastOrDefault().CoinTime.Date;
                        for (var loopIndex = 0; loopIndex < userSignLogs.Count; loopIndex++)
                        {
                            var findItem = userSignLogs.FirstOrDefault(a => a.CoinTime.Date == loopDate);
                            if (findItem != null)
                            {
                                loopCount++;
                            }
                            else
                            {
                                break;
                            }
                            loopDate = loopDate.AddDays(-1).Date;
                        }
                        userext.UserSignContinueCount = loopCount;
                    }
                    else
                    {
                        userext.UserMonthSignContinueCount = 0;
                        userext.UserSignContinueCount = 0;
                        userext.UserSignTotalCount = 0;
                    }
                });
                DB.SaveChanges();
            }
        }
    }
}