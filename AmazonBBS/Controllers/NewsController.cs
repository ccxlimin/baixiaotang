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
    public class NewsController : BaseController
    {
        public ActionResult List()
        {
            var list = GetNews();
            return View(list);
        }

        public ActionResult LoadNews()
        {
            var list = GetNews();
            return PartialView("_LoadNews", list);
        }

        private BaseListViewModel<News> GetNews()
        {
            var list = NewsBLL.Instance.SearchByRows<News>(InitPage(30));
            return list;
        }

        // GET: News
        public ActionResult Detail(int id = 0)
        {
            if (id > 0)
            {
                News model = NewsBLL.Instance.GetModel(id);
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "About");
            }
        }

        #region 发布新闻
        [IsMaster]
        public ActionResult Publish()
        {
            if (IsLogin)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/news/publish" });
            }
        }

        [IsMaster]
        [HttpPost]
        public ActionResult Publish(string title, string body)
        {
            ResultInfo ri = new ResultInfo();
            if (UserBaseBLL.Instance.IsRoot)
            {
                News model = new News()
                {
                    NTitle = title,
                    NBody = HttpUtility.UrlDecode(body),
                    IsDelete = 0,
                    CreateTime = DateTime.Now,
                    CreateUser = UserID.ToString(),
                    IsTop = 0,
                    PVCount = 0
                };
                int result = NewsBLL.Instance.Add(model);
                if (result > 0)
                {
                    ri.Url = ConfigHelper.AppSettings("NewsDetail").FormatWith(result);
                    ri.Ok = true;
                    ri.Msg = "新闻发布成功";
                }
                else
                {
                    ri.Msg = "新闻发布失败";
                }
            }
            else
            {
                ri.Msg = "你没有权限";
            }
            return Result(ri);
        }
        #endregion

        #region 编辑
        [IsMaster]
        public ActionResult Edit(long id = 0)
        {
            if (id > 0)
            {
                var model = NewsBLL.Instance.GetModel(id);
                if (model != null && model.IsDelete == 0)
                {
                    return View(model);
                }
                else
                {
                    return RedirectToAction("/");
                }
            }
            else
            {
                return RedirectToAction("/");
            }
        }

        [IsMaster]
        [HttpPost]
        public ActionResult Edit(long id, string body, string title)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var model = NewsBLL.Instance.GetModel(id);
                if (model == null)
                {
                    ri.Msg = "新闻不存在";
                }
                else if (model.IsDelete == 0)
                {
                    model.NTitle = title;
                    model.NBody = HttpUtility.UrlDecode(body);
                    ri = NewsBLL.Instance.Update(model);
                    if (ri.Ok)
                    {
                        ri.Url = ConfigHelper.AppSettings("NewsDetail").FormatWith(id);
                    }
                }
                else
                {
                    ri.Msg = "新闻已被删除";
                }
            }
            else
            {
                ri.Msg = "新闻异常";
            }
            return Result(ri);
        }
        #endregion

        #region 删除
        [IsMaster]
        [HttpPost]
        public ActionResult Delete(long id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var model = NewsBLL.Instance.GetModel(id);
                if (model != null)
                {
                    if (model.IsDelete == 0)
                    {
                        model.IsDelete = 1;
                        ri = NewsBLL.Instance.Update(model);
                    }
                    else
                    {
                        ri.Ok = true;
                        ri.Msg = "新闻已被删除";
                    }
                }
                else
                {
                    ri.Msg = "新闻不存在";
                }
            }
            else
            {
                ri.Msg = "异常";
            }
            return Result(ri);
        }
        #endregion
    }
}