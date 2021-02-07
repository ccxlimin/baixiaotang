using AmazonBBS.BLL;
using AmazonBBS.Common;
using AmazonBBS.Model;
using JiebaNet.Segmenter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Controllers
{
    public class ArticleController : BaseController
    {
        public INoticeService noticeService { get; set; }

        private readonly IScoreService _scoreService;
        public ArticleController(IScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        #region 文章列表
        public ActionResult Index()
        {
            var model = GetArticleList();
            return View(model);
        }

        public ActionResult PageA()
        {
            var model = GetArticleList();
            return PartialView("_Search", model);
        }

        private ArticleViewModel GetArticleList()
        {
            ArticleViewModel model = new ArticleViewModel();
            model.ARticlePage = InitPage(20);
            model.Articles = ArticleBLL.Instance.GetAllArticles(model.ARticlePage);
            return model;
        }
        #endregion

        #region 文章明细
        public ActionResult Detail(long id = 0)
        {
            if (id > 0)
            {
                Paging page = InitPage();
                _Article model = ArticleBLL.Instance.GetArticleDetail(id, UserID, page);

                if (model != null)
                {
                    if (!model.IsAnonymous || UserBaseBLL.Instance.IsMaster)
                    {
                        model.DiscuzLeftUserInfo = MapperHelper.MapTo<DiscuzLeftUserInfo>(model);
                        model.DiscuzLeftUserInfo.Articles_3 = DB.Article.Where(a => a.IsChecked == 2 && a.IsDelete == 0 && a.UserID == model.UserID && a.ArticleId != id && !a.IsAnonymous).OrderByDescending(a => a.CreateTime).Take(3).ToList();
                    }
                    //获取文章的附件
                    int filetype = AttachEnumType.Article.GetHashCode();
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
                    return View(model);
                }
                else
                {
                    return RedirectToAction("/Index", "Article");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region 发表文章
        [CanPublishArticle]
        /// <summary>
        /// 发表文章
        /// </summary>
        /// <returns></returns>
        public ActionResult Publish()
        {
            return View();
        }

        [HttpPost]
        [CanPublishArticle]
        public ActionResult Publish(string title, string body)
        {
            ResultInfo ri = new ResultInfo();
            if (!string.IsNullOrEmpty(title))
            {
                if (!string.IsNullOrEmpty(body))
                {
                    //是否匿名、
                    bool isanonymous = GetRequest<bool>("isanonymous");

                    bool contentNeedFee = GetRequest<bool>("contentNeedFee");
                    int contentFeeType = GetRequest<int>("contentFeeType");
                    int contentFee = GetRequest<int>("contentFee");
                    string attachIndex = GetRequest("attachIndexs");
                    var fileFee = JsonHelper.JsonToModel<List<FileFee>>("[" + GetRequest("fees") + "]");//附件费用

                    string tagsrequest = GetRequest("tags");
                    string[] tags = tagsrequest.IsNotNullOrEmpty() ? tagsrequest.Split(',') : null;

                    List<AttachMent> attachMents = new List<AttachMent>();
                    try
                    {
                        var fileOk = true;

                        #region 先上传附件再生成数据
                        if (attachIndex.IsNotNullOrEmpty())
                        {
                            var attachIndexs = attachIndex.Split(',');
                            attachIndexs.ForEach(i =>
                            {
                                var file = Request.Files["file" + i];
                                var fileResult = UpLoadFile(file, "/Content/U/Art/FuJian", needValidatFile: false, rename: false);
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
                                        MainType = AttachEnumType.Article.GetHashCode(),
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
                            Article model = new Article()
                            {
                                Body = HttpUtility.UrlDecode(body),
                                CreateTime = DateTime.Now,
                                IsDelete = 0,
                                Title = title,
                                UserID = UserID,
                                PVCount = 0,
                                //FilePath = fileResultInfo.Url,
                                EditCount = 0,
                                CreateUser = UserID.ToString(),
                                UpdateUser = UserID.ToString(),
                                UpdateTime = DateTime.Now,

                                IsAnonymous = isanonymous,
                            };

                            model.ContentNeedPay = contentNeedFee;
                            //内容付费
                            if (contentNeedFee)
                            {
                                model.ContentFeeType = contentFeeType;
                                model.ContentFee = contentFee;
                            }

                            UserExt userext = UserExtBLL.Instance.GetExtInfo(UserID);

                            //先判断该用户是否被设置成了需要审核，如果没设置的话，再判断系统系统
                            if (userext.CheckBBS == 1)
                            {
                                model.IsChecked = 1;
                            }
                            else
                            {
                                //判断是否触发审核机制：积分大于1000分时，必须要参加审核
                                int mustcheckscore = Convert.ToInt32(ConfigHelper.AppSettings("MustCheckByScore"));
                                //积分大于多少时自动审核
                                int autocheckscore = Convert.ToInt32(ConfigHelper.AppSettings("AutoCheck_NeedScore"));
                                model.IsChecked = userext.TotalScore >= mustcheckscore ? userext.TotalScore <= autocheckscore ? 1 : 2 : 2;
                            }
                            int result = ArticleBLL.Instance.Add(model, Tran);
                            if (result > 0)
                            {
                                //创建成功后 第一时间 添加附件数据
                                attachMents.ForEach(attach => attach.MainId = result);
                                if (QuestionBLL.Instance.AddAttachMent(attachMents, Tran))
                                {
                                    //处理标签
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
                                            var tagName = WordSpliterHelper.GetKeyword(fittler, true);
                                            if (tagName.Length > 6)
                                            {
                                                tagName = tagName.Substring(0, 6);
                                            }
                                            Tag tag = new Tag()
                                            {
                                                CreateTime = DateTime.Now,
                                                CreateUser = UserID.ToString(),
                                                TagBelongId = 1038,
                                                TagName = tagName,
                                                TagCreateType = 3,
                                                IsDelete = 0,
                                            };
                                            var resultTag = TagBLL.Instance.Add(tag);
                                            newTags.Add(resultTag.ToString());
                                        }
                                        tags = newTags.ToArray();
                                    }

                                    if (MenuBelongTagBLL.Instance.HandleTags(result, CommentEnumType.Article, 1038, tags, Tran))
                                    {
                                        string uri = ConfigHelper.AppSettings("ArticleDetail").FormatWith(result);
                                        if (model.IsChecked == 1)
                                        {
                                            //通知发布用户 和 管理员
                                            noticeService.On_BBS_Article_Publish_Success_Notice(UserInfo, uri, title, 2);
                                            ri.Msg = "文章发表成功，您发布的文章成功触发系统审核机制，等待管理员审核成功后即可在页面里查看";
                                            ri.Url = "/Article";
                                        }
                                        else
                                        {
                                            ri.Msg = "文章发表成功";
                                            ri.Url = uri;
                                        }
                                        //异步通知关注者
                                        NoticeBLL.Instance.OnAdd_Notice_Liker(UserInfo.UserName, UserID, uri, model.Title, NoticeTypeEnum.Article_Add, GetDomainName);
                                        _scoreService.AddScoreOnPublish_BBS_Article(UserID, result, ScoreBeloneMainEnumType.Publish_Article, CoinSourceEnum.NewArticle);
                                        Commit();
                                        ri.Ok = true;
                                    }
                                    else
                                    {
                                        RollBack();
                                    }
                                }
                                else
                                {
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
                                RollBack();
                                ri.Msg = "文章发表失败";
                                //删除附件
                                attachMents.ForEach(attach =>
                                {
                                    UploadHelper.DeleteUpFile(attach.FilePath);
                                });
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
                        ri.Msg = "新增文章失败";
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
                    ri.Msg = "文章内容不能为空";
                }
            }
            else
            {
                ri.Msg = "文章标题不能为空";
            }
            return Result(ri);
        }

        #endregion

        #region 文章PV
        public ActionResult PV(long id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var model = ArticleBLL.Instance.GetModel(id);
                if (model != null)
                {
                    if (ArticleBLL.Instance.UpdataPV(model.ArticleId))
                    {
                        ri.Ok = true;
                    }
                }
            }
            return Result(ri);
        }
        #endregion

        #region 编辑文章

        //[IsMaster]
        [LOGIN]
        public ActionResult Edit(long id = 0)
        {
            if (id > 0)
            {
                Article model = ArticleBLL.Instance.GetModel(id);
                if (model != null)
                {
                    if (model.UserID == UserID || UserBaseBLL.Instance.IsMaster)
                    {
                        //获取文章的附件
                        int filetype = AttachEnumType.Article.GetHashCode();
                        ViewBag.AttachMents = DB.AttachMent.Where(a => a.MainType == filetype && a.MainId == id).ToList();
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

        //[IsMaster]
        [LOGIN]
        [HttpPost]
        public ActionResult Edit(long id, string title, string body)
        //public ActionResult Edit(long id, string title, string body, bool isdelete = false, bool ischanged = false)
        {
            ResultInfo ri = new ResultInfo();
            Article amodel = ArticleBLL.Instance.GetModel(id);
            if (amodel.EditCount < 3)
            {
                if (amodel.IsDelete == 0)
                {
                    //是否匿名、
                    bool isanonymous = GetRequest<bool>("isanonymous");

                    bool contentNeedFee = GetRequest<bool>("contentNeedFee");
                    int contentFeeType = GetRequest<int>("contentFeeType");
                    int contentFee = GetRequest<int>("contentFee");
                    string attachIndex = GetRequest("attachIndexs");
                    var fileFee = JsonHelper.JsonToModel<List<FileFee>>("[" + GetRequest("fees") + "]");//附件费用

                    string tagsrequest = GetRequest("tags");
                    string[] tags = tagsrequest.IsNotNullOrEmpty() ? tagsrequest.Split(',') : null;

                    //var newri = new ResultInfo<AttachMent>() { Ok = true };
                    var fileOk = true;

                    //判断附件是否更改了。更改则先删除之前的再上传后来的
                    //if (ischanged || isdelete)
                    //{
                    //    UploadHelper.DeleteUpFile(amodel.FilePath);
                    //    if (ischanged)
                    //    {
                    //        newri = UpLoadFile(Request.Files["file"], "/Content/U/Art/FuJian", needValidatFile: false, rename: false);
                    //    }
                    //} 

                    BeginTran();

                    List<AttachMent> attachMents_new = new List<AttachMent>();
                    List<string> removePaths = new List<string>();
                    try
                    {
                        #region 判断附件是否更改了。更改则先删除之前的再上传后来的
                        if (attachIndex.IsNotNullOrEmpty())
                        {
                            var attachIndexs = attachIndex.Split(',');

                            //原有附件ID
                            var maintype = AttachEnumType.Article.GetHashCode();
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
                                                MainType = AttachEnumType.Article.GetHashCode(),
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

                        #endregion

                        if (fileOk)
                        {
                            //添加新附件
                            if (QuestionBLL.Instance.AddAttachMent(attachMents_new, Tran))
                            {
                                //先将旧的标签全部删除，再新增编辑后的标签，不管有没有改变都这么执行
                                if (MenuBelongTagBLL.Instance.RemoveAndAddNew(id, 1038, CommentEnumType.Article, tags, Tran))
                                {
                                    amodel.Title = title;
                                    amodel.Body = HttpUtility.UrlDecode(body);
                                    //amodel.FilePath = newri.Url;
                                    amodel.UpdateTime = DateTime.Now;
                                    amodel.UpdateUser = UserID.ToString();

                                    amodel.IsAnonymous = isanonymous;

                                    if (!UserBaseBLL.Instance.IsMaster)
                                    {
                                        ++amodel.EditCount;
                                    }

                                    amodel.ContentNeedPay = contentNeedFee;
                                    //内容付费
                                    if (contentNeedFee)
                                    {
                                        amodel.ContentFeeType = contentFeeType;
                                        amodel.ContentFee = contentFee;
                                    }

                                    ri = ArticleBLL.Instance.Update(amodel, Tran);
                                    string uri = $"/article/detail/{id}";
                                    if (ri.Ok)
                                    {
                                        ri.Url = uri;
                                        Commit();
                                        DB.SaveChanges();
                                        //删除旧文件
                                        removePaths.ForEach(item =>
                                        {
                                            UploadHelper.DeleteUpFile(item);
                                        });
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
                    ri.Msg = "文章异常";
                }
            }
            else
            {
                ri.Msg = "该文章已被修改3次，不能再修改！请在留言区进行追问！";
            }
            return Result(ri);
        }

        #endregion

        #region 删除文章
        [IsMaster]
        [HttpPost]
        public ActionResult DeleteA(int id)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var bll = ArticleBLL.Instance;
                Article model = bll.GetModel(id);
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

        #region 搜索 
        [HttpGet]
        public ActionResult Search(string key)
        {
            ResultInfo ri = new ResultInfo();

            if (key.IsNotNullOrEmpty())
            {
                ArticleViewModel model = new ArticleViewModel();
                model.ARticlePage = InitPage(20);
                model.Articles = ArticleBLL.Instance.GetAllArticles(model.ARticlePage, key);
                return PartialView("_Search", model);
            }
            else
            {
                ri.Msg = "请输入关键词";
            }
            return Result(ri);
        }
        #endregion

        #region 下载附件
        public void DownLoad(long id)
        {
            if (id > 0)
            {
                var model = ArticleBLL.Instance.GetModel(id);
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
    }
}