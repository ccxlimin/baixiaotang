using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 首页、话题页展示
    /// </summary>
    public class HomeTagsViewModel : Tag
    {
        /// <summary>
        ///  标签下有多少内容
        /// </summary>
        public int ItemCount { get; set; }
    }
}
