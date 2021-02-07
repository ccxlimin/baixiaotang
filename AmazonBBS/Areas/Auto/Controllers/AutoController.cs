using AmazonBBS.Areas.Auto.BLL;
using AmazonBBS.Areas.Auto.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Areas.Auto.Controllers
{
    public class AutoController : Controller
    {
        // GET: Test/Test
        public ActionResult Index()
        {
#if DEBUG
            DataTable tab = new AutoHelpBiz().GetTable();

            ViewBag.dt = tab;

            AutoUI model = new AutoUI();
            return View(model);
#endif
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Auto(AutoUI model, FormCollection form)
        {
#if DEBUG
            model.Tables = form["tables"];
            model.Methods = form["Methods"];
            var ri = new AutoHelpBiz().Create(model);

            return Json(ri);
#endif

            return Content("运行环境不正常");
        }
    }
}