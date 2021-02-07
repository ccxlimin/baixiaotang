using AmazonBBS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonBBS.BLL.OAuth2
{
    public class LoginConfig
    {
        #region QQ 配置信息

        /// <summary>
        /// QQ appid
        /// </summary>
        public static string QQ_AppID = ConfigHelper.AppSettings("QQ.AppID");

        /// <summary>
        /// QQ appkey
        /// </summary>
        public static string QQ_AppKey = ConfigHelper.AppSettings("QQ.AppKey");

        /// <summary>
        /// QQ 网站回调域
        /// </summary>
        public static string QQ_CallBack = ConfigHelper.AppSettings("QQ.CallBackUrl");

        #endregion

        #region 微信配置信息

        /// <summary>
        /// 微信 appid
        /// </summary>
        public static string Wx_AppID = ConfigHelper.AppSettings("WeiXin.AppID");

        /// <summary>
        /// 微信 appkey
        /// </summary>
        public static string Wx_AppKey = ConfigHelper.AppSettings("WeiXin.AppKey");

        /// <summary>
        /// 微信回调地址
        /// </summary>
        public static string Wx_CallBack = ConfigHelper.AppSettings("WeiXin.CallBackUrl");
        #endregion
    }
}
