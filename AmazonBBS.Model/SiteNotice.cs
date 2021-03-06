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
    /// 网站滚屏公告
    /// </summary>
    public partial class SiteNotice
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public long Id { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public int NoticeType { get; set; }
    
    	/// <summary>
    	/// 公告小标题
    	/// </summary>
        public string ShortTitle { get; set; }
    
    	/// <summary>
    	/// 公告标题
    	/// </summary>
        public string Title { get; set; }
    
    	/// <summary>
    	/// 短标题背景色
    	/// </summary>
        public string ShortTitleBGColor { get; set; }
    
    	/// <summary>
    	/// 短标题字颜色
    	/// </summary>
        public string ShortTitleFontColor { get; set; }
    
    	/// <summary>
    	/// 标题背景色
    	/// </summary>
        public string TitleBGColor { get; set; }
    
    	/// <summary>
    	/// 标题字颜色
    	/// </summary>
        public string TitleFontColor { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string Url { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public bool IsDelete { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long UserId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime CreateTime { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime UpdateTime { get; set; }
    
    }
}
