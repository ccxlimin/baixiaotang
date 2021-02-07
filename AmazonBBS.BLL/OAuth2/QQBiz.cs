using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace AmazonBBS.BLL.OAuth2
{
    public class QQBiz
    {
        /// <summary>
        /// 获取请求Code的网址
        /// </summary>
        /// <returns></returns>
        public string GetCode()
        {
            string state = StringHelper.GetNickName(32);
            string url = string.Format(QQApi.QQ_GetCode, LoginConfig.QQ_AppID, LoginConfig.QQ_CallBack, state);
            HttpContext.Current.Session["qqState"] = state;
            SessionHelper.Set("qqState", state);
            return url;
        }

        public ResultInfo GetToken(string code, string state)
        {
            ResultInfo ri = new ResultInfo();
            //检查用户是否非法进入
            if (HttpContext.Current.Session["qqState"] == null || HttpContext.Current.Session["qqState"].ToString() != state)
            {
                ri.Msg = "请返回重新登录";
                return ri;
            }

            string tokenUrl = string.Format(QQApi.QQ_GetToken, LoginConfig.QQ_AppID, LoginConfig.QQ_AppKey, code, LoginConfig.QQ_CallBack);

            string result = StringHelper.GetResponse(tokenUrl, "", "GET");

            string tokens = SpiltToken(result);

            if (string.IsNullOrWhiteSpace(tokens))
            {
                ri.Msg = "无法获取token";
                return ri;
            }

            string openUrl = string.Format(QQApi.QQ_GetOpenId, tokens);
            string qqResult = StringHelper.GetResponse(openUrl, "", "GET");

            if (string.IsNullOrWhiteSpace(qqResult))
            {
                ri.Msg = "调用接口异常（openid）";

                return ri;
            }

            qqResult = qqResult.Replace("callback(", "").Replace(" );", "");

            QQOpen model;
            try
            {
                model = Newtonsoft.Json.JsonConvert.DeserializeObject<QQOpen>(qqResult); //解析JSON
            }
            catch (Exception e)
            {
                ri.Msg = "调用接口异常";
                return ri;
            }

            #region 检查openid是否存在
            if (model == null || string.IsNullOrWhiteSpace(model.openid))
            {
                ri.Msg = "无法获取用户OpenId";

                return ri;
            }
            #endregion
            return ri;
        }

        /// <summary>
        /// 解析 token
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private string SpiltToken(string result)
        {
            if (string.IsNullOrWhiteSpace(result)) return null;

            string value = string.Empty;
            var arr = result.Split('&');

            foreach (string str in arr)
            {
                string[] token = str.Split('=');
                if (token[0] == "access_token")
                {
                    value = token[1];
                    break;
                }
            }

            return value;
        }
    }
}