using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class GiftViewModel
    {
        public List<_Gift> Gifts { get; set; }

        public Paging GiftPage { get; set; }
    }
}
