using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class UserManageViewModel
    {
        //public List<UserBase> UserBaseList { get; set; }

        //public List<UserExt> UserExtList { get; set; }

        public List<UserInfoViewModel> UserInfoList { get; set; }

        public Paging Page { get; set; }
    }
}
