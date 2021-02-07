using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 通用加载数据集合及分页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseListViewModel<T>
    {
        /// <summary>
        /// 实体数据
        /// </summary>
        public List<T> DataList { get; set; }

        /// <summary>
        /// 页数据
        /// </summary>
        public Paging Page { get; set; }
    }
}
