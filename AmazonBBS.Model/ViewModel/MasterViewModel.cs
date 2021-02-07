using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class MasterViewModel : Master
    {
        public string UserName { get; set; }

        public string HeadUrl { get; set; }

        public int UserV { get; set; }
    }
}
