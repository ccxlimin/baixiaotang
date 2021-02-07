using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class SearchResultViewModel
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }
        public List<S> SList { get; set; }
        public Paging SearchPage { get; set; }
    }

    public class S
    {
        /// <summary>
        /// 类型 1 帖子 2文章 3用户
        /// </summary>
        public int type { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
    }
}
