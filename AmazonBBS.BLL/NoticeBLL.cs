using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using AmazonBBS.Common;
using AmazonBBS.DAL;
using AmazonBBS.Model;
using System.Threading.Tasks;
using System.Threading;

namespace AmazonBBS.BLL
{

    /// <summary>
    /// 系统通知
    /// </summary>
    public class NoticeBLL : Auto_NoticeBLL
    {
        public static NoticeBLL Instance
        {
            get { return SingleHepler<NoticeBLL>.Instance; }
        }
        NoticeDAL dal = new NoticeDAL();

        #region Base
        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(Notice model, SqlTransaction tran = null)
        {
            ResultInfo ri = new ResultInfo();

            if (model == null) return ri;

            int result = Add(model, tran);

            if (result > 0)
            {
                ri.Ok = true;
                ri.Msg = "添加成功";
            }

            return ri;
        }
        #endregion

        #region update
        /// <summary>
        /// 修改 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultInfo Update(Notice model, SqlTransaction tran = null)
        {
            ResultInfo ri = new ResultInfo();
            if (Edit(model, tran))
            {
                ri.Ok = true;
                ri.Msg = "修改成功";
            }

            return ri;
        }
        #endregion

        #region delete
        /// <summary>
        /// 删除 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public ResultInfo Delete(long id)
        {
            ResultInfo ri = new ResultInfo();

            var model = GetModel(id);
            if (model == null)
            {
                ri.Msg = "删除的信息不存在";
                return ri;
            }
            if (DeleteByID(id))
            {
                ri.Ok = true;
            }
            else
            {
                ri.Msg = "删除记录时候出错了";
            }
            return ri;

        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public Notice GetModel(long id)
        {
            return GetItem(id);
        }
        #endregion

        #region query
        ///// <summary>
        ///// 基本数据查询 
        ///// </summary>
        ///// <param name="pageIndex">查询页码</param>
        ///// <returns></returns>
        //public Paging Query(int pageIndex)
        //{
        //    Paging paging = new Paging();
        //
        //    int count = Count();
        //    if (count == 0) return paging;
        //
        //    int pageCount = count % paging.PageSize == 0 ? count / paging.PageSize : count / paging.PageSize + 1;
        //    if (pageIndex < 1 || pageIndex > pageCount) pageIndex = 1;
        //
        //    DataTable tab = dal.Query(pageIndex, paging.PageSize);
        //
        //    List<Notice> li = ModelConvertHelper<Notice>.ConvertToList(tab);
        //    paging.RecordCount = count;
        //    paging.PageIndex = pageIndex;
        //    //paging.SetDataSource(li);
        //    paging.PageCount = pageCount;
        //
        //    return paging;
        //}
        #endregion

        #region query
        /// <summary>
        /// 基本数据查询 
        /// </summary>
        /// <param name="pageIndex">查询页码</param>
        /// <returns></returns>
        public Paging SearchByRows(int pageIndex)
        {
            Paging paging = new Paging(Count(), pageIndex);
            if (paging.RecordCount > 0)
            {
                var tab = dal.SearchByRows(paging.StartIndex, paging.EndIndex);
                var li = ModelConvertHelper<Notice>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;
        }
        #endregion
        #endregion

        #region 通知置已读
        public void Read(long userID, DateTime now)
        {
            try
            {
                dal.Read(userID, now);
            }
            catch
            {

            }
        }
        #endregion

        #region 邮箱帐户
        public string[] SendEmailAccount
        {
            get
            {
                string[] sendEmailAccount = ConfigHelper.AppSettings("Emailor").Split('|');
                string emailhost = ConfigHelper.AppSettings("EmailHost");
                emailhost = emailhost.IsNotNullOrEmpty() ? emailhost : "smtp.{0}".FormatWith(sendEmailAccount[0].Split('@')[1]);
                return new string[] { sendEmailAccount[0], sendEmailAccount[1], emailhost };
            }
        }
        #endregion

        #region 站内信通知
        /// <summary>
        /// 站内信通知
        /// </summary>
        /// <param name="likers"></param>
        /// <param name="noticeType"></param>
        /// <param name="notice_Desc"></param>
        /// <param name="onError"></param>
        public void NoticeSystem(List<UserBase> likers, NoticeTypeEnum noticeType, string[] notice_Desc, string typedesc)
        {
            //if (IsSystemNoticeLiker)
            //{
            try
            {
                List<Notice> list = new List<Notice>();
                int _noticeType = noticeType.GetHashCode();
                //过滤不正常的userid
                likers = likers.Where(liker => { return liker.UserID > 0; }).ToList();
                if (likers.Count > 0)
                {
                    likers.ForEach(liker =>
                    {
                        list.Add(new Notice()
                        {
                            CreateTime = DateTime.Now,
                            IsDelete = 0,
                            IsRead = 0,
                            NoticeTitle = notice_Desc[0],
                            NoticeContent = notice_Desc[1],
                            NoticeType = _noticeType,
                            ToUserID = liker.UserID
                        });
                    });

                    //var all = this.SearchAll();
                    var alNotices = CSharpCacheHelper.Get<List<Notice>>(
                        "notices",
                        new List<Notice>());
                    var newList = new List<Notice>();
                    foreach (var item in list)
                    {
                        if (alNotices.Any(x =>
                        x.NoticeTitle == item.NoticeTitle
                        && x.ToUserID == item.ToUserID
                        && x.NoticeType == item.NoticeType
                        && x.NoticeContent == item.NoticeContent))
                        {
                            continue;
                        }

                        newList.Add(item);
                    }
                    if (newList.Any())
                    {
                        SqlHelper.SqlBulkCopyByDatatable(SqlHelper.DefaultConnectionString, "Notice", newList.ToDataTable());
                    }
                }
            }
            catch (Exception e)
            {
                ErrorBLL.Instance.Log($"新增{typedesc}通知时出错\r\n{e.ToString()}");
            }
            //}
        }

        /// <summary>
        /// 站内信通知
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="noticeType"></param>
        /// <param name="notice_Desc"></param>
        /// <param name="onError"></param>
        public void NoticeSystem(List<long> userIds, NoticeTypeEnum noticeType, string[] notice_Desc, string typedesc)
        {
            //if (IsSystemNoticeLiker)
            //{
            try
            {
                List<Notice> list = new List<Notice>();
                int _noticeType = noticeType.GetHashCode();
                //过滤不正常的userid
                userIds = userIds.Where(userId => { return userId > 0; }).ToList();
                if (userIds.Count > 0)
                {
                    userIds.ForEach(userId =>
                    {
                        list.Add(new Notice()
                        {
                            CreateTime = DateTime.Now,
                            IsDelete = 0,
                            IsRead = 0,
                            NoticeTitle = notice_Desc[0],
                            NoticeContent = notice_Desc[1],
                            NoticeType = _noticeType,
                            ToUserID = userId
                        });
                    });
                    SqlHelper.SqlBulkCopyByDatatable(SqlHelper.DefaultConnectionString, "Notice", list.ToDataTable());
                }
            }
            catch (Exception e)
            {
                ErrorBLL.Instance.Log($"新增{typedesc}通知时出错\r\n{e.ToString()}");
            }
            //}
        }
        #endregion

        #region 邮件通知
        /// <summary>
        /// 邮件通知
        /// </summary>
        public void NoticeEmail(List<UserBase> likers, string[] notice_desc)
        {
            //该判断不要取消，是本地调试时用的，避免发送垃圾邮件给别人
            if (IsEmailNoticeLiker)
            {
                string[] sendEmailAccount = SendEmailAccount;
                //过滤掉 非邮箱帐号
                likers = likers.Where(liker => { return liker.Account.IsNotNullOrEmpty(); }).Where(liker => { return MatchHelper.IsEmail.IsMatch(liker.Account); }).ToList();
                likers.ForEach(liker =>
                {
                    try
                    {
                        EmailHelper email = new EmailHelper();
                        email.From = sendEmailAccount[0];
                        email.FromName = "百晓堂";

                        email.Recipients = new List<EmailHelper.RecipientClass>();
                        email.Recipients.Add(new EmailHelper.RecipientClass()
                        {
                            Recipient = liker.Account,
                            RecipientName = liker.UserName
                        });

                        email.Subject = notice_desc[0];
                        email.Body = notice_desc[1];
                        email.IsBodyHtml = true;
                        email.ServerHost = sendEmailAccount[2];
                        email.ServerPort = 465;
                        email.Username = sendEmailAccount[0];
                        email.Password = sendEmailAccount[1];

                        bool ok = email.Send2();
                    }
                    catch (Exception e)
                    {
                        ErrorBLL.Instance.Log(liker.Account + "/r/n" + e.ToString());
                    }
                });
            }
        }
        #endregion

        #region 获取我的所有通知
        public List<Notice> GetMyNotice(Paging noticePage, long userID)
        {
            DataSet ds = dal.GetMyNotice(noticePage.StartIndex, noticePage.EndIndex, userID);
            noticePage.RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            return ModelConvertHelper<Notice>.ConvertToList(ds.Tables[1]);
        }
        #endregion

        #region 用户发表问题、文章等是否 通过邮件方式 通知关注者
        /// <summary>
        /// 用户发表问题、文章等是否通知关注者
        /// </summary>
        public bool IsEmailNoticeLiker
        {
            get
            {
                string notice = ConfigHelper.AppSettings("EmailNotice");
                return notice.IsNullOrEmpty() ? false : Convert.ToBoolean(notice);
            }
        }
        #endregion

        #region 用户发表问题、文章等是否 通过站内信方式 通知关注者
        /// <summary>
        /// 用户发表问题、文章等是否通知关注者
        /// </summary>
        public bool IsSystemNoticeLiker
        {
            get
            {
                string notice = ConfigHelper.AppSettings("SystemNotice");
                return notice.IsNullOrEmpty() ? false : Convert.ToBoolean(notice);
            }
        }
        #endregion

        #region 用户关注某人的帖子、文章、招聘、求职时，进行通知！
        /// <summary>
        /// 用户关注某人的帖子、文章、招聘、求职时，进行通知！
        /// </summary>
        public void OnLikeItem_Notice_User(long belikedUserId, string likeUserName, string actionTypeDesc, string mainUrl, string mainName, string domain, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                //尊敬的百晓生，您好<br>伟哥刚刚关注了您的文章/帖子/招聘/求职帖子！
                //尊敬的{0}，您好<br><a href='{1}/User/Detail/{2}' target='_blank' style='color:red;'>{2}</a>刚刚关注了您的{5}<a href='{3}'>《{4}》</a>！
                var user = UserBaseBLL.Instance.GetUserInfo(belikedUserId);
                string[] onLikeItem = ConfigHelper.AppSettings("OnLikeItem_Notice_User").FormatWith(user.UserName, domain, likeUserName, mainUrl, mainName, actionTypeDesc).Split('|');
                var users = new List<UserBase>() { user };
                NoticeSystem(users, noticeType, onLikeItem, actionTypeDesc);
            });
        }

        #endregion

        #region 新增帖子、文章、招聘、求职时，通知关注者
        public void OnAdd_Notice_Liker(string userName, long userId, string uri, string publishTitle, NoticeTypeEnum noticeType, string domain)
        {
            if (IsSystemNoticeLiker)
            {
                CreateTask(() =>
                {
                    //查找所有关注该用户的用户
                    List<UserBase> likers = UserLikeBLL.Instance.FindLikerListByLikeType(userId, LikeTypeEnum.Like_User);
                    if (likers.Count > 0)
                    {
                        string mainDesc = EnumHelper.GetDescription<NoticeTypeEnum>(noticeType.GetHashCode());
                        string[] new_add_notice = ConfigHelper.AppSettings("New_Add_Notice").FormatWith(domain, userId, userName, uri, publishTitle, mainDesc).Split('|');
                        NoticeSystem(likers, noticeType, new_add_notice, mainDesc);
                        NoticeEmail(likers, new_add_notice);
                    }
                });
            }
        }
        #endregion

        #region 有评论时，通知关注帖子/文章的用户
        /// <summary>
        /// 有评论时，通知关注帖子/文章的用户
        /// </summary>
        public void OnComment_Notice_Liker(string commentUserName, long commentUserID, string commentContent, string mainTitle,
           long mainID, string mainUrl, string typeDesc, NoticeTypeEnum noticeType, LikeTypeEnum likeType, string domain, long authorID)
        {
            if (IsSystemNoticeLiker)
            {
                CreateTask(() =>
                {
                    //获取所有关注此帖子/文章的用户
                    List<UserBase> likers = UserLikeBLL.Instance.FindLikerListByLikeType(mainID, likeType);
                    //过滤评论人和楼主
                    likers = likers.Where(liker => { return liker.UserID != authorID && liker.UserID != commentUserID; }).ToList();
                    if (likers.Count > 0)
                    {
                        string[] onCommentNotice = ConfigHelper.AppSettings("onComment_Notice_Liker").FormatWith(commentUserName, typeDesc, mainUrl, mainTitle, commentContent, domain, commentUserID).Split('|');
                        NoticeSystem(likers, noticeType, onCommentNotice, typeDesc);
                        NoticeEmail(likers, onCommentNotice);
                    }
                });
            }
        }
        #endregion

        #region 有评论时，通知关注该评论者的用户
        public void Notice_CommentUser_Liker(string commentUserName, long commentUserID, string commentContent, string mainTitle,
            string mainUrl, string typeDesc, NoticeTypeEnum noticeType, LikeTypeEnum likeType, string domain, long authorID)
        {
            if (IsSystemNoticeLiker)
            {
                //新评论提醒-{文章-百晓生}
                //您关注的用户<a href='/User/Detail/{0}'>{百晓生}</a>对{文章}<a href='{URL}'>{标题}</a>发表了评论：{评论内容}
                //新评论提醒-{1}-{0}
                //您关注的用户<a href='/User/Detail/{0}'>{0}</a>对{1}<a href='{2}'>{3}</a>发表了评论：{4}
                CreateTask(() =>
                {
                    List<UserBase> likers = UserLikeBLL.Instance.FindLikerListByLikeType(commentUserID, likeType);
                    likers = likers.Where(liker => { return liker.UserID != authorID; }).ToList();
                    if (likers.Count > 0)
                    {
                        string[] onCommentNotice = ConfigHelper.AppSettings("onComment_Notice_CommentUser_Liker").FormatWith(commentUserName, typeDesc, mainUrl, mainTitle, commentContent, domain, commentUserID).Split('|');
                        NoticeSystem(likers, noticeType, onCommentNotice, typeDesc);
                        NoticeEmail(likers, onCommentNotice);
                    }
                });
            }
        }
        #endregion

        #region 有评论时，通知作者
        /// <summary>
        /// 有评论时站内信通知作者
        /// </summary>
        /// <param name="commentUserName"></param>
        /// <param name="commentContent"></param>
        /// <param name="mainTitle"></param>
        /// <param name="mainID"></param>
        /// <param name="mainUrl"></param>
        /// <param name="typeDesc"></param>
        /// <param name="author"></param>
        /// <param name="noticeType"></param>
        /// <param name="domain"></param>
        public void OnComment_Notice_Author_System(string commentUserName, long commentUserID, string commentContent, string mainTitle, long mainID, string mainUrl, string typeDesc, UserBase author, NoticeTypeEnum noticeType, string domain)
        {
            CreateTask(() =>
            {
                //新评论提醒-{0}！|用户&lt;a href='{1}/User/Detail/{2}' style='color:red;'&gt;{3}&lt;/a&gt;对您的{0}&lt;a href='{4}'&gt;《{5}》&lt;/a&gt发表了如下评论：&lt;span style='color:green;'&gt;【{6}】>&lt;/span&gt;
                string[] onCommentNotice = ConfigHelper.AppSettings("onComment_Notice_Author").FormatWith(typeDesc, domain, commentUserID, commentUserName, mainUrl, mainTitle, commentContent).Split('|');
                var users = new List<UserBase>() { author };
                NoticeSystem(users, noticeType, onCommentNotice, typeDesc);
            });
        }

        /// <summary>
        /// 发生评论时邮件通知作者
        /// </summary>
        /// <param name="user"></param>
        public void OnComment_Notice_Author_Email(UserBase user, int mainType, long mainID, string mainTypeDesc, string mainUrl, string mainTitle, string domain)
        {
            CreateTask(() =>
            {
                if (user.Source == 2 && MatchHelper.IsEmail.IsMatch(user.Account))
                {
                    if (IsEmailNoticeLiker)
                    {
                        long authorID = user.UserID;
                        //获取状态判断是否需要通知
                        var emailNoticeModel = EmailNoticeBLL.Instance.GetModelByAuthor(authorID, mainType, mainID);
                        //没有则创建
                        if (emailNoticeModel == null)
                        {
                            //生成MD5Key
                            string md5 = StringHelper.GetNickName(8);
                            //生成签名
                            string sign = DESEncryptHelper.Encrypt(authorID.ToString() + mainType.ToString() + mainID.ToString(), md5);
                            EmailNotice emodel = new EmailNotice()
                            {
                                AuthorID = authorID,
                                EmailNoticeAuthor = 1,
                                MainID = mainID,
                                MainType = mainType,
                                MD5Key = md5,
                                MD5Sign = sign,
                                CreateTime = DateTime.Now
                            };
                            EmailNoticeBLL.Instance.Add(emodel);
                            EmailNoticeFun(authorID, mainTypeDesc, mainUrl, mainTitle, mainID, mainType, sign, domain);
                        }
                        else
                        {
                            if (emailNoticeModel.EmailNoticeAuthor == 1)
                            {
                                EmailNoticeFun(authorID, mainTypeDesc, mainUrl, mainTitle, mainID, mainType, emailNoticeModel.MD5Sign, domain);
                            }
                        }
                    }
                }
            });
        }
        #endregion

        #region 回复时通知留言者
        /// <summary>
        /// 回复时通知留言者
        /// </summary>
        public void OnReply_Notice(string replyUserName, long replyUserID, string replyContent, string mainTitle, string mainUrl, string domain, NoticeTypeEnum noticeType, long noticeUserID)
        {
            CreateTask(() =>
            {
                //新回复通知！
                //您在<a href='{2}{0}'>{1}</a>的评论，<a href='{2}/User/Detail/{3}'>{4}</a>回复了您：{5}
                string[] onReplyNotice = ConfigHelper.AppSettings("onReply_Notice").FormatWith(mainUrl, mainTitle, domain, replyUserID, replyUserName, replyContent).Split('|');
                List<UserBase> users = new List<UserBase>() { UserBaseBLL.Instance.GetUserInfo(noticeUserID) };
                NoticeSystem(users, noticeType, onReplyNotice, "回复");
                NoticeEmail(users, onReplyNotice);
            });
        }
        #endregion

        #region 关注用户时通知
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doLike_UserName">进行关注操作的用户名</param>
        /// <param name="doLike_UserID">进行关注操作的用户ID</param>
        /// <param name="noticeType">操作类型</param>
        /// <param name="beLikedUser">被关注用户ID</param>
        /// <param name="domain">域名</param>
        public void OnLikeUser_Notice(string doLike_UserName, long doLike_UserID, NoticeTypeEnum noticeType, long beLikedUserID, string domain)
        {
            CreateTask(() =>
            {
                //用户关注通知！
                //用户<a href='[0}/User/Detail/{1}'>{2}</a>关注了您！对方以后能及时收到您的相关动态！
                string[] onLikeUserNotice = ConfigHelper.AppSettings("onLikeUser_Notice").FormatWith(domain, doLike_UserID, doLike_UserName).Split('|');
                List<UserBase> users = new List<UserBase>() { UserBaseBLL.Instance.GetUserInfo(beLikedUserID) };
                NoticeSystem(users, noticeType, onLikeUserNotice, "关注");
                NoticeEmail(users, onLikeUserNotice);
            });
        }
        #endregion

        #region 点赞时通知 作者
        /// <summary>
        /// 点赞时通知 作者
        /// </summary>
        public void OnPrise_Main_Notice_Author(string priseUserName, long priseUserID, string mainTitle, string mainDesc, string mainUrl, string domain, UserBase noticeUser, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                //点赞通知
                //用户<a href='{0}/User/Detail/{1}'>{百晓生}</a>对您的{文章/评论}<a>《文章标题》</a>点了个赞！nice~~
                //用户<a href='{0}/User/Detail/{1}'>{1}</a>对您的{2}<a href='{4}'>《3》</a>点了个赞！nice~~
                string[] onPriseNotice = ConfigHelper.AppSettings("onPrise_Main_Notice_Author").FormatWith(domain, priseUserName, mainDesc, mainTitle, mainUrl, priseUserID).Split('|');
                var users = new List<UserBase>() { noticeUser };
                NoticeSystem(users, noticeType, onPriseNotice, mainDesc);
                NoticeEmail(users, onPriseNotice);
            });
        }
        #endregion

        #region 给评论点赞时通知作者
        /// <summary>
        /// 给评论点赞时通知作者
        /// </summary>
        /// <param name="priseUserName"></param>
        /// <param name="mainTitle">null</param>
        /// <param name="mainDesc"></param>
        /// <param name="mainUrl">null</param>
        /// <param name="domain"></param>
        /// <param name="noticeUser"></param>
        /// <param name="noticeType"></param>
        /// <param name="commentTitle">null</param>
        public void OnPrise_Comment_Notice_Author(string priseUserName, long priseUserID, string mainTitle, string mainDesc, string mainUrl, string domain, UserBase noticeUser, NoticeTypeEnum noticeType, string commentTitle)
        {
            //点赞通知
            //用户<a href='{0}/User/Detail/{百晓生}'>{百晓生}</a>对您在{文章/评论}<a href=''>《文章标题》</a>的留言{留言内容}点了个赞！nice~~
            //用户&lt;a href='{0}/User/Detail/{1}'&gt;{1}&lt;/a&gt;对您在{2}的留言点了个赞！nice~~
            CreateTask(() =>
            {
                string[] onPriseCommentNotice = ConfigHelper.AppSettings("onPrise_Comment_Notice_Author").FormatWith(domain, priseUserID, priseUserName, mainDesc, mainUrl, mainTitle, commentTitle).Split('|');
                var users = new List<UserBase>() { noticeUser };
                NoticeSystem(users, noticeType, onPriseCommentNotice, mainDesc);
                NoticeEmail(users, onPriseCommentNotice);
            });
        }
        #endregion

        #region 反对时通知 作者
        /// <summary>
        /// 反对时通知 作者
        /// </summary>
        public void OnAgainst_Main_Notice_Author(string priseUserName, long priseUserID, string mainTitle, string mainDesc, string mainUrl, string domain, UserBase noticeUser, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                //点赞通知
                //用户<a href='{0}/User/Detail/{1}'>{百晓生}</a>对您的{文章/评论}<a>《文章标题》</a>点了个赞！nice~~
                //用户<a href='{0}/User/Detail/{1}'>{1}</a>对您的{2}<a href='{4}'>《3》</a>点了个赞！nice~~
                string[] onPriseNotice = ConfigHelper.AppSettings("OnAgainst_Main_Notice_Author").FormatWith(domain, priseUserName, mainDesc, mainTitle, mainUrl, priseUserID).Split('|');
                var users = new List<UserBase>() { noticeUser };
                NoticeSystem(users, noticeType, onPriseNotice, mainDesc);
                NoticeEmail(users, onPriseNotice);
            });
        }
        #endregion

        #region 给评论反对时通知作者
        /// <summary>
        /// 给评论反对时通知作者
        /// </summary>
        /// <param name="priseUserName"></param>
        /// <param name="mainTitle">null</param>
        /// <param name="mainDesc"></param>
        /// <param name="mainUrl">null</param>
        /// <param name="domain"></param>
        /// <param name="noticeUser"></param>
        /// <param name="noticeType"></param>
        /// <param name="commentTitle">null</param>
        public void OnAgainst_Comment_Notice_Author(string priseUserName, long priseUserID, string mainTitle, string mainDesc, string mainUrl, string domain, UserBase noticeUser, NoticeTypeEnum noticeType, string commentTitle)
        {
            //点赞通知
            //用户<a href='{0}/User/Detail/{百晓生}'>{百晓生}</a>对您在{文章/评论}<a href=''>《文章标题》</a>的留言{留言内容}点了个赞！nice~~
            //用户&lt;a href='{0}/User/Detail/{1}'&gt;{1}&lt;/a&gt;对您在{2}的留言点了个赞！nice~~
            CreateTask(() =>
            {
                string[] onPriseCommentNotice = ConfigHelper.AppSettings("OnAgainst_Comment_Notice_Author").FormatWith(domain, priseUserID, priseUserName, mainDesc, mainUrl, mainTitle, commentTitle).Split('|');
                var users = new List<UserBase>() { noticeUser };
                NoticeSystem(users, noticeType, onPriseCommentNotice, mainDesc);
                NoticeEmail(users, onPriseCommentNotice);
            });
        }
        #endregion

        #region 邮件通知作者(有退订、恢复操作)
        public void EmailNoticeFun(long authorID, string pagemodel, string url, string title, long mainID, int mainType, string sign, string domain)
        {
            #region 发送邮件通知
            //获取作者邮箱
            UserBase authorInfo = UserBaseBLL.Instance.GetUserInfo(Convert.ToInt64(authorID));

            string[] sendEmailAccount = SendEmailAccount;
            EmailHelper email = new EmailHelper();
            email.From = sendEmailAccount[0];
            email.FromName = "百晓堂";

            email.Recipients = new List<EmailHelper.RecipientClass>();
            email.Recipients.Add(new EmailHelper.RecipientClass()
            {
                Recipient = authorInfo.Account,
                RecipientName = authorInfo.UserName
            });

            email.Subject = "新动态提醒";
            email.Body = $"您在百晓堂{pagemodel}发的内容《<a style='color:red;' href='{url}'>{title}</a>》<br/>有人在评论你。<br/>退订邮件可以<a href='{domain}/Notice/TD?actionid={mainID}&actionid2={sign}&actionid3={mainType}'>点击本处</a>,<br/>恢复邮件可以<a href='{domain}/Notice/HF?actionid={mainID}&actionid2={sign}&actionid3={mainType}'>点击本处</a>";
            email.IsBodyHtml = true;
            email.ServerHost = sendEmailAccount[2];
            email.ServerPort = 465;
            email.Username = sendEmailAccount[0];
            email.Password = sendEmailAccount[1];
            email.IsBodyHtml = true;
            bool ok = email.Send2();
            #endregion
        }

        #endregion

        #region 邀请面试时通知求职者
        public void OnInviteInterview_Notice_Author(string inviteUserName, string gangwei, string zhaoPinID, string viewTime, string viewPlace, string qiuzhiName, string qiuZhiID, string domain, long noticeUserID, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                //面试邀请通知
                //百晓生 在您的求职贴《我要求职》 里，向您发起面试邀请，职位《面试岗位》：<br>面试时间：2018-09-01<br>面试地点：上海市
                //<a href='' target='_blank'></a>
                //<a href='/User/Detail/{0}' target='_blank'>{0}</a>在您的求职贴<a href='/QiuZhi/Detail/{1}' target='_blank' style='color:red;'>《{2}》</a>中，邀请您面试<a href='/ZhaoPin/Detail/{3}' target='_blank' style='color:blue;'>《{4}》</a>：<br>面试时间：{5}<br>面试地点：{6}
                string[] onInviteinterview = ConfigHelper.AppSettings("onInviteInterview_Notice_Author").FormatWith(inviteUserName, qiuZhiID, qiuzhiName, zhaoPinID, gangwei, viewTime, viewPlace, domain).Split('|');
                List<UserBase> users = new List<UserBase>() { UserBaseBLL.Instance.GetUserInfo(noticeUserID) };
                NoticeSystem(users, noticeType, onInviteinterview, "面试邀请");
                NoticeEmail(users, onInviteinterview);
            });
        }
        #endregion

        #region 求职发送简历通知用人单位
        public void OnSendCV_Notice_Company(UserBase zuserInfo, string zhaoPinID, string zhaoPinName, string sendUserName, string sendUserID, string domain, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                //收到新简历通知
                //尊敬的百晓生您好，伟哥在您的招聘职位《招工啦》里投递了简历！<br>投递时间：2018-09-07<br>简历下载地址：XXX
                //收到新简历通知|尊敬的{1}，您好！<br><a href='{0}/User/Detail/{2}' target='_blank'>{2}</a>在您的招聘职位<a href='{0}/ZhaoPin/Detail/{3}'>《{4}》</a>里投递了简历！<br>投递时间：{5}<br>简历下载地址：<a href='{0}/ZhaoPin/Download/{3}?uid={6}'>点击下载简历</a>
                string[] onSendCV = ConfigHelper.AppSettings("OnSendCV_Notice_Company").FormatWith(domain, zuserInfo.UserName, sendUserName, zhaoPinID, zhaoPinName, DateTime.Now.ToString(1), sendUserID).Split('|');
                var users = new List<UserBase>() { zuserInfo };
                NoticeSystem(users, noticeType, onSendCV, "收到新简历通知");
                NoticeEmail(users, onSendCV);
            });
        }
        #endregion

        #region 产品预约购买，通知商家
        /// <summary>
        /// 产品预约购买，通知商家
        /// </summary>
        /// <param name="salerInfo"></param>
        /// <param name="productId"></param>
        /// <param name="productName"></param>
        /// <param name="buyerName"></param>
        /// <param name="buyerUserID"></param>
        /// <param name="domain"></param>
        /// <param name="noticeType"></param>
        public void OnOrderBuyProduct_Notice_Saler(UserBase salerInfo, string productId, string productName, string buyerName, long buyerUserID, string domain, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                //预约购买通知
                //尊敬的百晓生，您好！<br>买家伟哥向您发送了一个预约购买请求，他想购买您的产品《产品1》，您可以打开他的个人中心进行私聊！
                //预约购买通知|尊敬的{1}，您好！<br>买家<a href='/User/Detail/{2}' target='_blank'>{2}</a>向您发送了一个预约购买请求，他想购买您的产品<a href='{0}/Product/Detail/{3}'>{4}</a>，您可以打开他的个人中心进行私聊！
                string[] onOrderBuyProduct = ConfigHelper.AppSettings("OnOrderBuyProduct_Notice_Saler").FormatWith(domain, salerInfo.UserName, buyerName, productId, productName).Split('|');
                List<UserBase> users = new List<UserBase>() { salerInfo };
                NoticeSystem(users, noticeType, onOrderBuyProduct, "预约购买通知");
                NoticeEmail(users, onOrderBuyProduct);
            });
        }
        #endregion

        #region 用户签到成功通知
        public void OnSign_Notice_User(int score, UserBase user, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                //签到成功提醒|尊敬的{0}，您好！<br>每日签到赠送{1}积分已到账！请再接再厉！
                string[] onSign = ConfigHelper.AppSettings("OnSign_Notice_User").FormatWith(user.UserName, score).Split('|');
                List<UserBase> users = new List<UserBase>() { user };
                NoticeSystem(users, noticeType, onSign, "签到成功提醒");
            });
        }
        #endregion

        #region 采纳最佳答案后，通知回答者
        public void OnBestAnswer_Notice_Author(string questionUserName, long questionId, string questionTitle, long answerUserID, int score, string domain, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                //最佳回答通知|尊敬的百晓生，您好！<br>您在帖子《怎么做》里回答的内容被楼主百晓生采纳为最佳回答，并由系统自动结算赠送0积分！请再接再厉！
                //最佳回答通知|尊敬的{0}，您好！<br>您在帖子<a href='{1}/Question/Detail/{2}'>{3}</a>的回答被楼主<a href='{1}/User/Detail/{4}'>{4}</a>采纳为最佳回答，{5}请再接再厉！
                var answerInfo = UserBaseBLL.Instance.GetUserInfo(answerUserID);
                string[] onBeseAnswer = ConfigHelper.AppSettings("OnBestAnswer_Notice_Author").FormatWith(answerInfo.UserName, domain, questionId, questionTitle, questionUserName, score > 0 ? "并由系统自动结算赠送{0}积分！".FormatWith(score) : string.Empty).Split('|');
                List<UserBase> users = new List<UserBase>() { answerInfo };
                NoticeSystem(users, noticeType, onBeseAnswer, "最佳回答通知");
                NoticeEmail(users, onBeseAnswer);
            });
        }
        #endregion

        #region 充值VIP分成功通知用户
        public void OnPayVIPScoreSuccess_Notice_User(long userId, DateTime payTime, string payFee, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                //尊敬的百晓生，您好！<br>您在2018.8.1充值的10VIP分已成功到账！
                //尊敬的{0}，您好！<br>您在{1}充值的{2}VIP分已成功到账！
                var user = UserBaseBLL.Instance.GetUserInfo(userId);
                string[] onPayVIPSuccess = ConfigHelper.AppSettings("OnPayVIPScoreSuccess_Notice_User").FormatWith(user.UserName, payTime.ToString(1), payFee).Split('|');
                List<UserBase> users = new List<UserBase>() { user };
                NoticeSystem(users, noticeType, onPayVIPSuccess, "最佳回答通知");
            });
        }
        #endregion

        #region 参加活动、购买礼物成功后进行通知
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buyerUserId"></param>
        /// <param name="payTime"></param>
        /// <param name="feeOrNot">是否免费 true免费</param>
        /// <param name="payfee"></param>
        /// <param name="mainUrl"></param>
        /// <param name="mainName"></param>
        /// <param name="buyOrJoin">true:购买，false:参加</param>
        /// <param name="noticeType"></param>
        public void OnBuySuccess_Notice_Buyer(long buyerUserId, DateTime time, bool feeOrNot, string payfee, int count, string mainUrl, string mainName, bool buyOrJoin, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                //尊敬的百晓生，您好！<br>恭喜您于2018.9.9成功支付了费用100元/100VIP分/100积分，成功参加/购买活动/礼物《活动XXX》！
                var user = UserBaseBLL.Instance.GetUserInfo(buyerUserId);
                string actionDesc = (buyOrJoin ? "成功购买" : "成功参加") + "{0}份".FormatWith(count) + EnumHelper.GetDescription<NoticeTypeEnum>(noticeType.GetHashCode());
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("尊敬的{0}，您好！<br>恭喜您于{1}", user.UserName, time);
                if (!feeOrNot)
                {
                    sb.AppendFormat("支付了费用{0}，", payfee);
                }
                sb.AppendFormat("{0}<a href='{2}' target='_blank' style='color:blue;'>《{1}》</a>！", actionDesc, mainName, mainUrl);
                string[] onBuySuccess = new[] { actionDesc, sb.ToString() };
                List<UserBase> users = new List<UserBase>() { user };
                NoticeSystem(users, noticeType, onBuySuccess, actionDesc);
            });
        }

        /// <summary>
        /// 自动回复
        /// </summary>
        /// <param name="buyerUserId"></param>
        /// <param name="noticeType"></param>
        public void OnBuySuccess_Notice_AutoReply_Buyer(long buyerUserId, string msg, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                var user = UserBaseBLL.Instance.GetUserInfo(buyerUserId);
                List<UserBase> users = new List<UserBase>() { user };
                string[] notice = new[] { "自动发货通知", msg };
                NoticeSystem(users, noticeType, notice, notice[0]);
            });
        }
        #endregion

        #region 支付宝支付 发布求职、招聘、产品通知
        public void OnPayPublish_Notice_Author(long publishUserId, DateTime time, string mainUrl, string mainName, int feetype, string fee, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                //尊敬的百晓生，您好！<br>恭喜您成功支付1元发布招聘信息《公司招聘》！
                //发布{4}成功通知|尊敬的{0}，您好！<br>恭喜您于{5}成功支付{1}发布{4}信息<a href='{2}'>《{3}》</a>！
                var user = UserBaseBLL.Instance.GetUserInfo(publishUserId);
                string[] onPayPublish = ConfigHelper.AppSettings("OnPayPublish_Notice_Author").FormatWith(user.UserName, (feetype == 30 ? "{0}元" : "{0}VIP分").FormatWith(fee), mainUrl, mainName, EnumHelper.GetDescription<NoticeTypeEnum>(noticeType.GetHashCode()), time).Split('|');
                List<UserBase> users = new List<UserBase>() { user };
                NoticeSystem(users, noticeType, onPayPublish, onPayPublish[0]);
            });
        }
        #endregion

        #region 成功发布文章、帖子 积分到账通知
        public void OnPublish_BBS_Article_AddScore_Notice_Author(long publishUserId, DateTime time, string publishTypeName, string score, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                var user = UserBaseBLL.Instance.GetUserInfo(publishUserId);
                string[] onPayPublish = ConfigHelper.AppSettings("OnPublish_BBS_Article_AddScore_Notice_Author").FormatWith(user.UserName, publishTypeName, score, EnumHelper.GetDescription<NoticeTypeEnum>(noticeType.GetHashCode()), time).Split('|');
                List<UserBase> users = new List<UserBase>() { user };
                NoticeSystem(users, noticeType, onPayPublish, onPayPublish[0]);
            });
        }
        #endregion

        #region 评论文章、帖子 评论者 积分到账通知
        public void OnComment_BBS_Article_AddScore_Notice_Commenter(long commentUserId, DateTime time, string ocmmentTypeName, string score, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                var user = UserBaseBLL.Instance.GetUserInfo(commentUserId);
                string[] onPayPublish = ConfigHelper.AppSettings("OnComment_BBS_Article_AddScore_Notice_Commenter").FormatWith(user.UserName, ocmmentTypeName, score, EnumHelper.GetDescription<NoticeTypeEnum>(noticeType.GetHashCode()), time).Split('|');
                List<UserBase> users = new List<UserBase>() { user };
                NoticeSystem(users, noticeType, onPayPublish, onPayPublish[0]);
            });
        }
        #endregion

        #region 有用户留言之后，通知管理员(百晓生)
        /// <summary>
        /// 有用户留言之后，通知管理员(百晓生)
        /// </summary>
        /// <param name="masterId"></param>
        /// <param name="leaveWord"></param>
        /// <param name="noticeType"></param>
        public void OnLeaveWord_Notice_Master(long masterId, LeaveWord leaveWord, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                var user = UserBaseBLL.Instance.GetUserInfo(masterId);
                string[] onLeave = ConfigHelper.AppSettings("OnLeaveWord_Notice_Master").FormatWith(leaveWord.Telephone, leaveWord.Name, leaveWord.Age, leaveWord.DIY1, leaveWord.DIY2, leaveWord.DIY3).Split('|');
                List<UserBase> users = new List<UserBase>() { user };
                NoticeSystem(users, noticeType, onLeave, onLeave[0]);
                NoticeEmail(users, onLeave);
            });
        }
        #endregion

        #region 注册成功后通知分享人
        public void OnShareRegistSuccess_Notice_Sharer(UserBase sharer, string registerName, DateTime registTime, string domain, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                //尊敬的百晓生，您好！<br>小小伟于2018年9月12日成功通过您的分享链接注册百晓堂网站用户，请前往我的分享查看分享奖励是否达标！
                //分享用户注册通知|尊敬的{0}，您好！<br><a href='{1}/user/detail/{2}' target="_blank" style="color:red;">{2}</a>于{3}成功通过您的分享链接注册百晓堂网站用户，请前往<a href="{1}/user/myshare" target="_blank" style="color:blue;">我的分享</a>查看分享奖励是否达标！
                string[] onRegistSuccess = ConfigHelper.AppSettings("OnShareRegistSuccess_Notice_Sharer").FormatWith(sharer.UserName, domain, registerName, registTime.ToString(1)).Split('|');
                List<UserBase> users = new List<UserBase>() { sharer };
                NoticeSystem(users, noticeType, onRegistSuccess, onRegistSuccess[0]);
                NoticeEmail(users, onRegistSuccess);
            });
        }
        #endregion

        #region 举报通知
        /// <summary>
        /// 举报通知-帖子、文章
        /// </summary>
        /// <param name="masterId"></param>
        /// <param name="leaveWord"></param>
        /// <param name="noticeType"></param>
        public void OnReport_Main_Notice_Master(long userId, string url, string title, long authorUserId, string mainDesc, string reportDesc, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                var user = UserBaseBLL.Instance.GetUserInfo(userId);
                var user2 = UserBaseBLL.Instance.GetUserInfo(authorUserId);
                var userReport = UserBaseBLL.Instance.GetUserInfo(10006);
                string[] onLeave = ConfigHelper.AppSettings("OnReport_Main_Notice_Master").FormatWith(userId, user.UserName, authorUserId, user2.UserName, mainDesc, url, title, reportDesc).Split('|');
                List<UserBase> users = new List<UserBase>() { userReport };
                NoticeSystem(users, noticeType, onLeave, onLeave[0]);
                //NoticeEmail(users, onLeave);
            });
        }

        /// <summary>
        /// 举报通知-帖子、文章
        /// </summary>
        /// <param name="masterId"></param>
        /// <param name="leaveWord"></param>
        /// <param name="noticeType"></param>
        public void OnReport_Comment_Notice_Master(long userId, long commentId, string commentDesc, string url, string title, long authorUserId, string reportDesc, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                var user = UserBaseBLL.Instance.GetUserInfo(userId);
                var user2 = UserBaseBLL.Instance.GetUserInfo(authorUserId);
                var userReport = UserBaseBLL.Instance.GetUserInfo(10006);
                string[] onLeave = ConfigHelper.AppSettings("OnReport_Comment_Notice_Master").FormatWith(userId, user.UserName, authorUserId, user2.UserName, url, title, commentDesc, reportDesc).Split('|');
                List<UserBase> users = new List<UserBase>() { userReport };
                NoticeSystem(users, noticeType, onLeave, onLeave[0]);
                //NoticeEmail(users, onLeave);
            });
        }
        #endregion

        #region 待发货通知管理员
        /// <summary>
        /// 待发货通知管理员
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="authorUserId"></param>
        /// <param name="mainDesc"></param>
        /// <param name="reportDesc"></param>
        /// <param name="noticeType"></param>
        public void OnUserBuySuccess_Notice_Master(UserBase buyerUser, string url, string mainName, NoticeTypeEnum noticeType)
        {
            CreateTask(() =>
            {
                var master = UserBaseBLL.Instance.GetUserInfo(10006);
                string[] onLeave = ConfigHelper.AppSettings("OnUserBuySuccess_Notice_Master").FormatWith(buyerUser.UserID, buyerUser.UserName, url, mainName).Split('|');
                List<UserBase> users = new List<UserBase>() { master };
                NoticeSystem(users, noticeType, onLeave, onLeave[0]);
            });
        }
        #endregion

        #region 用户购买问题或文章里的积分内容时提醒 用户 
        public void OnUserBuy_Content_Success_For_BBS_Arcitle_Notice_Buyer(long buyerId, string coinMsg, string url, string mainTitle, DateTime time)
        {
            CreateTask(() =>
            {
                string[] msg = ConfigHelper.AppSettings("OnUserBuy_Content_Success_For_BBS_Arcitle_Notice_Buyer").FormatWith(time.ToString(1), coinMsg, url, mainTitle).Split('|');
                NoticeSystem(new List<long> { buyerId }, NoticeTypeEnum.BuyBBSOrArticle_Notice_Buyer, msg, "用户购买问题或文章里的积分内容时提醒 用户");
            });
        }
        #endregion

        #region 用户购买问题或文章里的积分内容时提醒 作者
        public void OnUserBuy_Content_Success_For_BBS_Arcitle_Notice_Author(UserBase buyer, long authorId, string coinMsg, string url, string mainTitle, DateTime time)
        {
            CreateTask(() =>
            {
                string[] msg = ConfigHelper.AppSettings("OnUserBuy_Content_Success_For_BBS_Arcitle_Notice_Author").FormatWith(buyer.UserID, buyer.UserName, time.ToString(1), url, mainTitle, coinMsg).Split('|');
                NoticeSystem(new List<long> { authorId }, NoticeTypeEnum.BuyBBSOrArticle_Notice_Buyer, msg, "用户购买问题或文章里的积分内容时提醒 作者");
            });
        }
        #endregion

        #region 发布帖子或文章需要审核时，通知网站 作者本人
        public void On_BBS_Article_Publish_Success_Notice_Publisher(UserBase publisher, string mainMsg, string mainTitle, string url, NoticeTypeEnum noticeTypeEnum)
        {
            //待审核通知|尊敬的用户您好，刚刚您{2}《&lt;a href='{0}' target='_blank' style='color:red;'&gt;{1}&lt;/a&gt;》已经在审核中，待网站管理员审核通过后即可正常浏览，请注意通知信息。
            CreateTask(() =>
            {
                string[] message = ConfigHelper.AppSettings("On_BBS_Article_Publish_Success_Notice_Publisher").FormatWith(url, mainTitle, mainMsg).Split('|');
                NoticeSystem(new List<UserBase> { publisher }, noticeTypeEnum, message, "发布帖子或文章或活动需要审核时，通知网站 作者本人");
            });
        }
        #endregion

        #region 发布帖子或文章需要审核时，通知网站管理员
        public void On_BBS_Article_Publish_Success_Notice_Master(UserBase publisher, string mainMsg, string mainTitle, string url, NoticeTypeEnum noticeTypeEnum)
        {
            //待审核通知|网站管理员您好，网站活跃用户&lt;a href='/user/detail/{0}' target='_blank' style='color:red;'&gt;{1}&lt;/a&gt;{2}《&lt;a href='{3}' target='_blank' style='color:blue;'&gt;{4}&lt;/a&gt;》待审核，请及时跟进！
            CreateTask(() =>
            {
                string[] msg = ConfigHelper.AppSettings("On_BBS_Article_Publish_Success_Notice_Master").FormatWith(publisher.UserID, publisher.UserName, mainMsg, url, mainTitle).Split('|');
                NoticeSystem(new List<long> { 10006 }, NoticeTypeEnum.BuyBBSOrArticle_Notice_Buyer, msg, "发布帖子或文章或活动需要审核时，通知网站管理员");
            });
        }
        #endregion

        #region 当审核 帖子、文章、活动 结束时，通知发布用户
        public void On_Check_Handled_Notice_Publisher(long publishUserId, string mainMsg, string uri, string mainTitle, string passStatusMsg, DateTime time)
        {
            CreateTask(() =>
            {
                //审核结果通知|尊敬的用户您好，网站管理员于2019年对您发布的帖子《XXX》进行了审核操作，结果：不通过！
                //审核结果通知|尊敬的用户您好，网站管理员于{0}对您发布的{1}《&lt;a href='{2}' target='_blank' style='color:red;'&gt;{3}&lt;/a&gt;》进行了审核操作，结果：{4}！
                string[] msg = ConfigHelper.AppSettings("On_Check_Handled_Notice_Publisher").FormatWith(time.ToString(1), mainMsg, uri, mainTitle, passStatusMsg).Split('|');
                NoticeSystem(new List<long> { publishUserId }, NoticeTypeEnum.BuyBBSOrArticle_Notice_Buyer, msg, "审核 帖子、文章、活动 结束时，通知发布用户");
            });
        }
        #endregion

        #region 如果是普通管理员赠送积分，则通知超级管理员
        public void OnGiveScoreSuccess_Notice_Root(UserBase masterUser, UserBase giveToUser, int coin, int coinType, DateTime time)
        {
            CreateTask(() =>
            {
                //管理员赠送积分通知|网站管理员月落于2019年赠送百晓生999积分。
                //管理员赠送积分通知|网站管理员 &lt;a href='/user/detail/{0}' target='_blank' style='color:red;'&gt;{1}&lt;/a&gt; 于 {2} 赠送 &lt;a href='/user/detail/{3}' target='_blank' style='color:red;'&gt;{4}&lt;/a&gt; {5} {6}。
                string[] msg = ConfigHelper.AppSettings("OnGiveScoreSuccess_Notice_Root").FormatWith(masterUser.UserID, masterUser.UserName, time.ToString(1), giveToUser.UserID, giveToUser.UserName, coin, coinType == 1 ? "积分" : "VIP积分").Split('|');
                NoticeSystem(new List<long> { 10006 }, NoticeTypeEnum.Give_score_notice_root, msg, "赠送积分通知管理员");
            });
        }
        #endregion

        #region 管理员操作赠送积分，通知被赠人
        public void OnGiveScoreSuccess_Notice_User(long giveToUserId, int coin, int coinType, DateTime time)
        {
            CreateTask(() =>
            {
                //管理员赠送积分通知|尊敬的用户您好，网站管理员于2019年赠送您999积分。
                //管理员赠送积分通知|尊敬的用户您好，网站管理员于 {0} 赠送您 {1} {2} 。
                string[] msg = ConfigHelper.AppSettings("OnGiveScoreSuccess_Notice_User").FormatWith(time.ToString(1), coin, coinType == 1 ? "积分" : "VIP积分").Split('|');
                NoticeSystem(new List<long> { giveToUserId }, NoticeTypeEnum.Give_score_notice_user, msg, "赠送积分通知用户");
            });
        }
        #endregion

        #region 连续签到赠送积分 通知用户
        /// <summary>
        /// 连续签到赠送积分 通知用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enouthSignCount"></param>
        /// <param name="coin"></param>
        /// <param name="time"></param>
        public void OnSign_Enough_Notice_User(long userId, int enouthSignCount, int coin, bool isNewUser, DateTime time)
        {
            CreateTask(() =>
            {
                //月连续签到赠积分通知|尊敬的用户您好，本月您在2019年已连续签到满3次，系统自动赠送您10积分，请加油，月连续签到次数越多更多积分等着你。
                //月连续签到赠积分通知|尊敬的用户您好，本月您在{0}已连续签到满{1}次，系统自动赠送您{2}积分，请加油，月连续签到次数越多更多积分等着你。
                string typeDesc = null;
                string appsetting = null;
                if (isNewUser)
                {
                    typeDesc = "连续签到赠积分";
                    appsetting = "On_NewUser_Sign_Enough_Notice_User";
                }
                else
                {
                    typeDesc = "累计签到赠积分";
                    appsetting = "On_OldUser_Sign_Enough_Notice_User";

                }
                string[] msg = ConfigHelper.AppSettings(appsetting).FormatWith(time.ToString(1), enouthSignCount, coin).Split('|');
                NoticeSystem(new List<long> { userId }, NoticeTypeEnum.Sign_EnoughNoticeUser, msg, typeDesc);
            });
        }
        #endregion

        #region 创建任务
        private void CreateTask(Action action)
        {
            if (Convert.ToBoolean(ConfigHelper.AppSettings("OpenAsync")))
            {
                //异步
                Task.Run(action);
            }
            else
            {
                //同步
                Task.Run(action).Wait();
            }
            //Task task = new Task(action);
            ////启用同步
            //task.Start();
            //if (!Convert.ToBoolean(ConfigHelper.AppSettings("OpenAsync")))
            //{
            //    task.Wait();
            //}
        }
        #endregion
    }
}