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
    /// 用户学习记录表
    /// </summary>
    public partial class UserStudy
    {
    	/// <summary>
    	/// 
    	/// </summary>
        public System.Guid UserStudyId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.Guid StudyUnitId { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.Guid StudyClassId { get; set; }
    
    	/// <summary>
    	/// 是否学完
    	/// </summary>
        public bool IsStudyed { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public long UserID { get; set; }
    
    	/// <summary>
    	/// 
    	/// </summary>
        public System.DateTime CreateTime { get; set; }
    
    }
}
