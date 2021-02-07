using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class CustomerVM
    {
        public List<Customer> QQs { get; set; }
        public List<Customer> WXs { get; set; }
        public List<Customer> GZHs { get; set; }
    }
}
