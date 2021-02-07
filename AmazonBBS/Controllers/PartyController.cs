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
    public class PartyController : BaseController
    {
        public IOrderService orderService { get; set; }
        public INoticeService noticeService { get; set; }

        #region 活动列表页
        public ActionResult Index()
        {
            var model = GetPartyLists();
            return View(model);
        }

        public ActionResult LoadMore()
        {
            var model = GetPartyLists();
            return PartialView("_Search", model);
        }

        private ActiveListViewModel GetPartyLists()
        {
            ActiveListViewModel model = new ActiveListViewModel();
            model.ActivityPage = InitPage(12);
            model.Activis = ActivityBLL.Instance.GetAllActivits(model.ActivityPage);
            return model;
        }
        #endregion

        #region 新增活动 
        //[IsMaster]
        [LOGIN]
        public ActionResult Create()
        {
            return View();
        }

        //[IsMaster]
        [LOGIN]
        [HttpPost]
        public ActionResult Create(PartyCreateViewModel model)
        {
            ResultInfo ri = new ResultInfo();

            BeginTran();

            Activity activity = model.Party;
            activity.Body = HttpUtility.UrlDecode(activity.Body);
            activity.UserID = UserID;
            activity.UserName = UserInfo.UserName;
            activity.ActivityCreateTIme = DateTime.Now;
            activity.PVCount = 0;
            activity.IsDelete = 0;

            //UserExt userext = UserExtBLL.Instance.GetExtInfo(UserID);
            ////判断是否触发审核机制：积分大于1000分时，必须要参加审核
            //int mustcheckscore = Convert.ToInt32(ConfigHelper.AppSettings("MustCheckByScore"));
            ////积分大于多少时自动审核
            //int autocheckscore = Convert.ToInt32(ConfigHelper.AppSettings("AutoCheck_NeedScore"));
            //activity.IsChecked = userext.TotalScore >= mustcheckscore ? userext.TotalScore <= autocheckscore ? 1 : 2 : 2;

            //非管理员创建的活动都要经过审核
            activity.IsChecked = UserBaseBLL.Instance.IsMaster ? 2 : 1;

            int result = ActivityBLL.Instance.Add(activity, Tran);
            if (result > 0)
            {
                ri.Url = $"/party/detail/{result}";
                var finalFees = model.PartyFee.Where(item => { return item.Fee.HasValue && item.FeeCount.HasValue && item.FeeType.HasValue && item.FeeName.IsNotNullOrEmpty(); });
                foreach (var item in finalFees)
                {
                    item.ActivityId = result;
                    item.FeeType = item.FeeType ?? 0;
                    if (ActivityFeeBLL.Instance.Add(item, Tran) < 1)
                    {
                        result = 0;
                        break;
                    }
                }
                if (result > 0)
                {
                    #region 添加活动报名填写项
                    if (model.JoinItemQues != null)
                    {
                        var finalJoins = model.JoinItemQues.Where(item => { return item.ItemName.IsNotNullOrEmpty(); });
                        foreach (var item in finalJoins)
                        {
                            item.IsDelete = 0;
                            item.CreateTime = DateTime.Now;
                            item.CreateUser = UserID.ToString();
                            item.UpdateTime = DateTime.Now;
                            item.UpdateUser = UserID.ToString();
                            item.IsMustWrite = 1;
                            item.MainID = result;
                            item.MainType = 1;
                            if (JoinItemQuestionExtBLL.Instance.Add(item, Tran) < 1)
                            {
                                result = 0;
                                break;
                            }
                        }
                    }
                    #endregion
                    if (result > 0)
                    {
                        Commit();
                        ri.Ok = true;
                        string uri = ConfigHelper.AppSettings("PartyDetail").FormatWith(result);
                        if (activity.IsChecked == 1)
                        {
                            //通知审核
                            noticeService.On_BBS_Article_Publish_Success_Notice(UserInfo, uri, model.Party.Title, 3);
                            ri.Msg = "活动创建成功，您发布的活动成功触发系统审核机制，等待管理员审核成功后即可在页面里查看";
                            ri.Url = "/";
                        }
                        else
                        {
                            ri.Msg = "活动创建成功";
                            ri.Url = uri;
                            //通知
                            NoticeBLL.Instance.OnAdd_Notice_Liker(UserInfo.UserName, UserID, uri, activity.Title, NoticeTypeEnum.Party_Add, GetDomainName);
                        }
                    }
                    else
                    {
                        RollBack();
                        ri.Msg = "活动创建失败";
                        ri.Url = string.Empty;
                    }
                }
                else
                {
                    RollBack();
                    ri.Msg = "活动创建失败";
                    ri.Url = string.Empty;
                }
            }
            else
            {
                RollBack();
            }
            return Result(ri);
        }
        #endregion

        #region 活动详情页
        public ActionResult Detail(int id = 0)
        {
            if (id > 0)
            {
                //获取活动信息
                Paging page = InitPage();
                _Activity activity = ActivityBLL.Instance.GetPartyDetail(id, UserID, page);
                if (activity == null)
                {
                    return RedirectToAction("Index");
                }
                return View(activity);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region 活动报名
        [HttpPost]
        public ActionResult Submit(long id, int fee, long feeid, int count, string LinkMan, string LinkTel, List<UserJoinItemViewModel> JoinItems)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                //根据ID查找详情
                _Activity activity = ActivityBLL.Instance.GetDetail(id, 0, false);
                if (activity != null)
                {
                    ri = ActivityBLL.Instance.CanJoinParty(activity);
                    if (ri.Ok)
                    {
                        ri = new ResultInfo();
                        BeginTran();

                        #region 报名
                        #region 获取报名费用

                        List<ActivityFee> feelist = activity.FeeList;
                        var feeInfo = feelist.FirstOrDefault(a => { return a.ActivityFeeId == feeid && a.Fee == fee; });
                        if (feeInfo != null)
                        {
                            if (feeInfo.FeeCount > 0)
                            {
                                if (feeInfo.FeeCount >= count)
                                {
                                    //判断是否需要扣费
                                    //TODO

                                    int coinType = feeInfo.FeeType == 10 ? 1 : feeInfo.FeeType == 20 ? 2 : 0;
                                    int coin = Convert.ToInt32(feeInfo.Fee.Value) * count;//计算总费用

                                    bool canJoin = true;
                                    if (fee != 0)
                                    {
                                        canJoin = UserExtBLL.Instance.HasEnoughCoin(coinType, coin, UserID);
                                    }
                                    //记录参加活动
                                    if (canJoin)
                                    {
                                        ActivityJoin ajoin = new ActivityJoin();
                                        ajoin.ActivityId = activity.ActivityId;
                                        ajoin.FeeType = feeInfo.FeeType;
                                        ajoin.IsFeed = 1;
                                        ajoin.RealPayFee = coin;
                                        ajoin.JoinCount = count;
                                        ajoin.JoinTime = DateTime.Now;
                                        ajoin.JoinUserID = UserID;
                                        ajoin.JoinUserName = UserInfo.UserName;
                                        ajoin.ActivityFeeId = feeInfo.ActivityFeeId;
                                        ajoin.LinkMan = LinkMan;
                                        ajoin.LinkTel = LinkTel;
                                        if (ActivityJoinBLL.Instance.Add(ajoin, Tran) > 0)
                                        {
                                            bool insertOk = true;
                                            //记录报名填写项
                                            if (JoinItems != null && JoinItems.Count > 0)
                                            {
                                                foreach (var join in JoinItems)
                                                {
                                                    JoinItemAnswerExt model = new JoinItemAnswerExt()
                                                    {
                                                        BuyerID = UserID,
                                                        CreateTime = DateTime.Now,
                                                        ItemAnswer = join.Value,
                                                        JoinItemQuestionExtId = join.Id,
                                                        JoinMainID = activity.ActivityId,
                                                        JoinType = 1
                                                    };
                                                    if (JoinItemAnswerExtBLL.Instance.Add(model, Tran) <= 0)
                                                    {
                                                        insertOk = false;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (insertOk)
                                            {
                                                //更新剩余名额
                                                if (ActivityFeeBLL.Instance.UpdateCount(count, feeInfo.ActivityFeeId, Tran))
                                                {
                                                    ////生成订单
                                                    //string orderDesc = GetRequest("desc", "参加" + activity.Title);
                                                    ////添加订单
                                                    //int _orderResult = 0;
                                                    //string _orderNo = string.Empty;
                                                    //var addOrder = _orderService.CreateOrder(UserID, fee, id, OrderEnumType.Party.GetHashCode(), orderDesc, "HD", count, Tran, out _orderResult, out _orderNo);
                                                    //if (_orderResult > 0)
                                                    //{
                                                    if (fee == 0)
                                                    {
                                                        ri.Ok = true;
                                                        ri.Msg = "报名成功";
                                                        Commit();
                                                    }
                                                    else
                                                    {
                                                        //报名成功
                                                        //扣除相应数据
                                                        //用户帐户减去相应的积分或金钱
                                                        if (UserExtBLL.Instance.SubScore(UserID, coin, coinType, Tran))
                                                        {
                                                            if (ScoreCoinLogBLL.Instance.Log(-coin, coinType, CoinSourceEnum.JoinActivity, UserID, UserInfo.UserName, Tran))
                                                            {
                                                                Commit();
                                                                ri.Ok = true;
                                                                ri.Msg = "报名成功";
                                                            }
                                                            else
                                                            {
                                                                ri.Msg = "报名失败";
                                                                RollBack();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            ri.Msg = "报名失败";
                                                            RollBack();
                                                        }
                                                    }
                                                    if (ri.Ok)
                                                    {
                                                        //通知
                                                        NoticeBLL.Instance.OnBuySuccess_Notice_Buyer(UserID, DateTime.Now, fee == 0 ? true : false, "{0}{1}".FormatWith(coin, feeInfo.FeeType == 10 ? "积分" : feeInfo.FeeType == 20 ? "VIP分" : string.Empty), count, ConfigHelper.AppSettings("PartyDetail").FormatWith(id), activity.Title, false, NoticeTypeEnum.Party_Join);
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
                                        ri.Msg = $"对不起，您没有足够的{(coinType == 1 ? "积分" : "金钱")}";
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
                            ri.Msg = "费用异常!";
                        }
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    ri.Msg = "活动不存在";
                }
            }
            else
            {
                ri.Msg = "活动详情错误";
            }
            return Result(ri);
        }
        #endregion

        #region 编辑
        //[IsMaster]
        [LOGIN]
        public ActionResult Edit(long id = 0)
        {
            if (id > 0)
            {
                PartyCreateViewModel model = ActivityBLL.Instance.GetEditDetail(id);
                if (model != null)
                {
                    if (model.Party != null)
                    {
                        if (model.Party.UserID == UserID || UserBaseBLL.Instance.IsMaster)
                        {
                            return View(model);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //[IsMaster]
        [LOGIN]
        [HttpPost]
        public ActionResult Edit(PartyCreateViewModel model)
        {
            ResultInfo ri = new ResultInfo();
            if (model.Party.ActivityId == model.PartyFee[0].ActivityId)
            {
                Activity _model = ActivityBLL.Instance.GetModel(model.Party.ActivityId);
                if (_model != null)
                {
                    if (_model.IsDelete == 0)
                    {
                        BeginTran();

                        _model.Title = model.Party.Title;
                        _model.ActivityIMG = model.Party.ActivityIMG;
                        _model.BeginTime = model.Party.BeginTime;
                        _model.EndTime = model.Party.EndTime;
                        _model.CanJoinOnBegin = model.Party.CanJoinOnBegin;
                        _model.JoinBeginTime = model.Party.JoinBeginTime;
                        _model.JoinEndTime = model.Party.JoinEndTime;
                        _model.Address = model.Party.Address;
                        _model.Body = HttpUtility.UrlDecode(model.Party.Body);

                        if (ActivityBLL.Instance.Update(_model, Tran).Ok)
                        {
                            var finalFees = model.PartyFee.Where(item => { return item.Fee.HasValue && item.FeeCount.HasValue && item.FeeType.HasValue && item.FeeName.IsNotNullOrEmpty(); });
                            //更新费用
                            foreach (ActivityFee fee in finalFees)
                            {
                                if (fee.ActivityFeeId > 0)
                                {
                                    if (fee.ActivityId == _model.ActivityId)
                                    {
                                        //更新
                                        var feeModel = ActivityFeeBLL.Instance.GetModel(fee.ActivityFeeId);
                                        feeModel.FeeType = fee.FeeType;
                                        feeModel.FeeName = fee.FeeName;
                                        feeModel.Fee = fee.Fee;
                                        feeModel.FeeCount = fee.FeeCount;
                                        feeModel.ActivityId = _model.ActivityId;

                                        if (ActivityFeeBLL.Instance.Update(feeModel, Tran).Ok)
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
                                        ri.Msg = "该活动费用列表异常！";
                                        break;
                                    }
                                }
                                else
                                {
                                    fee.ActivityId = model.Party.ActivityId;
                                    if (ActivityFeeBLL.Instance.Add(fee, Tran) > 0)
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
                                            if (item.MainID == _model.ActivityId)
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
                                            item.MainID = model.Party.ActivityId;
                                            item.MainType = 1;
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
                        ri.Msg = "该活动已被删除";
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
                Activity model = ActivityBLL.Instance.GetModel(id);
                if (model.IsDelete == 1)
                {
                    ri.Msg = "活动已被删除";
                    ri.Ok = true;
                }
                else
                {
                    model.IsDelete = 1;
                    ri = ActivityBLL.Instance.Update(model);
                    if (ri.Ok) { ri.Msg = "删除成功"; }
                }
            }
            else
            {
                ri.Msg = "异常";
            }
            return Result(ri);
        }
        #endregion

        #region 搜索 
        [HttpGet]
        public ActionResult Search(string key)
        {
            ResultInfo ri = new ResultInfo();

            if (key.IsNotNullOrEmpty())
            {
                ActiveListViewModel model = new ActiveListViewModel();
                model.ActivityPage = InitPage(20);
                model.Activis = ActivityBLL.Instance.GetAllActivits(model.ActivityPage, key);
                return PartialView("_Search", model);
            }
            else
            {
                ri.Msg = "请输入关键词";
            }
            return Result(ri);
        }
        #endregion

        #region 获取参加活动的人信息
        [HttpGet]
        public ActionResult GetJoinedUser(long id)
        {
            ResultInfo<List<_UserBaseInfo>> ri = new ResultInfo<List<_UserBaseInfo>>();
            var model = ActivityJoinBLL.Instance.GetJoinedUsers(id);
            ri.Data = model.Distinct(a => a.Uid).ToList();
            return Result(ri);
        }
        #endregion

        #region 活动报名管理
        [LOGIN]
        public ActionResult JoinManage(long id = 0)
        {
            if (id > 0)
            {
                var model = ActivityJoinBLL.Instance.GetJoinList(id);
                if (model != null)
                {
                    if (model.PartyInfo.UserID == UserID || UserBaseBLL.Instance.IsRoot)
                    {
                        return View(model);
                    }
                    else
                    {
                        return RedirectToAction("Detail", "Party", new { id = id });
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Party");
                }
            }
            else
            {
                return RedirectToAction("Index", "Party");
            }
        }
        #endregion

        #region 导出数据
        [LOGIN]
        public ActionResult Export(long id = 0)
        {
            if (id > 0)
            {
                var model = ActivityJoinBLL.Instance.GetJoinList(id);

                if (model.PartyInfo.UserID == UserID || UserBaseBLL.Instance.IsRoot)
                {
                    var partyInfo = model.PartyInfo;
                    if (partyInfo != null)
                    {
                        var joinQuestions = model.JoinQuestions;
                        var joinAnswers = model.JoinAnswers;

                        DataTable dt = new DataTable();
                        dt.Columns.Add(new DataColumn("活动名称"));
                        dt.Columns.Add(new DataColumn("昵称"));
                        dt.Columns.Add(new DataColumn("姓名"));
                        dt.Columns.Add(new DataColumn("手机"));
                        dt.Columns.Add(new DataColumn("票种"));
                        dt.Columns.Add(new DataColumn("数量"));
                        dt.Columns.Add(new DataColumn("付费类型"));
                        dt.Columns.Add(new DataColumn("单价"));
                        dt.Columns.Add(new DataColumn("实付金额"));
                        dt.Columns.Add(new DataColumn("付费状态"));
                        dt.Columns.Add(new DataColumn("报名时间"));
                        joinQuestions.ForEach(a =>
                        {
                            dt.Columns.Add(new DataColumn(a.ItemName));
                        });

                        var lsit = model.JoinList;
                        string none = "-";
                        lsit.ForEach(join =>
                        {
                            DataRow dr = dt.NewRow();
                            dr["活动名称"] = partyInfo.Title;
                            dr["昵称"] = join.JoinUserName;
                            dr["姓名"] = join.LinkMan ?? join.JoinUserName;
                            dr["手机"] = join.LinkTel ?? none;
                            dr["票种"] = join.FeeName;
                            dr["数量"] = join.JoinCount;
                            dr["付费类型"] = join.FeeType == 0 ? "免费" : join.FeeType == 10 ? "积分付费" : join.FeeType == 20 ? "VIP分付费" : "RMB付费";
                            dr["单价"] = join.ItemSourceFee;
                            dr["实付金额"] = join.RealPayFee;
                            dr["付费状态"] = join.IsFeed == 1 ? "已付费" : "未付费";
                            dr["报名时间"] = join.JoinTime;
                            joinQuestions.ForEach(a =>
                            {
                                var answer = joinAnswers.FirstOrDefault(joinanswer => { return joinanswer.JoinItemQuestionExtId == a.JoinItemQuestionExtId && joinanswer.BuyerID == join.JoinUserID; });
                                dr[a.ItemName] = answer == null ? none : answer.ItemAnswer;
                            });
                            dt.Rows.Add(dr);
                        });
                        NPOIExcelHelper.ExportByWeb(dt, "活动报名信息", "活动报名信息.xls");
                    }
                    return Redirect(Request.Url.AbsoluteUri);
                }
                else
                {
                    return RedirectToAction("Index", "Party");
                }
            }
            else
            {
                return RedirectToAction("Index", "Party");
            }
        }
        #endregion
    }
}