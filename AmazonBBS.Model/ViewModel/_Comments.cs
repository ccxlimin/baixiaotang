using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class _Comments
    {
        public List<_Comment> Comments { get; set; }

        public Paging CommentPage { get; set; }
    }
}
