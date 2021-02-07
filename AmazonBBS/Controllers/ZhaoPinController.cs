using AmazonBBS.BLL;
using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Controllers
{
    public class ZhaoPinController : BaseController
    {
        // GET: ZhaoPin
        public ActionResult Index()
        {
            return View(GetZhaoPins());
        }

        private ZhaoPinViewModel GetZhaoPins()
        {
            Paging page = InitPage(20);
            ZhaoPinViewModel model = ZhaoPinBLL.Instance.FindAll(page);
            return model;
        }

        [HttpGet]
        public ActionResult ZhaoPins()
        {
            var model = GetZhaoPins();
            return PartialView("_Search", model);
        }

        #region 发布招聘
        [LOGIN]
        public ActionResult Publish()
        {
            return View();
        }

        [LOGIN]
        [HttpPost]
        public ActionResult Publish(ZhaoPin model, int deadTime, bool hasPic)
        {
            ResultInfo resultinfo = UpLoadImg("JobPic", "/Content/Job/ZP", beforeSaveFile: (save, ri) =>
            {
                try
                {
                    BeginTran();
                    //判断VIP分是否足够
                    int publishScore = Convert.ToInt32(ConfigHelper.AppSettings("PUBLISH_ZHAOPIN"));
                    int type = 2;
                    if (UserExtBLL.Instance.HasEnoughCoin(type, publishScore, UserID))
                    {
                        //扣除相应积分
                        if (UserExtBLL.Instance.SubScore(UserID, publishScore, type, Tran))
                        {
                            //记录流水
                            if (ScoreCoinLogBLL.Instance.Log(-publishScore, type, CoinSourceEnum.PublishProduct, UserID, UserInfo.UserName, Tran))
                            {
                                if (hasPic)
                                {
                                    ri = save();
                                }

                                if (model.WorkeType == 1)
                                {
                                    model.WorkTime = null;
                                }

                                model.ValidTime = DateTime.Now.AddDays(deadTime);
                                model.UpdateTime = DateTime.Now;
                                model.UpdateUser = UserID.ToString();
                                model.CreateTime = DateTime.Now;
                                model.Publisher = UserID.ToString();
                                model.IsDelete = 0;
                                model.CLogo = UserInfo.HeadUrl;
                                model.JobPic = ri?.Url;
                                model.IsTop = 0;
                                model.IsJinghua = 0;
                                model.IsRemen = 0;
                                model.PVCount = 0;

                                //创建招聘信息
                                int result = ZhaoPinBLL.Instance.Add(model, Tran);

                                if (result > 0)
                                {
                                    string uri = ConfigHelper.AppSettings("ZhaoPinDetail").FormatWith(result);
                                    ri.Url = uri;
                                    ri.Ok = true;
                                    Commit();

                                    //通知作者
                                    NoticeBLL.Instance.OnPayPublish_Notice_Author(UserID, DateTime.Now, GetDomainName + uri, model.Gangwei, 20, publishScore.ToString(), NoticeTypeEnum.ZhaoPin_Pay_Publish);
                                    //通知关注作者的用户
                                    NoticeBLL.Instance.OnAdd_Notice_Liker(UserInfo.UserName, UserID, uri, model.Gangwei, NoticeTypeEnum.ZhaoPin_Add, GetDomainName);
                                }
                                else
                                {
                                    ri.Msg = "发布失败";
                                    RollBack();
                                    UploadHelper.DeleteUpFile(ri.Url);
                                }
                            }
                            else
                            {
                                RollBack();
                                ri.Msg = "发布失败";
                            }
                        }
                        else
                        {
                            RollBack();
                            ri.Msg = "发布失败";
                        }
                    }
                    else
                    {
                        ri.Msg = "VIP分不足，请去个人中心充值";
                    }
                }
                catch
                {
                    ri.Msg = "发布异常 ";
                    RollBack();
                    UploadHelper.DeleteUpFile(ri.Url);
                }
            }, isNeedFile: hasPic);
            return Result(resultinfo);
        }
        #endregion

        #region 数据校验
        private ResultInfo CheckEmpty(ZhaoPin model)
        {
            ResultInfo ri = new ResultInfo();
            if (CheckPrototype(model.CName, "公司名称", ri))
            {
                if (CheckPrototype(model.CPeople, "公司人数", ri))
                {
                    if (CheckPrototype(model.Gangwei, "招聘岗位", ri))
                    {
                        if (CheckPrototype(model.Money, "薪资待遇", ri))
                        {
                            if (CheckPrototype(model.Study, "学历要求", ri))
                            {
                                if (CheckPrototype(model.WorkHistory, "工作经验", ri))
                                {
                                    if (CheckPrototype(model.WorkPlace, "工作地点", ri))
                                    {
                                        if (CheckPrototype(model.Contact, "联系方式", ri))
                                        {
                                            if (CheckPrototype(model.CDesc, "公司简介", ri))
                                            {
                                                if (CheckPrototype(model.JobRequire, "职位要求", ri))
                                                {
                                                    ri.Ok = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return ri;
        }

        private bool CheckPrototype<T>(T str, string msg, ResultInfo ri)
        {
            if (str == null || str.ToString().IsNullOrEmpty())
            {
                ri.Msg = "{0}不能为空".FormatWith(msg);
                return false;
            }
            return true;
        }
        #endregion

        #region 招聘明细
        public ActionResult Detail(int id = 0)
        {
            if (id > 0)
            {
                Paging page = InitPage();
                _ZhaoPin model = ZhaoPinBLL.Instance.GetZhaoPinDetail(id, UserID, page);
                if (model == null)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.canSeeContact = UserInfo != null && (model.Publisher == UserID.ToString() || FeeHRBLL.Instance.IsPayContact(UserID, id, 1));

                //List<JobComment> comments = JobCommentBLL.Instance.GetJobComments(id, 1, page);
                //ViewBag.Comments = comments;
                //ViewBag.CommentPage = page;
                //分页获取评论
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region 评论
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">1招聘 2求职</param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public ActionResult JobComment(int id = 0, int type = 1, string comment = null)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                if (type == 1)
                {
                    //招聘
                    ZhaoPin model = ZhaoPinBLL.Instance.GetModel(id);
                    if (model.IsDelete == 1)
                    {
                        ri.Msg = "该帖子已被删除";
                    }
                    else
                    {
                        ri.Ok = true;
                    }
                }
                else
                {
                    //求职
                    ZhaoPin model = ZhaoPinBLL.Instance.GetModel(id);
                    if (model.IsDelete == 1)
                    {
                        ri.Msg = "该帖子已被删除";
                    }
                    else
                    {
                        ri.Ok = true;
                    }
                }
                if (ri.Ok)
                {
                    string defaultHeadUrl = "/Content/img/head_default.gif";
                    JobComment jobcomment = new Model.JobComment()
                    {
                        CommentType = type,
                        Comment = HttpUtility.UrlDecode(comment),
                        CreateHeadUrl = UserInfo.HeadUrl.IsNotNullOrEmpty() ? UserInfo.HeadUrl : defaultHeadUrl,
                        CreateNickName = UserInfo.UserName,
                        CreateTime = DateTime.Now,
                        CreateUser = UserID.ToString(),
                        IsDelete = 0,
                        MainID = id,
                    };
                    int result = JobCommentBLL.Instance.Add(jobcomment);
                    if (result > 0)
                    {
                        ri.Ok = true;
                        ri.Msg = "评论成功";
                    }
                    else
                    {
                        ri.Ok = false;
                        ri.Msg = "评论失败";
                    }
                }
            }
            else
            {
                ri.Msg = "该条数据不存在";
            }
            return Result(ri);
        }
        #endregion

        #region 付费查看联系方式
        [HttpPost]
        public ActionResult Fee(int id = 0, int type = 1)
        {
            ResultInfo<string> ri = new ResultInfo<string>();
            if (IsLogin)
            {
                if (id > 0)
                {
                    string contact = string.Empty;
                    string keyType = string.Empty;
                    if (type == 1)
                    {
                        ZhaoPin model = ZhaoPinBLL.Instance.GetModel(id);
                        keyType = "FeeHRZhaoPinValue";
                        contact = model.Contact;
                    }
                    else if (type == 2)
                    {
                        keyType = "FeeHRQiuZhiValue";
                        QiuZhi model = QiuZhiBLL.Instance.GetModel(id);
                        contact = model.Contact;
                        ri.Url = model.JianLiPic;
                    }
                    else
                    {
                        keyType = "FeeHRProductValue";
                        Product model = ProductBLL.Instance.GetModel(id);
                        contact = model.Contact;
                    }

                    BeginTran();

                    int value = Convert.ToInt32(ConfigHelper.AppSettings(keyType));
                    int cointype = 2;

                    //判断有无足够vip分
                    if (UserExtBLL.Instance.HasEnoughCoin(cointype, value, UserID))
                    {
                        //扣除相应分数
                        if (UserExtBLL.Instance.SubScore(UserID, value, cointype, Tran))
                        {
                            //记录流水
                            if (ScoreCoinLogBLL.Instance.Log(-value, cointype, CoinSourceEnum.SeeZhaoPinInfo, UserID, UserInfo.UserName, Tran))
                            {
                                //记录答案表
                                FeeHR fee = new FeeHR()
                                {
                                    FeeTime = DateTime.Now,
                                    FeeType = type,
                                    IsDelete = 0,
                                    MainID = id,
                                    UserID = UserID,
                                    FeeCoin = value,
                                    FeeCoinType = cointype
                                };
                                int result = FeeHRBLL.Instance.Add(fee, Tran);
                                if (result > 0)
                                {
                                    ri.Ok = true;
                                    ri.Data = contact;
                                    Commit();
                                }
                                else
                                {
                                    RollBack();
                                    ri.Msg = "消费失败";
                                }
                            }
                            else
                            {
                                RollBack();
                                ri.Msg = "消费失败";
                            }
                        }
                        else
                        {
                            RollBack();
                            ri.Msg = "消费失败";
                        }
                    }
                    else
                    {
                        ri.Msg = "VIP分不足，请前往个人中心充值";
                    }
                }
                else
                {
                    ri.Msg = "信息不存在";
                }
            }
            else
            {
                ri.Msg = "请先登录";
                ri.Url = "/Account/Login";
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
                var ZhaoPin = ZhaoPinBLL.Instance.GetModel(id);
                return View(ZhaoPin);
            }
            else
            {
                return RedirectToAction("Index", "ZhaoPin");
            }
        }

        [IsMaster]
        [HttpPost]
        public ActionResult Edit(ZhaoPin model, int deadTime)
        {
            ResultInfo ri = new ResultInfo();
            if (ModelState.IsValid)
            {
                ZhaoPin _model = ZhaoPinBLL.Instance.GetModel(model.ZhaoPinID);
                if (_model == null)
                {
                    ri.Msg = "该求职信息已被删除";
                }
                else
                {
                    Action<string> action = (imagePath) =>
                    {
                        _model.CName = model.CName;
                        _model.CPeople = model.CPeople;
                        _model.Gangwei = model.Gangwei;
                        _model.Money = model.Money;
                        _model.Study = model.Study;
                        _model.WorkHistory = model.WorkHistory;
                        _model.WorkPlace = model.WorkPlace;
                        _model.Contact = model.Contact;
                        _model.CDesc = model.CDesc;
                        _model.JobRequire = model.JobRequire;
                        _model.JobPic = imagePath;

                        _model.BelongJobTrade = model.BelongJobTrade;
                        _model.BelongJob = model.BelongJob;
                        _model.WorkeType = model.WorkeType;
                        _model.WorkTime = model.WorkTime;
                        _model.NeedCount = model.NeedCount;
                        _model.JobFuLi = model.JobFuLi;

                        _model.ValidTime = DateTime.Now.AddDays(deadTime);
                        _model.UpdateTime = DateTime.Now;
                        if (_model.WorkeType == 1)
                        {
                            _model.WorkTime = null;
                        }

                        ri = ZhaoPinBLL.Instance.Update(_model);
                        if (ri.Ok)
                        {
                            ri.Url = ConfigHelper.AppSettings("ZhaoPinDetail").FormatWith(_model.ZhaoPinID);
                        }
                    };
                    //是否修改上传照片
                    if (GetRequest<bool>("ischange"))
                    {
                        string oldImagePath = _model.JobPic;
                        ri = UpLoadImg("JobPic", "/Content/Job/ZP");
                        if (ri.Ok)
                        {
                            action(ri.Url);
                            UploadHelper.DeleteUpFile(oldImagePath);
                        }
                    }
                    else
                    {
                        action(_model.JobPic);
                    }
                }
            }
            return Result(ri);
        }
        #endregion

        #region 删除
        [HttpPost]
        public ActionResult Delete(long id)
        {
            ResultInfo ri = new ResultInfo();
            if (UserBaseBLL.Instance.IsRoot)
            {
                if (id > 0)
                {
                    ZhaoPin model = ZhaoPinBLL.Instance.GetModel(id);
                    if (model != null)
                    {
                        if (model.IsDelete == 1)
                        {
                            ri.Msg = "该求职信息已被删除";
                        }
                        else
                        {
                            model.IsDelete = 1;
                            ri = ZhaoPinBLL.Instance.Update(model);
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
            }
            else
            {
                ri.Msg = "你没有权限进行此操作";
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
                Paging page = InitPage(20);
                ZhaoPinViewModel model = ZhaoPinBLL.Instance.FindAll(page, key);
                return PartialView("_Search", model);
            }
            else
            {
                ri.Msg = "请输入关键词";
            }
            return Result(ri);
        }
        #endregion

        #region 发送简历
        [LOGIN]
        [HttpPost]
        public ActionResult CV(long id)
        {
            ResultInfo ri = new ResultInfo();
            var model = ZhaoPinBLL.Instance.GetModel(id);
            if (model != null && model.IsDelete == 0)
            {
                var useUserInfo = UserBaseBLL.Instance.GetUserInfo(model.Publisher.ToInt64());
                //判断是否已投递过简历
                if (!SendCVBLL.Instance.HasSendCV(model.ZhaoPinID, UserID))
                {
                    var fileResult = UpLoadFile(Request.Files["file"], "/Content/Job/ZP/FuJian");
                    if (fileResult.Ok)
                    {
                        SendCV sendCV = new SendCV()
                        {
                            CreateUser = UserID.ToString(),
                            CVPath = fileResult.Url,
                            IsDelete = 0,
                            IsRead = 0,
                            SendTime = DateTime.Now,
                            ZhaoPinPublisher = model.Publisher.ToInt32(),
                            ZhaoPinID = model.ZhaoPinID,
                            ReadTime = null,
                        };
                        if (SendCVBLL.Instance.Add(sendCV) > 0)
                        {
                            ri.Ok = true;
                            NoticeBLL.Instance.OnSendCV_Notice_Company(useUserInfo, model.ZhaoPinID.ToString(), model.Gangwei, UserInfo.UserName, UserID.ToString(), GetDomainName, NoticeTypeEnum.SendCV);
                        }
                    }
                }
                else
                {
                    ri.Msg = "您已投递过简历了！";
                }
            }
            else
            {
                ri.Msg = "招聘信息不存在！";
            }
            return Result(ri);
        }
        #endregion

        #region 邀请面试时获取当前发布人的所有招聘信息(已邀约过的除外)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">求职ID </param>
        /// <returns></returns>
        [LOGIN]
        public ActionResult ZhaoPinInfos(long id)
        {
            var list = ZhaoPinBLL.Instance.GetZhaoPinInfos(id, UserID);
            return Json(list.Select(a => new { I = a.ZhaoPinID, Name = a.Gangwei }), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 邀请面试
        [LOGIN]
        [HttpPost]
        public ActionResult IV(long id, long zid, string place, DateTime time)
        {
            ResultInfo ri = new ResultInfo();
            var zmodel = ZhaoPinBLL.Instance.GetModel(zid);
            if (zmodel != null && zmodel.IsDelete == 0)
            {
                var qmodel = QiuZhiBLL.Instance.GetModel(id);
                if (qmodel != null && qmodel.IsDelete == 0)
                {
                    //判断是否已邀请面试
                    if (!InviteInterviewBLL.Instance.Hasinvited(qmodel.QiuZhiID, UserID))
                    {
                        if (IsSafe(place))
                        {
                            InviteInterview inviteInterview = new InviteInterview()
                            {
                                CreateTime = DateTime.Now,
                                CreateUser = UserID.ToString(),
                                InviteTime = time,
                                IsDelete = 0,
                                ZhaoPinID = zid,
                                QiuZhiID = qmodel.QiuZhiID,
                                QiuZhiPublisher = qmodel.Publisher.ToInt64(),
                                ViewAddress = place
                            };
                            if (InviteInterviewBLL.Instance.Add(inviteInterview) > 0)
                            {
                                ri.Ok = true;

                                #region 邮件通知求职者
                                NoticeBLL.Instance.OnInviteInterview_Notice_Author(UserInfo.UserName, zmodel.Gangwei, zmodel.ZhaoPinID.ToString(), time.ToString(), place, qmodel.IWant, qmodel.QiuZhiID.ToString(), GetDomainName, qmodel.Publisher.ToInt64(), NoticeTypeEnum.InviteInterview);
                                #endregion
                            }
                        }
                    }
                    else
                    {
                        ri.Msg = "您已邀请面试过了！";
                    }
                }
                else
                {
                    ri.Msg = "求职信息已被删除，不能邀请面试！";
                }
            }
            else
            {
                ri.Msg = "抱歉，您的该招聘信息已被删除！";
            }
            return Result(ri);
        }
        #endregion

        #region 安全过滤
        private bool IsSafe(string condition)
        {
            if ("delete,update,select".IndexOf(condition.ToLower()) > -1)
            {
                return false;
            }
            else
            {
                if (condition.IsNotNullOrEmpty())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region 条件筛选
        [HttpGet]
        public ActionResult Select(string search_jobTrade, string search_job, string search_companyName, string search_workPlace, string search_money, string search_study, string search_worktype)
        {
            ResultInfo ri = new ResultInfo();
            Paging page = InitPage(20);
            ZhaoPinViewModel model = ZhaoPinBLL.Instance.SelectByCondition(page, search_jobTrade, search_job, search_companyName, search_workPlace, search_money, search_study, search_worktype);
            return PartialView("_Search", model);
        }
        #endregion

        #region 设置精华(1)、热门(2)、置顶(3)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">操作类型：精华1 热门2 置顶3</param>
        /// <param name="action">1设置/0取消</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetPropertity(int id, int type, int action, string enumPageType)
        {
            ResultInfo ri = new ResultInfo();
            if (UserBaseBLL.Instance.IsRoot)
            {
                if (id > 0)
                {
                    if (type > 0 && type < 4)
                    {
                        int flag = action == 1 ? 1 : 0;

                        if (enumPageType == "qiuzhi")
                        {
                            #region 求职
                            QiuZhiBLL qiuzhibll = QiuZhiBLL.Instance;
                            var qzmodel = qiuzhibll.GetModel(id);
                            if (qzmodel != null)
                            {
                                if (type == 1)
                                {
                                    qzmodel.IsJinghua = flag;
                                }
                                else if (type == 2)
                                {
                                    qzmodel.IsRemen = flag;
                                }
                                else
                                {
                                    qzmodel.IsTop = flag;
                                }
                                ri = qiuzhibll.Update(qzmodel);
                            }
                            else
                            {
                                ri.Msg = "求职信息不存在";
                            }
                            #endregion
                        }
                        else if (enumPageType == "zhaopin")
                        {
                            #region 招聘
                            ZhaoPinBLL zhaopinbll = ZhaoPinBLL.Instance;
                            var zpmodel = zhaopinbll.GetModel(id);
                            if (zpmodel != null)
                            {
                                if (type == 1)
                                {
                                    zpmodel.IsJinghua = flag;
                                }
                                else if (type == 2)
                                {
                                    zpmodel.IsRemen = flag;
                                }
                                else
                                {
                                    zpmodel.IsTop = flag;
                                }
                                ri = zhaopinbll.Update(zpmodel);
                            }
                            else
                            {
                                ri.Msg = "招聘信息不存在";
                            }
                            #endregion
                        }
                        else
                        {
                            #region 产品信息
                            ProductBLL productbll = ProductBLL.Instance;
                            var pmodel = productbll.GetModel(id);
                            if (pmodel != null)
                            {
                                if (type == 1)
                                {
                                    pmodel.IsJinghua = flag;
                                }
                                else if (type == 2)
                                {
                                    pmodel.IsRemen = flag;
                                }
                                else
                                {
                                    pmodel.IsTop = flag;
                                }
                                ri = productbll.Update(pmodel);
                            }
                            else
                            {
                                ri.Msg = "产品信息不存在";
                            }
                            #endregion
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
            }
            else
            {
                ri.Msg = "你没有权限";
            }
            return Result(ri);
        }
        #endregion

        #region 下载附件
        public void DownLoad(long id = 0, long uid = 0)
        {
            if (id > 0 && uid > 0)
            {
                var zmodel = ZhaoPinBLL.Instance.GetModel(id);
                if (zmodel != null && zmodel.IsDelete == 0)
                {
                    var path = SendCVBLL.Instance.GetFilePath(id, uid);
                    if (path.IsNotNullOrEmpty())
                    {
                        string fileName = zmodel.Gangwei + "." + path.Split('.').Last();
                        string filePath = Server.MapPath(path);
                        FileStream fs = new FileStream(filePath, FileMode.Open);
                        byte[] bytes = new byte[(int)fs.Length];
                        fs.Read(bytes, 0, bytes.Length);
                        fs.Close();
                        Response.ContentType = "application/octet-stream";
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
        }
        #endregion

        #region 擦亮
        public ActionResult Light(long id = 0)
        {
            ResultInfo<DateTime> ri = new ResultInfo<DateTime>();
            if (id > 0)
            {
                ZhaoPin model = ZhaoPinBLL.Instance.GetModel(id);
                if (model != null)
                {
                    if (DateTime.Now >= model.ValidTime)
                    {
                        DateTime time = DateTime.Now.AddDays(30);
                        model.ValidTime = time;
                        if (ZhaoPinBLL.Instance.Update(model).Ok)
                        {
                            ri.Ok = true;
                            ri.Msg = "成功擦亮，有效时间延长30天！";
                            ri.Data = time;
                        }
                    }
                    else
                    {
                        ri.Msg = "招聘信息未失效，无需擦亮！";
                    }
                }
                else
                {
                    ri.Msg = "招聘信息不存在！";
                }
            }
            else
            {
                ri.Msg = "招聘信息不存在！";
            }
            return Result(ri);
        }
        #endregion
    }
}