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
    /// 投递简历记录表
    /// </summary>
    public partial class SendCV
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public long SendCVId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long? ZhaoPinPublisher { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long? ZhaoPinID { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string CreateUser { get; set; }
    
    	/// <summary>
    	/// 简历路径
    	/// </summary>
        public string CVPath { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime? SendTime { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public int? IsRead { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime? ReadTime { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public int? IsDelete { get; set; }
    
    }
}
