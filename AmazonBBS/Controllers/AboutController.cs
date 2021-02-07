using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmazonBBS.BLL;
using AmazonBBS.Common;

namespace AmazonBBS.Controllers
{
    public class AboutController : BaseController
    {
        // GET: About
        public ActionResult Index()
        {
            //记录PV访问量
            AboutBLL.Instance.PV();
            About model = AboutBLL.Instance.FindNew();
            ViewBag.News = NewsBLL.Instance.FindList(5);
            return View(model);
        }

        #region 编辑主页
        [LOGIN]
        [HttpPost]
        public ActionResult Add(string desc)
        {
            ResultInfo ri = new ResultInfo();
            if (UserBaseBLL.Instance.IsRoot)
            {
                if (desc.IsNotNullOrEmpty())
                {
                    AboutBLL bll = AboutBLL.Instance;
                    int count = bll.Count();
                    if (count == 0 || (count > 0 && bll.DeleteALL()))
                    {
                        About model = new About()
                        {
                            Desc = HttpUtility.UrlDecode(desc),
                            CreateTime = DateTime.Now,
                            IsDelete = 0
                        };
                        int result = bll.Add(model);
                        if (result > 0)
                        {
                            ri.Msg = "编辑成功";
                            ri.Ok = true;
                        }
                        else
                        {
                            ri.Msg = "编辑保存";
                        }

                    }
                }
                else
                {
                    ri.Msg = "公司介绍不能为空";
                }
            }
            else
            {
                ri.Msg = "你没有权限";
            }
            return Result(ri);
        }
        #endregion
    }
}