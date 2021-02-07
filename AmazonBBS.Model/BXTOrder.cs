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
    /// 订单
    /// </summary>
    public partial class BXTOrder
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public long OrderID { get; set; }
    
    	/// <summary>
    	/// 支付订单号
    	/// </summary>
        public string PayOrderID { get; set; }
    
    	/// <summary>
    	/// 订单类型(1：充值 2：礼物订单 3：活动订单 4：数据订单)
    	/// </summary>
        public int? OrderType { get; set; }
    
    	/// <summary>
    	/// 商品号
    	/// </summary>
        public long? ItemID { get; set; }
    
    	/// <summary>
    	/// 订单金额
    	/// </summary>
        public decimal? Fee { get; set; }
    
    	/// <summary>
    	/// 订单描述
    	/// </summary>
        public string OrerDesc { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string CreateUser { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime? CreateTime { get; set; }
    
    	/// <summary>
    	/// 是否支付成功( 0未支付 1：支付成功 )
    	/// </summary>
        public int? IsPay { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public int? IsDelete { get; set; }
    
    	/// <summary>
    	/// 购买份数
    	/// </summary>
        public int? BuyCount { get; set; }
    
    }
}