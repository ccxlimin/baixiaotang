using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class ArticleViewModel
    {
        public List<_Article> Articles { get; set; }

         public Paging ARticlePage { get; set; }
    }
}
