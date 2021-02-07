using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 话题分类使用
    /// </summary>
    public class TagCategoryVM
    {
        //public BaseListViewModel<_QuestionInfo> QuestionTags { get; set; }

        //public BaseListViewModel<_Article> ArticleTags { get; set; }

        public List<_QuestionInfo> QuestionInfos { get; set; }
        public List<_Article> Articles { get; set; }

        public Paging Page { get; set; }
        /// <summary>
        /// 话题
        /// </summary>
        public string TagName { get; set; }

        public long TagID { get; set; }
    }
}
