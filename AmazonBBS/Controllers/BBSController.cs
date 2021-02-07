using AmazonBBS.BLL;
using AmazonBBS.Common;
using AmazonBBS.Model;
using JiebaNet.Segmenter;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Controllers
{
    public class BBSController : BaseController
    {
        public INoticeService noticeService { get; set; }
        public IUserService _userService { get; set; }
        public IScoreService _scoreService { get; set; }

        #region Topic板块
        public ActionResult Topic(string id)
        {
            if (id.IsNullOrEmpty())
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //var canVisit = true;
                ////如果为other板块，要验证用户是否为VIP用户
                //if (id.ToLower() == "other")
                //{
                //    canVisit = UserBaseBLL.Instance.IsVIP(UserID);
                //}
                //if (canVisit)
                //{

                var isother = id.ToLower() == "other";
                var model = GetBBSList(id);
                if (isother)
                {
                    bool isVip = UserBaseBLL.Instance.IsVIP(UserID);//当前用户是否VIP用户
                    ViewBag.IsOther = true;
                    model.QuestionList?.ForEach(item =>
                    {
                        if (IsLogin)
                        {
                            //是会员 或者 购买过了 则显示
                            item.HideForNoVipUserOrNotBuy = !isVip && DB.BuyOtherLog.FirstOrDefault(a => a.CreateUser == UserID && a.MainID == item.QuestionId) == null;
                        }
                        else
                        {
                            item.HideForNoVipUserOrNotBuy = true;
                        }
                    });
                }
                return View(model);
                //}
                //else
                //{
                //    return Redirect("/user/scoreexchange?showtip=true#scoreexchange");
                //}
            }
        }

        public ActionResult PageB(string id)
        {
            return PartialView("_Search", GetBBSList(id));
        }

        private BBSListViewModel GetBBSList(string id)
        {
            BBSListViewModel model = new BBSListViewModel();
            //列表页
            BBSEnum emodel = BBSEnumBLL.Instance.GetInfo(id);
            if (emodel != null)
            {
                ViewBag.IsOther = id.ToLower() == "other";
                ViewBag.Topic = emodel.EnumDesc;
                ViewBag.TopicCode = emodel.EnumCode.ToLower();
                ViewBag.HasTopicMaster = UserBaseBLL.Instance.HasTopicMaster(emodel.BBSEnumId, UserID);
                model.QuestionPage = InitPage(20);
                model.QuestionList = QuestionBLL.Instance.GetQuestionList(emodel.BBSEnumId, model.QuestionPage);
            }
            return model;
        }
        #endregion

        #region 详细页
        public ActionResult Detail(int id = 0)
        {
            if (id > 0)
            {
                Paging page = InitPage();

                _QuestionInfo model = QuestionBLL.Instance.GetQuestionDetail(id, UserID, page);
                if (model != null)
                {
                    if (model.IsChecked != 2)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        //判断该帖子是否属于other下面的，并判断用户是否为VIP会员
                        bool canVisit = true;
                        if (model.TopicID == 1037)
                        {
                            canVisit = UserBaseBLL.Instance.IsVIP(UserID) || DB.BuyOtherLog.FirstOrDefault(a => a.CreateUser == UserID && a.MainID == id) != null;
                        }
                        if (canVisit)
                        {
                            //如果不匿名 或者 当前登录人为管理员，则 加载作者信息  
                            if (!model.IsAnonymous || model.UserID == UserID || UserBaseBLL.Instance.IsMaster)
                            {
                                model.DiscuzLeftUserInfo = MapperHelper.MapTo<DiscuzLeftUserInfo>(model);
                                model.DiscuzLeftUserInfo.Questions_3 = DB.Question.Where(a => a.IsDelete == 0 && a.UserID == model.UserID && a.IsChecked == 2 && a.QuestionId != id && !a.IsAnonymous).OrderByDescending(a => a.CreateTime).Take(3).ToList();
                            }

                            //获取文章的附件
                            int filetype = AttachEnumType.BBS.GetHashCode();
                            model.AttachMents = DB.AttachMent.Where(a => a.MainType == filetype && a.MainId == id && a.IsDelete == 0).Select(a => new AttachMentWithBuyInfo
                            {
                                FileName = a.FileName,
                                FileSize = a.FileSize,
                                DownCount = a.DownCount,
                                Fee = a.Fee,
                                FeeType = a.FeeType,
                                IsFee = a.IsFee,
                                AttachMentId = a.AttachMentId,
                                CreateTime = a.CreateTime,
                                FilePath = a.FilePath,
                                IsBuy = DB.AttachMentBuyLog.FirstOrDefault(buy => buy.BuyerId == UserID && buy.BuyerId != 0 && buy.AttachMentId == a.AttachMentId && buy.MainID == a.MainId) != null
                            }).ToList();
                        }
                        else
                        {
                            return Redirect("/user/scoreexchange?showtip=true#scoreexchange");
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion

        #region 审核帖子详细页
        public ActionResult Check(int id = 0)
        {
            if (id > 0)
            {
                Paging page = InitPage(5);

                _QuestionInfo model = new _QuestionInfo();
                model = QuestionBLL.Instance.GetQuestionDetail(id, UserID, page);
                if (model == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region 添加问题页面
        [LOGIN]
        public ActionResult Add()
        {
            ViewBag.IsLogin = IsLogin;
            string topic = GetRequest("topic");
            UserExt userext = UserExtBLL.Instance.GetExtInfo(UserID);

            List<SelectListItem> selectList = new List<SelectListItem>();

            selectList.Add(new SelectListItem()
            {
                Selected = true,
                Text = "选择悬赏金额",
                Value = string.Empty
            });

            if (userext != null)
            {
                selectList.Add(new SelectListItem() { Text = "积分10", Value = "score10" });
                selectList.Add(new SelectListItem() { Text = "积分20", Value = "score20" });
                selectList.Add(new SelectListItem() { Text = "积分30", Value = "score30" });
                selectList.Add(new SelectListItem() { Text = "积分40", Value = "score40" });
                selectList.Add(new SelectListItem() { Text = "积分50", Value = "score50" });
                selectList.Add(new SelectListItem() { Text = "积分100", Value = "score100" });
                selectList.Add(new SelectListItem() { Text = "积分150", Value = "score150" });
                selectList.Add(new SelectListItem() { Text = "积分200", Value = "score200" });
                selectList.Add(new SelectListItem() { Text = "积分300", Value = "score300" });
                selectList.Add(new SelectListItem() { Text = "积分400", Value = "score400" });
                selectList.Add(new SelectListItem() { Text = "积分500", Value = "score500" });
            }

            List<SelectListItem> menuselect = new List<SelectListItem>();

            if (string.IsNullOrEmpty(topic))
            {
                menuselect.Add(new SelectListItem()
                {
                    Selected = true,
                    Text = "选择发布板块",
                    Value = string.Empty
                });
            }

            var menulist = BBSEnumBLL.Instance.GetBBSMenus();
            menulist.ForEach(menu =>
            {
                menuselect.Add(new SelectListItem()
                {
                    Text = menu.EnumDesc,
                    Value = menu.BBSEnumId.ToString(),
                    Selected = topic.ToLower() == menu.EnumCode.ToLower()
                });
            });
            ViewBag.selectList = selectList;
            ViewBag.EnumType = menuselect;


            return View();
        }
        #endregion

        #region 提交问题
        /// <summary>
        /// 提交问题
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="questionCoin"></param>
        /// <param name="coin"></param>
        /// <param name="menuType"></param>
        /// <param name="contentNeedFee">true 需要付费 false 不需要付费</param>
        /// <param name="contentFeeType">付费类型 10 积分  20 金钱（vip分）</param>
        /// <param name="contentFee"></param>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult Add(string title, string body, bool questionCoin, string coin, int menuType)
        {
            ResultInfo ri = new ResultInfo();
            string tagsrequest = GetRequest("tags");
            string attachIndex = GetRequest("attachIndexs");
            bool contentNeedFee = GetRequest<bool>("contentNeedFee");
            int contentFeeType = GetRequest<int>("contentFeeType");
            int contentFee = GetRequest<int>("contentFee");
            var fileFee = JsonHelper.JsonToModel<List<FileFee>>("[" + GetRequest("fees") + "]");
            string[] tags = tagsrequest.IsNotNullOrEmpty() ? tagsrequest.Split(',') : null;
            int _coin = 0;
            int coinType = 0;
            List<AttachMent> attachMents = new List<AttachMent>();
            try
            {
                //是否匿名、
                bool isanonymous = GetRequest<bool>("isanonymous");

                //检查积分.金钱 是否足够
                UserExt userext = UserExtBLL.Instance.GetExtInfo(UserID);

                if (questionCoin)
                {
                    if (coin.IndexOf("coin") > -1)
                    {
                        //金钱悬赏
                        coinType = 2;
                        int.TryParse(coin.Replace("coin", string.Empty), out _coin);
                        if (_coin < userext.TotalCoin)
                        {
                            ri.Msg = "金钱不够";
                            return Json(ri, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //积分悬赏
                        coinType = 1;
                        int.TryParse(coin.Replace("score", string.Empty), out _coin);
                        if (_coin > userext.TotalScore)
                        {
                            ri.Msg = "积分不够";
                            return Json(ri, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                var fileOk = true;
                #region 先上传附件再生成数据
                if (attachIndex.IsNotNullOrEmpty())
                {
                    var attachIndexs = attachIndex.Split(',');
                    attachIndexs.ForEach(i =>
                    {
                        var file = Request.Files["file" + i];
                        var fileResult = UpLoadFile(file, "/Content/U/Ques/FuJian", needValidatFile: false, rename: false);
                        fileOk = fileResult.Ok;
                        if (fileResult.Ok)
                        {
                            var attachment = new AttachMent
                            {
                                AttachMentId = Guid.NewGuid(),
                                CreateTime = DateTime.Now,
                                CreateUser = UserID,
                                DownCount = 0,
                                FileName = fileResult.Data.FileName,
                                FilePath = fileResult.Data.FilePath,
                                FileSize = fileResult.Data.FileSize,
                                MainType = AttachEnumType.BBS.GetHashCode(),
                                IsDelete = 0
                            };
                            int _index = i.ToInt32();
                            var attachFeeInfo = fileFee.FirstOrDefault(a => a.Index == _index);
                            if (attachFeeInfo != null)
                            {
                                attachment.FeeType = attachFeeInfo.FeeType;
                                attachment.Fee = attachFeeInfo.Fee;
                                attachment.IsFee = attachFeeInfo.FeeType != 0;
                            }
                            attachMents.Add(attachment);
                        }
                    });
                }
                #endregion

                if (fileOk)
                {
                    BeginTran();

                    Question qmodel = new Question();
                    qmodel.Title = title;
                    qmodel.Body = HttpUtility.UrlDecode(body);
                    qmodel.Coin = _coin;
                    qmodel.CoinType = coinType;
                    qmodel.UserID = UserID;
                    qmodel.PVCount = 0;
                    qmodel.TopicID = menuType;
                    qmodel.IsDelete = 0;
                    qmodel.CreateTime = DateTime.Now;
                    qmodel.UpdateTime = DateTime.Now;
                    qmodel.CreateUser = UserID.ToString();
                    qmodel.UpdateUser = UserID.ToString();
                    qmodel.IsJinghua = 0;
                    qmodel.IsRemen = 0;
                    qmodel.IsTop = 0;
                    qmodel.BestAnswerId = 0;
                    qmodel.NiceAnswerId = 0;
                    //qmodel.FilePath = ri.Url;
                    qmodel.EditCount = 0;

                    qmodel.IsAnonymous = isanonymous;

                    qmodel.ContentNeedPay = contentNeedFee;
                    //内容付费
                    if (contentNeedFee)
                    {
                        qmodel.ContentFeeType = contentFeeType;
                        qmodel.ContentFee = contentFee;
                    }

                    //先判断该用户是否被设置成了需要审核，如果没设置的话，再判断系统系统
                    if (userext.CheckBBS == 1)
                    {
                        qmodel.IsChecked = 1;
                    }
                    else
                    {
                        //判断是否触发审核机制：积分大于1000分时，必须要参加审核
                        int mustcheckscore = Convert.ToInt32(ConfigHelper.AppSettings("MustCheckByScore"));
                        //积分大于多少时自动审核
                        int autocheckscore = Convert.ToInt32(ConfigHelper.AppSettings("AutoCheck_NeedScore"));
                        qmodel.IsChecked = userext.TotalScore >= mustcheckscore ? userext.TotalScore <= autocheckscore ? 1 : 2 : 2;
                    }
                    int result = QuestionBLL.Instance.Add(qmodel, Tran);
                    if (result > 0)
                    {
                        bool ok = true;

                        //创建成功后 第一时间 添加附件数据
                        attachMents.ForEach(attach => attach.MainId = result);

                        if (QuestionBLL.Instance.AddAttachMent(attachMents, Tran))
                        {
                            if (questionCoin)
                            {
                                //用户帐户减去相应的积分或金钱
                                if (UserExtBLL.Instance.SubScore(UserID, _coin, coinType, Tran))
                                {
                                    //记录流水
                                    ScoreCoinLog scorecoinlog = new ScoreCoinLog()
                                    {
                                        UserID = UserID,
                                        Coin = -_coin,
                                        CoinSource = CoinSourceEnum.AskQuestion.GetHashCode(),
                                        CoinTime = DateTime.Now,
                                        CoinType = coinType,
                                        CreateUser = UserID.ToString(),
                                        UserName = UserInfo.UserName
                                    };
                                    ok = ScoreCoinLogBLL.Instance.Add(scorecoinlog, Tran) > 0;
                                }
                            }

                            if (ok)
                            {
                                #region 处理标签
                                //如果用户没有添加标签，则根据算法匹配
                                if (tags == null || tags.Length == 0)
                                {
                                    List<string> newTags = new List<string>();
                                    //获取目前所有标签
                                    var currentTags = DB.Tag.Where(a => a.IsDelete == 0).Select(a => new { tagId = a.TagId, name = a.TagName }).ToList();
                                    var fittler = HtmlRegexHelper.ToText(HttpUtility.UrlDecode(body)) + HtmlRegexHelper.ToText(title);
                                    var segObj = new JiebaSegmenter();
                                    var segs = segObj.Cut(fittler, cutAll: true).GroupBy(a => a).Select(a => a.Key).ToList();
                                    foreach (var seg in segs)
                                    {
                                        var match = currentTags.FirstOrDefault(a => a.name == seg);
                                        if (match != null)
                                        {
                                            if (newTags.Count < 3)
                                            {
                                                newTags.Add(match.tagId.ToString());
                                            }
                                        }
                                        if (newTags.Count == 3)
                                        {
                                            break;
                                        }
                                    }
                                    if (newTags.Count == 0)
                                    {
                                        //如果没有匹配到，则创建
                                        var tagName = WordSpliterHelper.GetKeyword(fittler, true, segs[0]);
                                        if (tagName.Length > 6)
                                        {
                                            tagName = tagName.Substring(0, 6);
                                        }
                                        Tag tag = new Tag()
                                        {
                                            CreateTime = DateTime.Now,
                                            CreateUser = UserID.ToString(),
                                            TagBelongId = menuType,
                                            TagName = tagName,
                                            TagCreateType = 3,
                                            IsDelete = 0,
                                        };
                                        var resultTag = TagBLL.Instance.Add(tag);
                                        newTags.Add(resultTag.ToString());
                                    }
                                    tags = newTags.ToArray();
                                }

                                if (MenuBelongTagBLL.Instance.HandleTags(result, CommentEnumType.BBS, menuType, tags, Tran))
                                {
                                    #endregion

                                    #region 将动态 以邮件形式/站内信开式 发送给关注者
                                    string uri = ConfigHelper.AppSettings("BBSDetail").FormatWith(result);
                                    NoticeBLL.Instance.OnAdd_Notice_Liker(UserInfo.UserName, UserID, uri, title, NoticeTypeEnum.BBS_Add, GetDomainName);
                                    #endregion

                                    if (qmodel.IsChecked == 1)
                                    {
                                        //通知发布用户 和 管理员
                                        noticeService.On_BBS_Article_Publish_Success_Notice(UserInfo, uri, title, 1);

                                        ri.Msg = "提问成功，您发布的帖子成功触发系统审核机制，等待管理员审核成功后即可在页面里查看！";
                                        ri.Url = "/";
                                    }
                                    else
                                    {
                                        ri.Msg = "提问成功！";
                                        ri.Url = uri;
                                    }

                                    #region 给作者添加积分 和 通知

                                    _scoreService.AddScoreOnPublish_BBS_Article(UserID, result, ScoreBeloneMainEnumType.Publish_BBS, CoinSourceEnum.NewBBS);

                                    #endregion

                                    ri.Ok = true;
                                    Commit();
                                }
                                else
                                {
                                    ri.Msg = "添加标签时失败！";
                                    RollBack();
                                }
                            }
                            else
                            {
                                ok = false;
                                ri.Msg = "添加问题失败";
                                RollBack();
                            }
                        }
                        else
                        {
                            ok = false;
                            ri.Msg = "添加附件时失败，请重试";
                            RollBack();
                            //删除附件
                            attachMents.ForEach(attach =>
                            {
                                UploadHelper.DeleteUpFile(attach.FilePath);
                            });
                        }
                    }
                    else
                    {
                        ri.Msg = "提问失败，请重新尝试提问一次！";
                        RollBack();
                    }

                    if (!ri.Ok)
                    {
                        UploadHelper.DeleteUpFile(ri.Url);
                    }
                }
                else
                {
                    //删除附件
                    attachMents.ForEach(attach =>
                    {
                        UploadHelper.DeleteUpFile(attach.FilePath);
                    });
                }
            }
            catch (Exception e)
            {
                ri.Msg = "添加问题失败";
                RollBack();
                //删除附件
                attachMents.ForEach(attach =>
                {
                    UploadHelper.DeleteUpFile(attach.FilePath);
                });
                ErrorBLL.Instance.Log("添加问题失败:" + e.ToString());
            }
            return Result(ri);
        }
        #endregion

        #region 编辑帖子
        [LOGIN]
        public ActionResult Edit(long id = 0)
        {
            if (id > 0)
            {
                Question model = QuestionBLL.Instance.GetModel(id);
                if (model != null)
                {
                    //只能是管理员或者作者本人编辑修改
                    if (UserID == model.UserID || UserBaseBLL.Instance.HasTopicMaster(model.TopicID.Value, UserID))
                    {
                        //if (model.EditCount < 3)
                        //{
                        List<SelectListItem> menuselect = new List<SelectListItem>();

                        var menulist = BBSEnumBLL.Instance.GetBBSMenus();
                        menulist.ForEach(menu =>
                        {
                            menuselect.Add(new SelectListItem()
                            {
                                Text = menu.EnumDesc,
                                Value = menu.BBSEnumId.ToString(),
                                Selected = model.TopicID == menu.BBSEnumId
                            });
                        });
                        ViewBag.EnumType = menuselect;
                        ViewBag.Tags = TagBLL.Instance.GetTagByMainID(1, id);

                        //附件
                        int filetype = AttachEnumType.BBS.GetHashCode();
                        ViewBag.AttachMents = DB.AttachMent.Where(a => a.MainType == filetype && a.MainId == id).ToList();
                        return View(model);
                        //}
                        //else
                        //{
                        //    return Content("<script>alert('您已修改过3次了！不能再修改！')</script>");
                        //}
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

        [LOGIN]
        [HttpPost]
        public ActionResult Edit(long id, string title, string body, int menuType)
        //public ActionResult Edit(long id, string title, string body, int menuType, bool isdelete = false, bool ischanged = false)
        {
            ResultInfo ri = new ResultInfo();
            string tagsrequest = GetRequest("tags");
            string[] tags = tagsrequest.IsNotNullOrEmpty() ? tagsrequest.Split(',') : null;

            Question qmodel = QuestionBLL.Instance.GetModel(id);
            if (qmodel != null)
            {
                if (UserID == qmodel.UserID || UserBaseBLL.Instance.HasTopicMaster(qmodel.TopicID.Value, UserID))
                {
                    if (qmodel.EditCount < 3)
                    {
                        if (qmodel.IsDelete == 0)
                        {
                            //是否匿名、
                            bool isanonymous = GetRequest<bool>("isanonymous");

                            bool contentNeedFee = GetRequest<bool>("contentNeedFee");
                            int contentFeeType = GetRequest<int>("contentFeeType");
                            int contentFee = GetRequest<int>("contentFee");
                            string attachIndex = GetRequest("attachIndexs");
                            var fileFee = JsonHelper.JsonToModel<List<FileFee>>("[" + GetRequest("fees") + "]");//附件费用

                            var fileOk = true;
                            List<AttachMent> attachMents_new = new List<AttachMent>();
                            List<string> removePaths = new List<string>();
                            BeginTran();
                            try
                            {
                                #region 判断附件是否更改了。更改则先删除之前的再上传后来的
                                if (attachIndex.IsNotNullOrEmpty())
                                {
                                    var attachIndexs = attachIndex.Split(',');

                                    //原有附件ID
                                    var maintype = AttachEnumType.BBS.GetHashCode();
                                    var oldAttachMentIds = DB.AttachMent.Where(a => a.MainId == id && a.MainType == maintype).ToList();

                                    oldAttachMentIds.ForEach(attach =>
                                    {
                                        if (fileFee.FirstOrDefault(a => a.AttachId == attach.AttachMentId) == null)
                                        {
                                            removePaths.Add(attach.FilePath);
                                            DB.AttachMent.Remove(attach);
                                        }
                                    });

                                    attachIndexs.ForEach(i =>
                                    {
                                        int _index = i.ToInt32();
                                        var feeinfo = fileFee.FirstOrDefault(fee => fee.Index == _index);
                                        if (feeinfo != null)
                                        {
                                            if (feeinfo.isNew)
                                            {
                                                //新增
                                                var file = Request.Files["file" + i];
                                                var fileResult = UpLoadFile(file, "/Content/U/Art/FuJian", needValidatFile: false, rename: false);
                                                fileOk = fileResult.Ok;
                                                if (fileResult.Ok)
                                                {
                                                    attachMents_new.Add(new AttachMent()
                                                    {
                                                        AttachMentId = Guid.NewGuid(),
                                                        CreateTime = DateTime.Now,
                                                        CreateUser = UserID,
                                                        DownCount = 0,
                                                        FileName = fileResult.Data.FileName,
                                                        FilePath = fileResult.Data.FilePath,
                                                        FileSize = fileResult.Data.FileSize,
                                                        MainType = AttachEnumType.BBS.GetHashCode(),
                                                        IsDelete = 0,
                                                        FeeType = feeinfo.FeeType,
                                                        Fee = feeinfo.Fee,
                                                        IsFee = feeinfo.FeeType != 0,
                                                        MainId = id
                                                    });
                                                }
                                            }
                                            else if (feeinfo.IsChange || feeinfo.FeeTypeChange || feeinfo.FeeChange)
                                            {
                                                //替换
                                                var oldAttachModel = DB.AttachMent.FirstOrDefault(a => a.AttachMentId == feeinfo.AttachId);
                                                if (oldAttachModel != null)
                                                {
                                                    if (feeinfo.IsChange)
                                                    {
                                                        //先保存后删除
                                                        string oldPath = oldAttachModel.FilePath;
                                                        var file = Request.Files["file" + i];
                                                        var fileResult = UpLoadFile(file, "/Content/U/Art/FuJian", needValidatFile: false, rename: false);
                                                        fileOk = fileResult.Ok;
                                                        if (fileResult.Ok)
                                                        {
                                                            oldAttachModel.FileName = fileResult.Data.FileName;
                                                            oldAttachModel.FileSize = fileResult.Data.FileSize;
                                                            oldAttachModel.FilePath = fileResult.Data.FilePath;
                                                            removePaths.Add(oldPath);
                                                        }
                                                    }
                                                    if (feeinfo.FeeTypeChange)
                                                    {
                                                        oldAttachModel.FeeType = feeinfo.FeeType;
                                                    }
                                                    if (feeinfo.FeeChange)
                                                    {
                                                        oldAttachModel.Fee = feeinfo.Fee;
                                                    }
                                                }
                                            }
                                        }
                                    });
                                }
                                //判断附件是否更改了。更改则先删除之前的再上传后来的
                                //if (ischanged || isdelete)
                                //{
                                //    UploadHelper.DeleteUpFile(qmodel.FilePath);
                                //    if (ischanged)
                                //    {
                                //        newri = UpLoadFile(Request.Files["file"], "/Content/U/Ques/FuJian", needValidatFile: false, rename: false);
                                //    }
                                //}
                                #endregion

                                if (fileOk)
                                {
                                    //添加新附件
                                    if (QuestionBLL.Instance.AddAttachMent(attachMents_new, Tran))
                                    {
                                        //先将旧的标签全部删除，再新增编辑后的标签，不管有没有改变都这么执行
                                        if (MenuBelongTagBLL.Instance.RemoveAndAddNew(id, menuType, CommentEnumType.BBS, tags, Tran))
                                        {
                                            qmodel.Title = title;
                                            qmodel.Body = HttpUtility.UrlDecode(body);
                                            qmodel.TopicID = menuType;
                                            //qmodel.FilePath = newri.Url;
                                            qmodel.UpdateTime = DateTime.Now;
                                            qmodel.UpdateUser = UserID.ToString();

                                            qmodel.IsAnonymous = isanonymous;

                                            qmodel.ContentNeedPay = contentNeedFee;
                                            //内容付费
                                            if (contentNeedFee)
                                            {
                                                qmodel.ContentFeeType = contentFeeType;
                                                qmodel.ContentFee = contentFee;
                                            }

                                            if (!UserBaseBLL.Instance.IsMaster)
                                            {
                                                ++qmodel.EditCount;
                                            }
                                            ri.Ok = QuestionBLL.Instance.Edit(qmodel, Tran);
                                            if (ri.Ok)
                                            {
                                                Commit();
                                                ri.Url = $"/bbs/detail/{id}";
                                                DB.SaveChanges();
                                                //删除旧文件
                                                removePaths.ForEach(item =>
                                                {
                                                    UploadHelper.DeleteUpFile(item);
                                                });
                                            }
                                            else
                                            {
                                                RollBack();
                                            }
                                        }
                                        else
                                        {
                                            attachMents_new.ForEach(item =>
                                            {
                                                UploadHelper.DeleteUpFile(item.FilePath);
                                            });
                                            RollBack();
                                        }
                                    }
                                    else
                                    {
                                        attachMents_new.ForEach(item =>
                                        {
                                            UploadHelper.DeleteUpFile(item.FilePath);
                                        });
                                        RollBack();
                                    }
                                }
                                else
                                {
                                    attachMents_new.ForEach(item =>
                                    {
                                        UploadHelper.DeleteUpFile(item.FilePath);
                                    });
                                    RollBack();
                                }
                            }
                            catch (Exception e)
                            {
                                attachMents_new.ForEach(item =>
                                {
                                    UploadHelper.DeleteUpFile(item.FilePath);
                                });
                                RollBack();
                            }
                        }
                        else
                        {
                            ri.Msg = "帖子已被删除，不能修改！";
                        }
                    }
                    else
                    {
                        ri.Msg = "该贴已被修改3次，不能再修改！请在留言区进行追问！";
                    }
                }
                else
                {
                    ri.Msg = "你不能编辑别人的帖子！本次行为已被记录！多次恶意操作将被封号！";
                }
            }
            else
            {
                ri.Msg = "帖子不存在！";
            }
            return Result(ri);
        }
        #endregion

        #region 关注
        [HttpPost]
        [LOGIN]
        public ActionResult Like(int id, int type, bool unlike = false)
        {
            ResultInfo ri = new ResultInfo();

            //判断是否已关注过
            int likeId = UserLikeBLL.Instance.IsLiked(id, type, UserID);
            if (likeId == 0)
            {
                if (unlike)
                {
                    ri.Msg = $"你还未{LikeDesc(type)}";
                }
                else
                {
                    //关注
                    UserLike likeModel = new UserLike()
                    {
                        LikeTargetID = id,
                        LikeTime = DateTime.Now,
                        LikeType = type,
                        UserID = UserID,
                        IsDelete = 0,
                    };

                    int result = UserLikeBLL.Instance.Add(likeModel);
                    if (result > 0)
                    {
                        long belikedUserId = 0;
                        //关注 问题/文章 成功的 自动关注该作者
                        if (type != 3)
                        {
                            string queryTable = string.Empty;
                            string uesrColumn = string.Empty;
                            string mainUrl = string.Empty;
                            string mainDesc = string.Empty;
                            string mainName = string.Empty;
                            NoticeTypeEnum noticeType = NoticeTypeEnum.None;
                            switch (type)
                            {
                                case 1:
                                    queryTable = "question";
                                    uesrColumn = "UserID";
                                    mainDesc = "帖子";
                                    mainUrl = ConfigHelper.AppSettings("BBSDetail").FormatWith(id);
                                    noticeType = NoticeTypeEnum.BBS_BeLiked;
                                    mainName = QuestionBLL.Instance.GetModel(id)?.Title;
                                    break;
                                case 2:
                                    queryTable = "article";
                                    uesrColumn = "UserID";
                                    mainDesc = "文章";
                                    mainUrl = ConfigHelper.AppSettings("ArticleDetail").FormatWith(id);
                                    noticeType = NoticeTypeEnum.Article_BeLiked;
                                    mainName = ArticleBLL.Instance.GetModel(id)?.Title;
                                    break;
                                case 4:
                                    queryTable = "ZhaoPin";
                                    uesrColumn = "Publisher";
                                    mainDesc = "招聘";
                                    mainUrl = ConfigHelper.AppSettings("ZhaoPinDetail").FormatWith(id);
                                    noticeType = NoticeTypeEnum.ZhaoPin_BeLiked;
                                    mainName = ZhaoPinBLL.Instance.GetModel(id)?.Gangwei;
                                    break;
                                case 5:
                                    queryTable = "QiuZhi";
                                    uesrColumn = "Publisher";
                                    mainDesc = "求职";
                                    mainUrl = ConfigHelper.AppSettings("QiuZhiDetail").FormatWith(id);
                                    noticeType = NoticeTypeEnum.QiuZhi_BeLiked;
                                    mainName = QiuZhiBLL.Instance.GetModel(id)?.IWant;
                                    break;
                            }
                            belikedUserId = Convert.ToInt32(UserLikeBLL.Instance.FindUserIdByID(queryTable, uesrColumn, id));
                            //判断是否已关注用户
                            if (UserLikeBLL.Instance.IsLiked(belikedUserId, 3, UserID) == 0)
                            {
                                UserLike likeusermodel = new UserLike()
                                {
                                    LikeTargetID = belikedUserId,
                                    LikeTime = DateTime.Now,
                                    LikeType = 3,
                                    UserID = UserID,
                                    IsDelete = 0
                                };
                                UserLikeBLL.Instance.Add(likeusermodel);
                            }
                            //关注成功通知被关注者
                            NoticeBLL.Instance.OnLikeItem_Notice_User(belikedUserId, UserInfo.UserName, mainDesc, mainUrl, mainName, GetDomainName, noticeType);
                        }
                        else
                        {
                            belikedUserId = id;
                        }
                        ri.Msg = $"关注成功,\r\n你将会以邮件接收到{LikeDesc(type)}的后续相关动态";
                        ri.Ok = true;

                    }
                    else
                    {
                        ri.Msg = "关注失败，稍后再试";
                    }
                }
            }
            else
            {
                if (unlike)
                {
                    bool result = UserLikeBLL.Instance.UnLike(likeId);
                    if (result)
                    {
                        ri.Msg = "取消关注成功";
                        ri.Ok = true;
                    }
                    else
                    {
                        ri.Msg = "取消关注失败，稍后再试";
                    }
                }
                else
                {
                    ri.Msg = "您已关注过了";
                }
            }
            return Json(ri, JsonRequestBehavior.AllowGet);
        }

        private string LikeDesc(int type)
        {
            string desc = string.Empty;
            switch (type)
            {
                case 1:
                    desc = "关注问题的提问者";
                    break;
                case 2:
                    desc = "关注文章的作者";
                    break;
                case 3:
                    desc = "关注的用户";
                    break;
                case 4: desc = "招聘"; break;
                case 5: desc = "求职"; break;
                default: desc = "关注"; break;
            }
            return desc;
        }
        #endregion

        #region 最佳答案
        [HttpPost]
        [LOGIN]
        public ActionResult Bset(long id, long qid)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                //判断回答是否为匿名 不能被采取
                if (!DB.Comment.FirstOrDefault(a => a.CommentId == id).IsAnonymous)
                {
                    Question qmodel = QuestionBLL.Instance.GetModel(qid);
                    //判断是否已采纳过
                    if (qmodel.IsDelete == 0)
                    {
                        if (qmodel.BestAnswerId == 0)
                        {
                            BeginTran();

                            long commentUserID = 0;
                            if (QuestionBLL.Instance.BsetAnswer(id, qid, Tran))
                            {
                                //如果问题设置了悬赏，则更新回答者的积分/金钱明细
                                if (qmodel.CoinType != 0)
                                {
                                    //更新回答者的明细
                                    commentUserID = CommentBLL.Instance.FindUserIDByCommentID(id, Tran);
                                    bool updateOk = UserExtBLL.Instance.AddScore(commentUserID, (qmodel.Coin ?? 10) * 7 / 10, (int)qmodel.CoinType, Tran);
                                    if (updateOk)
                                    {
                                        //记录变更明细
                                        ScoreCoinLog sclModel = new ScoreCoinLog()
                                        {
                                            Coin = (qmodel.Coin ?? 10) * 7 / 10,
                                            CoinSource = CoinSourceEnum.Comment.GetHashCode(),
                                            CoinTime = DateTime.Now,
                                            CoinType = qmodel.CoinType.Value,
                                            CreateUser = "System",
                                            UserID = UserID,
                                            UserName = UserInfo.UserName
                                        };
                                        if (ScoreCoinLogBLL.Instance.Add(sclModel, Tran) > 0)
                                        {
                                            ri.Ok = true;
                                            ri.Msg = "采纳成功，相应积分已赠送给回答者";
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
                                else
                                {
                                    ri.Ok = true;
                                    Commit();
                                }
                                if (ri.Ok)
                                {
                                    //采纳通知回答者
                                    NoticeBLL.Instance.OnBestAnswer_Notice_Author(UserBaseBLL.Instance.GetUserInfo(qmodel.UserID.Value).UserName, qmodel.QuestionId, qmodel.Title, commentUserID, qmodel.CoinType != 0 ? qmodel.Coin.Value : 0, GetDomainName, NoticeTypeEnum.BestAnswer);
                                }
                            }
                            else
                            {
                                ri.Msg = "采纳失败，稍后再试";
                                RollBack();
                            }
                        }
                        else
                        {
                            ri.Msg = "已采纳最佳答案";
                        }
                    }
                    else
                    {
                        ri.Msg = "问题失效";
                    }
                }
                else
                {
                    ri.Msg = "匿名回答不能被采纳！";
                }
            }
            else
            {
                ri.Msg = "问题异常";
            }
            return Json(ri, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 消费查看答案 
        [HttpPost]
        public ActionResult FeeAnswer(int id)
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                //查找
                Comment comment = CommentBLL.Instance.GetModel(id);
                if (comment != null)
                {
                    BeginTran();

                    int cointype = (int)comment.FeeCoinType;
                    int coin = (int)comment.NeedCoin;
                    if (UserExtBLL.Instance.HasEnoughCoin(cointype, coin, UserID))
                    {
                        //用户帐户减去相应的积分或金钱
                        if (UserExtBLL.Instance.SubScore(UserID, coin, cointype, Tran))
                        {
                            //记录流水
                            ScoreCoinLog scorecoinlog = new ScoreCoinLog()
                            {
                                UserID = UserID,
                                Coin = -coin,
                                CoinSource = CoinSourceEnum.SeeAnswer.GetHashCode(),
                                CoinTime = DateTime.Now,
                                CoinType = cointype,
                                CreateUser = UserID.ToString(),
                                UserName = UserInfo.UserName
                            };
                            if (ScoreCoinLogBLL.Instance.Add(scorecoinlog, Tran) > 0)
                            {

                                #region 购买答案记录
                                FeeAnswerLog feelog = new FeeAnswerLog()
                                {
                                    AnswerId = comment.CommentId,
                                    UserID = UserID,
                                    FeeTime = DateTime.Now,
                                    IsDelete = 0
                                };
                                ri.Ok = FeeAnswerLogBLL.Instance.Add(feelog, Tran) > 0;
                                #endregion

                                #region 回答者账户 并记录流水
                                if (ri.Ok)
                                {
                                    //回答者账户 并记录流水
                                    long commentUserID = (long)comment.CommentUserID;
                                    if (UserExtBLL.Instance.AddScore(commentUserID, coin, cointype, Tran))
                                    {
                                        //记录流水
                                        ScoreCoinLog scorecoinlog_AnswerUser = new ScoreCoinLog()
                                        {
                                            UserID = commentUserID,
                                            Coin = coin,
                                            CoinSource = CoinSourceEnum.BeSeeAnswer.GetHashCode(),
                                            CoinTime = DateTime.Now,
                                            CoinType = cointype,
                                            CreateUser = commentUserID.ToString(),
                                            UserName = UserBaseBLL.Instance.GetUserNameByUserID(commentUserID)
                                        };
                                        if (ScoreCoinLogBLL.Instance.Add(scorecoinlog_AnswerUser) > 0)
                                        {
                                            ri.Ok = true;
                                            ri.Msg = "答案购买成功";
                                            ri.Data = comment.CommentContent;//答案
                                            Commit();
                                        }
                                        else
                                        {
                                            ri.Msg = "消费失败";
                                            RollBack();
                                        }
                                    }
                                    else
                                    {
                                        ri.Msg = "消费失败";
                                        RollBack();
                                    }
                                }
                                else
                                {
                                    ri.Msg = "消费失败";
                                    RollBack();
                                }
                                #endregion
                            }
                            else
                            {
                                ri.Msg = "消费失败";
                                RollBack();
                            }
                        }
                        else
                        {
                            ri.Msg = "消费失败";
                            RollBack();
                        }
                    }
                    else
                    {
                        ri.Msg = $"你的{(cointype == 1 ? "积分" : "金钱")}不足够";
                    }
                }
                else
                {
                    ri.Msg = "答案不存在";
                }
            }
            catch
            {
                ri.Msg = "异常";
                RollBack();
            }
            return Result(ri);
        }
        #endregion

        #region 修改浏览量
        [IsMaster]
        [HttpPost]
        public ActionResult SetPV(long id, int number)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                if (number > 0)
                {
                    var bll = QuestionBLL.Instance;
                    Question model = bll.GetModel(id);
                    model.PVCount = number;
                    ri = bll.Update(model);
                }
                else
                {
                    ri.Msg = "浏览量必须大于0";
                }
            }
            else
            {
                ri.Msg = "异常";
            }
            return Result(ri);
        }
        #endregion

        #region 删除问题
        [IsMaster]
        [HttpPost]
        public ActionResult DeleteQ(int id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var bll = QuestionBLL.Instance;
                Question model = bll.GetModel(id);
                model.IsDelete = 1;
                ri = bll.Update(model);
            }
            else
            {
                ri.Msg = "异常";
            }
            return Result(ri);
        }
        #endregion

        #region 设置精华(1)、热门(2)、置顶(3)
        [IsMaster]
        [HttpPost]
        public ActionResult SetPropertity(int id, int type, int action)
        {
            ResultInfo
                 ri = new ResultInfo();
            if (id > 0)
            {
                if (type > 0 && type < 4)
                {
                    QuestionBLL bll = QuestionBLL.Instance;
                    Question model = bll.GetModel(id);
                    if (model != null)
                    {
                        int flag = action == 1 ? 1 : 0;
                        if (type == 1)
                        {
                            model.IsJinghua = flag;
                        }
                        else if (type == 2)
                        {
                            model.IsRemen = flag;
                        }
                        else
                        {
                            model.IsTop = flag;
                        }
                        ri = bll.Update(model);
                    }
                    else
                    {
                        ri.Msg = "该帖不存在";
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

        #region 搜索
        [HttpGet]
        public ActionResult Search(string key, string topic)
        {
            ResultInfo ri = new ResultInfo();

            if (key.IsNotNullOrEmpty())
            {
                BBSEnum emodel = BBSEnumBLL.Instance.GetInfo(topic);
                if (emodel == null)
                {
                    ri.Msg = "贴吧类型异常";
                }
                else
                {
                    BBSListViewModel model = new BBSListViewModel();
                    model.QuestionPage = InitPage(20);
                    model.QuestionList = QuestionBLL.Instance.GetQuestionList(emodel.BBSEnumId, model.QuestionPage, key);
                    ViewBag.HasTopicMaster = UserBaseBLL.Instance.HasTopicMaster(emodel.BBSEnumId, UserID);

                    var isother = topic.ToLower() == "other";
                    if (isother)
                    {
                        bool isVip = UserBaseBLL.Instance.IsVIP(UserID);//当前用户是否VIP用户
                        ViewBag.IsOther = true;
                        model.QuestionList?.ForEach(item =>
                        {
                            if (IsLogin)
                            {
                                //是会员 或者 购买过了 则显示
                                item.HideForNoVipUserOrNotBuy = !isVip && DB.BuyOtherLog.FirstOrDefault(a => a.CreateUser == UserID && a.MainID == item.QuestionId) == null;
                            }
                            else
                            {
                                item.HideForNoVipUserOrNotBuy = true;
                            }
                        });
                    }

                    return PartialView("_Search", model);
                }
            }
            else
            {
                ri.Msg = "请输入关键词";
            }

            return Result(ri);
        }
        #endregion

        #region 获取附件详情
        public ActionResult FileInfo(long id)
        {
            ResultInfo ri = new ResultInfo();



            return Json(id);
        }
        #endregion

        #region 下载附件
        public void DownLoad(long id)
        {
            if (id > 0)
            {
                var model = QuestionBLL.Instance.GetModel(id);
                if (model != null)
                {
                    if (model.FilePath.IsNotNullOrEmpty())
                    {
                        string fileName = model.Title.Substring(0, model.Title.Length > 10 ? 10 : model.Title.Length) + "." + model.FilePath.Split('.').Last();
                        string filePath = Server.MapPath(model.FilePath);
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

        #region 付费查看Other专题
        [LOGIN]
        [HttpPost]
        public ActionResult Feeseeother(long id)
        {
            ResultInfo ri = new ResultInfo();

            var question = DB.Question.FirstOrDefault(a => a.QuestionId == id);
            if (question != null)
            {
                if (question.TopicID == 1037)
                {
                    //判断用户是否已付过费
                    var log = DB.BuyOtherLog.FirstOrDefault(a => a.MainID == id && a.CreateUser == UserID);
                    if (log == null)
                    {
                        var fee = ConfigHelper.AppSettings("normalUserSee_Score").ToInt32();
                        var result = _scoreService.HasEnoughCoinAndSubCoin(1, fee, UserID, CoinSourceEnum.FeeBBS_Orher, false);
                        if (result.Item1)
                        {
                            //添加付费记录
                            DB.BuyOtherLog.Add(new BuyOtherLog
                            {
                                CreateTime = DateTime.Now,
                                CreateUser = UserID,
                                Fee = fee,
                                MainID = id,
                            });
                            DB.SaveChanges();
                            ri.Ok = true;
                            ri.Data = "/bbs/detail/" + id.ToString();
                        }
                        else
                        {
                            ri.Msg = result.Item2;
                        }
                    }
                    else
                    {
                        ri.Msg = "您已购买过了，可以直接浏览，正在跳转";
                        ri.Data = "/bbs/detail/" + id.ToString();
                    }
                }
                else
                {
                    ri.Msg = "您要查看的帖子不属于付费范畴";
                }
            }
            else
            {
                ri.Msg = "查看的目标不存在";
            }

            return Result(ri);
        }
        #endregion
    }
}