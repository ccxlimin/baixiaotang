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
    /// 系统通知
    /// </summary>
    public partial class Notice
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public long NoticeId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string NoticeTitle { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string NoticeContent { get; set; }
    
    	/// <summary>
    	/// 通知类型( 1 关注用户的 发布问题 10 自己问题的新回答 11 关注的帖子的新回答 12 自己的问题被点赞 13 自己的问题回答被回复 2 关注用户的 发布文章 20 自己文章的新回答 21 关注的文章的新回答 22 自己的文章被点赞 23 自己的文章回答被回复 3 关注用户的 发布招聘 4 关注用户的 发布求职 5 关注用户的 发布产品 6 关注用户的 发布活动 7 关注用户 8 关注用户的 新回答 )
    	/// </summary>
        public int? NoticeType { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long? ToUserID { get; set; }
    
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
        public System.DateTime? CreateTime { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public int? IsDelete { get; set; }
    
    }
}
