using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class UserStudyCenterVM
    {
        public StudyUnit StudyUnit { get; set; }
        public List<StudyClassVM> StudyClasses { get; set; }
    }

    public class StudyClassVM
    {
        public StudyClass StudyClass { get; set; }

        /// <summary>
        /// 学习状态 null表示未开始学习 true 表示已学习 false 表示学习中 
        /// </summary>
        public bool? StudyStatus { get; set; }
    }
}
