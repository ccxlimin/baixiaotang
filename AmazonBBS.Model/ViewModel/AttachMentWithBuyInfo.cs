using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class AttachMentWithBuyInfo : AttachMent
    {
        public string FileIcon
        {
            get
            {
                var fileicon = FileName.Split('.').LastOrDefault();
                if (fileicon == "xlsx")
                {
                    fileicon = "xls";
                }
                else if (fileicon == "docx")
                {
                    fileicon = "doc";

                }
                else if (fileicon == "pptx")
                {
                    fileicon = "ppt";

                }
                else if ("jpg,png,gif,jpeg".Contains(fileicon))
                {
                    fileicon = "photo";
                }
                return fileicon;
            }
        }

        /// <summary>
        /// 是否已购买
        /// </summary>
        public bool IsBuy { get; set; }
    }
}
