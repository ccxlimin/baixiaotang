using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AmazonBBS.Common
{
    public class CookieHelper
    {
        public static void CreateCookie(string key, string obj, DateTime date)
        {
            HttpCookie cookie = new HttpCookie(key);
            cookie.Value = obj;
            cookie.Expires = date;
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        public static void CreateCookie(string key, string obj)
        {
            HttpCookie cookie = new HttpCookie(key);
            cookie.Value = obj;
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 获取Cookie内容
        /// </summary>
        /// <param name="key">主键</param>
        /// <returns>值</returns>
        public static string GetCookie(string key)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[key] != null)
            {
                return HttpContext.Current.Request.Cookies[key].Value.ToString();
            }
            return string.Empty;
        }
        /// <summary>
        /// 清除本地信息
        /// </summary>
        public static void RemoveCookie(string key)
        {
            HttpCookie cookie = new HttpCookie(key);
            cookie.Name = key;
            cookie.Expires = DateTime.Now.AddSeconds(-1);
            HttpContext.Current.Response.AppendCookie(cookie);
        }
    }
}
