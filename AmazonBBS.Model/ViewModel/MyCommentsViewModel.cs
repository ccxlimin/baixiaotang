using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class MyCommentsViewModel
    {
        public List<_MyComments> CommentList { get; set; }

        public Paging CommentPage { get; set; }

        /// <summary>
        /// 回答者头像
        /// </summary>
        public string CommentHeadUrl { get; set; }
    }
}
