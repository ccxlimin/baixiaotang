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
    /// 聊天记录
    /// </summary>
    public partial class Chat
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public long ChatID { get; set; }
    
    	/// <summary>
    	/// 发信人
    	/// </summary>
        public long? FromID { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string FromUserName { get; set; }
    
    	/// <summary>
    	/// 收信人
    	/// </summary>
        public long? ToID { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public string ToUserName { get; set; }
    
    	/// <summary>
    	/// 消息
    	/// </summary>
        public string Message { get; set; }
    
    	/// <summary>
    	/// 消息发送时间
    	/// </summary>
        public System.DateTime? SendTime { get; set; }
    
    	/// <summary>
    	/// 是否已读
    	/// </summary>
        public int? IsRead { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime? ReadTime { get; set; }
    
    	/// <summary>
    	/// 是否批量发送消息
    	/// </summary>
        public bool? Batch { get; set; }
    
    }
}