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
    /// 反对表
    /// </summary>
    public partial class Against
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public System.Guid AgainstId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long TargetID { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public int Type { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long UserID { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime AgainstTime { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public int IsDelete { get; set; }
    
    }
}
