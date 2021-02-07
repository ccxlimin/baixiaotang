using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonBBS.BLL.OAuth2
{
    [Serializable]
    public class WeChatUser
    {

        public string openid { get; set; }

        public string nickname { get; set; }

        public string sex { get; set; }

        public string province { get; set; }

        public string city { get; set; }

        public string country { get; set; }

        public string headimgurl { get; set; }

        public string unionid { get; set; }

      
    }
}
