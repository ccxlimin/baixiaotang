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
    /// 文章表
    /// </summary>
    public partial class Article
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public long ArticleId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long? UserID { get; set; }
    
    	/// <summary>
    	/// 文章标题
    	/// </summary>
        public string Title { get; set; }
    
    	/// <summary>
    	/// 文章内容
    	/// </summary>
        public string Body { get; set; }
    
    	/// <summary>
    	/// 浏览量
    	/// </summary>
        public int? PVCount { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime? CreateTime { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public int? IsDelete { get; set; }
    
    	/// <summary>
    	/// 需要审核（ 1  未审核  2 不需要审核/已审核 3 审核不通过 ）
    	/// </summary>
        public int? IsChecked { get; set; }
    
    	/// <summary>
    	/// 附件路径
    	/// </summary>
        public string FilePath { get; set; }
    
    	/// <summary>
    	/// 修改次数
    	/// </summary>
        public int? EditCount { get; set; }
    
    	/// <summary>
    	/// 更新时间
    	/// </summary>
        public System.DateTime? UpdateTime { get; set; }
    
    	/// <summary>
    	/// 更新人
    	/// </summary>
        public string UpdateUser { get; set; }
    
    	/// <summary>
    	/// 创建人
    	/// </summary>
        public string CreateUser { get; set; }
    
    	/// <summary>
    	/// 内容需要付费
    	/// </summary>
        public bool? ContentNeedPay { get; set; }
    
    	/// <summary>
    	/// 付费类型（10积分 20金钱）
    	/// </summary>
        public int? ContentFeeType { get; set; }
    
    	/// <summary>
    	/// 内容费用
    	/// </summary>
        public int? ContentFee { get; set; }
    
    	/// <summary>
    	/// 是否匿名发布
    	/// </summary>
        public bool IsAnonymous { get; set; }
    
    }
}
