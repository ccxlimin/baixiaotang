using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonBBS.BLL.OAuth2
{
    public class QQApi
    {
        /// <summary>
        /// 获取Code
        /// </summary>
        public static readonly string QQ_GetCode = "https://graph.qq.com/oauth2.0/authorize?response_type=code&client_id={0}&redirect_uri={1}&state={2}";


        /// <summary>
        /// 获取token
        /// </summary>
        public static readonly string QQ_GetToken = "https://graph.qq.com/oauth2.0/token?grant_type=authorization_code&client_id={0}&client_secret={1}&code={2}&redirect_uri={3}";

        /// <summary>
        /// 获取OpenID
        /// </summary>
        public static readonly string QQ_GetOpenId = "https://graph.qq.com/oauth2.0/me?access_token={0}";

        /// <summary>
        /// 获取用户基础信息
        /// </summary>
        public static readonly string QQ_GetUserInfo = "https://graph.qq.com/user/get_user_info?access_token={0}&oauth_consumer_key={1}&openid={2}";

    }
}
