using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace AmazonBBS.Common
{
    public class CSharpCacheHelper
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            Cache cache = HttpRuntime.Cache;
            return cache[key];
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key, T defaultValue)
        {
            var value = HttpRuntime.Cache[key];
            if (value == null)
            {
                return defaultValue;
            }
            else
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Set<T>(string key, T obj)
        {
            Cache cache = HttpRuntime.Cache;
            cache.Insert(key, obj);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="minutes"></param>
        public static void Set<T>(string key, T obj, long minutes)
        {
            Cache cache = HttpRuntime.Cache;
            cache.Insert(key, obj, null, DateTime.Now.AddMinutes(minutes), TimeSpan.Zero);
        }

        /// <summary>
        /// 删除指定缓存
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            Cache cache = HttpRuntime.Cache;
            cache.Remove(key);
        }

        /// <summary>
        /// 删除全部缓存
        /// </summary>
        public static void RemoveAll()
        {
            Cache cache = HttpRuntime.Cache;
            IDictionaryEnumerator cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                cache.Remove(cacheEnum.Key.ToString());
            }
        }
    }
}
