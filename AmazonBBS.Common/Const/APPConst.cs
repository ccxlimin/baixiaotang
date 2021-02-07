using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.BLL
{
    public class APPConst
    {
        /// <summary>
        /// 客服
        /// </summary>
        public static readonly string Customoer = "Customoer";

        /// <summary>
        /// 广告
        /// </summary>
        public static readonly string AD = "AD";

        /// <summary>
        /// Balance
        /// </summary>
        public static readonly string Slider = "Slider";

        /// <summary>
        /// 首页标签固定个数
        /// </summary>
        public static readonly string TagFixedNumber = "TagFixedNumber";

        /// <summary>
        /// 首页标签随机展示个数
        /// </summary>
        public static readonly string TagRandomNumber = "TagRandomNumber";

        /// <summary>
        /// 首页右侧公告显示条数
        /// </summary>
        public static readonly string NewShowCount = "NewShowCount";

        /// <summary>
        /// 鼠标点击提示语
        /// </summary>
        public static readonly string ClickMsgs = "ClickMsgs";

        /// <summary>
        /// 有效时间（分钟）
        /// </summary>
        public static class ExpriseTime
        {
            public static readonly long Hour1 = 60;
            public static readonly long Hour2 = 120;
            public static readonly long Hour3 = 180;
            public static readonly long Hour6 = 360;
            public static readonly long Day1 = 1440;
            public static readonly long Day2 = 2880;
            public static readonly long Day5 = 7200;
            public static readonly long Week1 = 10080;
            public static readonly long Week2 = 20160;
            public static readonly long Week3 = 30240;
            public static readonly long Month1 = 43200;
            public static readonly long Month2 = 86400;
            public static readonly long Month3 = 129600;
            public static readonly long Year1 = 525600;
        }
    }
}
