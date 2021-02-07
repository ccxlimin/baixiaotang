using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonBBS.BLL.OAuth2
{
    /// <summary>
    /// 第三方微信登录接口
    /// </summary>
    public class WeChatApi
    {
        /// <summary>
        /// 获取Code
        /// </summary>
        public static readonly string Wx_GetCode = "https://open.weixin.qq.com/connect/qrconnect?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_login&state={2}#wechat_redirect";

        /// <summary>
        /// 获取AcessToken
        /// </summary>
        public static readonly string Wx_GetToken = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";

        /// <summary>
        /// 获取用户基础信息
        /// </summary>
        public static readonly string Wx_GetBaseUser = "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}";
    }
}
