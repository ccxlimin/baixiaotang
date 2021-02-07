using AmazonBBS.BLL;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Controllers
{
    public class KeChengController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Gift", new { id = 3 });
        }

        public ActionResult Detail(long id = 0)
        {
            if (id > 0)
            {
                Paging page = InitPage();
                _Gift gift = GiftBLL.Instance.GetGiftDetail(id, UserID, page, CommentEnumType.KeCheng, PriseEnumType.KeChengCommen, JoinItemTypeEnum.KeCheng);
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