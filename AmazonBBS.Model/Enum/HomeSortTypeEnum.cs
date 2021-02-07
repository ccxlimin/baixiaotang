using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 首页 问题 列表 排序类型
    /// </summary>
    public enum HomeSortTypeEnum
    {
        /// <summary>
        /// 默认
        /// </summary>
        [Description("默认")]
        Sort_Default = 0,

        /// <summary>
        /// 热门
        /// </summary>
        [Description("热门")]
        Sort_Hot = 1,

        /// <summary>
        /// 精华
        /// </summary>
        [Description("精华")]
        Sort_JinHua = 2,

        /// <summary>
        /// 置顶
        /// </summary>
        [Description("置顶")]
        Sort_Top = 3,

        /// <summary>
        /// 待回复
        /// </summary>
        [Description("待回复")]
        Sort_NoComment = 4,

        /// <summary>
        /// 新贴
        /// </summary>
        [Description("新贴")]
        Sort_New = 5,

        [Description("帖子")]
        Sort_AllBBS = 6,

        [Description("文章")]
        Sort_AllArticle = 7,
    }
}
