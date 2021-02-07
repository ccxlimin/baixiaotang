using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class BBSListViewModel
    {
        public List<_QuestionInfo> QuestionList { get; set; }

        public Paging QuestionPage { get; set; }

        public string QuestionHeadUrl { get; set; }

    }
}
