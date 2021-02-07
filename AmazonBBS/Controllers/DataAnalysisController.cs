using AmazonBBS.BLL;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Controllers
{
    /// <summary>
    /// 数据分析
    /// </summary>
    public class DataAnalysisController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Gift", new { id = 2 });
        }

        public ActionResult Detail(long id = 0)
        {
            if (id > 0)
            {
                Paging page = InitPage();
                _Gift gift = GiftBLL.Instance.GetGiftDetail(id, UserID, page, CommentEnumType.DataAnalysis, PriseEnumType.DataComment, JoinItemTypeEnum.DataAnalysis);
                if (gift == null)
                {
                    return RedirectToAction("Index");
                }
                return View("/Views/Gift/Detail.cshtml", gift);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}