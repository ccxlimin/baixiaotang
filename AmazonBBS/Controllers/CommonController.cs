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
    public class CommonController : BaseController
    {
        #region 增加标签
        /// <summary>
        /// 增加标签
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageModel">标签所属分类</param>
        /// <param name="fromConsole"></param>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult AddTag(string name, int pageModel = 0, bool fromConsole = false)
        {
            ResultInfo ri = new ResultInfo();
            if (name.IsNotNullOrEmpty())
            {
                if (name.Trim().Length <= 6)
                {
                    if (fromConsole || TagBLL.Instance.CanAddTag(UserID))
                    {
                        if (!TagBLL.Instance.ExistTag(name, pageModel))
                        {
                            Tag tag = new Tag()
                            {
                                CreateTime = DateTime.Now,
                                CreateUser = UserID.ToString(),
                                TagBelongId = pageModel,
                                TagName = name,
                                TagCreateType = fromConsole ? 1 : 2,
                                IsDelete = 0,
                            };
                            var result = TagBLL.Instance.Add(tag);
                            ri.Ok = result > 0;
                            ri.ID = result;
                        }
                        else
                        {
                            ri.Msg = "该标签已有人添加！请直接使用！";
                        }
                    }
                    else
                    {
                        ri.Type = 1;
                        ri.Msg = "本月您已添加过3次标签了！请使用已有标签吧！";
                    }
                }
                else
                {
                    ri.Msg = "标签长度最大为6！";
                }
            }
            else
            {
                ri.Msg = "标签名不能为空！";
            }
            return Result(ri);
        }
        #endregion

        #region 编辑标签
        [HttpPost]
        [IsMaster]
        public ActionResult EditTag(long id, string name)
        {
            ResultInfo ri = new ResultInfo();
            if (name.IsNotNullOrEmpty())
            {
                if (name.Trim().Length <= 10)
                {
                    var model = TagBLL.Instance.GetModel(id);
                    if (model != null)
                    {
                        if (model.IsDelete == 0)
                        {
                            model.IsDelete = 0;
                            model.TagName = name;
                            ri = TagBLL.Instance.Update(model);
                        }
                    }
                    else
                    {
                        ri.Msg = "标签不存在";
                    }
                }
                else
                {
                    ri.Msg = "标签长度最大为10！";
                }
            }
            else
            {
                ri.Msg = "标签名不能为空！";
            }

            return Result(ri);
        }
        #endregion

        #region 匹配查找标签
        //public JsonResult MatchTags(string queryType, string matchKey)
        //{
        //    if (queryType.IsNullOrEmpty() && matchKey.IsNullOrEmpty())
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        var list = TagBLL.Instance.MatchTags(matchKey, queryType).Select(a => new { Key = a.TagName, Value = a.TagId });
        //        return Json(list);
        //    }
        //}

        public JsonResult MatchTags(string matchKey)
        {
            if (matchKey.IsNullOrEmpty())
            {
                return null;
            }
            else
            {
                var list = TagBLL.Instance.MatchTags(matchKey).Select(a => new { Key = a.TagName, Value = a.TagId });
                return Json(list);
            }
        }
        #endregion

        #region 新增行业
        [IsMaster]
        public ActionResult AddTrade(string tradeName, bool isfromConsole = false)
        {
            ResultInfo ri = new ResultInfo();
            if (tradeName.IsNotNullOrEmpty())
            {
                JobTrade mdoel = new JobTrade()
                {
                    JobTradeId = Guid.NewGuid(),
                    CreateTime = DateTime.Now,
                    CreateType = isfromConsole ? 1 : 2,
                    CreateUser = UserID.ToString(),
                    IsDelete = 0,
                    JobTradeName = tradeName
                };
                if (JobTradeBLL.Instance.Add(mdoel) > 0)
                {
                    ri.Ok = true;
                }
            }
            else
            {
                ri.Msg = "行业名称不能为空";
            }
            return Result(ri);
        }
        #endregion

        #region 编辑行业
        [IsMaster]
        [HttpPost]
        public ActionResult EditTrade(Guid id, string tradeName)
        {
            ResultInfo ri = new ResultInfo();
            var model = JobTradeBLL.Instance.GetModel(id);
            if (model != null)
            {
                model.JobTradeName = tradeName;
                ri = JobTradeBLL.Instance.Update(model);
            }
            else
            {
                ri.Msg = "行业类型不存在！";
            }
            return Result(ri);
        }
        #endregion

        #region 新增职位分类
        [IsMaster]
        [HttpPost]
        public ActionResult AddJobType(string jobTypeName, string jobName, string parentId, bool isfromConsole = false, bool isJob = true)
        {
            ResultInfo ri = new ResultInfo();
            Job job = new Job()
            {
                CreateTime = DateTime.Now,
                CreateType = 1,
                CreateUser = UserID.ToString(),
                IsDelete = 0,
                JobId = Guid.NewGuid(),
            };
            if (isJob)
            {
                job.JobName = jobName;
                job.ParentJobId = Guid.Parse(parentId);
                job.IsJob = 1;
                job.IsJobType = 0;
            }
            else
            {
                job.JobType = jobTypeName;
                job.IsJob = 0;
                job.IsJobType = 1;
            }
            ri.Ok = JobBLL.Instance.Add(job) > 0;
            return Result(ri);
        }
        #endregion

        #region 编辑职位
        [IsMaster]
        [HttpPost]
        public ActionResult EditJob(Guid id, string jobName)
        {
            ResultInfo ri = new ResultInfo();
            var model = JobBLL.Instance.GetModel(id);
            if (model != null)
            {
                if (model.IsDelete == 0)
                {
                    model.JobName = jobName;
                    ri = JobBLL.Instance.Update(model);
                }
                else
                {
                    ri.Msg = "职位已被删除！";
                }
            }
            else
            {
                ri.Msg = "职位不存在！";
            }
            return Result(ri);
        }
        #endregion

        #region 获取点赞人数 
        [HttpGet]
        public ActionResult getPriseUsers(int type, long mid)
        {
            ResultInfo ri = new ResultInfo();

            ri.Data = DB.Prise.Where(a => a.Type == type && a.TargetID == mid && a.IsDelete == 0)
                .Join(DB.UserBase.Where(a => a.IsDelete == 0), a => a.UserID, b => b.UserID, (a, b) => new
                {
                    name = b.UserName,
                    id = b.UserID,
                    time = a.PriseTime,
                }).ToList();
            ri.Ok = true;
            return Result(ri);
        }
        #endregion

        #region 获取反对人数 
        [HttpGet]
        public ActionResult getAgainstUsers(int type, long mid)
        {
            ResultInfo ri = new ResultInfo();

            ri.Data = DB.Against.Where(a => a.Type == type && a.TargetID == mid && a.IsDelete == 0)
                .Join(DB.UserBase.Where(a => a.IsDelete == 0), a => a.UserID, b => b.UserID, (a, b) => new
                {
                    name = b.UserName,
                    id = b.UserID,
                    time = a.AgainstTime,
                }).ToList();
            ri.Ok = true;
            return Result(ri);
        }
        #endregion

        #region 举报
        /// <summary>
        /// 举报
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public ActionResult Report(int type, long id, string desc)
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                //管理员你好，用户 李巴巴 举报 了 李巴巴 发表的 文章 《》，举报理由：123
                string mainDesc = string.Empty;
                long authorUserId = 0;
                string title = string.Empty;//标题
                long mainId = 0;
                string url = string.Empty;
                //举报帖子
                if (type == 1)
                {
                    var bbs = DB.Question.FirstOrDefault(a => a.QuestionId == id && a.IsDelete == 0);
                    if (bbs != null)
                    {
                        mainDesc = "帖子";
                        authorUserId = bbs.UserID.Value;
                        title = bbs.Title;
                        mainId = id;
                        url = "/bbs/detail/{0}".FormatWith(mainId);
                        NoticeBLL.Instance.OnReport_Main_Notice_Master(UserID, url, title, authorUserId, mainDesc, desc, NoticeTypeEnum.Report);
                        ri.Ok = true;
                    }
                }
                else if (type == 2)
                {
                    var article = DB.Article.FirstOrDefault(a => a.ArticleId == id && a.IsDelete == 0);
                    if (article != null)
                    {
                        mainDesc = "文章";
                        authorUserId = article.UserID.Value;
                        title = article.Title;
                        mainId = id;
                        url = "/article/detail/{0}".FormatWith(mainId);
                        NoticeBLL.Instance.OnReport_Main_Notice_Master(UserID, url, title, authorUserId, mainDesc, desc, NoticeTypeEnum.Report);
                        ri.Ok = true;
                    }
                }
                else if (type == 3)
                {
                    var comment = DB.Comment.FirstOrDefault(a => a.CommentId == id && a.IsDelete == 0);
                    if (comment != null)
                    {
                        var bbs = DB.Question.FirstOrDefault(a => a.QuestionId == comment.MainID && a.IsDelete == 0);
                        if (bbs != null)
                        {
                            authorUserId = bbs.UserID.Value;
                            title = bbs.Title;
                            mainId = bbs.QuestionId;
                            mainDesc = "评论";
                            url = "/bbs/detail/{0}".FormatWith(mainId);
                            NoticeBLL.Instance.OnReport_Comment_Notice_Master(UserID, comment.CommentId, comment.CommentContent, url, title, authorUserId, desc, NoticeTypeEnum.Report);
                            ri.Ok = true;
                        }
                    }
                }
                else if (type == 4)
                {
                    var comment = DB.Comment.FirstOrDefault(a => a.CommentId == id && a.IsDelete == 0);
                    if (comment != null)
                    {
                        var article = DB.Article.FirstOrDefault(a => a.ArticleId == id && a.IsDelete == 0);
                        if (article != null)
                        {
                            mainDesc = "评论";
                            authorUserId = article.UserID.Value;
                            title = article.Title;
                            mainId = article.ArticleId;
                            url = "/article/detail/{0}".FormatWith(mainId);
                            NoticeBLL.Instance.OnReport_Comment_Notice_Master(UserID, comment.CommentId, comment.CommentContent, url, title, authorUserId, desc, NoticeTypeEnum.Report);
                            ri.Ok = true;
                        }
                    }
                }
            }
            catch
            {
                ri.Msg = "举报失败！";
            }
            return Result(ri);
        }
        #endregion

        #region 获取鼠标点击提示语
        [HttpGet]
        public ActionResult ClickMsgs()
        {
            return Result(new ResultInfo
            {
                Ok = true,
                Data = ClickMsgBLL.Instance.FindALL(0).Select(a => new { msg = a.Msg, color = a.Color }).ToList(),
            });
        }
        #endregion
    }
}