using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 菜单顶级名称
    /// </summary>
    public enum MenuParentEnumType
    {
        [Description("社区贴吧")]
        BBS = 1,
        [Description("招聘求职")]
        Job = 2,
        [Description("干货课程")]
        KeCheng = 3,
        [Description("文章")]
        Article = 4,
        [Description("百晓堂")]
        BXT = 5,
        [Description("活动")]
        Party = 6,
        [Description("导航")]
        Link = 7
    }
}
