using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public enum UserSetEnum
    {
        /// <summary>
        /// 隐藏/显示个人资料
        /// </summary>
        [Description("隐藏/显示个人资料")]
        HideOrShowUserInfo = 1,
    }
}
