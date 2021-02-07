using AmazonBBS.BLL;
using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Controllers
{
    public class GiftController : BaseController
    {
        private readonly INoticeService _noticeService;
        private readonly IAutoSendService _autoSendService;
        private readonly IOrderService _orderService;
        public GiftController(INoticeService noticeService, IAutoSendService autoSendService, IOrderService orderService)
        {
            _noticeService = noticeService;
            _autoSendService = autoSendService;
            _orderService = orderService;
        }

        #region Index
        public ActionResult Index(int id = 1)
        {
            var model = GetGiftLists(id);
            return View(model);
        }

        public ActionResult LoadMore(int id)
        {
            var model = GetGiftLists(id);
            return PartialView("_Search", model);
        }

        private GiftViewModel GetGiftLists(int id)
        {
            ViewBag.ID = id;
            GiftViewModel model = new GiftViewModel();
            model.GiftPage = InitPage(12);
            model.Gifts = GiftBLL.Instance.GetAllGifts(model.GiftPage, id, sortConfig: ConfigHelper.AppSettings(id == 1 ? "SortConfig_Gift" : id == 2 ? "SortConfig_Data" : "SortConfig_KeCheng"));
            return model;
        }
        #endregion

        #region 明细
        public ActionResult Detail(long id = 0)
        {
            if (id > 0)
            {
                Paging page = InitPage();
                _Gift gift = GiftBLL.Instance.GetGiftDetail(id, UserID, page, CommentEnumType.Gift, PriseEnumType.GiftComment, JoinItemTypeEnum.Gift);
                if (gift == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    if (gift.GType == 1)
                    {
                        return View(gift);
                    }
                    else if (gift.GType == 2)
                    {
                        return RedirectToAction("Detail", "DataAnalysis", new { id });
                    }
                    else
                    {
                        return RedirectToAction("Detail", "KeCheng", new { id });
                    }
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region 新增
        [IsMaster]
        public ActionResult Create(int id = 1)
        {
            ViewBag.ID = id;
            return View();
        }

        /// <summary>
        /// 创建礼物
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [IsMaster]
        [HttpPost]
        public ActionResult Create(GiftCreateViewModel model)
        {
            ResultInfo ri = new ResultInfo();
            if (ModelState.IsValid)
            {
                try
                {
                    var gift = model.Gift;

                    NoticeTypeEnum noticeType;
                    string desc = string.Empty;
                    int joinItemType = 2;

                    if (gift.GType == 1)
                    {
                        desc = "礼物";
                        noticeType = NoticeTypeEnum.Gift_Add;
                        joinItemType = 2;
                    }
                    else if (gift.GType == 2)
                    {
                        desc = "数据";
                        noticeType = NoticeTypeEnum.DataAnalysis_Add;
                        joinItemType = 4;
                    }
                    else
                    {
                        desc = "课程";
                        noticeType = NoticeTypeEnum.KeCheng_Add;
                        joinItemType = 3;
                    }
                    if (!string.IsNullOrEmpty(gift.GiftName))
                    {
                        if (!string.IsNullOrEmpty(gift.GiftDesc))
                        {
                            if (!string.IsNullOrEmpty(gift.GiftInfo))
                            {
                                BeginTran();

                                gift.GiftInfo = HttpUtility.UrlDecode(gift.GiftInfo);
                                gift.GiftCreateUserID = UserID;
                                gift.GiftCreateTime = DateTime.Now;
                                gift.PVCount = 0;
                                gift.IsDelete = 0;
                                gift.OpenJoinItem = gift.OpenJoinItem ?? 0;

                                int result = GiftBLL.Instance.Add(gift, Tran);
                                if (result > 0)
                                {
                                    var finalFees = model.GiftFees.Where(item => { return item.Fee.HasValue && item.FeeCount.HasValue && item.FeeType.HasValue && item.FeeName.IsNotNullOrEmpty(); });
                                    //处理价格信息
                                    foreach (var item in finalFees)
                                    {
                                        item.GiftID = result;
                                        item.FeeType = item.FeeType ?? 0;
                                        if (GiftFeeBLL.Instance.Add(item, Tran) < 1)
                                        {
                                            result = 0;
                                            break;
                                        }
                                    }
                                    if (result > 0)
                                    {
                                        if (gift.OpenJoinItem == 1)
                                        {
                                            #region 添加活动报名填写项
                                            if (model.JoinItemQues != null)
                                            {
                                                var finalJoins = model.JoinItemQues.Where(item => { return item.ItemName.IsNotNullOrEmpty(); });
                                                foreach (var item in finalJoins)
                                                {
                                                    item.IsDelete = 0;
                                                    item.CreateTime = DateTime.Now;
                                                    item.UpdateTime = DateTime.Now;
                                                    item.CreateUser = UserID.ToString();
                                                    item.UpdateUser = UserID.ToString();
                                                    item.IsMustWrite = 1;
                                                    item.MainID = result;
                                                    item.MainType = joinItemType;
                                                    if (JoinItemQuestionExtBLL.Instance.Add(item, Tran) < 1)
                                                    {
                                                        result = 0;
                                                        break;
                                                    }
                                                }
                                            }
                                            #endregion
                                        }

                                        if (result > 0)
                                        {
                                            Commit();
                                            string uri = $"/gift/detail/{result}";
                                            ri.Ok = true;
                                            ri.Msg = "{0}创建成功".FormatWith(desc);
                                            ri.Url = uri;

                                            //通知
                                            NoticeBLL.Instance.OnAdd_Notice_Liker(UserInfo.UserName, UserID, uri, gift.GiftName, noticeType, GetDomainName);
                                        }
                                        else
                                        {
                                            RollBack();
                                            ri.Msg = "{0}创建失败".FormatWith(desc);
                                        }
                                    }
                                    else
                                    {
                                        RollBack();
                                        ri.Msg = "{0}创建失败".FormatWith(desc);
                                    }
                                }
                                else
                                {
                                    RollBack();
                                    ri.Msg = "{0}创建失败".FormatWith(desc);
                                }
                            }
                            else
                            {
                                ri.Msg = "{0}详细介绍不能为空".FormatWith(desc);
                            }
                        }
                        else
                        {
                            ri.Msg = "{0}简介不能为空".FormatWith(desc);
                        }
                    }
                    else
                    {
                        ri.Msg = "{0}名称不能为空".FormatWith(desc);
                    }
                }
                catch (Exception e)
                {

                }
            }
            return Result(ri);
        }
        #endregion

        #region 购买礼物
        /// <summary>
        /// 购买礼物
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [LOGIN]
        public ActionResult Submit(long id, int fee, long feeid, int count, string LinkMan, string LinkTel, List<UserJoinItemViewModel> JoinItems)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                //获取礼物详情
                var model = GiftBLL.Instance.GetDetail(id, UserID, JoinItemTypeEnum.Gift, false);
                if (model != null)
                {
                    BeginTran();

                    #region 获取购买费用
                    List<GiftFee> feelist = model.FeeList;
                    var feeInfo = feelist.FirstOrDefault(a => { return a.GiftFeeId == feeid && a.Fee == fee; });
                    #endregion
                    if (feeInfo != null)
                    {
                        if (feeInfo.FeeCount > 0)
                        {
                            if (feeInfo.FeeCount >= count)
                            {
                                //判断是否需要扣费
                                int coinType = feeInfo.FeeType == 10 ? 1 : feeInfo.FeeType == 20 ? 2 : 0;
                                int coin = Convert.ToInt32(feeInfo.Fee.Value) * count;

                                bool canJoin = true;
                                if (fee != 0)
                                {
                                    //判断积分是否足够
                                    canJoin = UserExtBLL.Instance.HasEnoughCoin(coinType, coin, UserID);
                                }
                                if (canJoin)
                                {
                                    UserGift buyModel = new UserGift()
                                    {
                                        BuyCount = count,
                                        BuyTime = DateTime.Now,
                                        BuyUserID = UserID,
                                        Fee = coin,
                                        FeeType = feeInfo.FeeType,
                                        GiftFeeId = feeInfo.GiftFeeId,
                                        GiftID = model.GiftID,
                                        GType = model.GType,
                                        IsPay = 1,
                                    };
                                    if (LinkMan.IsNotNullOrEmpty())
                                    {
                                        buyModel.LinkMan = LinkMan;
                                    }
                                    if (LinkTel.IsNotNullOrEmpty())
                                    {
                                        buyModel.LinkTel = LinkTel;
                                    }
                                    var usergiftId = 0;
                                    if ((usergiftId = UserGiftBLL.Instance.Add(buyModel, Tran)) > 0)
                                    {
                                        bool insertOk = true;
                                        if (JoinItems != null && JoinItems.Count > 0)
                                        {
                                            foreach (var join in JoinItems)
                                            {
                                                JoinItemAnswerExt joinModel = new JoinItemAnswerExt()
                                                {
                                                    BuyerID = UserID,
                                                    CreateTime = DateTime.Now,
                                                    ItemAnswer = join.Value,
                                                    JoinItemQuestionExtId = join.Id,
                                                    JoinMainID = model.GiftID,
                                                    JoinType = (model.GType == 1 ? JoinItemTypeEnum.Gift : model.GType == 2 ? JoinItemTypeEnum.DataAnalysis : JoinItemTypeEnum.KeCheng).GetHashCode()
                                                };
                                                if (JoinItemAnswerExtBLL.Instance.Add(joinModel, Tran) <= 0)
                                                {
                                                    insertOk = false;
                                                    break;
                                                }
                                            }
                                        }
                                        if (insertOk)
                                        {
                                            //更新剩余名额
                                            feeInfo.FeeCount -= count;
                                            if (GiftFeeBLL.Instance.Update(feeInfo, Tran).Ok)
                                            {
                                                var ordertype = (model.GType == 1 ? OrderEnumType.Gift : model.GType == 2 ? OrderEnumType.Data : OrderEnumType.KeCheng).GetHashCode();

                                                string orderDesc = GetRequest("desc", "购买" + model.GiftName);
                                                //添加订单
                                                //int _orderResult = 0;
                                                //string _orderNo = string.Empty;
                                                //var addOrder = _orderService.CreateOrder(UserID, fee, id, ordertype, orderDesc, model.GType == 1 ? "LP" : model.GType == 2 ? "SJ" : "KC", count, Tran, out _orderResult, out _orderNo);
                                                //if (_orderResult > 0)
                                                //{
                                                if (fee == 0)
                                                {
                                                    var auto = _autoSendService.AutoSendReply(UserID, ordertype, usergiftId, id);
                                                    if (auto.Item1)
                                                    {
                                                        Commit();
                                                        ri.Msg = "购买成功，待发货产品将由管理员发出，请在个人中心查看我的订单";
                                                        ri.Ok = true;
                                                        ri.Data = auto.Item2;
                                                    }
                                                    else
                                                    {
                                                        ri.Msg = "购买失败";
                                                        RollBack();
                                                    }
                                                }
                                                else
                                                {
                                                    //购买成功
                                                    //扣除相应数据
                                                    //用户帐户减去相应的积分或金钱
                                                    if (UserExtBLL.Instance.SubScore(UserID, coin, coinType, Tran))
                                                    {
                                                        if (ScoreCoinLogBLL.Instance.Log(-coin, coinType, CoinSourceEnum.BuyGift, UserID, UserInfo.UserName, Tran))
                                                        {
                                                            var auto = _autoSendService.AutoSendReply(UserID, ordertype, usergiftId, id);
                                                            if (auto.Item1)
                                                            {
                                                                Commit();
                                                                ri.Ok = true;
                                                                ri.Msg = "购买成功，待发货产品将由管理员发出，请在个人中心查看我的订单";
                                                                ri.Data = auto.Item2;
                                                            }
                                                            else
                                                            {
                                                                ri.Msg = "购买失败";
                                                                RollBack();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            ri.Msg = "购买失败";
                                                            RollBack();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ri.Msg = "购买失败";
                                                        RollBack();
                                                    }
                                                }
                                                if (ri.Ok)
                                                {
                                                    //通知
                                                    NoticeBLL.Instance.OnBuySuccess_Notice_Buyer(UserID, DateTime.Now, fee == 0 ? true : false, "{0}{1}".FormatWith(coin, feeInfo.FeeType == 10 ? "积分" : feeInfo.FeeType == 20 ? "VIP分" : string.Empty), count, ConfigHelper.AppSettings("GiftDetail").FormatWith(id), model.GiftName, true, NoticeTypeEnum.Gift_Buy);
                                                }
                                                //}
                                                //else
                                                //{
                                                //    RollBack();
                                                //    ri.Msg = addOrder;
                                                //}
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
                                        RollBack();
                                    }
                                }
                                else
                                {
                                    ri.Msg = "你的积分不足以购买此礼物";
                                }
                            }
                            else
                            {
                                ri.Type = 1;
                                ri.Msg = "当前余票不足{0}份，请重新选择数量！".FormatWith(count);
                            }
                        }
                        else
                        {
                            ri.Msg = "该票种已无余票！请购买其它票种或联系主办方！";
                        }
                    }
                    else
                    {
                        ri.Msg = "费用异常！";
                    }
                }
                else
                {
                    ri.Msg = "该礼物不存在";
                }
            }
            return Result(ri);
        }
        #endregion

        #region 页面浏览量
        /// <summary>
        /// 页面浏览量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PV(long id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                if (GiftBLL.Instance.PVCount(id))
                {
                    ri.Ok = true;
                    ri.Msg = "记录成功";
                }
            }
            return Result(ri);
        }
        #endregion

        #region 编辑
        [IsMaster]
        public ActionResult Edit(long id)
        {
            if (id > 0)
            {
                int type = 0;
                type = GiftBLL.Instance.GetModel(id).GType.Value;
                ViewBag.ID = type;
                GiftCreateViewModel model = GiftBLL.Instance.GetEditDetail(id, type == 1 ? JoinItemTypeEnum.Gift : type == 2 ? JoinItemTypeEnum.DataAnalysis : JoinItemTypeEnum.KeCheng);
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [IsMaster]
        [HttpPost]
        public ActionResult Edit(GiftCreateViewModel model)
        {
            ResultInfo ri = new ResultInfo();
            if (model.Gift.GiftID == model.GiftFees[0].GiftID)
            {
                Gift _model = GiftBLL.Instance.GetModel(model.Gift.GiftID);
                if (_model != null)
                {
                    if (_model.IsDelete == 0)
                    {
                        BeginTran();

                        _model.GiftName = model.Gift.GiftName;
                        _model.GiftDesc = model.Gift.GiftDesc;
                        _model.GiftImgs = model.Gift.GiftImgs;
                        _model.GiftInfo = HttpUtility.UrlDecode(model.Gift.GiftInfo);

                        _model.OpenJoinItem = model.Gift.OpenJoinItem;

                        if (GiftBLL.Instance.Update(_model, Tran).Ok)
                        {
                            var finalFees = model.GiftFees.Where(item => { return item.Fee.HasValue && item.FeeCount.HasValue && item.FeeType.HasValue && item.FeeName.IsNotNullOrEmpty(); });
                            //更新费用
                            foreach (GiftFee fee in finalFees)
                            {
                                if (fee.GiftFeeId > 0)
                                {
                                    if (fee.GiftID == _model.GiftID)
                                    {
                                        //更新
                                        var feeModel = GiftFeeBLL.Instance.GetModel(fee.GiftFeeId);
                                        feeModel.FeeType = fee.FeeType;
                                        feeModel.FeeName = fee.FeeName;
                                        feeModel.Fee = fee.Fee;
                                        feeModel.FeeCount = fee.FeeCount;
                                        feeModel.GiftID = _model.GiftID;

                                        if (GiftFeeBLL.Instance.Update(feeModel, Tran).Ok)
                                        {
                                            ri.Ok = true;
                                        }
                                        else
                                        {
                                            ri.Ok = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        ri.Ok = false;
                                        ri.Msg = "该费用列表异常！";
                                        break;
                                    }
                                }
                                else
                                {
                                    fee.GiftID = model.Gift.GiftID;
                                    if (GiftFeeBLL.Instance.Add(fee, Tran) > 0)
                                    {
                                        ri.Ok = true;
                                    }
                                    else
                                    {
                                        ri.Ok = false;
                                        break;
                                    }
                                }
                            }
                            if (ri.Ok)
                            {
                                //更新报名填写项
                                if (model.JoinItemQues != null)
                                {
                                    var finalJoins = model.JoinItemQues.Where(item => { return item.ItemName.IsNotNullOrEmpty(); });
                                    foreach (var item in finalJoins)
                                    {
                                        if (item.JoinItemQuestionExtId > 0)
                                        {
                                            if (item.MainID == _model.GiftID)
                                            {
                                                //更新
                                                var joinItem = JoinItemQuestionExtBLL.Instance.GetModel(item.JoinItemQuestionExtId);
                                                joinItem.ItemName = item.ItemName;
                                                joinItem.UpdateTime = DateTime.Now;
                                                joinItem.UpdateUser = UserID.ToString();
                                                if (JoinItemQuestionExtBLL.Instance.Update(joinItem, Tran).Ok)
                                                {
                                                    ri.Ok = true;
                                                }
                                                else
                                                {
                                                    ri.Ok = false;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                ri.Ok = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            item.MainID = model.Gift.GiftID;
                                            item.MainType = (_model.GType == 1 ? JoinItemTypeEnum.Gift : _model.GType == 2 ? JoinItemTypeEnum.DataAnalysis : JoinItemTypeEnum.KeCheng).GetHashCode();
                                            item.CreateTime = DateTime.Now;
                                            item.CreateUser = UserID.ToString();
                                            item.UpdateTime = DateTime.Now;
                                            item.UpdateUser = UserID.ToString();
                                            item.IsDelete = 0;
                                            item.IsMustWrite = 1;
                                            if (JoinItemQuestionExtBLL.Instance.Add(item, Tran) > 0)
                                            {
                                                ri.Ok = true;
                                            }
                                            else
                                            {
                                                ri.Ok = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (ri.Ok)
                                {
                                    ri.Msg = "编辑成功";
                                    Commit();
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
                    }
                    else
                    {
                        ri.Msg = "该商品已被删除，编辑失败";
                    }
                }
                else
                {
                    ri.Msg = "异常";
                }
            }
            else
            {
                ri.Msg = "异常";
            }
            return Result(ri);
        }
        #endregion

        #region 删除
        [IsMaster]
        [HttpPost]
        public ActionResult Delete(long id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                Gift model = GiftBLL.Instance.GetModel(id);
                if (model != null)
                {
                    if (model.IsDelete == 0)
                    {
                        model.IsDelete = 1;
                        ri = GiftBLL.Instance.Update(model);
                        if (ri.Ok) { ri.Msg = "删除成功"; }
                    }
                    else
                    {
                        ri.Ok = true;
                        ri.Msg = "该商品已被删除，请勿重复操作";
                    }
                }
                else
                {
                    ri.Msg = "未查找到该商品，可能已被删除，请刷新页面重试";
                }
            }
            else
            {
                ri.Msg = "异常";
            }

            return Result(ri);
        }
        #endregion

        #region 留言
        /// <summary>
        /// 留言
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult Leave(string Telephone, string Name, int Age, string diy1, string diy2, string diy3)
        {
            ResultInfo ri = new ResultInfo();
            LeaveWord model = new LeaveWord()
            {
                Telephone = Telephone,
                Age = Age,
                Name = Name,
                DIY1 = diy1,
                DIY2 = diy2,
                DIY3 = diy3,
                IsDelete = 0,
                CreateTime = DateTime.Now,
            };
            DB.LeaveWord.Add(model);
            DB.SaveChanges();
            ri.Ok = true;
            //ri.Ok = LeaveWordBLL.Instance.Add(model) > 0;

            #region 留言成功 后 通知 百晓生

            _noticeService.OnLeaveWord_Notice_Master(10006, model);

            #endregion

            return Result(ri);
        }
        #endregion

        #region 搜索 
        [HttpGet]
        public ActionResult Search(string key, int id)
        {
            ResultInfo ri = new ResultInfo();

            if (key.IsNotNullOrEmpty())
            {
                GiftViewModel model = new GiftViewModel();
                model.GiftPage = InitPage();
                model.Gifts = GiftBLL.Instance.GetAllGifts(model.GiftPage, id, key);
                ViewBag.ID = id;
                return PartialView("_Search", model);
            }
            else
            {
                ri.Msg = "请输入关键词";
            }
            return Result(ri);
        }
        #endregion

        #region 获取购买礼物的人
        /// <summary>
        /// 获取购买礼物的人
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetBuyUsers(long id = 0)
        {
            ResultInfo<List<_UserBaseInfo>> ri = new ResultInfo<List<_UserBaseInfo>>();
            if (id > 0)
            {
                var model = UserGiftBLL.Instance.GetBuyUsers(id);
                ri.Data = model.Distinct(a => a.Uid).ToList();
                return Result(ri);
            }
            else
            {
                ri.Msg = "商品不存在！";
            }
            return Result(ri);
        }
        #endregion

        #region 购买礼物人员管理
        [LOGIN]
        public ActionResult BuyerManage(int gtype, long id = 0)
        {
            if (id > 0)
            {
                var model = UserGiftBLL.Instance.GetBuyerList(id, gtype == 1 ? JoinItemTypeEnum.Gift : gtype == 2 ? JoinItemTypeEnum.DataAnalysis : JoinItemTypeEnum.KeCheng);
                if (model != null)
                {
                    if (model.GiftInfo.GiftCreateUserID == UserID || UserBaseBLL.Instance.IsRoot)
                    {
                        return View(model);
                    }
                    else
                    {
                        return RedirectToAction("Detail", "Gift", new { id = id });
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Gift");
                }
            }
            else
            {
                return RedirectToAction("Index", "Gift");
            }
        }
        #endregion

        #region 导出数据
        [LOGIN]
        public ActionResult Export(int gtype, long id = 0)
        {
            if (id > 0)
            {
                var model = UserGiftBLL.Instance.GetBuyerList(id, gtype == 1 ? JoinItemTypeEnum.Gift : gtype == 2 ? JoinItemTypeEnum.DataAnalysis : JoinItemTypeEnum.KeCheng);

                if (model.GiftInfo.GiftCreateUserID == UserID || UserBaseBLL.Instance.IsRoot)
                {
                    string desc = gtype == 1 ? "礼品" : gtype == 2 ? "数据" : "百晓堂课程";
                    var partyInfo = model.GiftInfo;
                    if (partyInfo != null)
                    {
                        var joinQuestions = model.JoinQuestions;
                        var joinAnswers = model.JoinAnswers;
                        string mainName = "{0}名称".FormatWith(desc);

                        DataTable dt = new DataTable();
                        dt.Columns.Add(new DataColumn(mainName));
                        dt.Columns.Add(new DataColumn("昵称"));
                        dt.Columns.Add(new DataColumn("姓名"));
                        dt.Columns.Add(new DataColumn("手机"));
                        dt.Columns.Add(new DataColumn("票种"));
                        dt.Columns.Add(new DataColumn("数量"));
                        dt.Columns.Add(new DataColumn("付费类型"));
                        dt.Columns.Add(new DataColumn("单价"));
                        dt.Columns.Add(new DataColumn("实付金额"));
                        dt.Columns.Add(new DataColumn("付费状态"));
                        dt.Columns.Add(new DataColumn("购买时间"));
                        joinQuestions.ForEach(a =>
                        {
                            dt.Columns.Add(new DataColumn(a.ItemName));
                        });

                        var lsit = model.BuyList;
                        string none = "-";
                        lsit.ForEach(join =>
                        {
                            DataRow dr = dt.NewRow();
                            dr[mainName] = partyInfo.GiftName;
                            dr["昵称"] = join.BuyerName;
                            dr["姓名"] = join.LinkMan ?? join.BuyerName;
                            dr["手机"] = join.LinkTel ?? none;
                            dr["票种"] = join.FeeName;
                            dr["数量"] = join.BuyCount;
                            dr["付费类型"] = join.FeeType == 0 ? "免费" : join.FeeType == 10 ? "积分付费" : join.FeeType == 20 ? "VIP分付费" : "RMB付费";
                            dr["单价"] = join.ItemSourceFee;
                            dr["实付金额"] = join.Fee;
                            dr["付费状态"] = join.IsPay == 1 ? "已付费" : "未付费";
                            dr["购买时间"] = join.BuyTime;
                            joinQuestions.ForEach(a =>
                            {
                                var answer = joinAnswers.FirstOrDefault(joinanswer => { return joinanswer.JoinItemQuestionExtId == a.JoinItemQuestionExtId && joinanswer.BuyerID == join.BuyUserID; });
                                dr[a.ItemName] = answer == null ? none : answer.ItemAnswer;
                            });
                            dt.Rows.Add(dr);
                        });
                        string excelname = "{0}购买信息".FormatWith(desc);
                        NPOIExcelHelper.ExportByWeb(dt, excelname, "{0}.xls".FormatWith(excelname));
                    }
                    return Redirect(Request.Url.AbsoluteUri);
                }
                else
                {
                    return RedirectToAction("Index", "Gift");
                }
            }
            else
            {
                return RedirectToAction("Index", "Gift");
            }
        }
        #endregion

        #region 获取用户已购买的个人信息
        [LOGIN]
        [HttpGet]
        public ActionResult Userbuyinfo(long id = 0)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var model = DB.Gift.FirstOrDefault(a => a.GiftID == id && a.IsDelete == 0);
                var type = (model.GType == 1 ? JoinItemTypeEnum.Gift : model.GType == 2 ? JoinItemTypeEnum.DataAnalysis : JoinItemTypeEnum.KeCheng).GetHashCode();
                ri.Data =
                    new
                    {
                        uinfo = DB.UserGift.Where(g => g.GType == model.GType && g.GiftID == id && g.BuyUserID == UserID).OrderByDescending(g => g.BuyTime).Select(g => new { name = g.LinkMan, tel = g.LinkTel }).FirstOrDefault(),
                        ext = DB.JoinItemQuestionExt.Where(a => a.MainID == id && a.MainType == type).Select(a => new
                        {
                            id = a.JoinItemQuestionExtId,
                            question = a.ItemName,
                            answer = DB.JoinItemAnswerExt.Where(answer => answer.JoinMainID == id && answer.JoinItemQuestionExtId == a.JoinItemQuestionExtId && answer.BuyerID == UserID).Select(ans => ans.ItemAnswer).FirstOrDefault(),
                        })
                    };
                ri.Ok = true;
            }
            return Result(ri);
        }
        #endregion
    }
}