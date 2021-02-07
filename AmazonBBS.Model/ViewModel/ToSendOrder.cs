using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    public class ToSendOrder
    {
        public OrderSend OrderSend { get; set; }

        public UserBase BuyerInfo { get; set; }
        public UserGift UserGiftInfo { get; set; }
        public Gift GiftInfo { get; set; }

        public Express Express { get; set; }

        /// <summary>
        /// 是否自动发货
        /// </summary>
        public bool IsAutoSend { get; set; }
    }
}
