using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.BLL
{
    public interface IAutoSendService
    {
        /// <summary>
        /// 自动发货
        /// </summary>
        /// <param name="ordertype"></param>
        /// <returns></returns>
        Tuple<bool, string> AutoSendReply(long buyerUserId, int ordertype, long userGiftIdOrPartyId, long mainId);
    }
}
