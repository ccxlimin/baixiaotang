using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using AmazonBBS.Common;
using AmazonBBS.DAL;
using AmazonBBS.Model;
using System.Data.SqlClient;

namespace AmazonBBS.BLL
{

    /// <summary>
    /// 提问表
    /// </summary>
    public class QuestionBLL : Auto_QuestionBLL
    {
        public static QuestionBLL Instance
        {
            get { return SingleHepler<QuestionBLL>.Instance; }
        }
        QuestionDAL dal = new QuestionDAL();

        /// <summary>
        /// 获取浏览量最多的N条记录
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public List<_QuestionInfo> GetListForIndex(int top)
        {
            return ModelConvertHelper<_QuestionInfo>.ConvertToList(dal.GetListForIndex(top));
        }

        /// <summary>
        /// 获取评论量最多的N条记录
        /// </summary>
        /// <returns></returns>
        public List<_QuestionInfo> GetTop5QuesForIndex(int top)
        {
            return ModelConvertHelper<_QuestionInfo>.ConvertToList(dal.GetTop5QuesForIndex(top));
        }

        public List<_QuestionInfo> GetListBySortType(HomeSortTypeEnum sortType, int totalCount, string sortConfig, Paging page)
        {
            string sqlCondition = string.Empty;
            StringBuilder orderBy = new StringBuilder();
            if (sortConfig.IsNullOrEmpty())
            {
                orderBy.Append("a.IsTop desc,a.IsJinghua desc,a.PVCount desc,");//默认排序
            }
            else
            {
                List<string> sortConfigs = sortConfig.Split('+').ToList();
                string orderType = sortConfigs.Contains("7") ? "asc" : "desc";
                sortConfigs.ForEach(sortItem =>
                {
                    switch (sortItem)
                    {
                        //1置顶 2热门 3精华 4浏览量 5创建时间
                        case "1": orderBy.AppendFormat(" a.IsTop {0},", orderType); break;
                        case "2": orderBy.AppendFormat(" a.IsRemen {0},", orderType); break;
                        case "3": orderBy.AppendFormat(" a.IsJinghua {0},", orderType); break;
                        case "4": orderBy.AppendFormat(" a.PVCount {0},", orderType); break;
                        case "5": orderBy.AppendFormat(" a.CreateTime {0},", orderType); break;
                    }
                });
            }
            orderBy.Remove(orderBy.Length - 1, 1);
            switch (sortType)
            {
                case HomeSortTypeEnum.Sort_JinHua:
                    //--精华
                    sqlCondition = " and a.IsJinghua=1 ";
                    break;
                case HomeSortTypeEnum.Sort_Hot:
                    //--热门
                    sqlCondition = " and a.IsRemen=1 ";
                    break;
                case HomeSortTypeEnum.Sort_Top:
                    // --置顶
                    sqlCondition = " and a.IsTop=1 ";
                    break;
                case HomeSortTypeEnum.Sort_New:
                    //-- 新贴（7天内的所有帖子）
                    sqlCondition = " and a.CreateTime>='{0}' ".FormatWith(DateTime.Now.AddDays(-ConfigHelper.AppSettings("NewTieZiDays").ToInt32()).ToString(1));
                    orderBy.Clear().Append(" a.CreateTime desc ");
                    break;
                case HomeSortTypeEnum.Sort_NoComment:
                    //--待回复(即评论数为0的)
                    sqlCondition = " and (select count(1) from Comment where MainID=a.QuestionId and MainType=1 and IsDelete=0)=0 ";
                    orderBy.Clear().Append(" a.CreateTime desc ");
                    break;
                default: sqlCondition = " and 1=1 "; break;
            }
            DataSet ds = dal.GetListBySortType(orderBy.ToString(), sqlCondition, totalCount, page.StartIndex, page.EndIndex);
            page.RecordCount = ds.Tables[0].Rows[0][0].ToString().ToInt32();
            return ModelConvertHelper<_QuestionInfo>.ConvertToList(ds.Tables[1]);
        }

        public List<_QuestionInfo> GetAllBBSList(string orderType, Paging page)
        {
            DataSet ds = dal.GetAllBBSList(orderType, page.StartIndex, page.EndIndex);
            page.RecordCount = ds.Tables[0].Rows[0][0].ToString().ToInt32();
            return ModelConvertHelper<_QuestionInfo>.ConvertToList(ds.Tables[1]);
        }

        /// <summary>
        /// 根据问题ID获取点赞数最多的评论
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public _Comment GetNiceComment(long questionId, long? bestanswerid)
        {
            return ModelConvertHelper<_Comment>.ConverToModel(dal.GetNiceComment(questionId, bestanswerid));
        }

        public List<_QuestionInfo> GetALLQuestion()
        {
            return ModelConvertHelper<_QuestionInfo>.ConvertToList(dal.GetALLQuestion());
        }

        public List<_QuestionInfo> GetCheckList()
        {
            DataTable ds = dal.GetCheckList();
            List<_QuestionInfo> list = new List<_QuestionInfo>();
            list.AddRange(ModelConvertHelper<_QuestionInfo>.ConvertToList(ds).OrderByDescending(a => Convert.ToDateTime(a.CreateTime).ToString("yyyy-MM-dd")).ThenByDescending(a => a.PVCount).ToList());
            return list;
        }

        public void PVUpdate(long id)
        {
            dal.PVUpdate(id);
        }

        /// <summary>
        /// 查找用户所有提问问题
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public BBSListViewModel GetQuestionListByUserid(long userID, Paging page)
        {
            BBSListViewModel bmodel = new BBSListViewModel();
            DataSet ds = dal.GetQuestionListByUserid(userID, page.StartIndex, page.EndIndex, UserBaseBLL.Instance.IsMaster);
            DataTable questiondt = ds.Tables[1];

            if (questiondt.IsNotNullAndRowCount())
            {
                page.RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                bmodel.QuestionPage = page;
                bmodel.QuestionList = ModelConvertHelper<_QuestionInfo>.ConvertToList(questiondt);
                bmodel.QuestionHeadUrl = ds.Tables[2].Rows[0][0].ToString();
            }
            return bmodel;
        }


        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(Question model, SqlTransaction tran)
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

        public _QuestionInfo GetQuestionDetail(int id, long userid, Paging page)
        {
            //获取问题 基本信息
            DataTable dt = dal.GetQuestionDetail(id, userid);
            _QuestionInfo qmodel = ModelConvertHelper<_QuestionInfo>.ConverToModel(dt);
            if (qmodel != null)
            {
                #region MyRegion
                //qmodel.Comments = new _Comments();
                ////获取问题对应评论
                //DataSet ds = CommentBLL.Instance.GetCommentsByType(id, userid, 1, page.StartIndex, page.EndIndex);
                //int recordCount =Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                //if (recordCount>0)
                //{
                //    page.RecordCount = recordCount;
                //    qmodel.Comments.Comments = ModelConvertHelper<_Comment>.ConvertToList(ds.Tables[1]);
                //    //获取评论的回复
                //    qmodel.Comments.Comments.ForEach(item =>
                //    {
                //        //(非回答作者)如果该回答需要付费并且没有付过费，则先不加载评论的回复
                //        if (item.CommentUserID == userid || item.IsHideOrFeeToSee != 1 || item.IsFeeAnswer)
                //        {
                //            item.ReplyList = ModelConvertHelper<_ReplyComment>.ConvertToList(dal.GetReplyList(item.CommentId, userid));
                //        }
                //    });
                //}
                //qmodel.Comments.CommentPage = page; 
                #endregion
                qmodel.Comments = CommentBLL.Instance.GetCommentCallBack(id, CommentEnumType.BBS.GetHashCode(), PriseEnumType.BBSComment.GetHashCode(), userid, page, ScoreBeloneMainEnumType.Comment_BBS.GetHashCode());
                qmodel.Tags = TagBLL.Instance.GetTagByMainID(CommentEnumType.BBS.GetHashCode(), id);
                return qmodel;
            }
            return null;
        }

        /// <summary>
        /// 根据类型获取问题列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="questionPage"></param>
        /// <param name="keyWords">关键词</param>
        /// <returns></returns>
        public List<_QuestionInfo> GetQuestionList(int id, Paging questionPage, string keyWords = null, long tagId = 0)
        {
            DataSet ds = dal.GetQuestionList(id, questionPage.StartIndex, questionPage.EndIndex, keyWords, tagId);
            List<_QuestionInfo> list = new List<_QuestionInfo>();
            if (keyWords.IsNotNullOrEmpty() || tagId > 0)
            {
                questionPage.RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                list.AddRange(ModelConvertHelper<_QuestionInfo>.ConvertToList(ds.Tables[1]).OrderByDescending(a => Convert.ToDateTime(a.CreateTime).ToString("yyyy-MM-dd")).ThenByDescending(a => a.PVCount).ToList());
            }
            else
            {
                questionPage.RecordCount = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
                //如果不是第一页，则不加载置顶、精华等数据
                if (questionPage.PageIndex == 1)
                {
                    list.AddRange(ModelConvertHelper<_QuestionInfo>.ConvertToList(ds.Tables[0]).OrderByDescending(a => Convert.ToDateTime(a.CreateTime).ToString("yyyy-MM-dd")).ThenByDescending(a => a.PVCount).ToList());
                }
                list.AddRange(ModelConvertHelper<_QuestionInfo>.ConvertToList(ds.Tables[2]).OrderByDescending(a => Convert.ToDateTime(a.CreateTime).ToString("yyyy-MM-dd")).ThenByDescending(a => a.PVCount).ToList());
            }
            return list;
        }

        #region 搜索帖子
        /// <summary>
        /// 搜索帖子
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<_QuestionInfo> SearchQuestion(string keywords, Paging page)
        {
            DataSet ds = dal.SearchQuestion(page.StartIndex, page.EndIndex, keywords);
            List<_QuestionInfo> list = new List<_QuestionInfo>();
            page.RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            list.AddRange(ModelConvertHelper<_QuestionInfo>.ConvertToList(ds.Tables[1]).OrderByDescending(a => Convert.ToDateTime(a.CreateTime).ToString("yyyy-MM-dd")).ThenByDescending(a => a.PVCount).ToList());
            return list;
        }
        #endregion
        #endregion

        #region update
        /// <summary>
        /// 修改 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultInfo Update(Question model, SqlTransaction tran = null)
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

        public bool AddAttachMent(List<AttachMent> attachMents, SqlTransaction tran)
        {
            if (attachMents.Count == 0)
                return true;
            return dal.AddAttachMent(attachMents, tran);
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public Question GetModel(long id)
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
        //    List<Question> li = ModelConvertHelper<Question>.ConvertToList(tab);
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
                var li = ModelConvertHelper<Question>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">回答ID</param>
        /// <param name="qid">问题ID</param>
        /// <returns></returns>
        public bool HasBestAnswer(long id, long qid)
        {
            return Convert.ToInt32(dal.HasBestAnswer(id, qid)) > 0;
        }

        public bool BsetAnswer(long id, long qid, SqlTransaction tran)
        {
            return dal.BsetAnswer(id, qid, tran);
        }
        #endregion
    }
}

