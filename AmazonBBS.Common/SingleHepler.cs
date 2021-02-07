using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Common
{
    /// <summary>
    /// (单例模式帮助器)
    /// </summary>
    public sealed class SingleHepler<T> where T : class,new()
    {
        private static T _instance;
        private static object _lock = new object();
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance = _instance ?? new T();
                    }
                }
                return _instance;
            }
        }
    }
}
