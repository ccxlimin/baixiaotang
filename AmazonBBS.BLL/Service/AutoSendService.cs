using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.BLL
{
    public class AutoSendService : IAutoSendService
    {
        private readonly AmazonBBSDBContext _amazonBBSDBContext;
        private readonly INoticeService _noticeService;

        public AutoSendService(AmazonBBSDBContext amazonBBSDBContext, INoticeService noticeService)
        {
            _amazonBBSDBContext = amazonBBSDBContext;
            _noticeService = noticeService;
        }
        public Tuple<bool, string> AutoSendReply(long buyerUserId, int ordertype, long userGiftIdOrPartyId, long mainId)
        {
            var ok = false;
            var msg = string.Empty;

            bool isAutoSend = false;//是否自动发货

            var tran = _amazonBBSDBContext.Database.BeginTransaction();
            try
            {
                //判断有没有自动回复
                var autoReply = _amazonBBSDBContext.AutoReply.FirstOrDefault(a => a.OrderType == ordertype && a.ItemID == mainId);
                if (autoReply != null)
                {
                    var autoReplyItem = _amazonBBSDBContext.AutoReplyItem.Where(a => a.AutoReplyId == autoReply.AutoReplyId && a.IsDelete == 0 && !a.IsUsed).OrderBy(a => a.CreateTime).ThenBy(a => a.GroupId).GroupBy(a => a.GroupId).FirstOrDefault();
                    if (autoReplyItem != null)
                    {
                        msg = autoReply.Content;
                        autoReplyItem.ToList().ForEach(a =>
                        {
                            msg = msg.Replace($"#{a.ReplaceKey}#", a.ReplaceValue);
                            var item = _amazonBBSDBContext.AutoReplyItem.FirstOrDefault(auto => auto.AutoReplyItemId == a.AutoReplyItemId);
                            item.IsUsed = true;
                        });
                        ok = true;
                        isAutoSend = true;
                    }
                    else
                    {
                        //没有自动发货了
                        isAutoSend = false;
                        msg = "自动发货通知，本产品库存不足，无法自动发出货物，请联系相关产品的管理员进行发出！";
                        ok = true;
                    }
                    _noticeService.OnBuySuccess_Notice_AutoReply_Buyer(buyerUserId, msg);
                }
                else
                {
                    ok = true;
                }

                #region 添加发货状态
                var existOrderSend = _amazonBBSDBContext.OrderSend.FirstOrDefault(x =>
                  x.MainID == mainId
                  && x.MainType == ordertype
                  && x.UserGiftId == userGiftIdOrPartyId
                  && x.CreateUser == buyerUserId
                );
                if (existOrderSend == null)
                {
                    _amazonBBSDBContext.OrderSend.Add(new OrderSend()
                    {
                        MainID = mainId,
                        MainType = ordertype,
                        IsDelete = 0,
                        //OrderID = orderId,
                        UserGiftId = userGiftIdOrPartyId,
                        SendStatus = isAutoSend ? OrderSendEnumType.Sended.GetHashCode() : OrderSendEnumType.NoSend.GetHashCode(),
                        UpdateTime = DateTime.Now,
                        UpdateUser = buyerUserId,
                        CreateTime = DateTime.Now,
                        CreateUser = buyerUserId,
                    });
                }
                else
                {

                }
                #endregion

                #region 添加收货状态
                var existOrderCheck = _amazonBBSDBContext.OrderCheck.FirstOrDefault(x =>
                  x.CreateUser == buyerUserId
                  && x.UserGiftId == userGiftIdOrPartyId
                  && x.MainID == mainId
                  && x.MainType == ordertype);
                if (existOrderCheck == null)
                {
                    _amazonBBSDBContext.OrderCheck.Add(new OrderCheck
                    {
                        UpdateUser = buyerUserId,
                        UpdateTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                        CreateUser = buyerUserId,
                        CheckStatus = OrderCheckEnumType.NoCheck.GetHashCode(),
                        IsDelete = 0,
                        //OrderID = orderId,
                        UserGiftId = userGiftIdOrPartyId,
                        MainID = mainId,
                        MainType = ordertype
                    });
                }
                #endregion

                #region 如果没有自动发货，则后台通知管理员尽快发货
                if (!isAutoSend)
                {
                    _noticeService.OnUserBuySuccess_Notice_Master(buyerUserId, ordertype, mainId);
                }
                #endregion

                _amazonBBSDBContext.SaveChanges();
                tran.Commit();
            }
            catch (Exception e)
            {
                ErrorBLL.Instance.Log(e.ToString());
                tran.Rollback();
                ok = false;
            }
            return new Tuple<bool, string>(ok, msg);
        }
    }
}
