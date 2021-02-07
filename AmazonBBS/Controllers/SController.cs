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
    public class SController : BaseController
    {
        #region 搜索
        public ActionResult Index(string key, string searchfrom)
        {
            if (!Convert.ToBoolean(ConfigHelper.AppSettings("searchNeedLogin")) || UserBaseBLL.Instance.IsLogin)
            {
                string urlPrefix = string.Empty;
                var model = new SearchResultViewModel()
                {
                    SearchKey = key,
                    SList = new List<S>(),
                    SearchPage = InitPage(20),
                };

                //搜索帖子
                if (SearchTypeEnum.question.ToString() == searchfrom)
                {
                    model.SList.AddRange(SearchQuestion(key, model.SearchPage));
                }
                //搜索文章
                else if (searchfrom == SearchTypeEnum.article.ToString())
                {
                    model.SList.AddRange(SearchArticles(key, model.SearchPage));
                }
                //搜索用户
                else if (searchfrom == SearchTypeEnum.user.ToString())
                {
                    model.SList.AddRange(SearchUsers(key, model.SearchPage));
                }
                //搜索文章和帖子
                else if (searchfrom == SearchTypeEnum.questionandarticle.ToString())
                {
                    model.SList.AddRange(SearchQuestion(key, model.SearchPage));
                    model.SList.AddRange(SearchArticles(key, model.SearchPage));
                }
                if (model.SearchPage.PageIndex == 1)
                {
                    return View(model);
                }
                else
                {
                    return PartialView("_LoadSearchData", model);
                }
            }
            else
            {
                return RedirectToAction("login", "account", new { ReturnUrl = HttpContext.Request.Url.ToString() });
            }
        }
        #endregion

        #region 搜索帖子
        /// <summary>
        /// 搜索帖子
        /// </summary>
        /// <param name="key"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private List<S> SearchQuestion(string key, Paging page)
        {
            var models = new List<S>();
            int normalUserSee_Score = ConfigHelper.AppSettings("normalUserSee_Score").ToInt32();//普通用户查看 other 版块 所需积分
            long userId = UserID;
            bool islogin = IsLogin;
            bool isVip = islogin ? UserBaseBLL.Instance.IsVIP(userId) : false;//当前用户是否VIP用户
            var list = QuestionBLL.Instance.SearchQuestion(key, page);

            //获取 1037 是否付费
            List<BuyOtherLog> payFor1037logs = null;
            List<ContentBuyLog> contentBuyLogs = null;
            if (islogin)
            {
                List<long> qids = list.Select(a => a.QuestionId).ToList();
                int maintype = ContentFeeMainEnumType.BBS.GetHashCode();
                payFor1037logs = DB.BuyOtherLog.Where(a => a.CreateUser == userId && qids.Contains(a.MainID)).ToList();
                contentBuyLogs = DB.ContentBuyLog.Where(a => a.BuyerId == userId && a.MainType == maintype && qids.Contains(a.MainID)).ToList();
            }

            list.ForEach(q =>
            {
                var s = new S();
                string body = HtmlRegexHelper.ToText(q.Body);
                s.Desc = body;
                s.type = 1;
                s.Title = q.Title;
                s.Id = q.QuestionId.ToString();
                if (q.TopicID == 1037)
                {
                    if (islogin)
                    {
                        //q.HideForNoVipUserOrNotBuy = !isVip && DB.BuyOtherLog.FirstOrDefault(buyother => buyother.CreateUser == userId && buyother.MainID == q.QuestionId) == null;
                        q.HideForNoVipUserOrNotBuy = !isVip && payFor1037logs.FirstOrDefault(a => a.MainID == q.QuestionId) == null;
                    }
                    else
                    {
                        q.HideForNoVipUserOrNotBuy = true;
                    }

                    if (q.HideForNoVipUserOrNotBuy)
                    {
                        s.Desc = "本条<a class='btn-buyOtherItem' data-id='{0}' data-c='{1}'>查看</a>需要 {1} 积分，<a href='/user/scoreexchange#scoreexchange' target='_self'>会员</a>可享受免费查看".FormatWith(q.QuestionId, normalUserSee_Score);
                    }
                    else
                    {
                        //判断是否内容付费
                        if (q.ContentNeedPay.HasValue && q.ContentNeedPay.Value)
                        {
                            if (contentBuyLogs.FirstOrDefault(a => a.MainID == q.QuestionId) == null)
                            {
                                s.Desc = "该贴内容需要付费，{0}{1}，请付费后查看。".FormatWith(q.ContentFee, q.ContentFeeType == 1 ? "积分" : "金钱");
                            }
                            else
                            {
                                if (body.Length > 200)
                                {
                                    s.Desc = body.Substring(0, 200);
                                }
                            }
                        }
                        else
                        {
                            if (body.Length > 200)
                            {
                                s.Desc = body.Substring(0, 200);
                            }
                        }
                    }
                }
                else
                {
                    //判断是否需要内容付费
                    if (q.ContentNeedPay.HasValue && q.ContentNeedPay.Value)
                    {
                        if (islogin)
                        {
                            if (q.UserID != userId && contentBuyLogs.FirstOrDefault(a => a.MainID == q.QuestionId) == null)
                            {
                                s.Desc = "该贴内容需要付费，<span style='color:red;'>{0}{1}</span>，请付费后查看。".FormatWith(q.ContentFee, q.ContentFeeType == ContentFeeTypeEnum.score.GetHashCode() ? "积分" : "金钱");
                            }
                            else
                            {
                                if (body.Length > 200)
                                {
                                    s.Desc = body.Substring(0, 200);
                                }
                            }
                        }
                        else
                        {
                            s.Desc = "该贴内容需要付费，<span style='color:red;'>{0}{1}</span>，请付费后查看。".FormatWith(q.ContentFee, q.ContentFeeType == ContentFeeTypeEnum.score.GetHashCode() ? "积分" : "金钱");
                        }
                    }
                    else
                    {
                        if (body.Length > 200)
                        {
                            s.Desc = body.Substring(0, 200);
                        }
                    }
                }
                models.Add(s);
            });
            return models;
        }
        #endregion

        #region 搜索文章
        /// <summary>
        /// 搜索文章
        /// </summary>
        /// <param name="key"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private List<S> SearchArticles(string key, Paging page)
        {
            long userId = UserID;
            bool islogin = IsLogin;
            var models = new List<S>();
            ArticleViewModel articleModel = new ArticleViewModel();
            var list = ArticleBLL.Instance.GetAllArticles(page, key);

            //内容付费记录
            List<ContentBuyLog> contentBuyLogs = null;
            if (islogin)
            {
                List<long> aids = list.Select(a => a.ArticleId).ToList();
                int maintype = ContentFeeMainEnumType.Article.GetHashCode();
                contentBuyLogs = DB.ContentBuyLog.Where(a => a.BuyerId == userId && a.MainType == maintype && aids.Contains(a.MainID)).ToList();
            }
            list.ForEach(a =>
            {
                var s = new S();
                string body = HtmlRegexHelper.ToText(a.Body);
                s.Desc = body;
                s.Title = a.Title;
                s.Id = a.ArticleId.ToString();
                s.type = 2;
                if (a.ContentNeedPay.HasValue && a.ContentNeedPay.Value)
                {
                    if (islogin)
                    {
                        //如果已登录，且不是该作者并用没有付费
                        if (a.UserID != userId && contentBuyLogs.FirstOrDefault(t => t.MainID == a.ArticleId) == null)
                        {
                            s.Desc = "该贴内容需要付费，<span style='color:red;'>{0}{1}</span>，请付费后查看。".FormatWith(a.ContentFee, a.ContentFeeType == ContentFeeTypeEnum.score.GetHashCode() ? "积分" : "金钱");
                        }
                        else
                        {
                            if (body.Length > 200)
                            {
                                s.Desc = body.Substring(0, 200);
                            }
                        }
                    }
                    else
                    {
                        s.Desc = "该贴内容需要付费，<span style='color:red;'>{0}{1}</span>，请付费后查看。".FormatWith(a.ContentFee, a.ContentFeeType == ContentFeeTypeEnum.score.GetHashCode() ? "积分" : "金钱");
                    }
                }
                else
                {
                    if (body.Length > 200)
                    {
                        s.Desc = body.Substring(0, 200);
                    }
                }
                models.Add(s);
            });
            return models;
        }
        #endregion

        #region 搜索用户
        /// <summary>
        /// 搜索用户
        /// </summary>
        /// <param name="key"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private List<S> SearchUsers(string key, Paging page)
        {
            var models = new List<S>();
            var ulist = UserBaseBLL.Instance.GetUserByKey(key).UserInfoList;
            ulist?.ForEach(u =>
            {
                var s = new S();
                s.Title = u.UserName;
                s.Desc = u.Sign;
                s.Id = u.UserID.ToString();
                s.type = 3;
                models.Add(s);
            });
            return models;
        }
        #endregion
    }
}