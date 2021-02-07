using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class Paging
    {
        /// <summary>
        /// 自动计算分页
        /// </summary>
        /// <param name="totalCount">总数</param>
        /// <param name="pageIndex">起始页</param>
        /// <param name="url">请求的路径</param>
        /// <param name="className">前端dom-class</param>
        public Paging(int totalCount = 0, int pageIndex = 1, int pageSize = 20)
        {
            PageSize = pageSize;
            RecordCount = totalCount;
            PageIndex = pageIndex < 1 || PageCount < pageIndex ? 1 : pageIndex;
        }


        #region 数据库查询 用
        /// <summary>
        /// 开始
        /// </summary>
        public int StartIndex
        {
            get { return (PageIndex - 1) * PageSize + 1; }
            set { }
        }

        /// <summary>
        /// 结束
        /// </summary>
        public int EndIndex
        {
            get { return PageIndex * PageSize; }
            set { }
        }
        #endregion

        #region 前台页面分页用
        private int _pageindex = 0;
        /// <summary>
        /// 页码（默认从第一页开始)
        /// </summary>
        public int PageIndex
        {
            get { return _pageindex; }
            set
            {
                _pageindex = value < 1 ? 1 : value;
            }
        }

        private int _pagesize = 0;
        /// <summary>
        /// 每页总条数(默认20行数据)
        /// </summary>
        public int PageSize
        {
            get { return _pagesize; }
            set
            {
                _pagesize = value < 1 ? 1 : value;
            }
        }

        /// <summary>
        /// 数据总数
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 总页数(只读)
        /// </summary>
        public int PageCount
        {
            get { return RecordCount % PageSize == 0 ? RecordCount / PageSize : RecordCount / PageSize + 1; }
            //set { }
        }
        #endregion

        //public string Link = string.Empty;//ajax连接的网址

        //public string ClassName = string.Empty;

        ///// <summary>
        ///// 记录数据集
        ///// </summary>
        //private object dataSource = null;

        /// <summary>
        /// 是否为空
        /// </summary>
        //private bool isNull { get; set; }

        //public void SetDataSource<T>(IEnumerable<T> list)
        //{
        //    isNull = list == null || list.Count() == 0;
        //    dataSource = list;
        //}

        //public IEnumerable<T> GetDataSource<T>()
        //{
        //    if (isNull)
        //    {
        //        return new T[] { };
        //    }
        //    else
        //    {
        //        return (IEnumerable<T>)dataSource;
        //    }
        //}

    }
}
