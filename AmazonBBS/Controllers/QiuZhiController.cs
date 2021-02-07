using AmazonBBS.BLL;
using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Controllers
{
    public class QiuZhiController : BaseController
    {
        // GET: QiuZhi
        public ActionResult Index()
        {
            return View(GetQiuZhis());
        }

        private QiuZhiViewModel GetQiuZhis()
        {
            Paging page = InitPage(20);
            QiuZhiViewModel model = QiuZhiBLL.Instance.FindAll(page);
            return model;
        }

        [HttpGet]
        public ActionResult QiuZhis()
        {
            var model = GetQiuZhis();
            return PartialView("_Search", model);
        }

        #region 发布求职
        [LOGIN]
        public ActionResult Publish()
        {
            return View();
        }

        [LOGIN]
        [HttpPost]
        public ActionResult Publish(QiuZhi model, int deadTime, bool hasPic)
        {
            ResultInfo ri = CheckEmpty(model);
            if (ri.Ok)
            {
                ri = UpLoadImg("JianLiPic", "/Content/Job/QZ", beforeSaveFile: (save, resultInfo) =>
                {
                    try
                    {
                        BeginTran();
                        //判断VIP分是否足够
                        int publishScore = Convert.ToInt32(ConfigHelper.AppSettings("PUBLISH_QIUZHI"));
                        int type = 2;
                        if (UserExtBLL.Instance.HasEnoughCoin(type, publishScore, UserID))
                        {
                            //扣除相应积分
                            if (UserExtBLL.Instance.SubScore(UserID, publishScore, type, Tran))
                            {
                                if (ScoreCoinLogBLL.Instance.Log(-publishScore, type, CoinSourceEnum.PublishQiuZhi, UserID, UserInfo.UserName, Tran))
                                {
                                    if (hasPic)
                                    {
                                        resultInfo = save();
                                    }

                                    if (model.WorkType == 1)
                                    {
                                        model.WorkTime = null;
                                    }

                                    model.ValidTime = DateTime.Now.AddDays(deadTime);
                                    model.UpdateTime = DateTime.Now;
                                    model.CreateTime = DateTime.Now;
                                    model.UpdateUser = UserID.ToString();
                                    model.Publisher = UserID.ToString();
                                    model.IsDelete = 0;
                                    model.JianLiPic = resultInfo?.Url;
                                    model.IsTop = 0;
                                    model.IsJinghua = 0;
                                    model.IsRemen = 0;
                                    model.PVCount = 0;

                                    int result = QiuZhiBLL.Instance.Add(model);

                                    if (result > 0)
                                    {
                                        string uri = ConfigHelper.AppSettings("QiuZhiDetail").FormatWith(result);
                                        resultInfo.Url = uri;
                                        resultInfo.Ok = true;
                                        Commit();

                                        //通知作者
                                        NoticeBLL.Instance.OnPayPublish_Notice_Author(UserID, DateTime.Now, GetDomainName + uri, model.IWant, 20, publishScore.ToString(), NoticeTypeEnum.QiuZhi_Pay_Publish);
                                        //通知关注作者的用户
                                        NoticeBLL.Instance.OnAdd_Notice_Liker(UserInfo.UserName, UserID, uri, model.IWant, NoticeTypeEnum.QiuZhi_Add, GetDomainName);
                                    }
                                    else
                                    {
                                        resultInfo.Msg = "发布失败";
                                        RollBack();
                                        UploadHelper.DeleteUpFile(resultInfo.Url);
                                    }
                                }
                                else
                                {
                                    RollBack();
                                    resultInfo.Msg = "发布失败";
                                }
                            }
                            else
                            {
                                RollBack();
                                resultInfo.Msg = "发布失败";
                            }
                        }
                        else
                        {
                            resultInfo.Msg = "VIP分不足，请去个人中心充值";
                        }

                    }
                    catch
                    {
                        resultInfo.Msg = "发布异常 ";
                        RollBack();
                        UploadHelper.DeleteUpFile(resultInfo.Url);
                    }
                }, isNeedFile: hasPic);
            }
            return Result(ri);
        }
        #endregion

        #region 数据校验
        private ResultInfo CheckEmpty(QiuZhi model)
        {
            ResultInfo ri = new ResultInfo();
            if (CheckPrototype(model.IWant, "求职意向", ri))
            {
                if (CheckPrototype(model.Money, "求职薪资", ri))
                {
                    if (CheckPrototype(model.NowWork, "目前岗位", ri))
                    {
                        if (CheckPrototype(model.WorkStatus, "离职状态", ri))
                        {
                            if (CheckPrototype(model.Study, "学历", ri))
                            {
                                if (CheckPrototype(model.WorkYear, "工作年限", ri))
                                {
                                    if (CheckPrototype(model.MyDesc, "自我简介", ri))
                                    {
                                        if (CheckPrototype(model.Contact, "联系方式", ri))
                                        {
                                            if (CheckPrototype(model.SelfAssessment, "自我评价", ri))
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

        #region 明细
        public ActionResult Detail(long id = 0)
        {
            if (id > 0)
            {
                Paging page = InitPage();
                _QiuZhi model = QiuZhiBLL.Instance.GetQiuZhiDetail(id, UserID, page);
                if (model == null)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.canSeeContact = UserInfo != null && (model.Publisher == UserID.ToString() || FeeHRBLL.Instance.IsPayContact(UserID, id, 2));
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region 编辑
        [IsMaster]
        public ActionResult Edit(long id)
        {
            if (id > 0)
            {
                var QiuZhi = QiuZhiBLL.Instance.GetModel(id);
                return View(QiuZhi);
            }
            else
            {
                return RedirectToAction("Index", "QiuZhi");
            }
        }

        [IsMaster]
        [HttpPost]
        public ActionResult Edit(QiuZhi model, int deadTime)
        {
            ResultInfo ri = new ResultInfo();
            if (ModelState.IsValid)
            {
                ri = CheckEmpty(model);
                if (ri.Ok)
                {
                    QiuZhi _model = QiuZhiBLL.Instance.GetModel(model.QiuZhiID);
                    if (_model == null)
                    {
                        ri.Msg = "该求职信息已被删除";
                    }
                    else
                    {
                        Action<string> action = (imagePath) =>
                        {
                            _model.IWant = model.IWant;
                            _model.Money = model.Money;
                            _model.NowWork = model.NowWork;
                            _model.WorkStatus = model.WorkStatus;
                            _model.Study = model.Study;
                            _model.WorkYear = model.WorkYear;
                            _model.MyDesc = model.MyDesc;
                            _model.Contact = model.Contact;
                            _model.SelfAssessment = model.SelfAssessment;
                            _model.JianLiPic = imagePath;
                            _model.IWantPlace = model.IWantPlace;
                            _model.BelongJob = model.BelongJob;
                            _model.BelongJobTrade = model.BelongJobTrade;
                            _model.WorkType = model.WorkType;
                            _model.WorkTime = model.WorkTime;

                            _model.ValidTime = DateTime.Now.AddDays(deadTime);
                            _model.UpdateTime = DateTime.Now;
                            if (_model.WorkType == 1)
                            {
                                _model.WorkTime = null;
                            }

                            ri = QiuZhiBLL.Instance.Update(_model);
                            if (ri.Ok)
                            {
                                ri.Url = ConfigHelper.AppSettings("QiuZhiDetail").FormatWith(_model.QiuZhiID);
                            }
                        };

                        //是否修改上传照片
                        if (GetRequest<bool>("ischange"))
                        {
                            string oldImagePath = _model.JianLiPic;
                            ri = UpLoadImg("JianLiPic", "/Content/Job/QZ");
                            if (ri.Ok)
                            {
                                action(ri.Url);
                                UploadHelper.DeleteUpFile(oldImagePath);
                            }
                        }
                        else
                        {
                            action(_model.JianLiPic);
                        }
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
                    QiuZhi model = QiuZhiBLL.Instance.GetModel(id);
                    if (model != null)
                    {
                        if (model.IsDelete == 1)
                        {
                            ri.Msg = "该求职信息已被删除";
                        }
                        else
                        {
                            model.IsDelete = 1;
                            ri = QiuZhiBLL.Instance.Update(model);
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
                QiuZhiViewModel model = QiuZhiBLL.Instance.FindAll(page, key);
                return PartialView("_Search", model);
            }
            else
            {
                ri.Msg = "请输入关键词";
            }
            return Result(ri);
        }
        #endregion

        #region 条件筛选
        [HttpGet]
        public ActionResult Select(string search_jobTrade, string search_job, string search_WorkYear, string search_workPlace, string search_money, string search_study, string search_worktype)
        {
            ResultInfo ri = new ResultInfo();
            Paging page = InitPage(20);
            QiuZhiViewModel model = QiuZhiBLL.Instance.SelectByCondition(page, search_jobTrade, search_job, search_WorkYear, search_workPlace, search_money, search_study, search_worktype);
            return PartialView("_Search", model);
        }
        #endregion

        #region 擦亮
        public ActionResult Light(long id = 0)
        {
            ResultInfo<DateTime> ri = new ResultInfo<DateTime>();
            if (id > 0)
            {
                QiuZhi model = QiuZhiBLL.Instance.GetModel(id);
                if (model != null)
                {
                    if (DateTime.Now >= model.ValidTime)
                    {
                        DateTime time = DateTime.Now.AddDays(30);
                        model.ValidTime = time;
                        if (QiuZhiBLL.Instance.Update(model).Ok)
                        {
                            ri.Ok = true;
                            ri.Msg = "成功擦亮，有效时间延长30天！";
                            ri.Data = time;
                        }
                    }
                    else
                    {
                        ri.Msg = "求职信息未失效，无需擦亮！";
                    }
                }
                else
                {
                    ri.Msg = "求职信息不存在！";
                }
            }
            else
            {
                ri.Msg = "求职信息不存在！";
            }
            return Result(ri);
        }
        #endregion
    }
}