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
    /// 用户封禁记录表
    /// </summary>
    public partial class UserDisabledLog
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public long Id { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long UserId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long ActionUserId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime ExpriseTime { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public bool IsDelete { get; set; }
    
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
