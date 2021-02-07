using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Common
{
    /// <summary>
    /// Json格式帮助类
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="value">需要转换的对象值</param>
        /// <returns></returns>
        public static string ToJson<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// 将json转为List集合
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static List<T> JsonToList<T>(string json)
        {
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        /// <summary>
        /// json反序列化
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static T JsonToModel<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
