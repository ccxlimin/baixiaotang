using AmazonBBS.BLL;
using System.Web.Mvc;
using System.Web.Routing;
using System;
using Newtonsoft.Json;
using AmazonBBS.Model;

namespace AmazonBBS
{
    public class LOGINAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isLogin = UserBaseBLL.Instance.UserInfo != null;
            if (!isLogin)
            {
                //如果是ajax请求，返回json对象
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new ResultInfo
                        {
                            Msg = "请先登录",
                            Ok = false,
                            ID = -9999
                        }
                    };
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "account", action = "login", ReturnUrl = filterContext.HttpContext.Request.Url.ToString() }));
                }
            }
        }
    }
}