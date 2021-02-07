using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class MyMessage
    {
        public List<_MsgBox> ChatList { get; set; }
        public Paging ChatPage { get; set; }

        public long MeID { get; set; }

        public long ToID { get; set; }

        public string ToUserName { get; set; }
    }
}
