using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class MyArticleViewModel
    {
        public List<_Article> Articles { get; set; }
        public Paging ArticlePage { get; set; }

        public string ArticleHeadUrl { get; set; }
    }
}
