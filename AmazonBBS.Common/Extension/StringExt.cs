using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Common
{
    public static class StringExt
    {
        /// <summary>
        /// 指示指定的字符串是 null 还是 System.String.Empty 字符串。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 判断指定的字符串不为null并且不为empty
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// 将指定字符串中的格式项替换为指定数组中相应对象的字符串表示形式。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        public static int ToInt32(this string str)
        {
            if (str.IsNotNullOrEmpty())
            {
                return Convert.ToInt32(str);
            }
            else
            {
                return 0;
            }
        }
        public static long ToInt64(this string str)
        {
            if (str.IsNotNullOrEmpty())
            {
                return Convert.ToInt64(str);
            }
            else
            {
                return 0;
            }
        }
    }
}
