using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AmazonBBS.Common
{
    public class MatchHelper
    {
        public static Regex IsChina = new Regex("^[^\x00-\xFF]");
        public static Regex IsNum = new Regex("^[0-9]+$");
        public static Regex IsEmail = new Regex(@"\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}");
    }
}
