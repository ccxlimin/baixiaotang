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
    /// 礼物费用表
    /// </summary>
    public partial class GiftFee
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public long GiftFeeId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long? GiftID { get; set; }
    
    	/// <summary>
    	/// 票种名称
    	/// </summary>
        public string FeeName { get; set; }
    
    	/// <summary>
    	/// 费用类型 (0免费 10 积分付费 20 金钱付费 30 RMB付费)
    	/// </summary>
        public int? FeeType { get; set; }
    
    	/// <summary>
    	/// 具体费用
    	/// </summary>
        public decimal? Fee { get; set; }
    
    	/// <summary>
    	/// 剩余数量
    	/// </summary>
        public int? FeeCount { get; set; }
    
    }
}