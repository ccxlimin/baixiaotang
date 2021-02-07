using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class PartyJoinManageViewModel
    {
        public Activity PartyInfo { get; set; }

        public List<_JoinManageInfo> JoinList { get; set; }

        public List<JoinItemQuestionExt> JoinQuestions { get; set; }

        public List<JoinItemAnswerExt> JoinAnswers { get; set; }
    }
}
