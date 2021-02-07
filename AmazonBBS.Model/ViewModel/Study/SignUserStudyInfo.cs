using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 签到会员用户学习情况表
    /// </summary>
    public class SignUserStudyInfo
    {
        public string UserName { get; set; }

        public long Uid { get; set; }

        public int SignCountMonth { get; set; }

        public int SignCountTotal { get; set; }

        /// <summary>
        /// 当前学习课程
        /// </summary>
        public UserStudy CurrentStudyInfo { get; set; }

        /// <summary>
        /// 当前学习课程具体内容
        /// </summary>
        public StudyClass CurrentStudyClassInfo { get; set; }

        ///// <summary>
        ///// 学习内容
        ///// </summary>
        //public List<StudyClass> StudyClassName { get; set; }

        /// <summary>
        /// 学习进度 eg: 33.33%
        /// </summary>
        public string StudyRate { get; set; }

        ///// <summary>
        ///// 当前系列课程的所有排序，用于计算学习进度 
        ///// </summary>
        //public List<Guid> StudyClasses { get; set; }
    }
}
