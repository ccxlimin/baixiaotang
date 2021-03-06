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
    /// 邮件通知
    /// </summary>
    public partial class EmailNotice
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public long EmailNoticeId { get; set; }
    
    	/// <summary>
    	/// 邮件通知作者(1通知 0不通知)
    	/// </summary>
        public int? EmailNoticeAuthor { get; set; }
    
    	/// <summary>
    	/// 作者ID 
    	/// </summary>
        public long? AuthorID { get; set; }
    
    	/// <summary>
    	/// 对象主键
    	/// </summary>
        public long? MainID { get; set; }
    
    	/// <summary>
    	/// 对象类型(1文章 2问题....）
    	/// </summary>
        public int? MainType { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string MD5Key { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string MD5Sign { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime? CreateTime { get; set; }
    
    }
}
