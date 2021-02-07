using AmazonBBS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AmazonBBS.BLL
{
    public class AliPayBLL
    {
        public static AliPayBLL Instance
        {
            get
            {
                return SingleHepler<AliPayBLL>.Instance;
            }
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> GetRequestGet(string query)
        {
            IDictionary<string, string> sArray = new Dictionary<string, string>();
            //对url第一个字符？过滤
            if (!string.IsNullOrEmpty(query))
            {
                //根据&符号分隔成数组
                string[] coll = query.Split('&');
                //定义临时数组
                string[] temp = { };
                //循环各数组
                for (int i = 0; i < coll.Length; i++)
                {
                    //根据=号拆分
                    temp = coll[i].Split('=');
                    //把参数名和值分别添加至SortedDictionary数组
                    sArray.Add(temp[0], HttpUtility.UrlDecode(temp[1]));
                }
            }
            return sArray;
        }
    }
}
