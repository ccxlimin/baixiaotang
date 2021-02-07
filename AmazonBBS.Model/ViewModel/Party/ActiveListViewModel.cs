using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class ActiveListViewModel
    {
        public List<_Activity> Activis { get; set; }
        public Paging ActivityPage { get; set; }
    }
}
