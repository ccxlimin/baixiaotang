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
    /// 活动报名表
    /// </summary>
    public partial class ActivityJoin
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public long ActivityJoinId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long? ActivityId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long? JoinUserID { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string JoinUserName { get; set; }
    
    	/// <summary>
    	/// 用户选择费用类型
    	/// </summary>
        public int? FeeType { get; set; }
    
    	/// <summary>
    	/// 是否已付费
    	/// </summary>
        public int? IsFeed { get; set; }
    
    	/// <summary>
    	/// 报名时间
    	/// </summary>
        public System.DateTime? JoinTime { get; set; }
    
    	/// <summary>
    	/// 报名份数
    	/// </summary>
        public int? JoinCount { get; set; }
    
    	/// <summary>
    	/// 联系人
    	/// </summary>
        public string LinkMan { get; set; }
    
    	/// <summary>
    	/// 联系方式
    	/// </summary>
        public string LinkTel { get; set; }
    
    	/// <summary>
    	/// 费用类型
    	/// </summary>
        public long? ActivityFeeId { get; set; }
    
    	/// <summary>
    	/// 实际付费
    	/// </summary>
        public decimal? RealPayFee { get; set; }
    
    }
}
