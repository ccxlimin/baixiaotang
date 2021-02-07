using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class GiftCreateViewModel
    {
        public List<GiftFee> GiftFees { get; set; }

        public List<JoinItemQuestionExt> JoinItemQues { get; set; }

        public Gift Gift { get; set; }
    }
}
