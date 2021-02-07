using AmazonBBS.BLL;
using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace AmazonBBS.Controllers
{
    public class PVController : BaseController
    {
        #region 记录PV访问

        /// <summary>
        /// 记录PV访问
        /// </summary>
        [HttpPost]
        public ActionResult Record(long id, string pvenum)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                PVTableEnum _pv = GetPVType(pvenum.ToLower());
                if (_pv != PVTableEnum.None)
                {
                    ri.Ok = PVBLL.Instance.RecordPVCount(_pv, id);
                }
            }
            return Result(ri);
        }
        #endregion

        #region 修改PV访问量

        /// 修改PV访问量

        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(long id, long number, string editEnum)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                if (number > -1)
                {
                    PVTableEnum _pv = GetPVType(editEnum.ToLower());
                    ri.Ok = PVBLL.Instance.EditPVCount(_pv, id, number);
                }
                else
                {
                    ri.Msg = "浏览量咋能为负呢？";
                }
            }
            else
            {
                ri.Msg = "数据源异常";
            }
            return Result(ri);
        }
        #endregion

        #region 广告浏览量修改
        [HttpPost]
        public ActionResult ADPV(long id, int number)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                if (number > -1)
                {
                    var adcache = (List<AD>)CSharpCacheHelper.Get(APPConst.AD);
                    var model = adcache.First(a => { return a.ADID == id; });
                    if (model.IsDelete == 0)
                    {
                        model.ADViewCount = number;
                        ADBLL.Instance.Update(model);
                        ri.Ok = true;
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
                }
                else
                {
                    ri.Msg = "浏览量咋能为负呢？";
                }
            }
            else
            {
                ri.Msg = "数据源异常";
            }
            return Result(ri);
        }
        #endregion

        #region 新闻浏览量修改
        [HttpPost]
        public ActionResult NewsPV(long id, int number)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                if (number > -1)
                {
                    var model = NewsBLL.Instance.GetModel(id);
                    model.PVCount = number;
                    ri = NewsBLL.Instance.Update(model);
                }
                else
                {
                    ri.Msg = "浏览量咋能为负呢？";
                }
            }
            else
            {
                ri.Msg = "数据源异常";
            }
            return Result(ri);
        }
        #endregion

        private PVTableEnum GetPVType(string pvenum)
        {
            var _pv = PVTableEnum.None;
            switch (pvenum)
            {
                case "bbs": _pv = PVTableEnum.Question; break;
                case "article": _pv = PVTableEnum.Article; break;
                case "party": _pv = PVTableEnum.Activity; break;
                case "gift": _pv = PVTableEnum.Gift; break;
                case "softLink": _pv = PVTableEnum.SoftLink; break;
                case "zhaopin": _pv = PVTableEnum.ZhaoPin; break;
                case "qiuzhi": _pv = PVTableEnum.QiuZhi; break;
                case "product": _pv = PVTableEnum.Product; break;
                case "dataanalysis": _pv = PVTableEnum.Gift; break;
                case "about": _pv = PVTableEnum.About; break;
                case "kecheng": _pv = PVTableEnum.Gift; break;
                case "news": _pv = PVTableEnum.News; break;
                default: _pv = PVTableEnum.None; break;
            }
            return _pv;
        }
    }
}