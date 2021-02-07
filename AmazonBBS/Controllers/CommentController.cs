using AmazonBBS.BLL;
using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Controllers
{
    [LOGIN]
    public class CommentController : BaseController
    {
        private readonly IScoreService _scoreService;
        public CommentController(IScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        #region 贴吧
        /// <summary>
        /// 贴吧评论
        /// </summary>
        /// <param name="body">评论内容</param>
        /// <param name="qid">贴子ID</param>
        /// <param name="needscore">是否需要积分查看</param>
        /// <param name="payscore">积分金额</param>
        /// <param name="commentEnum">评论类型（贴吧，文章，招聘等）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BBSComment(string body, long qid, bool needscore, string payscore, string commentEnum, bool isAnonymous)
        {
            return Comment(body, qid, needscore, payscore, commentEnum, isAnonymous);
        }

        [HttpPost]
        public ActionResult BBSReply(string body, long mid, long replytop, string commentEnum, long replytoanswer = 0, long reply2u = 0)
        {
            return Reply(body, mid, replytop, reply2u, commentEnum, replytoanswer);
        }

        [HttpPost]
        public ActionResult BBSPrise(long objId, int type, string priseEnum)
        {
            return Result(Prise(objId, type, priseEnum));
        }

        [HttpPost]
        public ActionResult BBSAgainst(long objId, int type, string priseEnum)
        {
            return Result(Against(objId, type, priseEnum));
        }
        #endregion

        #region 文章
        /// <summary>
        /// 文章评论
        /// </summary>
        /// <param name="body">评论内容</param>
        /// <param name="qid">贴子ID</param>
        /// <param name="needscore">是否需要积分查看</param>
        /// <param name="payscore">积分金额</param>
        /// <param name="commentEnum">评论类型（贴吧，文章，招聘等）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ArticleComment(string body, long qid, bool needscore, string payscore, string commentEnum, bool isAnonymous)
        {
            return Comment(body, qid, needscore, payscore, commentEnum, isAnonymous);
        }

        [HttpPost]
        public ActionResult ArticleReply(string body, long mid, long replytop, string commentEnum, long replytoanswer = 0, long reply2u = 0)
        {
            return Reply(body, mid, replytop, reply2u, commentEnum, replytoanswer);
        }

        [HttpPost]
        public ActionResult ArticlePrise(long objId, int type, string priseEnum)
        {
            return Result(Prise(objId, type, priseEnum));
        }

        [HttpPost]
        public ActionResult ArticleAgainst(long objId, int type, string priseEnum)
        {
            return Result(Against(objId, type, priseEnum));
        }
        #endregion

        #region 活动
        /// <summary>
        /// 活动评论
        /// </summary>
        /// <param name="body">评论内容</param>
        /// <param name="qid">贴子ID</param>
        /// <param name="needscore">是否需要积分查看</param>
        /// <param name="payscore">积分金额</param>
        /// <param name="commentEnum">评论类型（贴吧，文章，活动，招聘等）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PartyComment(string body, long qid, bool needscore, string payscore, string commentEnum, bool isAnonymous)
        {
            return Comment(body, qid, needscore, payscore, commentEnum, isAnonymous);
        }

        [HttpPost]
        public ActionResult PartyReply(string body, long mid, long replytop, string commentEnum, long replytoanswer = 0, long reply2u = 0)
        {
            return Reply(body, mid, replytop, reply2u, commentEnum, replytoanswer);
        }

        [HttpPost]
        public ActionResult PartyPrise(long objId, int type, string priseEnum)
        {
            return Result(Prise(objId, type, priseEnum));
        }
        #endregion

        #region 礼物
        /// <summary>
        /// 礼物评论
        /// </summary>
        /// <param name="body">评论内容</param>
        /// <param name="qid">贴子ID</param>
        /// <param name="needscore">是否需要积分查看</param>
        /// <param name="payscore">积分金额</param>
        /// <param name="commentEnum">评论类型（贴吧，文章，活动，招聘等）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GiftComment(string body, long qid, bool needscore, string payscore, string commentEnum, bool isAnonymous)
        {
            return Comment(body, qid, needscore, payscore, commentEnum, isAnonymous);
        }

        [HttpPost]
        public ActionResult GiftReply(string body, long mid, long replytop, string commentEnum, long replytoanswer = 0, long reply2u = 0)
        {
            return Reply(body, mid, replytop, reply2u, commentEnum, replytoanswer);
        }

        [HttpPost]
        public ActionResult GiftPrise(long objId, int type, string priseEnum)
        {
            return Result(Prise(objId, type, priseEnum));
        }
        #endregion

        #region 招聘
        /// <summary>
        /// 招聘评论
        /// </summary>
        /// <param name="body">评论内容</param>
        /// <param name="qid">贴子ID</param>
        /// <param name="needscore">是否需要积分查看</param>
        /// <param name="payscore">积分金额</param>
        /// <param name="commentEnum">评论类型（贴吧，文章，活动，招聘等）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ZhaoPinComment(string body, long qid, bool needscore, string payscore, string commentEnum, bool isAnonymous)
        {
            return Comment(body, qid, needscore, payscore, commentEnum, isAnonymous);
        }

        [HttpPost]
        public ActionResult ZhaoPinReply(string body, long mid, long replytop, string commentEnum, long replytoanswer = 0, long reply2u = 0)
        {
            return Reply(body, mid, replytop, reply2u, commentEnum, replytoanswer);
        }

        [HttpPost]
        public ActionResult ZhaoPinPrise(long objId, int type, string priseEnum)
        {
            return Result(Prise(objId, type, priseEnum));
        }
        #endregion

        #region 求职
        /// <summary>
        /// 求职评论
        /// </summary>
        /// <param name="body">评论内容</param>
        /// <param name="qid">贴子ID</param>
        /// <param name="needscore">是否需要积分查看</param>
        /// <param name="payscore">积分金额</param>
        /// <param name="commentEnum">评论类型（贴吧，文章，活动，招聘等）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult QiuZhiComment(string body, long qid, bool needscore, string payscore, string commentEnum, bool isAnonymous)
        {
            return Comment(body, qid, needscore, payscore, commentEnum, isAnonymous);
        }

        [HttpPost]
        public ActionResult QiuZhiReply(string body, long mid, long replytop, string commentEnum, long replytoanswer = 0, long reply2u = 0)
        {
            return Reply(body, mid, replytop, reply2u, commentEnum, replytoanswer);
        }

        [HttpPost]
        public ActionResult QiuZhiPrise(long objId, int type, string priseEnum)
        {
            return Result(Prise(objId, type, priseEnum));
        }
        #endregion

        #region 产品服务
        /// <summary>
        /// 产品服务评论
        /// </summary>
        /// <param name="body">评论内容</param>
        /// <param name="qid">贴子ID</param>
        /// <param name="needscore">是否需要积分查看</param>
        /// <param name="payscore">积分金额</param>
        /// <param name="commentEnum">评论类型（贴吧，文章，活动，招聘等）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProductComment(string body, long qid, bool needscore, string payscore, string commentEnum, bool isAnonymous)
        {
            return Comment(body, qid, needscore, payscore, commentEnum, isAnonymous);
        }

        [HttpPost]
        public ActionResult ProductReply(string body, long mid, long replytop, string commentEnum, long replytoanswer = 0, long reply2u = 0)
        {
            return Reply(body, mid, replytop, reply2u, commentEnum, replytoanswer);
        }

        [HttpPost]
        public ActionResult ProductPrise(long objId, int type, string priseEnum)
        {
            return Result(Prise(objId, type, priseEnum));
        }
        #endregion

        #region 数据分析
        /// <summary>
        /// 数据分析评论
        /// </summary>
        /// <param name="body">评论内容</param>
        /// <param name="qid">贴子ID</param>
        /// <param name="needscore">是否需要积分查看</param>
        /// <param name="payscore">积分金额</param>
        /// <param name="commentEnum">评论类型（贴吧，文章，活动，招聘等）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DataAnalysisComment(string body, long qid, bool needscore, string payscore, string commentEnum, bool isAnonymous)
        {
            return Comment(body, qid, needscore, payscore, commentEnum, isAnonymous);
        }

        [HttpPost]
        public ActionResult DataAnalysisReply(string body, long mid, long replytop, string commentEnum, long replytoanswer = 0, long reply2u = 0)
        {
            return Reply(body, mid, replytop, reply2u, commentEnum, replytoanswer);
        }

        [HttpPost]
        public ActionResult DataAnalysisPrise(long objId, int type, string priseEnum)
        {
            return Result(Prise(objId, type, priseEnum));
        }
        #endregion

        #region 课程
        /// <summary>
        /// 课程评论
        /// </summary>
        /// <param name="body">评论内容</param>
        /// <param name="qid">贴子ID</param>
        /// <param name="needscore">是否需要积分查看</param>
        /// <param name="payscore">积分金额</param>
        /// <param name="commentEnum">评论类型（贴吧，文章，活动，招聘等）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult KeChengComment(string body, long qid, bool needscore, string payscore, string commentEnum, bool isAnonymous)
        {
            return Comment(body, qid, needscore, payscore, commentEnum, isAnonymous);
        }

        [HttpPost]
        public ActionResult KeChengReply(string body, long mid, long replytop, string commentEnum, long replytoanswer = 0, long reply2u = 0)
        {
            return Reply(body, mid, replytop, reply2u, commentEnum, replytoanswer);
        }

        [HttpPost]
        public ActionResult KeChengPrise(long objId, int type, string priseEnum)
        {
            return Result(Prise(objId, type, priseEnum));
        }
        #endregion

        #region 评论
        /// <summary>
        /// 评论
        /// </summary>
        /// <param name="body">评论内容</param>
        /// <param name="mainID">指定该评论所属ID(如贴吧、文章、招聘、求职等)</param>
        /// <param name="needscore">该评论是否需要积分查看</param>
        /// <param name="payscore">查看所需积分金额</param>
        /// <param name="commentEnum">指定该评论所属(如贴吧、文章、招聘、求职等)</param>
        /// <returns></returns>
        private ActionResult Comment(string body, long mainID, bool needscore, string payscore, string commentEnum, bool isAnonymous)
        {
            ResultInfo ri = new ResultInfo();
            if (mainID > 0)
            {
                if (!string.IsNullOrEmpty(body))
                {
                    CommentEnumType mainType = GetCommentType(commentEnum);
                    if (mainType > 0)
                    {
                        if (needscore)
                        {
                            if (string.IsNullOrEmpty(payscore))
                            {
                                ri.Msg = "请设置查看回答所需积分";
                                return Json(ri, JsonRequestBehavior.AllowGet);
                            }
                        }

                        Comment commentModel = new Comment()
                        {
                            CommentContent = HttpUtility.UrlDecode(body),
                            CommentUserID = UserID,
                            CommentOrReplyType = 1,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                            CreateUser = UserID.ToString(),
                            UpdateUser = UserID.ToString(),
                            IsDelete = 0,
                            MainID = mainID,
                            MainType = mainType.GetHashCode(),

                            IsAnonymous = isAnonymous,
                        };
                        if (needscore && !string.IsNullOrEmpty(payscore))
                        {
                            commentModel.FeeCoinType = 1;
                            commentModel.IsHideOrFeeToSee = 1;
                            commentModel.NeedCoin = Convert.ToInt32(payscore);
                        }

                        int result = CommentBLL.Instance.Add(commentModel);
                        if (result > 0)
                        {
                            #region 通知
                            //要通知的 类型 ：帖子，文章，招聘 等等
                            //if (mainType == 1 || mainType == 2 || mainType == 5)
                            if (true)
                            {
                                try
                                {
                                    #region 获取基本信息
                                    CoinSourceEnum coinSourceEnum = CoinSourceEnum.UserComment_BBS;
                                    ScoreBeloneMainEnumType scoreBeloneMainEnumType = ScoreBeloneMainEnumType.None;
                                    string domain = GetDomainName;
                                    string mainTitle = string.Empty;//标题(文章，帖子)
                                    long authorID = 0;//获取作者
                                    string mainUrl = string.Empty;//对应的URL路径
                                    string pagemodel = string.Empty;//描述类型
                                    NoticeTypeEnum notice_author_type = NoticeTypeEnum.None;//通知作者本人
                                    NoticeTypeEnum notice_liker_Type = NoticeTypeEnum.None;//通知关注该类型的用户
                                    LikeTypeEnum likeType = LikeTypeEnum.None;//关注类型

                                    //1贴吧 2文章 3活动 4礼物 5招聘 6求职 7产品服务 8数据分析 9课程
                                    if (mainType == CommentEnumType.BBS)
                                    {
                                        Question qmodel = QuestionBLL.Instance.GetModel(mainID);
                                        authorID = qmodel.UserID.Value;
                                        mainTitle = qmodel.Title;
                                        mainUrl = domain + ConfigHelper.AppSettings("BBSDetail").FormatWith(mainID);
                                        pagemodel = "开发百晓";
                                        notice_author_type = NoticeTypeEnum.BBS_My_Comment;
                                        notice_liker_Type = NoticeTypeEnum.BBS_Like_Comment;
                                        likeType = LikeTypeEnum.Like_BBS;

                                        scoreBeloneMainEnumType = ScoreBeloneMainEnumType.Comment_BBS;
                                    }
                                    else if (mainType == CommentEnumType.Article)
                                    {
                                        Article amodel = ArticleBLL.Instance.GetModel(mainID);
                                        authorID = amodel.UserID.Value;
                                        mainTitle = amodel.Title;
                                        mainUrl = domain + ConfigHelper.AppSettings("ArticleDetail").FormatWith(mainID);
                                        pagemodel = "文章";
                                        notice_author_type = NoticeTypeEnum.Article_My_Comment;
                                        notice_liker_Type = NoticeTypeEnum.Article_Like_Comment;
                                        likeType = LikeTypeEnum.Like_Article;

                                        coinSourceEnum = CoinSourceEnum.UserComment_Article;
                                        scoreBeloneMainEnumType = ScoreBeloneMainEnumType.Comment_Article;
                                    }
                                    else if (mainType == CommentEnumType.Party)
                                    {
                                        Activity pmodel = ActivityBLL.Instance.GetModel(mainID);
                                        authorID = pmodel.UserID.Value;
                                        mainTitle = pmodel.Title;
                                        mainUrl = domain + ConfigHelper.AppSettings("PartyDetail").FormatWith(mainID);
                                        pagemodel = "活动";
                                        notice_author_type = NoticeTypeEnum.Party_My_Comment;
                                    }
                                    else if (mainType == CommentEnumType.ZhaoPin)
                                    {
                                        ZhaoPin zmodel = ZhaoPinBLL.Instance.GetModel(mainID);
                                        authorID = Convert.ToInt64(zmodel.Publisher);
                                        mainTitle = zmodel.Gangwei;
                                        mainUrl = domain + $"/zhaopin/detail/{mainID}";
                                        pagemodel = "招聘";
                                        notice_author_type = NoticeTypeEnum.ZhaoPin_My_Comment;
                                    }
                                    else if (mainType == CommentEnumType.QiuZhi)
                                    {
                                        QiuZhi qmodel = QiuZhiBLL.Instance.GetModel(mainID);
                                        authorID = Convert.ToInt64(qmodel.Publisher);
                                        mainTitle = qmodel.IWant;
                                        mainUrl = domain + $"/qiuzhi/detail/{mainID}";
                                        pagemodel = "求职";
                                        notice_author_type = NoticeTypeEnum.QiuZhi_My_Comment;
                                    }
                                    else if (mainType == CommentEnumType.Product)
                                    {
                                        //产品
                                        Product pmodel = ProductBLL.Instance.GetModel(mainID);
                                        authorID = pmodel.CreateUser.ToInt32();
                                        mainTitle = pmodel.PTitle;
                                        mainUrl = domain + ConfigHelper.AppSettings("ProductDetail").FormatWith(mainID);
                                        pagemodel = "产品";
                                        notice_author_type = NoticeTypeEnum.Product_My_Comment;
                                    }
                                    else
                                    {
                                        //礼物 数据 课程
                                        if (mainType == CommentEnumType.Gift)
                                        {
                                            notice_author_type = NoticeTypeEnum.Gift_My_Comment;
                                            pagemodel = "礼物";
                                        }
                                        else if (mainType == CommentEnumType.DataAnalysis)
                                        {
                                            notice_author_type = NoticeTypeEnum.DataAnalysis_My_Comment;
                                            pagemodel = "数据";
                                        }
                                        else if (mainType == CommentEnumType.KeCheng)
                                        {
                                            notice_author_type = NoticeTypeEnum.KeCheng_My_Comment;
                                            pagemodel = "课程";
                                        }
                                        Gift gmodel = GiftBLL.Instance.GetModel(mainID);
                                        authorID = gmodel.GiftCreateUserID.Value;
                                        mainTitle = gmodel.GiftName;
                                        mainUrl = domain + ConfigHelper.AppSettings("GiftDetail").FormatWith(mainID);
                                    }
                                    #endregion

                                    #region 通知作者(含退订操作的)
                                    //if (mainType == CommentEnumType.BBS || mainType == CommentEnumType.ZhaoPin)
                                    if (true)
                                    {
                                        //如果评论者 就是 作者，则跳过
                                        if (authorID != UserID)
                                        {
                                            //根据作者ID先判断是否邮箱帐号
                                            UserBase authorUserInfo = UserBaseBLL.Instance.GetUserInfo(authorID);

                                            #region 站内信通知作者
                                            NoticeBLL.Instance.OnComment_Notice_Author_System(UserInfo.UserName, UserID, commentModel.CommentContent, mainTitle, mainID, mainUrl, pagemodel, authorUserInfo, notice_author_type, domain);
                                            #endregion

                                            #region 邮件通知作者
                                            NoticeBLL.Instance.OnComment_Notice_Author_Email(authorUserInfo, mainType.GetHashCode(), mainID, pagemodel, mainUrl, mainTitle, domain);
                                            #endregion
                                        }
                                    }
                                    #endregion

                                    #region 通知其它关注该文章/帖子的用户(排除作者)
                                    if (mainType == CommentEnumType.BBS || mainType == CommentEnumType.Article)
                                    {
                                        NoticeBLL.Instance.OnComment_Notice_Liker(UserInfo.UserName, UserID, commentModel.CommentContent, mainTitle, mainID, mainUrl, pagemodel, notice_liker_Type, likeType, domain, authorID);
                                    }
                                    #endregion

                                    #region 通知 关注 发表评论的用户 的所有用户(排除作者)
                                    NoticeBLL.Instance.Notice_CommentUser_Liker(UserInfo.UserName, UserID, commentModel.CommentContent, mainTitle, mainUrl, pagemodel, NoticeTypeEnum.UserBeLiked, LikeTypeEnum.Like_User, domain, authorID);
                                    #endregion

                                    #region 给评论者 赠送积分 和 通知
                                    //自己评论自己不加分
                                    if (authorID != UserID)
                                    {
                                        _scoreService.AddScoreOnComment_BBS_Article(UserID, mainID, scoreBeloneMainEnumType, coinSourceEnum);
                                    }
                                    #endregion
                                }
                                catch (Exception e)
                                {
                                    ErrorBLL.Instance.Log(e.ToString());
                                }
                            }
                            #endregion

                            ri.Ok = true;
                            ri.Msg = "评论成功";
                        }
                        else
                        {
                            ri.Msg = "提交异常,请重新再试";
                        }
                    }
                    else
                    {
                        ri.Msg = "发起评论异常！请不要恶意操作！";
                    }
                }
                else
                {
                    ri.Msg = "评论内容不能为空";
                }
            }
            else
            {
                ri.Msg = "问题异常";
            }
            return Result(ri);
        }
        #endregion

        #region 回复
        /// <summary>
        /// 回复
        /// </summary>
        /// <param name="body"></param>
        /// <param name="mainID"></param>
        /// <param name="replyTopCommentId"></param>
        /// <param name="replyToUserId"></param>
        /// <param name="commentEnum">指定该评论所属(如贴吧、文章、招聘、求职等)</param>
        /// <param name="replyToCommentId"></param>
        /// <returns></returns>
        [HttpPost]
        private ActionResult Reply(string body, long mainID, long replyTopCommentId, long replyToUserId, string commentEnum, long replyToCommentId = 0)
        {
            ResultInfo ri = new ResultInfo();
            if (mainID > 0)
            {
                CommentEnumType mainType = GetCommentType(commentEnum);
                if (mainType > 0)
                {
                    if (!string.IsNullOrEmpty(body))
                    {
                        Comment commentModel = new Comment()
                        {
                            CommentContent = HttpUtility.UrlDecode(body),
                            CommentUserID = UserID,
                            CommentOrReplyType = 2,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                            CreateUser = UserID.ToString(),
                            UpdateUser = UserID.ToString(),
                            //CoinType = 0,
                            //IsCanSee = 0,
                            //NeedCoin = 0,
                            IsDelete = 0,
                            MainID = mainID,
                            ReplyTopCommentId = replyTopCommentId,
                            MainType = mainType.GetHashCode(),
                        };
                        if (replyToUserId > 0)
                        {
                            commentModel.ReplyToUserID = replyToUserId;
                        }
                        if (replyToCommentId > 0)
                        {
                            commentModel.ReplyToCommentID = replyToCommentId;
                        }

                        int result = CommentBLL.Instance.Add(commentModel);
                        if (result > 0)
                        {
                            ri.Ok = true;
                            ri.Msg = "回复成功";

                            #region 通知
                            //非自己
                            if (mainType == CommentEnumType.BBS || mainType == CommentEnumType.Article)
                            {
                                long noticeUserID = replyToUserId;
                                if (noticeUserID == 0)
                                {
                                    int _maintypeid = mainType.GetHashCode();
                                    noticeUserID = DB.Comment.FirstOrDefault(a => a.IsDelete == 0 && a.CommentId == replyTopCommentId && a.MainType == _maintypeid).CommentUserID.Value;
                                }

                                if (noticeUserID != UserID)
                                {
                                    string domain = GetDomainName;
                                    string mainTitle = string.Empty;
                                    string mainUrl = string.Empty;
                                    string mainDesc = EnumHelper.GetDescription<CommentEnumType>(mainType.GetHashCode());
                                    NoticeTypeEnum noticeType = NoticeTypeEnum.None;
                                    //string commentTitle = string.Empty;
                                    if (mainType == CommentEnumType.BBS)
                                    {
                                        //问题
                                        Question qmodel = QuestionBLL.Instance.GetModel(mainID);
                                        mainTitle = qmodel.Title;
                                        mainUrl = domain + "/bbs/detail/{0}".FormatWith(mainID);
                                        noticeType = NoticeTypeEnum.BBS_MyComment_Comment;
                                    }
                                    else
                                    {
                                        //文章
                                        Article amodel = ArticleBLL.Instance.GetModel(mainID);
                                        mainTitle = amodel.Title;
                                        mainUrl = domain + "/article/detail/{0}".FormatWith(mainID);
                                        noticeType = NoticeTypeEnum.Article_MyComment_Comment;
                                    }
                                    NoticeBLL.Instance.OnReply_Notice(UserInfo.UserName, UserID, commentModel.CommentContent, mainTitle, mainUrl, domain, noticeType, noticeUserID);
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            ri.Msg = "提交异常,请重新再试";
                        }
                    }
                    else
                    {
                        ri.Msg = "评论内容不能为空";
                    }
                }
                else
                {
                    ri.Msg = "发起评论异常！请不要恶意操作！";
                }
            }
            else
            {
                ri.Msg = "问题异常";
            }
            return Result(ri);
        }
        #endregion

        #region 点赞操作
        /// <summary>
        /// 点赞操作
        /// </summary>
        /// <param name="mainId"></param>
        /// <param name="type">点赞类型 1主内容点赞 2评论/回复点赞</param>
        /// <param name="priseEnum">点赞所属对象类型(贴吧，文章，招聘……)</param>
        /// <returns></returns>
        private ResultInfo Prise(long mainId, int type, string priseEnum)
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                if (mainId > 0)
                {
                    if (type == 1 || type == 2)
                    {
                        int priseType = GetPriseType(type, priseEnum) + type;
                        if (priseType > 0)
                        {
                            if (DB.Against.FirstOrDefault(a => a.UserID == UserID && a.IsDelete == 0 && a.Type == priseType && a.TargetID == mainId) == null)
                            {
                                //判断重复操作
                                if (!PriseBLL.Instance.Exist(mainId, priseType, UserID))
                                {
                                    Prise pmodel = new Prise();
                                    pmodel.UserID = UserID;
                                    pmodel.PriseTime = DateTime.Now;
                                    pmodel.IsDelete = 0;
                                    pmodel.TargetID = mainId;
                                    pmodel.Type = priseType;

                                    BeginTran();

                                    int result = PriseBLL.Instance.Add(pmodel, Tran);
                                    if (result > 0)
                                    {
                                        //自己给自己点赞不算加分
                                        string mainTitle = string.Empty;
                                        string mainDesc = string.Empty;
                                        string mainUrl = string.Empty;
                                        string commentTitle = string.Empty;
                                        NoticeTypeEnum noticeType = NoticeTypeEnum.None;
                                        long authorID = GetAuthorID(priseType, mainId, ref mainTitle, ref mainDesc, ref mainUrl, ref noticeType, ref commentTitle);
                                        if (UserID != authorID)
                                        {
                                            //判断是否当天超额加分
                                            var __source = CoinSourceEnum.Prise.GetHashCode();
                                            DateTime __now = DateTime.Now, __from = DateTime.Now.Date;
                                            bool logOk = false;
                                            var count__ = DB.ScoreCoinLog.Count(a => a.CoinType == 1 && a.CoinSource == __source && a.CoinTime >= __from && a.CoinTime <= __now);
                                            if (count__ < ConfigHelper.AppSettings("commentArticle").ToInt32())
                                            {
                                                #region 点赞加分
                                                int priseScore = Convert.ToInt32(ConfigHelper.AppSettings("Prise"));
                                                logOk = UserExtBLL.Instance.AddScore(UserID, priseScore, 1, Tran);
                                                if (logOk)
                                                {
                                                    #region 流水记录
                                                    //记录流水
                                                    ScoreCoinLog scorecoinlog = new ScoreCoinLog()
                                                    {
                                                        UserID = UserID,
                                                        Coin = priseScore,
                                                        CoinSource = CoinSourceEnum.Prise.GetHashCode(),
                                                        CoinTime = DateTime.Now,
                                                        CoinType = 1,
                                                        CreateUser = UserID.ToString(),
                                                        UserName = UserInfo.UserName
                                                    };
                                                    logOk = ScoreCoinLogBLL.Instance.Add(scorecoinlog, Tran) > 0;
                                                    #endregion
                                                }
                                                else
                                                {
                                                    RollBack();
                                                }
                                                #endregion

                                                #region 被点赞加分
                                                if (logOk)
                                                {
                                                    int priseForScore = Convert.ToInt32(ConfigHelper.AppSettings("PriseFor"));
                                                    logOk = UserExtBLL.Instance.AddScore(authorID, priseForScore, 1, Tran);
                                                    if (logOk)
                                                    {
                                                        #region 流水记录
                                                        //记录流水
                                                        ScoreCoinLog scorecoinlog = new ScoreCoinLog()
                                                        {
                                                            UserID = authorID,
                                                            Coin = priseForScore,
                                                            CoinSource = CoinSourceEnum.PriseFor.GetHashCode(),
                                                            CoinTime = DateTime.Now,
                                                            CoinType = 1,
                                                            CreateUser = UserID.ToString(),
                                                            UserName = UserBaseBLL.Instance.GetModel(Convert.ToInt64(authorID)).UserName
                                                        };
                                                        logOk = ScoreCoinLogBLL.Instance.Add(scorecoinlog, Tran) > 0;

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
                                                #endregion
                                            }
                                            else
                                            {
                                                logOk = true;
                                            }
                                            if (logOk)
                                            {
                                                Commit();
                                                ri.Msg = "点赞成功";
                                                ri.Ok = true;

                                                #region 点赞通知作者
                                                string domain = GetDomainName;
                                                UserBase author = UserBaseBLL.Instance.GetUserInfo(authorID);
                                                if (type == 1)
                                                {
                                                    NoticeBLL.Instance.OnPrise_Main_Notice_Author(UserInfo.UserName, UserID, mainTitle, mainDesc, mainUrl, domain, author, noticeType);
                                                }
                                                else
                                                {
                                                    //NoticeBLL.Instance.OnPrise_Comment_Notice_Author(UserInfo.UserName, string.Empty, mainDesc, string.Empty, domain, author, NoticeTypeEnum.Comment_Prise, string.Empty);
                                                    NoticeBLL.Instance.OnPrise_Comment_Notice_Author(UserInfo.UserName, UserID, mainTitle, mainDesc, mainUrl, domain, author, NoticeTypeEnum.Comment_Prise, commentTitle);
                                                }
                                                #endregion
                                            }
                                        }
                                        else
                                        {
                                            Commit();
                                            ri.Msg = "点赞成功";
                                            ri.Ok = true;
                                        }
                                    }
                                    else
                                    {
                                        ri.Msg = "点赞失败";
                                        RollBack();
                                    }
                                }
                                else
                                {
                                    ri.Ok = true;
                                    ri.Msg = "你已点赞，不要重复操作";
                                }
                            }
                            else
                            {
                                ri.Msg = "您已反对过了，不能再点赞";
                            }
                        }
                        else
                        {
                            ri.Msg = "请不要恶意操作！！！";
                        }
                    }
                    else
                    {
                        ri.Msg = "请不要恶意操作！！！";
                    }
                }
                else
                {
                    ri.Msg = "请求数据异常";
                }
            }
            catch (Exception e)
            {
                ri.Msg = e.Message;
                RollBack();
            }
            return ri;
        }
        #endregion

        #region 反对操作
        /// <summary>
        /// 反对操作
        /// </summary>
        /// <param name="mainId"></param>
        /// <param name="type">反对类型 1主内容点赞 2评论/回复点赞</param>
        /// <param name="priseEnum">反对所属对象类型(贴吧，文章，招聘……)</param>
        /// <returns></returns>
        private ResultInfo Against(long mainId, int type, string priseEnum)
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                if (mainId > 0)
                {
                    if (type == 1 || type == 2)
                    {
                        int priseType = GetPriseType(type, priseEnum) + type;
                        if (priseType > 0)
                        {
                            //如果同一对象已经进行点赞操作，则不可进行反对操作
                            if (DB.Prise.FirstOrDefault(a => a.UserID == UserID && a.IsDelete == 0 && a.Type == priseType && a.TargetID == mainId) == null)
                            {
                                //判断重复操作
                                if (DB.Against.FirstOrDefault(a => a.UserID == UserID && a.IsDelete == 0 && a.Type == priseType && a.TargetID == mainId) == null)
                                {
                                    Against pmodel = new Against();
                                    pmodel.UserID = UserID;
                                    pmodel.AgainstTime = DateTime.Now;
                                    pmodel.IsDelete = 0;
                                    pmodel.TargetID = mainId;
                                    pmodel.Type = priseType;
                                    pmodel.AgainstId = Guid.NewGuid();

                                    //BeginTran();
                                    var tran = DB.Database.BeginTransaction();

                                    //int result = PriseBLL.Instance.Add(pmodel, Tran);

                                    if (DB.Against.Add(pmodel) != null)
                                    {
                                        string mainTitle = string.Empty;
                                        string mainDesc = string.Empty;
                                        string mainUrl = string.Empty;
                                        string commentTitle = string.Empty;
                                        NoticeTypeEnum noticeType = NoticeTypeEnum.None;
                                        long authorID = GetAuthorID(priseType, mainId, ref mainTitle, ref mainDesc, ref mainUrl, ref noticeType, ref commentTitle);
                                        if (UserID != authorID)
                                        {
                                            ri.Msg = "反对成功";
                                            ri.Ok = true;
                                            DB.SaveChanges();
                                            tran.Commit();
                                            #region 反对后通知作者
                                            string domain = GetDomainName;
                                            UserBase author = UserBaseBLL.Instance.GetUserInfo(authorID);
                                            if (type == 1)
                                            {
                                                noticeType = priseType == 1 ? NoticeTypeEnum.BBS_Against : NoticeTypeEnum.Article_Against;
                                                NoticeBLL.Instance.OnAgainst_Main_Notice_Author(UserInfo.UserName, UserID, mainTitle, mainDesc, mainUrl, domain, author, noticeType);
                                            }
                                            else
                                            {
                                                NoticeBLL.Instance.OnAgainst_Comment_Notice_Author(UserInfo.UserName, UserID, mainTitle, mainDesc, mainUrl, domain, author, NoticeTypeEnum.Comment_Against, commentTitle);
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            DB.SaveChanges();
                                            tran.Commit();
                                            ri.Msg = "反对成功";
                                            ri.Ok = true;
                                        }
                                    }
                                    else
                                    {
                                        ri.Msg = "反对失败";
                                        RollBack();
                                    }
                                }
                                else
                                {
                                    ri.Ok = true;
                                    ri.Msg = "你已反对，不要重复操作";
                                }
                            }
                            else
                            {
                                ri.Msg = "您已赞过了，不能再点反对";
                            }
                        }
                        else
                        {
                            ri.Msg = "请不要恶意操作！！！";
                        }
                    }
                    else
                    {
                        ri.Msg = "请不要恶意操作！！！";
                    }
                }
                else
                {
                    ri.Msg = "请求数据异常";
                }
            }
            catch (Exception e)
            {
                ri.Msg = e.Message;
                RollBack();
            }
            return ri;
        }
        #endregion

        #region 编辑
        [HttpPost]
        public ActionResult Edit(long id, string content, string pageEnum)
        {
            ResultInfo ri = new ResultInfo();
            if (UserBaseBLL.Instance.IsRoot)
            {
                if (id > 0)
                {
                    int mainType = GetCommentType(pageEnum).GetHashCode();
                    if (mainType > 0)
                    {
                        ri.Ok = CommentBLL.Instance.EditComment(mainType, id, HttpUtility.UrlDecode(content));
                    }
                    else
                    {
                        ri.Msg = "修改评论异常";
                    }
                }
                else
                {
                    ri.Msg = "源评论异常";
                }
            }
            else
            {
                ri.Msg = "你没有权限进行评论的修改";
            }
            return Result(ri);
        }
        #endregion

        #region 删除
        [HttpPost]
        public ActionResult Delete(long id, string pageEnum)
        {
            ResultInfo ri = new ResultInfo();
            if (UserBaseBLL.Instance.IsRoot)
            {
                if (id > 0)
                {
                    int mainType = GetCommentType(pageEnum).GetHashCode();
                    if (mainType > 0)
                    {
                        ri.Ok = CommentBLL.Instance.DeleteComment(mainType, id);
                    }
                    else
                    {
                        ri.Msg = "修改评论异常";
                    }
                }
                else
                {
                    ri.Msg = "源评论异常";
                }
            }
            else
            {
                ri.Msg = "你没有权限进行评论的修改";
            }
            return Result(ri);
        }
        #endregion

        #region 评论对象类型
        /// <summary>
        /// 评论对象类型(1贴吧 2文章 3活动 4礼物 5招聘 6求职 7产品服务 8数据分析 9课程)
        /// </summary>
        /// <param name="commentEnum"></param>
        /// <returns></returns>
        private CommentEnumType GetCommentType(string commentEnum)
        {
            commentEnum = commentEnum.ToUpper();
            CommentEnumType type = CommentEnumType.None;
            switch (commentEnum)
            {
                case "BBS": type = CommentEnumType.BBS; break;
                case "ARTICLE": type = CommentEnumType.Article; break;
                case "PARTY": type = CommentEnumType.Party; break;
                case "GIFT": type = CommentEnumType.Gift; break;
                case "ZHAOPIN": type = CommentEnumType.ZhaoPin; break;
                case "QIUZHI": type = CommentEnumType.QiuZhi; break;
                case "PRODUCT": type = CommentEnumType.Product; break;
                case "DATAANALYSIS": type = CommentEnumType.DataAnalysis; break;
                case "KECHENG": type = CommentEnumType.KeCheng; break;
            }
            return type;
        }
        #endregion

        #region 返回点赞对象的源类型
        /// <summary>
        /// 返回点赞对象的源类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="priseEnum"></param>
        /// <returns></returns>
        private int GetPriseType(int type, string priseEnum)
        {
            priseEnum = priseEnum.ToUpper();
            int priseType = -2;
            switch (priseEnum)
            {
                case "BBS": priseType = 0; break;
                case "ARTICLE": priseType = 2; break;
                case "PARTY": priseType = 4; break;
                case "GIFT": priseType = 6; break;
                case "ZHAOPIN": priseType = 8; break;
                case "QIUZHI": priseType = 10; break;
                case "PRODUCT": priseType = 12; break;
                case "DATAANALYSIS": priseType = 14; break;
                case "KECHENG": priseType = 16; break;
            }
            return priseType;
        }
        #endregion

        #region 根据点赞类型获取对象作者的ID
        /// <summary>
        /// 根据点赞类型获取对象作者的ID
        /// </summary>
        /// <param name="priseType"></param>
        /// <param name="id"></param>
        /// <param name="mainTitle">标题</param>
        /// <returns></returns>
        private long GetAuthorID(int priseType, long id, ref string mainTitle, ref string mainDesc, ref string mainUrl, ref NoticeTypeEnum noticeType, ref string commentTitle)
        {
            long mainID = id;
            string domain = GetDomainName;
            long authorID = 0;
            string sql = string.Empty;
            switch (priseType)
            {
                case 1:
                    var qmodel = QuestionBLL.Instance.GetModel(id);
                    authorID = qmodel.UserID.Value;
                    mainTitle = qmodel.Title;
                    mainDesc = "帖子";
                    mainUrl = "{0}/bbs/detail/{1}";
                    noticeType = NoticeTypeEnum.BBS_My_Prise;
                    break;
                case 3:
                    var amodel = ArticleBLL.Instance.GetModel(id);
                    authorID = amodel.UserID.Value;
                    mainTitle = amodel.Title;
                    mainDesc = "文章";
                    mainUrl = "{0}/article/detail/{1}";
                    noticeType = NoticeTypeEnum.Article_My_Prise;
                    break;
                case 5:
                    authorID = Convert.ToInt64(ActivityBLL.Instance.GetModel(id).UserID);
                    mainDesc = "活动";
                    mainUrl = "{0}/party/detail/{1}";
                    break;
                case 9:
                    authorID = Convert.ToInt64(ZhaoPinBLL.Instance.GetModel(id).Publisher);
                    mainDesc = "招聘";
                    mainUrl = "{0}/zhaopin/detail/{1}";
                    break;
                case 11:
                    authorID = Convert.ToInt64(QiuZhiBLL.Instance.GetModel(id).Publisher);
                    mainDesc = "求职";
                    mainUrl = "{0}/qiuzhi/detail/{1}";
                    break;
                case 13:
                    authorID = Convert.ToInt64(ProductBLL.Instance.GetModel(id).CreateUser);
                    mainDesc = "产品";
                    mainUrl = "{0}/product/detail/{1}";
                    break;
                case 7:
                case 15:
                case 17:
                    authorID = GiftBLL.Instance.GetModel(id).GiftCreateUserID.Value;
                    mainDesc = priseType == 7 ? "礼物" : priseType == 15 ? "数据" : "课程";
                    mainUrl = "{0}/gift/detail/{1}";
                    break;
                case 2:
                    sql = @"select a.Title MainTitle,a.QuestionId MainID,b.CommentContent,b.CommentUserID from Question a 
                        right join Comment b on b.MainID=a.QuestionId 
                        where b.CommentId=@id and b.MainType=@mainType";
                    mainUrl = "{0}/bbs/detail/{1}";
                    goto case 18;
                case 4:
                    sql = @"select a.Title MainTitle,a.ArticleId MainID,b.CommentContent,b.CommentUserID from Article a 
                        right join Comment b on b.MainID=a.ArticleId 
                        where b.CommentId=@id and b.MainType=@mainType";
                    mainUrl = "{0}/article/detail/{1}";
                    goto case 18;
                case 6:
                    sql = @"select a.Title MainTitle,a.ActivityId MainID,b.CommentContent,b.CommentUserID from Activity a 
                        right join Comment b on b.MainID=a.ActivityId 
                        where b.CommentId=@id and b.MainType=@mainType";
                    mainUrl = "{0}/party/detail/{1}";
                    goto case 18;
                case 8:
                    sql = @"select a.GiftName MainTitle,a.GiftId MainID,b.CommentContent,b.CommentUserID from Gift a 
                        right join Comment b on b.MainID=a.GiftId and a.GType=1
                        where b.CommentId=@id and b.MainType=@mainType";
                    mainUrl = "{0}/gift/detail/{1}";
                    goto case 18;
                case 10:
                    sql = @"select a.Gangwei MainTitle,a.ZhaoPinID MainID,b.CommentContent,b.CommentUserID from ZhaoPin a 
                        right join Comment b on b.MainID=a.ZhaoPinID 
                        where b.CommentId=@id and b.MainType=@mainType";
                    mainUrl = "{0}/zhaopin/detail/{1}";
                    goto case 18;
                case 12:
                    sql = @"select a.IWant MainTitle,a.QiuZhiID MainID,b.CommentContent,b.CommentUserID from QiuZhi a 
                        right join Comment b on b.MainID=a.QiuZhiID 
                        where b.CommentId=@id and b.MainType=@mainType";
                    mainUrl = "{0}/qiuzhi/detail/{1}";
                    goto case 18;
                case 14:
                    sql = @"select a.PTitle MainTitle,a.ProductID MainID,b.CommentContent,b.CommentUserID from Product a 
                        right join Comment b on b.MainID=a.ProductID 
                        where b.CommentId=@id and b.MainType=@mainType";
                    mainUrl = "{0}/product/detail/{1}";
                    goto case 18;
                case 16:
                    sql = @"select a.GiftName MainTitle,a.GiftId MainID,b.CommentContent,b.CommentUserID from Gift a 
                        right join Comment b on b.MainID=a.GiftId and a.GType=2
                        where b.CommentId=@id and b.MainType=@mainType";
                    mainUrl = "{0}/gift/detail/{1}";
                    goto case 18;
                case 18:
                    if (sql.IsNullOrEmpty())
                    {
                        sql = @"select a.GiftName MainTitle,a.GiftId MainID,b.CommentContent,b.CommentUserID from Gift a 
                        right join Comment b on b.MainID=a.GiftId and a.GType=3
                        where b.CommentId=@id and b.MainType=@mainType";
                        mainUrl = "{0}/gift/detail/{1}";
                    }
                    var cmodel = CommentBLL.Instance.GetCommentInfoAndMainTitleByMainID(id, priseType / 2, sql);
                    mainDesc = EnumHelper.GetDescription<CommentEnumType>(priseType / 2);
                    var dr = cmodel.Rows[0];
                    mainTitle = dr["MainTitle"].ToString();
                    mainID = Convert.ToInt64(dr["MainID"]);
                    authorID = Convert.ToInt64(dr["CommentUserID"]);
                    commentTitle = dr["CommentContent"].ToString();
                    //authorID = CommentBLL.Instance.GetAuthorID(id, priseType / 2);
                    break;
            }
            if (mainUrl.IsNotNullOrEmpty())
            {
                mainUrl = mainUrl.FormatWith(domain, mainID);
            }
            return authorID;
        }

        private string GetMainDesc(int mainType)
        {
            string mainDesc = string.Empty;
            switch (mainType)
            {
                case 1:
                    mainDesc = "帖子"; break;
                case 2:
                    mainDesc = "文章"; break;
                case 3:
                    mainDesc = "活动"; break;
                case 4:
                    mainDesc = "礼物"; break;
                case 5:
                    mainDesc = "招聘"; break;
                case 6:
                    mainDesc = "求职"; break;
                case 7:
                    mainDesc = "产品"; break;
                case 8:
                    mainDesc = "数据"; break;
                case 9:
                    mainDesc = "课程"; break;
            }
            return mainDesc;
        }
        #endregion
    }
}