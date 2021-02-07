using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class ShareViewModel
    {
        public ShareLink ShareLink { get; set; }
        public UserBase UserInfo { get; set; }
        public long ShareID { get; set; }
        public string ShareToken { get; set; }
    }
}
