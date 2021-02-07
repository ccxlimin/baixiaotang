using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class _Answers
    {
        public List<_Answer> Answers { get; set; }

        public Paging AnswerPage { get; set; }
    }
}
