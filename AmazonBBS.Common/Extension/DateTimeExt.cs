using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonBBS.Common
{
    public static class DateTimeExt
    {
        /// <summary>
        /// DateTime时间转换扩展方法
        /// </summary>
        /// <param name="type">
        /// 1 : yyyy-MM-dd HH:mm:ss 
        /// 2 : yyyy-MM-dd 
        /// 3 : yyyy-MM-dd HH:mm:ss:fff 
        /// 4 : yyyy/MM/dd HH:mm:ss  
        /// 5 : yyyy/MM/dd 
        /// 6 : yyyyMMdd 
        /// 7 : yyyy年MM月dd日 
        /// 8 : yyyy/MM/dd HH:mm  
        /// 9 : yyyyMMddHHmmssffff  
        /// 10: yyyy.MM.dd
        /// 11 : yyyy-MM-dd HH:mm 
        /// </param>
        public static string ToString(this DateTime time, int type)
        {
            string rs = string.Empty;
            switch (type)
            {
                case 1:
                    rs = time.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case 2:
                    rs = time.ToString("yyyy-MM-dd");
                    break;
                case 3:
                    rs = time.ToString("yyyy-MM-dd HH:mm:ss:fff");
                    break;
                case 4:
                    rs = time.ToString("yyyy/MM/dd HH:mm:ss");
                    break;
                case 5:
                    rs = time.ToString("yyyy/MM/dd");
                    break;
                case 6:
                    rs = time.ToString("yyyyMMdd");
                    break;
                case 7:
                    rs = time.ToString("yyyy年MM月dd日");
                    break;
                case 8:
                    rs = time.ToString("yyyy/MM/dd HH:mm");
                    break;
                case 9:
                    rs = time.ToString("yyyyMMddHHmmssffff");
                    break;
                case 10:
                    rs = time.ToString("yyyy.MM.dd");
                    break;
                case 11:
                    rs = time.ToString("yyyy-MM-dd HH:mm"); break;
                default:
                    rs = time.ToString();
                    break;
            }
            return rs;
        }
    }
}
