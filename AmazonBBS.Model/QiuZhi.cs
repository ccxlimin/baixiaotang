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
    /// 人才求职
    /// </summary>
    public partial class QiuZhi
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public long QiuZhiID { get; set; }
    
    	/// <summary>
    	/// 发布人
    	/// </summary>
        public string Publisher { get; set; }
    
    	/// <summary>
    	/// 求职意向
    	/// </summary>
        public string IWant { get; set; }
    
    	/// <summary>
    	/// 求职薪资
    	/// </summary>
        public int? Money { get; set; }
    
    	/// <summary>
    	/// 目前岗位
    	/// </summary>
        public string NowWork { get; set; }
    
    	/// <summary>
    	/// 离职状态(1-在职 2-离职中 3-已离职)
    	/// </summary>
        public int? WorkStatus { get; set; }
    
    	/// <summary>
    	/// 学历（1不限2高中3中专4大专5本科6研究生7博士）
    	/// </summary>
        public int? Study { get; set; }
    
    	/// <summary>
    	/// 工作年限
    	/// </summary>
        public int? WorkYear { get; set; }
    
    	/// <summary>
    	/// 自我简介
    	/// </summary>
        public string MyDesc { get; set; }
    
    	/// <summary>
    	/// 联系方式
    	/// </summary>
        public string Contact { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime? CreateTime { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public int? IsDelete { get; set; }
    
    	/// <summary>
    	/// 自我评价
    	/// </summary>
        public string SelfAssessment { get; set; }
    
    	/// <summary>
    	/// 照片简历
    	/// </summary>
        public string JianLiPic { get; set; }
    
    	/// <summary>
    	/// 文档简历
    	/// </summary>
        public string JianLiWord { get; set; }
    
    	/// <summary>
    	/// 是否支付
    	/// </summary>
        public int? IsPay { get; set; }
    
    	/// <summary>
    	/// 支付方式
    	/// </summary>
        public int? PayType { get; set; }
    
    	/// <summary>
    	/// 浏览量
    	/// </summary>
        public long? PVCount { get; set; }
    
    	/// <summary>
    	/// 精华
    	/// </summary>
        public int? IsJinghua { get; set; }
    
    	/// <summary>
    	/// 热门
    	/// </summary>
        public int? IsRemen { get; set; }
    
    	/// <summary>
    	/// 置顶
    	/// </summary>
        public int? IsTop { get; set; }
    
    	/// <summary>
    	/// 求职类型
    	/// </summary>
        public int? WorkType { get; set; }
    
    	/// <summary>
    	/// 兼职周期
    	/// </summary>
        public string WorkTime { get; set; }
    
    	/// <summary>
    	/// 所属行业
    	/// </summary>
        public System.Guid? BelongJobTrade { get; set; }
    
    	/// <summary>
    	/// 所属岗位
    	/// </summary>
        public System.Guid? BelongJob { get; set; }
    
    	/// <summary>
    	/// 意向工作地点
    	/// </summary>
        public string IWantPlace { get; set; }
    
    	/// <summary>
    	/// 有效时间
    	/// </summary>
        public System.DateTime? ValidTime { get; set; }
    
    	/// <summary>
    	/// 更新时间
    	/// </summary>
        public System.DateTime? UpdateTime { get; set; }
    
    	/// <summary>
    	/// 更新人
    	/// </summary>
        public string UpdateUser { get; set; }
    
    }
}
