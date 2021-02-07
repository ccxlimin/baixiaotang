using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class GiftBuyerManageViewModel
    {
        public Gift GiftInfo { get; set; }

        public List<_BuyManageInfo> BuyList { get; set; }

        public List<JoinItemQuestionExt> JoinQuestions { get; set; }

        public List<JoinItemAnswerExt> JoinAnswers { get; set; }
    }
}
