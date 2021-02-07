using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AmazonBBS.Areas.Auto.Models
{
    public class AutoUI
    {
        /// <summary>
        /// 是否生成控制器(如果文件存在，则不会覆盖)
        /// </summary>
        public bool IsControl { get; set; }

        /// <summary>
        /// 是否生成模型文件
        /// </summary>
        public bool IsEntity { get; set; }

        /// <summary>
        /// 选择的表名
        /// </summary>
        public string Tables { get; set; }

        /// <summary>
        /// 是否生成业务层代码
        /// </summary>
        public bool IsBiz { get; set; }

        /// <summary>
        /// 是否生成数据层代码
        /// </summary>
        public bool IsDataAccess { get; set; }

        /// <summary>
        /// 生成方法
        /// </summary>
        public string Methods { get; set; }

        /// <summary>
        /// 获取所有表结构
        /// </summary>
        public DataTable GetTable { get; set; }

    }
}