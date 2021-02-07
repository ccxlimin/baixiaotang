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
    /// 评论表
    /// </summary>
    public partial class Comment
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public long CommentId { get; set; }
    
    	/// <summary>
    	/// 评论对象类型(1贴吧 2文章 3活动 4礼物 5招聘 6求职 7产品服务 8数据分析 9课程)
    	/// </summary>
        public int? MainType { get; set; }
    
    	/// <summary>
    	/// 指定该评论所属(如贴吧、文章、招聘、求职等)
    	/// </summary>
        public long? MainID { get; set; }
    
    	/// <summary>
    	/// 评论者
    	/// </summary>
        public long? CommentUserID { get; set; }
    
    	/// <summary>
    	/// 评论内容
    	/// </summary>
        public string CommentContent { get; set; }
    
    	/// <summary>
    	/// 是否设置显示或隐藏指定可见范围(1：需要消费查看 0：不需要，可直接查看评论内容）
    	/// </summary>
        public int? IsHideOrFeeToSee { get; set; }
    
    	/// <summary>
    	/// 查看评论消费类型(1 积分 2金钱）
    	/// </summary>
        public int? FeeCoinType { get; set; }
    
    	/// <summary>
    	/// 可见需要数值
    	/// </summary>
        public int? NeedCoin { get; set; }
    
    	/// <summary>
    	/// 评论还是回复(1 评论 2 回复)
    	/// </summary>
        public int? CommentOrReplyType { get; set; }
    
    	/// <summary>
    	/// 回复的最顶层的评论ID
    	/// </summary>
        public long? ReplyTopCommentId { get; set; }
    
    	/// <summary>
    	/// 回复谁
    	/// </summary>
        public long? ReplyToUserID { get; set; }
    
    	/// <summary>
    	/// 回复谁的那条评论的ID
    	/// </summary>
        public long? ReplyToCommentID { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime? CreateTime { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string CreateUser { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime? UpdateTime { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string UpdateUser { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public int? IsDelete { get; set; }
    
    	/// <summary>
    	/// 是否匿名发布
    	/// </summary>
        public bool IsAnonymous { get; set; }
    
    }
}