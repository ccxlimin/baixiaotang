using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 后台用
    /// </summary>
    public class TagViewModel
    {
        public List<_Tag> Tags { get; set; }
        public Paging TagPage { get; set; }
    }
}
