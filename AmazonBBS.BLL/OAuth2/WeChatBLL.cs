using AmazonBBS.Common;
using AmazonBBS.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace AmazonBBS.BLL.OAuth2
{
    public class WeChatBLL
    {
        /// <summary>
        /// 获取Code 接口地址
        /// </summary>
        /// <returns></returns>
        public string GetState()
        {
            string state = StringHelper.GetNickName(20);
            //HttpContext.Current.Session["webChatState"] = state;
            SessionHelper.Set("webChatState", state);
            return state;
        }

        public string GetUrl()
        {
            string url = string.Format(WeChatApi.Wx_GetCode, LoginConfig.Wx_AppID, LoginConfig.Wx_CallBack, GetState());
            return url;
        }

        /// <summary>
        /// 获取token ,openid ,如果用户已经存在则跳转，不存在添加在跳转
        /// </summary>
        /// <param name="code">code</param>
        /// <param name="state">防伪参数</param>
        /// <returns></returns>
        public ResultInfo<WeChatToken> GetToken(string code, string state)
        {
            ResultInfo<WeChatToken> ri = new ResultInfo<WeChatToken>();
            //检查用户是否非法进入还是二维码已过期
            if (HttpContext.Current.Session["webChatState"] == null || HttpContext.Current.Session["webChatState"].ToString() != state)
            {
                ri.Msg = "微信二维码已过期，请返回重新扫码登录！";
                return ri;
            }
            string url = string.Format(WeChatApi.Wx_GetToken, LoginConfig.Wx_AppID, LoginConfig.Wx_AppKey, code);//api 接口

            string result = StringHelper.GetResponse(url, string.Empty, "GET");

            if (string.IsNullOrWhiteSpace(result))
            {
                ri.Msg = "调用微信登录接口异常！";
                return ri;
            }

            //ri.Data = new WeChatToken();
            try
            {
                ri.Data = JsonConvert.DeserializeObject<WeChatToken>(result); //解析JSON
            }
            catch(Exception e)
            {
                ri.Msg = "调用微信登录接口异常！";
                return ri;
            }

            if (ri.Data == null || string.IsNullOrWhiteSpace(ri.Data.openid))
            {
                ri.Msg = "获取OpenID失败";
                return ri;
            }
            else
            {
                ri.Ok = true;
            }
            return ri;

        }

        public ResultInfo<WeChatUser> GetUserInfo(string token)
        {
            ResultInfo<WeChatUser> ri = new ResultInfo<WeChatUser>();
            string url = string.Format(WeChatApi.Wx_GetBaseUser, token, LoginConfig.Wx_AppID);
            string result = StringHelper.GetResponse(url, string.Empty, "GET");
            if (result.IsNotNullOrEmpty())
            {
                try
                {
                    ri.Data = JsonConvert.DeserializeObject<WeChatUser>(result);

                }
                catch
                {
                    ri.Msg = "调用接口异常";
                    return ri;
                }
                finally
                {
                    if (ri.Data == null || ri.Data.openid.IsNullOrEmpty())
                    {
                        ri.Msg = "获取用户信息异常";
                    }
                    else
                    {
                        ri.Ok = true;
                    }
                }
            }
            else
            {
                ri.Msg = "获取用户信息异常";
            }
            return ri;
        }
    }
}
