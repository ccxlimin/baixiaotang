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
    public class ADController : BaseController
    {
        // GET: AD
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 返回广告信息并添加浏览记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddPV(long id)
        {
            ResultInfo ri = new ResultInfo();
            var adcache = (List<AD>)CSharpCacheHelper.Get(APPConst.AD);
            var model = adcache.First(a => { return a.ADID == id; });
            if (model.IsDelete == 0)
            {
                model.ADViewCount += 1;
                ADBLL.Instance.Update(model);
                ri.Ok = true;
                //ri.Data = model;
                ri.Data = new
                {
                    title = model.ADTitle,
                    contact = model.ADContact,
                    wechat = model.ADWeChat
                };
                adcache.Remove(adcache.First(a => { return a.ADID == id; }));
                adcache.Add(model);
                CSharpCacheHelper.Set(APPConst.AD, adcache, APPConst.ExpriseTime.Day2);
            }
            return Result(ri);
        }

        [IsMaster]
        [HttpPost]
        public ActionResult Delete(long id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var adcache = (List<AD>)CSharpCacheHelper.Get(APPConst.AD);
                var model = adcache.First(a => { return a.ADID == id; });
                if (model != null)
                {
                    if (model.IsDelete == 0)
                    {
                        model.IsDelete = 1;
                        ri = ADBLL.Instance.Update(model);
                        if (ri.Ok)
                        {
                            adcache.Remove(adcache.First(a => { return a.ADID == id; }));
                            adcache.Add(model);
                            CSharpCacheHelper.Set(APPConst.AD, adcache, APPConst.ExpriseTime.Day2);
                        }
                    }
                    else
                    {
                        ri.Msg = "广告已被删除";
                    }
                }
                else
                {
                    ri.Msg = "广告信息不存在";
                }
            }
            else
            {
                ri.Msg = "异常";
            }
            return Result(ri);
        }

        [IsMaster]
        public ActionResult Info(long id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var adcache = (List<AD>)CSharpCacheHelper.Get(APPConst.AD);
                var model = adcache.First(a => { return a.ADID == id; });
                if (model == null)
                {
                    ri.Msg = "广告信息不存在！";
                }
                else if (model.IsDelete == 0)
                {
                    ri.Ok = true;
                    ri.Data = new
                    {
                        title = model.ADTitle,
                        desc = model.ADMsg,
                        contact = model.ADContact,
                        wechat = model.ADWeChat
                    };
                }
                else
                {
                    ri.Msg = "广告已被删除";
                }
            }
            else
            {
                ri.Msg = "异常";
            }
            return Result(ri);
        }

        public ActionResult Edit(long id, string title, string desc, string contact, string wechat)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var adcache = (List<AD>)CSharpCacheHelper.Get(APPConst.AD);
                var model = adcache.First(a => { return a.ADID == id; });
                if (model == null)
                {
                    ri.Msg = "广告信息不存在！";
                }
                else if (model.IsDelete == 0)
                {
                    model.ADTitle = title;
                    model.ADMsg = desc;
                    model.ADContact = contact;
                    model.ADWeChat = wechat;
                    ri = ADBLL.Instance.Update(model);
                }
                else
                {
                    ri.Msg = "广告已被删除";
                }
                adcache.Remove(adcache.First(a => { return a.ADID == model.ADID; }));
                adcache.Add(model);
                CSharpCacheHelper.Set(APPConst.AD, adcache, APPConst.ExpriseTime.Day2);
            }
            else
            {
                ri.Msg = "异常";
            }
            return Result(ri);
        }
    }
}