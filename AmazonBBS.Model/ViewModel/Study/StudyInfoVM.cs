using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class StudyInfoVM
    {
        public string UnitName { get; set; }

        public int Index { get; set; }

        public Guid UnitId { get; set; }

        public List<ClassInfoVM> ClassInfoVMs { get; set; }
    }

    public class ClassInfoVM
    {
        /// <summary>
        /// 课时名称
        /// </summary>
        public string ClassName { get; set; }

        public int Index { get; set; }

        public Guid ClassId { get; set; }
    }
}
