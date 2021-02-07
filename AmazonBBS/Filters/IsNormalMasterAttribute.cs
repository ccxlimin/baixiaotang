using AmazonBBS.BLL;
using AmazonBBS.Model;
using System.Web.Mvc;
using System.Web.Routing;

namespace AmazonBBS
{
    public class IsNormalMasterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = UserBaseBLL.Instance.UserInfo;
            if (user == null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new ResultInfo
                        {
                            Msg = "请先登录",
                            Ok = false,
                            Type = -9999
                        }
                    };
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "account", action = "login", ReturnUrl = filterContext.HttpContext.Request.Url.ToString() }));
                }
            }
            else
            {
                if (!UserBaseBLL.Instance.IsNormalMaster)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JsonResult
                        {
                            Data = new ResultInfo
                            {
                                Msg = "请先登录",
                                Ok = false,
                                Type = -9999
                            }
                        };
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "home", action = "index" }));
                    }
                }
            }
        }
    }
}