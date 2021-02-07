using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 网站枚举
    /// </summary>
    public enum BBSEnumType
    {
        /// <summary>
        /// 菜单
        /// </summary>
        Menu = 1,
        /// <summary>
        /// 积分等级
        /// </summary>
        ScoreLevel = 2,

        /// <summary>
        /// 头衔
        /// </summary>
        LevelName = 3,

        /// <summary>
        /// 专属头衔
        /// </summary>
        OnlyLevelName = 4,

        /// <summary>
        /// 分享链接奖励及对应次数
        /// </summary>
        ShareCoin = 5
    }
}
