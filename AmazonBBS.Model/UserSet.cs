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
    /// 用户设置表
    /// </summary>
    public partial class UserSet
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public int UserSetId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long UserId { get; set; }
    
    	/// <summary>
    	/// 显示或隐藏个人基本资料 true显示 false隐藏
    	/// </summary>
        public bool ShowOrHideBaseInfo { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime CreateTime { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public int IsDelete { get; set; }
    
    }
}
