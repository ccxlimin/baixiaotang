using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class MyNotice
    {
        public List<Notice> NoticeList { get; set; }
        public Paging NoticePage { get; set; }
    }
}
