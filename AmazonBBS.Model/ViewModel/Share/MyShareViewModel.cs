using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class MyShareViewModel
    {
        /// <summary>
        /// 首次分享，刚生成链接
        /// </summary>
        public bool IsFirst { get; set; }

        public ShareLink ShareLink { get; set; }

        /// <summary>
        /// 通过链接注册用户数
        /// </summary>
        public int RegistCount { get; set; }

        /// <summary>
        /// 分享奖励表
        /// </summary>
        public List<ShareCoinAndCount> ShareCoinList { get; set; }
    }

    /// <summary>
    /// 分享次数及对应奖励
    /// </summary>
    public class ShareCoinAndCount
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long BBSID { get; set; }
        /// <summary>
        /// 分享次数
        /// </summary>
        public int ShareCount { get; set; }

        /// <summary>
        /// 分享次数达标对应的奖励
        /// </summary>
        public string ShareCoin { get; set; }

        /// <summary>
        /// 当前阶段奖励是否已领取
        /// </summary>
        public bool IsPickCoin { get; set; }
    }
}
