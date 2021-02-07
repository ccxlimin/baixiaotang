using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class ProductViewModel
    {
        public List<_Product> ProductList { get; set; }

        public Paging ProductPage { get; set; }
    }
}
