//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace AmazonBBS.Model
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// 页面模块对应标签
    /// </summary>
    public partial class MenuBelongTag
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public int MenuBelongTagId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long? MainId { get; set; }
    
    	/// <summary>
    	/// 类型（1贴吧 2文章。。。）
    	/// </summary>
        public int? MainType { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long? TagId { get; set; }
    
    }
}
