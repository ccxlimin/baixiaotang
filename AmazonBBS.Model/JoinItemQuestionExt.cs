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
    /// 报名购买选项问题扩展表
    /// </summary>
    public partial class JoinItemQuestionExt
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public long JoinItemQuestionExtId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public int? MainType { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long? MainID { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string ItemName { get; set; }
    
    	/// <summary>
    	/// 是否必填项
    	/// </summary>
        public int? IsMustWrite { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime? CreateTime { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string CreateUser { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime? UpdateTime { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string UpdateUser { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public int? IsDelete { get; set; }
    
    }
}
