using AmazonBBS.BLL;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Controllers
{
    public class SoftLinkController : BaseController
    {
        // GET: SoftLink
        public ActionResult Index()
        {
            //List<SoftLink> list = SoftLinkBLL.Instance.SearchAll();
            var list = DB.SoftLink.Where(a => a.IsDelete == 0).OrderByDescending(a => a.SoftLinkID).ToList();
            return View(list);
        }

        #region 编辑
        //public ActionResult Edit(long id)
        //{

        //}
        #endregion

        #region 删除
        [IsMaster]
        [HttpPost]
        public ActionResult DeleteSoft(long id)
        {
            ResultInfo ri = new ResultInfo();

            if (id > 0)
            {
                try
                {
                    SoftLink model = SoftLinkBLL.Instance.GetModel(id);
                    if (model != null)
                    {
                        if (model.IsDelete == 1)
                        {
                            ri.Msg = "链接已被删除";
                            ri.Ok = true;
                        }
                        else
                        {
                            model.IsDelete = 1;
                            ri = SoftLinkBLL.Instance.Update(model);
                            if (ri.Ok) { ri.Msg = "删除成功"; }
                        }
                    }
                    else
                    {
                        ri.Msg = "链接不存在";
                    }
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                ri.Msg = "异常";
            }

            return Result(ri);
        }
        #endregion

        #region 搜索
        [HttpGet]
        public ActionResult Search(string key)
        {
            ResultInfo ri = new ResultInfo();
            ri.Data = DB.SoftLink.Where(a => a.IsDelete == 0 && (a.LinkMemo.Contains(key) || a.LinkName.Contains(key))).Select(a => new
            {
                name = a.LinkName,
                url = a.LinkAddress,
                desc = a.LinkMemo
            }).ToList();
            ri.Ok = true;
            return Result(ri);
        }
        #endregion
    }
}