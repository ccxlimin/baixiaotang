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
    /// 消费查看答案记录表
    /// </summary>
    public partial class FeeAnswerLog
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public long FeeAnswerLogId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long? AnswerId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long? UserID { get; set; }
    
    	/// <summary>
    	/// 消费时间
    	/// </summary>
        public System.DateTime? FeeTime { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public int? IsDelete { get; set; }
    
    }
}