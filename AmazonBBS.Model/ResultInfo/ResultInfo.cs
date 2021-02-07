using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 操作结果集
    /// </summary>
    public class ResultInfo
    {
        /// <summary>
        /// 成功or失败
        /// </summary>
        public bool Ok { get; set; }
        /// <summary>
        /// 消息头
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 返回操作ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 数据载体
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 操作完成之后需要跳转的url
        /// </summary>
        public string Url { get; set; }
    }

    /// <summary>
    /// 操作结果集
    /// </summary>
    public class ResultInfo<T>
    {
        /// <summary>
        /// 成功or失败
        /// </summary>
        public bool Ok { get; set; }
        /// <summary>
        /// 消息头
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 数据载体
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 操作完成之后需要跳转的url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 返回操作ID
        /// </summary>
        public int ID { get; set; }
    }
}
