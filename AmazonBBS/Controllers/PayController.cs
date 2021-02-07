using AmazonBBS.BLL;
using AmazonBBS.Common;
using AmazonBBS.Model;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Util;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Controllers
{
    public class PayController : BaseController
    {
        public AmazonBBSDBContext DB;
        private readonly IAutoSendService _autoSendService;
        private readonly IOrderService _orderService;
        public PayController(IAutoSendService autoSendService, IOrderService orderService,
            AmazonBBSDBContext amazonBBSDBContext)
        {
            _autoSendService = autoSendService;
            _orderService = orderService;
            DB = amazonBBSDBContext;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region 跳转支付
        /// <summary>
        /// 跳转支付
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        public ActionResult PayRedirect()
        {
            string sessionName = "AliPayForm" + UserID.ToString();
            object form = SessionHelper.Get(sessionName);
            if (form == null)
            {
                return RedirectToAction("Index", "Home");
            }
            SessionHelper.Remove(sessionName);
            ViewBag.form = form.ToString();
            return View();
        }
        #endregion

        #region 1 充值 2 礼物 3 活动 4 数据  5 招聘 6 求职 7 产品服务 8课程
        #region 1 充值 2 礼物 3 活动 4 数据  5 招聘 6 求职 7 产品服务 8课程
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="fee"></param>
        /// <param name="desc"></param>
        /// <param name="id"></param>
        /// <param name="type">
        /// 1 充值 2 礼物 3 活动 4 数据  5 招聘 6 求职 7 产品服务 8课程
        /// 购买商品类型（1：充值==1时，id必为0）</param>
        /// <param name="count">购买份数</param>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult Pay(decimal fee, string desc, List<UserJoinItemViewModel> joinItems, long id = 0, int type = 0, int count = 1)
        {
            ResultInfo ri = new ResultInfo();
            int result = 0;
            string seq = string.Empty;
            string returnUri = string.Empty;
            JoinItemTypeEnum jointype = JoinItemTypeEnum.None;
            try
            {
                BeginTran();
                long buyItemID = 0;
                var now = DateTime.Now;
                #region 校验
                if (fee <= 0)
                {
                    ri.Msg = "金额不正常(如何支付为0的金额呢？)，请刷新页面重试";
                    return Result(ri);
                }

                if (type == 0)
                {
                    ri.Msg = "支付异常，请刷新页面重试";
                    return Result(ri);
                }
                if (type == 1)
                {
                    if (id == 0)
                    {
                        seq = "CZ";
                        returnUri = "http://www.baixiaotangtop.com/User";
                    }
                    else
                    {
                        ri.Msg = "支付异常，请刷新页面重试";
                        return Result(ri);
                    }
                }
                else
                {
                    if (id == 0)
                    {
                        ri.Msg = "支付异常，请刷新页面重试";
                        return Result(ri);
                    }
                    if (type == 2)
                    {
                        #region 礼物、课程、数据相关逻辑
                        //先根据ID获取费用信息
                        var feeInfo = GiftFeeBLL.Instance.GetModel(id);
                        if (feeInfo != null)
                        {
                            long giftId = GetRequest<long>("mid");
                            if (giftId != feeInfo.GiftID)
                            {
                                ri.Msg = "商品信息不正确！";
                                return Result(ri);
                            }

                            if (feeInfo.FeeType != 30)
                            {
                                ri.Msg = "商品价格信息有误！";
                                return Result(ri);
                            }
                            else if (feeInfo.Fee != fee)
                            {
                                ri.Msg = "商品价格被篡改，请刷新页面重新支付";
                                return Result(ri);
                            }

                            if (feeInfo.FeeCount > 0)
                            {
                                if (feeInfo.FeeCount >= count)
                                {
                                    id = giftId;
                                    returnUri = "http://www.baixiaotangtop.com/gift/detail/{0}".FormatWith(id);

                                    Gift gift = GiftBLL.Instance.GetModel(giftId);
                                    if (gift != null)
                                    {
                                        #region 创建购买商品信息
                                        string linkMan = GetRequest("LinkMan");
                                        string linktel = GetRequest("LinkTel");
                                        UserGift buygift = new UserGift()
                                        {
                                            BuyCount = count,
                                            BuyTime = now,
                                            BuyUserID = UserID,
                                            Fee = feeInfo.Fee * count,
                                            FeeType = feeInfo.FeeType,
                                            GiftFeeId = feeInfo.GiftFeeId,
                                            GiftID = giftId,
                                            GType = gift.GType,
                                            IsPay = 0,
                                        };
                                        if (linkMan.IsNotNullOrEmpty())
                                        {
                                            buygift.LinkMan = linkMan;
                                        }
                                        if (linktel.IsNotNullOrEmpty())
                                        {
                                            buygift.LinkTel = linktel;
                                        }
                                        if ((buyItemID = UserGiftBLL.Instance.Add(buygift, Tran)) < 1)
                                        {
                                            RollBack();
                                            ri.Msg = "创建订单失败！";
                                            return Result(ri);
                                        }
                                        if (gift.GType == 1)
                                        {
                                            seq = "LP";
                                            jointype = JoinItemTypeEnum.Gift;
                                        }
                                        else if (gift.GType == 2)
                                        {
                                            seq = "SJ";
                                            type = 4;
                                            jointype = JoinItemTypeEnum.DataAnalysis;
                                        }
                                        else
                                        {
                                            seq = "KC";
                                            type = 8;
                                            jointype = JoinItemTypeEnum.KeCheng;
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        ri.Msg = "商品不存在";
                                        return Result(ri);
                                    }
                                }
                                else
                                {
                                    ri.Msg = "当前余票不足{0}份，请重新选择数量！".FormatWith(count);
                                    return Result(ri);
                                }
                            }
                            else
                            {
                                ri.Msg = "该票种已无余票！请购买其它票种或联系主办方！";
                                return Result(ri);
                            }
                        }
                        else
                        {
                            ri.Msg = "商品不存在";
                            return Result(ri);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 活动相关逻辑
                        seq = "HD";
                        jointype = JoinItemTypeEnum.Party;
                        ActivityFee activityfee = ActivityFeeBLL.Instance.GetModel(id);
                        if (activityfee != null)
                        {
                            long activityId = GetRequest<long>("mid");
                            if (activityId != activityfee.ActivityId)
                            {
                                ri.Msg = "活动信息不正确！";
                                return Result(ri);
                            }
                            if (activityfee.FeeType != 30)
                            {
                                ri.Msg = "活动价格信息有误！";
                                return Result(ri);
                            }
                            else if (activityfee.Fee != fee)
                            {
                                ri.Msg = "活动价格被篡改，请刷新页面重新支付";
                                return Result(ri);
                            }

                            if (activityfee.FeeCount > 0)
                            {
                                if (activityfee.FeeCount >= count)
                                {
                                    #region 判断活动是否能够报名
                                    Activity party = ActivityBLL.Instance.GetModel(activityId);
                                    if (party != null && party.IsDelete == 0)
                                    {
                                        if (!ActivityBLL.Instance.CanJoinParty(party).Ok)
                                        {
                                            return Result(ri);
                                        }
                                    }
                                    else
                                    {
                                        ri.Msg = "活动不存在！";
                                        return Result(ri);
                                    }
                                    #endregion
                                    id = activityId;
                                    returnUri = "http://www.baixiaotangtop.com/party/detail/{0}".FormatWith(id);

                                    #region 创建报名活动信息

                                    string linkMan = GetRequest("LinkMan");
                                    string linktel = GetRequest("LinkTel");
                                    ActivityJoin ajoin = new ActivityJoin();
                                    ajoin.ActivityId = activityId;
                                    ajoin.FeeType = activityfee.FeeType;
                                    ajoin.IsFeed = 0;
                                    ajoin.RealPayFee = activityfee.Fee * count;
                                    ajoin.JoinCount = count;
                                    ajoin.JoinTime = now;
                                    ajoin.JoinUserID = UserID;
                                    ajoin.JoinUserName = UserInfo.UserName;
                                    ajoin.ActivityFeeId = activityfee.ActivityFeeId;
                                    if (linkMan.IsNotNullOrEmpty())
                                    {
                                        ajoin.LinkMan = linkMan;
                                    }
                                    if (linktel.IsNotNullOrEmpty())
                                    {
                                        ajoin.LinkTel = linktel;
                                    }
                                    if ((buyItemID = ActivityJoinBLL.Instance.Add(ajoin, Tran)) < 1)
                                    {
                                        RollBack();
                                        ri.Msg = "创建订单失败！";
                                        return Result(ri);
                                    }
                                }
                                else
                                {
                                    ri.Msg = "当前余票不足{0}份，请重新选择数量！".FormatWith(count);
                                    return Result(ri);
                                }
                            }
                            else
                            {
                                ri.Msg = "该票种已无余票！请购买其它票种或联系主办方！";
                                return Result(ri);
                            }
                            #endregion
                        }
                        else
                        {
                            ri.Msg = "活动不存在";
                            return Result(ri);
                        }
                        #endregion
                    }
                }
                #endregion

                #region 记录报名填写项
                bool insertOk = true;
                //记录报名填写项
                if (joinItems != null && joinItems.Count > 0)
                {
                    foreach (var join in joinItems)
                    {
                        JoinItemAnswerExt model = new JoinItemAnswerExt()
                        {
                            BuyerID = UserID,
                            CreateTime = now,
                            ItemAnswer = join.Value,
                            JoinItemQuestionExtId = join.Id,
                            JoinMainID = id,
                            JoinType = jointype.GetHashCode()
                        };
                        if (JoinItemAnswerExtBLL.Instance.Add(model, Tran) <= 0)
                        {
                            insertOk = false;
                            break;
                        }
                    }
                }
                if (!insertOk)
                {
                    RollBack();
                    ri.Msg = "创建订单失败！";
                    return Result(ri);
                }
                #endregion

                #region 创建订单
                string payOrderID = string.Empty;
                string _msg_ = _orderService.CreateOrder(UserID, fee, id, type, desc, seq, count, now, Tran, out result, out payOrderID);
                if (result < 1)
                {
                    ri.Msg = _msg_;
                    RollBack();
                    return Result(ri);
                }
                #endregion

                //记录用户购买单号
                SessionHelper.Set(payOrderID, buyItemID);

                #region 跳转支付
                ri = Redirect2Pay(desc, fee * count, payOrderID, returnUri, ConfigHelper.AppSettings("AliPayNotify"));
                if (ri.Ok)
                {
                    Commit();
                }
                else
                {
                    RollBack();
                }
                #endregion
            }
            catch
            {
                RollBack();
                ri.Msg = "支付异常，请重试！";
            }
            return Result(ri);
        }
        #endregion

        #region 支付宝回调
        //[HttpPost]
        public void AliPayNotify(string requestParams)
        {
            string result = PayCallBack(order =>
            {
                BeginTran();
                long payUserId = order.CreateUser.ToInt64();
                var orderPayTime = order.CreateTime.Value;
                //VIP分充值
                if (order.OrderType == 1)
                {
                    //VIP分充值
                    //获取充值比例
                    int vipFee = Convert.ToInt32(ConfigHelper.AppSettings("vipScorePayByRMB")) * Convert.ToInt32(order.Fee);
                    //order.Fee* vipscoreConfig
                    //vipscoreConfig = 10;
                    if (UserExtBLL.Instance.AddScore(payUserId, vipFee, 2))
                    {
                        //记录流水
                        //记录变更明细
                        ScoreCoinLog sclModel = new ScoreCoinLog()
                        {
                            Coin = vipFee,
                            CoinSource = CoinSourceEnum.VipScorePay.GetHashCode(),
                            CoinTime = DateTime.Now,
                            CoinType = 2,
                            CreateUser = "System",
                            UserID = payUserId,
                            UserName = UserInfo.UserName,
                        };
                        if (ScoreCoinLogBLL.Instance.Add(sclModel, Tran) > 0)
                        {
                            Commit();
                            NoticeBLL.Instance.OnPayVIPScoreSuccess_Notice_User(payUserId, orderPayTime, vipFee.ToString(), NoticeTypeEnum.PayVipScore);
                        }
                        else
                        {
                            RollBack();
                        }
                    }
                    else
                    {
                        RollBack();
                    }
                }
                else
                {
                    //参加活动、购买礼物等等
                    //更新相应商品的数量
                    if (UpdateCount(order, Tran))
                    {
                        object userBuyItemID = SessionHelper.Get(order.PayOrderID);
                        if (userBuyItemID == null)
                        {
                            if (order.OrderType == 2 || order.OrderType == 4 || order.OrderType == 8)
                            {
                                userBuyItemID = UserGiftBLL.Instance.GetUserBuyItemID(order.Fee, order.BuyCount, order.CreateUser, Tran);
                            }
                        }
                        var auto = _autoSendService.AutoSendReply(payUserId, order.OrderType.Value, (long)userBuyItemID, order.ItemID.Value);
                        if (auto.Item1)
                        {
                            Commit();
                            #region 通知
                            bool buyorjoin = false;
                            var noticeType = NoticeTypeEnum.None;
                            string mainName = string.Empty;
                            string mainUrl = GetDomainName;
                            if (order.OrderType == 3)
                            {
                                //活动
                                var model = ActivityBLL.Instance.GetModel(order.ItemID.Value);
                                mainName = model.Title;
                                noticeType = NoticeTypeEnum.Party_Join;
                                mainUrl += ConfigHelper.AppSettings("PartyDetail").FormatWith(order.ItemID);
                            }
                            else
                            {
                                //order.OrderType == 3 4 8 礼物 数据 课程
                                var model = GiftBLL.Instance.GetModel(order.ItemID.Value);
                                mainName = model.GiftName;
                                buyorjoin = true;
                                noticeType = order.OrderType == 2 ? NoticeTypeEnum.Gift_Buy : order.OrderType == 4 ? NoticeTypeEnum.DatAnalysis_Buy : NoticeTypeEnum.KeCheng_Buy;
                                mainUrl += ConfigHelper.AppSettings("GiftDetail").FormatWith(order.ItemID);
                            }
                            //通知用户
                            NoticeBLL.Instance.OnBuySuccess_Notice_Buyer(payUserId, orderPayTime, false, "{0}元".FormatWith(order.Fee), order.BuyCount.Value, mainUrl, mainName, buyorjoin, noticeType);
                            #endregion
                        }
                        else
                        {
                            RollBack();
                        }
                    }
                    else
                    {
                        RollBack();
                    }
                }
            }, requestParams);
            Response.Write(result);
        }

        private bool UpdateCount(BXTOrder order, SqlTransaction tran)
        {
            if (order.OrderType == 3)
            {
                //活动
                //插入相关报名成功信息
                var joinItem = ActivityJoinBLL.Instance.GetDetailJoinInfo(order.CreateUser.ToInt64(), order.ItemID.Value, tran);
                joinItem.IsFeed = 1;
                if (ActivityJoinBLL.Instance.Update(joinItem, tran).Ok)
                {
                    return ActivityFeeBLL.Instance.UpdateCount(order.BuyCount.Value, joinItem.ActivityFeeId.Value, tran);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //礼物/课程/数据
                //更新报名成功信息
                var buyItem = UserGiftBLL.Instance.GetDetailBuyInfo(order.CreateUser.ToInt64(), order.ItemID.Value, tran);
                buyItem.IsPay = 1;
                if (UserGiftBLL.Instance.Update(buyItem, tran).Ok)
                {
                    return GiftFeeBLL.Instance.UpdateCount(order.BuyCount.Value, buyItem.GiftFeeId.Value, tran);
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion
        #endregion

        #region 招聘相关
        #region 发布招聘支付
        [LOGIN]
        [HttpPost]
        public ActionResult ZhaoPinPay(ZhaoPin model)
        {
            ResultInfo ri = new ResultInfo();
            int result = 0;
            try
            {
                ResultInfo ri_ = UpLoadImg("JobPic", "/Content/Job/ZP");
                if (ri_.Ok)
                {
                    var now = DateTime.Now;
                    BeginTran();
                    model.JobPic = ri_.Url;
                    model.ValidTime = now.AddDays(GetRequest("deadTime", 30));
                    model.CreateTime = now;
                    model.UpdateTime = now;
                    model.UpdateUser = UserID.ToString();
                    model.Publisher = UserID.ToString();
                    model.IsDelete = 1;//先将状态置为失效
                    model.CLogo = UserInfo.HeadUrl;
                    model.IsTop = 0;
                    model.IsJinghua = 0;
                    model.IsRemen = 0;
                    model.PVCount = 0;

                    int itemid = ZhaoPinBLL.Instance.Add(model, Tran);
                    if (itemid > 0)
                    {
                        int publishScore = Convert.ToInt32(ConfigHelper.AppSettings("PUBLISH_ZHAOPIN"));
                        int vipscorePay = Convert.ToInt32(ConfigHelper.AppSettings("vipScorePayByRMB"));
                        int payMoney = publishScore / vipscorePay;
                        string payOrderID = string.Empty;
                        string desc = "招聘信息发布支付";
                        #region 创建订单
                        string _msg_ = _orderService.CreateOrder(UserID, payMoney, itemid, 5, desc, "ZP", 1, now, Tran, out result, out payOrderID);
                        if (result < 1)
                        {
                            ri.Msg = _msg_;
                            RollBack();
                            return Result(ri);
                        }
                        #endregion
                        #region 跳转支付
                        //, () =>
                        //  {
                        //      OrderDelete(result);
                        //      RollBack();
                        //  }
                        ri = Redirect2Pay(desc, payMoney, payOrderID,
                              "http://www.baixiaotangtop.com" + ConfigHelper.AppSettings("ZhaoPinDetail").FormatWith(result),
                              ConfigHelper.AppSettings("AliPayNotify_PublishZhaoPin"));
                        if (ri.Ok)
                        {
                            Commit();
                        }
                        else
                        {
                            RollBack();
                        }
                        #endregion
                    }
                    else
                    {
                        ri.Msg = "发布招聘信息失败";
                        RollBack();
                    }
                }
                else
                {
                    ri = ri_;
                }
            }
            catch
            {
                RollBack();
                ri.Msg = "支付异常，请重试！";
            }
            return Result(ri);
        }
        #endregion

        #region 招聘 支付宝回调
        [HttpPost]
        public void AliPayNotifyZhaoPin()
        {
            string result = PayCallBack(order =>
            {
                //成功回调
                ZhaoPin model = ZhaoPinBLL.Instance.GetZhaoPinBuyPay(order.ItemID.Value);
                model.IsDelete = 0;
                if (ZhaoPinBLL.Instance.Update(model).Ok)
                {
                    //通知
                    NoticeBLL.Instance.OnPayPublish_Notice_Author(order.CreateUser.ToInt64(), order.CreateTime.Value, GetDomainName + ConfigHelper.AppSettings("ZhaoPinDetail").FormatWith(order.ItemID), model.Gangwei, 30, order.Fee.ToString(), NoticeTypeEnum.ZhaoPin_Pay_Publish);
                }
            });
            Response.Write(result);
        }
        #endregion
        #endregion

        #region 求职相关
        #region 发布求职支付
        [LOGIN]
        [HttpPost]
        public ActionResult QiuZhiPay(QiuZhi model)
        {
            ResultInfo ri = new ResultInfo();
            int result = 0;
            try
            {
                ResultInfo ri_ = UpLoadImg("JianLiPic", "/Content/Job/QZ");
                if (ri_.Ok)
                {
                    var now = DateTime.Now;
                    BeginTran();
                    model.ValidTime = now.AddDays(GetRequest<int>("deadTime", 30));
                    model.JianLiPic = ri_.Url;
                    model.CreateTime = now;
                    model.Publisher = UserID.ToString();
                    model.IsDelete = 1;//先将状态置为失效
                    model.UpdateTime = now;
                    model.UpdateUser = UserID.ToString();
                    model.IsTop = 0;
                    model.IsJinghua = 0;
                    model.IsRemen = 0;
                    model.PVCount = 0;

                    int itemid = QiuZhiBLL.Instance.Add(model);
                    if (itemid > 0)
                    {
                        int publishScore = Convert.ToInt32(ConfigHelper.AppSettings("PUBLISH_QIUZHI"));
                        int vipscorePay = Convert.ToInt32(ConfigHelper.AppSettings("vipScorePayByRMB"));
                        int payMoney = publishScore / vipscorePay;

                        string payOrderID = string.Empty;
                        string desc = "求职信息发布支付";
                        #region 创建订单
                        string _msg_ = _orderService.CreateOrder(UserID, payMoney, itemid, 6, desc, "QZ", 1, now, Tran, out result, out payOrderID);
                        if (result < 1)
                        {
                            ri.Msg = _msg_;
                            RollBack();
                            return Result(ri);
                        }
                        #endregion

                        #region 跳转支付
                        ri = Redirect2Pay(desc, payMoney, payOrderID,
                                 "http://www.baixiaotangtop.com" + ConfigHelper.AppSettings("QiuZhiDetail").FormatWith(result),
                                 ConfigHelper.AppSettings("AliPayNotify_PublishQiuZhi"));
                        if (ri.Ok)
                        {
                            Commit();
                        }
                        else
                        {
                            RollBack();
                        }
                        #endregion
                    }
                    else
                    {
                        ri.Msg = "发布求职信息失败";
                        RollBack();
                    }
                }
                else
                {
                    ri = ri_;
                }
            }
            catch
            {
                //if (result > 0)
                //{
                //    OrderDelete(result);
                //}
                RollBack();
                ri.Msg = "支付异常，请重试！";
            }
            return Result(ri);
        }
        #endregion

        #region 求职 支付宝回调
        [HttpPost]
        public void AliPayNotifyQiuZhi()
        {
            string result = PayCallBack(order =>
            {
                //成功回调
                QiuZhi model = QiuZhiBLL.Instance.GetQiuZhiBuyPay(order.ItemID.Value);
                if (model != null)
                {
                    model.IsDelete = 0;
                    if (QiuZhiBLL.Instance.Update(model).Ok)
                    {
                        //通知
                        NoticeBLL.Instance.OnPayPublish_Notice_Author(order.CreateUser.ToInt64(), order.CreateTime.Value, GetDomainName + ConfigHelper.AppSettings("QiuZhiDetail").FormatWith(order.ItemID), model.IWant, 30, order.Fee.ToString(), NoticeTypeEnum.QiuZhi_Pay_Publish);
                    }
                }
            });
            Response.Write(result);
        }
        #endregion
        #endregion

        #region 产品服务相关
        #region 发布产品服务支付
        [LOGIN]
        [HttpPost]
        public ActionResult ProductPay(Model.Product model)
        {
            ResultInfo ri = new ResultInfo();
            int result = 0;
            try
            {
                ResultInfo ri_ = UpLoadImg("ProductPic", "/Content/Job/CP");
                if (ri_.Ok)
                {
                    BeginTran();
                    var now = DateTime.Now;
                    model.CreateTime = now;
                    model.CreateUser = UserID.ToString();
                    model.PLogo = UserInfo.HeadUrl;
                    model.IsDelete = 1;//先将状态置为失效

                    model.IsDelete = 0;
                    model.PLogo = UserInfo.HeadUrl;
                    model.ProductPic = ri_.Url;
                    model.IsTop = 0;
                    model.IsJinghua = 0;
                    model.IsRemen = 0;
                    model.PVCount = 0;
                    model.UpdateTime = now;
                    model.UpdateUser = UserID.ToString();
                    model.ValidTime = now.AddDays(GetRequest("deadTime", 30));

                    int itemid = ProductBLL.Instance.Add(model, Tran);
                    if (itemid > 0)
                    {
                        int publishScore = Convert.ToInt32(ConfigHelper.AppSettings("PUBLISH_PRODUCT"));
                        int vipscorePay = Convert.ToInt32(ConfigHelper.AppSettings("vipScorePayByRMB"));
                        int payMoney = publishScore / vipscorePay;

                        string payOrderID = string.Empty;
                        string desc = "产品服务信息发布支付";
                        #region 创建订单
                        string _msg_ = _orderService.CreateOrder(UserID, payMoney, itemid, 7, desc, "CP", 1, now, Tran, out result, out payOrderID);
                        if (result < 1)
                        {
                            ri.Msg = _msg_;
                            RollBack();
                            return Result(ri);
                        }
                        #endregion

                        #region 跳转支付
                        ri = Redirect2Pay(desc, payMoney, payOrderID,
                                  "http://www.baixiaotangtop.com" + ConfigHelper.AppSettings("ProductDetail").FormatWith(result),
                                  ConfigHelper.AppSettings("AliPayNotify_PublishProduct"));
                        if (ri.Ok)
                        {
                            Commit();
                        }
                        else
                        {
                            RollBack();
                        }
                        #endregion
                    }
                    else
                    {
                        ri.Msg = "发布产品信息失败";
                        RollBack();
                    }
                }
                else
                {
                    ri = ri_;
                }
            }
            catch
            {
                //if (result > 0)
                //{
                //    OrderDelete(result);
                //}
                RollBack();
                ri.Msg = "支付异常，请重试！";
            }
            return Result(ri);
        }
        #endregion

        #region 产品服务 支付宝回调
        [HttpPost]
        public void AliPayNotifyProduct()
        {
            string result = PayCallBack(order =>
            {
                //成功回调
                Model.Product model = ProductBLL.Instance.GetProductBuyPay(order.ItemID.Value);
                if (model != null)
                {
                    model.IsDelete = 0;
                    if (ProductBLL.Instance.Update(model).Ok)
                    {
                        //通知
                        NoticeBLL.Instance.OnPayPublish_Notice_Author(order.CreateUser.ToInt64(), order.CreateTime.Value, GetDomainName + ConfigHelper.AppSettings("ProductDetail").FormatWith(order.ItemID), model.PTitle, 30, order.Fee.ToString(), NoticeTypeEnum.Product_Pay_Publish);
                    }
                }
            });
            Response.Write(result);
        }
        #endregion
        #endregion

        #region 公共
        #region 公用回调
        private string PayCallBack(Action<BXTOrder> callback, string requestParams = null)
        {
            string result = "fail";
            StringBuilder sb = new StringBuilder();
            PayCBLog paylog = new PayCBLog();
            paylog.IsPay = 0;

            string out_trade_no = string.Empty;
            string total_amount = string.Empty;
            string receipt_amount = string.Empty;
            string trade_status = string.Empty;
            try
            {
                sb.AppendLine("开始获取参数组：");
                //string requestParams = Request.Form.ToString().Replace("?", string.Empty);
                sb.AppendLine(requestParams);
                //获取所有get请求的参数
                var parms = AliPayBLL.Instance.GetRequestGet(requestParams);
                sb.AppendLine(JsonHelper.ToJson(parms));
                //签名校验对比
                bool isSign = AlipaySignature.RSACheckV1(parms, AliPayConfig.alipay_public_key, AliPayConfig.charset, AliPayConfig.sign_type, false);
                if (Convert.ToDateTime(parms["notify_time"]) <= DateTime.Now.AddMonths(-10))
                {
                    return result;
                }
                if (isSign)
                {
                    sb.AppendLine("两者签名一致");
                    out_trade_no = parms["out_trade_no"];//获取商户订单号
                    total_amount = parms["total_amount"];//订单金额	
                    receipt_amount = parms["receipt_amount"];//实收金额	
                    trade_status = parms["trade_status"];//交易状态:交易支付成功-TRADE_SUCCESS  交易结束，不可退款-TRADE_FINISHED
                    sb.AppendLine("支付宝返回交易结果：商品订单号：{0}，商品订单金额：{1}，商品实收金额{2}，商品交易状态：{3}".FormatWith(out_trade_no, total_amount, receipt_amount, trade_status));

                    //根据商品订单号获取实体数据
                    BXTOrder order = BXTOrderBLL.Instance.Search(out_trade_no);
                    if (order.IsPay != 1)
                    {
                        if (trade_status == "TRADE_FINISHED" || trade_status == "TRADE_SUCCESS")
                        {
                            result = "success";
                            //记录支付成功
                            order.IsPay = 1;
                            paylog.IsPay = 1;
                            if (BXTOrderBLL.Instance.Update(order).Ok)
                            {
                                //回调
                                callback(order);
                            }
                        }
                    }
                    else
                    {
                        if (order.OrderType == 2 || order.OrderType == 4 || order.OrderType == 8)
                        {
                            result = "success";
                            callback(order);
                        }
                    }
                }
                else
                {
                    sb.AppendLine("两者签名不一致！");
                }
            }
            catch (Exception e)
            {
                sb.AppendLine("处理异常：" + e.ToString());
            }
            //防止多次回调插数据
            if (out_trade_no.IsNotNullOrEmpty() && PayCBLogBLL.Instance.Search(out_trade_no) == null)
            {
                paylog.CallBackTime = DateTime.Now;
                paylog.IsDelete = 0;
                paylog.Memo = sb.ToString();
                paylog.TradeNo = out_trade_no;
                paylog.TradeStatus = trade_status;
                PayCBLogBLL.Instance.Add(paylog);
            }
            return result;
        }
        #endregion

        #region 公用请求支付
        //private ResultInfo Redirect2Pay(string desc, decimal fee, string payOrderID, string returnUrl, string notifyUrl, Action errorCallBack)
        private ResultInfo Redirect2Pay(string desc, decimal fee, string payOrderID, string returnUrl, string notifyUrl)
        {
            ResultInfo ri = new ResultInfo();

            DefaultAopClient client = new DefaultAopClient(AliPayConfig.gatewayUrl, AliPayConfig.app_id, AliPayConfig.private_key, "json", "1.0", AliPayConfig.sign_type, AliPayConfig.alipay_public_key, AliPayConfig.charset, false);
            AlipayTradePagePayModel alipayModel = new AlipayTradePagePayModel();
            alipayModel.Body = desc;
            alipayModel.Subject = desc;
            alipayModel.TotalAmount = fee.ToString();
            alipayModel.OutTradeNo = payOrderID;
            alipayModel.ProductCode = "FAST_INSTANT_TRADE_PAY";

            AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();
            // 设置同步回调地址
            request.SetReturnUrl(returnUrl);
            // 设置异步通知接收地址
            request.SetNotifyUrl(notifyUrl);
            // 将业务model载入到request
            request.SetBizModel(alipayModel);

            AlipayTradePagePayResponse response = null;
            try
            {
                response = client.pageExecute(request, null, "post");
                ri.Ok = true;
                ri.Url = "/Pay/PayRedirect";
                Session["AliPayForm" + UserID.ToString()] = response.Body;
            }
            catch
            {
                ri.Msg = "支付过程中出现异常，请重试！";
                //errorCallBack();
            }
            return ri;
        }
        #endregion

        #region 公用创建订单
        //private string CreateOrder(decimal fee, long id, int type, string desc, string seq, int count, SqlTransaction tran, out int result, out string payorderid)
        //{
        //    result = 0;
        //    string msg = string.Empty;

        //    var bll = BXTOrderBLL.Instance;
        //    var now = DateTime.Now;
        //    payorderid = seq + id.ToString() + now.ToString("yyyyMMddHHmmssfff") + type.ToString();

        //    BXTOrder order = new BXTOrder()
        //    {
        //        Fee = fee * count,//计算总费用
        //        CreateTime = now,
        //        CreateUser = UserID.ToString(),
        //        IsDelete = 0,
        //        IsPay = 0,
        //        ItemID = id,
        //        OrderType = type,
        //        OrerDesc = desc,
        //        PayOrderID = payorderid,
        //        BuyCount = count,
        //    };
        //    if ((result = bll.Add(order, tran)) <= 0)
        //    {
        //        msg = "创建订单时失败，请重试！";
        //    }
        //    return msg;
        //}
        #endregion

        #region 公用订单删除
        //private void OrderDelete(int primaryId)
        //{
        //    var bll = BXTOrderBLL.Instance;
        //    BXTOrder model = bll.GetModel(primaryId);
        //    if (model != null)
        //    {
        //        model.IsDelete = 1;
        //        bll.Update(model);
        //    }
        //}
        #endregion
        #endregion

        #region 更新用户购买情况
        //public string UpdateBuyInfo()
        //{
        //    var giftTypeList = new[] { 2, 4, 8 }.ToList();
        //    List<object> errorList = new List<object>();
        //    var bxtOrders = DB.BXTOrder.OrderBy(x => x.CreateTime).ToList();
        //    foreach (var bxtOrder in bxtOrders)
        //    {
        //        var tradeNo = bxtOrder.PayOrderID;
        //        var payResult = TradeQuery("9812hrhy9/.~oijo", tradeNo);
        //        if (payResult.TradeStatus == "TRADE_SUCCESS"
        //            || payResult.TradeStatus == "TRADE_FINISHED")
        //        {
        //            var orderType = bxtOrder.OrderType;
        //            if (!orderType.HasValue)
        //            {
        //                errorList.Add(tradeNo);
        //                continue;
        //            }
        //            if (!giftTypeList.Contains(orderType.Value))
        //            {
        //                errorList.Add(tradeNo);
        //                continue;
        //            }

        //            var buyerUserId = bxtOrder.CreateUser.ToInt64();
        //            var payCbLog =
        //                DB.PayCBLog.FirstOrDefault(x => x.TradeNo == tradeNo);
        //            var userGifts = DB.UserGift
        //                .Where(x => x.BuyUserID == buyerUserId)
        //                .ToList()
        //                .OrderBy(x => x.BuyTime)
        //                .ToList();
        //            var userGift =
        //                userGifts.FirstOrDefault(x =>
        //                x.BuyTime.Value.ToString("yyyy-MM-dd HH")
        //                == bxtOrder.CreateTime.Value.ToString("yyyy-MM-dd HH"));
        //            if (userGift == null)
        //            {
        //                errorList.Add(tradeNo);
        //                continue;
        //            }

        //            var userGiftTime = userGift.BuyTime;
        //            var bxtOrderTime = bxtOrder.CreateTime;

        //            if (payCbLog == null && bxtOrder.IsPay == 0)
        //            {
        //                continue;
        //            }

        //            if (userGift.IsPay == 1 && payCbLog.IsPay == 1)
        //            {
        //                continue;
        //            }

        //            if (payCbLog.IsPay != 1)
        //            {
        //                payCbLog.IsPay = 1;
        //            }
        //            if (userGift.IsPay != 1)
        //            {
        //                userGift.IsPay = 1;
        //            }
        //        }
        //        else
        //        {
        //            errorList.Add(new
        //            {
        //                tradeNo,
        //                payResult = new
        //                {
        //                    payResult.SubMsg,
        //                    payResult.SubCode,
        //                    payResult.TradeStatus,
        //                    payResult.TradeNo
        //                }
        //            });
        //        }
        //    }

        //    DB.SaveChanges();

        //    return JsonHelper.ToJson(errorList);
        //}
        #endregion

        #region 支付宝交易查询
        ///// <summary>
        ///// 支付宝交易查询
        ///// </summary>
        ///// <param name="tradeNo"></param>
        ///// <param name="outTradeNo"></param>
        ///// <returns></returns>
        //public AlipayTradeQueryResponse TradeQuery(string key, string outTradeNo, string tradeNo = null)
        //{
        //    if (key != "9812hrhy9/.~oijo") return null;
        //    DefaultAopClient client = new DefaultAopClient(AliPayConfig.gatewayUrl, AliPayConfig.app_id, AliPayConfig.private_key, "json", "1.0", AliPayConfig.sign_type, AliPayConfig.alipay_public_key, AliPayConfig.charset, false);
        //    AlipayTradeQueryModel alipayTradeQueryModel = new AlipayTradeQueryModel();
        //    alipayTradeQueryModel.OutTradeNo = outTradeNo;
        //    alipayTradeQueryModel.TradeNo = tradeNo;
        //    AlipayTradeQueryRequest request = new AlipayTradeQueryRequest();
        //    request.SetBizModel(alipayTradeQueryModel);
        //    AlipayTradeQueryResponse response = null;
        //    try
        //    {
        //        response = client.Execute(request);
        //    }
        //    catch (Exception e)
        //    {

        //    }

        //    AlipayTradeFastpayRefundQueryModel alipayTradeFastpayRefundQueryModel = new AlipayTradeFastpayRefundQueryModel();
        //    alipayTradeFastpayRefundQueryModel.OutTradeNo = outTradeNo;
        //    alipayTradeFastpayRefundQueryModel.OutRequestNo = outTradeNo;
        //    AlipayTradeFastpayRefundQueryRequest alipayTradeFastpayRefundQueryRequest = new AlipayTradeFastpayRefundQueryRequest();
        //    alipayTradeFastpayRefundQueryRequest.SetBizModel(alipayTradeFastpayRefundQueryModel);
        //    var refundQueryResponse = client.Execute(alipayTradeFastpayRefundQueryRequest);

        //    return response;

        //    //return JsonHelper.ToJson(new
        //    //{
        //    //    api = "交易信息查询",
        //    //    status = response.TradeStatus == "TRADE_SUCCESS" ? "支付成功" : response.TradeStatus,
        //    //    response.TradeStatus,
        //    //    response.BuyerPayAmount,
        //    //    response,
        //    //    refundQueryResponse,
        //    //});
        //} 
        #endregion

        #region 更新通知
        public void UpdateNotice()
        {
            var notices = DB.Notice.ToList();
            CSharpCacheHelper.Set("notices", notices);
            var payCbLogs = DB.PayCBLog.OrderByDescending(x => x.CallBackTime).ToList();
            foreach (var payCbLog in payCbLogs)
            {
                var body =
                    //"{\"gmt_create" +
                    "gmt_create" +
                    payCbLog.Memo
                    .Replace("\r\n", "")
                    .Split(new string[] { "两者签名一致" }, StringSplitOptions.RemoveEmptyEntries)[0]
                    .Split(new[] { "开始获取参数组：" }, StringSplitOptions.RemoveEmptyEntries)[0]
                    .Split(new[] { "gmt_create" }, StringSplitOptions.RemoveEmptyEntries)[0]
                    .Replace("{\"", "");
                AliPayNotify(body);
                Thread.Sleep(10 * 1000);
            }
        }
        #endregion
    }
}