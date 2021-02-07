using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public enum RegistEnumType
    {
        /// <summary>
        /// QQ登录
        /// </summary>
        QQAuth = 1,
        /// <summary>
        /// 邮箱注册
        /// </summary>
        EmailRegist = 2,
        /// <summary>
        /// 微信登录
        /// </summary>
        WeChatAuth = 3,

        /// <summary>
        /// 百晓堂采集器CS用户
        /// </summary>
        AmzSharer = 4,

    }
}
