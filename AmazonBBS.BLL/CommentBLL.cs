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
    /// 评论表
    /// </summary>
    public class CommentBLL : Auto_CommentBLL
    {
        public static CommentBLL Instance
        {
            get { return SingleHepler<CommentBLL>.Instance; }
        }
        CommentDAL dal = new CommentDAL();

        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(Comment model)
        {
            ResultInfo ri = new ResultInfo();

            if (model == null) return ri;

            int result = Add(model);

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
        public ResultInfo Update(Comment model)
        {
            ResultInfo ri = new ResultInfo();
            if (Edit(model))
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
        public Comment GetModel(long id)
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
        //    List<Comment> li = ModelConvertHelper<Comment>.ConvertToList(tab);
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
                var li = ModelConvertHelper<Comment>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;


        }

        public long FindUserIDByCommentID(long id, SqlTransaction tran)
        {
            return dal.FindUserIDByCommentID(id, tran);
        }
        #endregion

        public DataSet GetCommentsByType(long mainID, long userID, int mainType, int sbiMainType, int priseType, int startIndex, int endIndex)
        {
            return dal.GetCommentsByType(mainID, userID, mainType, sbiMainType, priseType, startIndex, endIndex);
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="mainID"></param>
        /// <param name="mainType">评论对象类型(1贴吧 2文章 3活动 4礼物 5招聘 6求职 7产品服务 8数据分析 9课程)</param>
        /// <param name="priseType">点赞类型</param>
        /// <param name="userID"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public _Comments GetCommentCallBack(long mainID, int mainType, int priseType, long userID, Paging page, int sbiMainType = 0)
        {
            _Comments comments = new _Comments() { Comments = new List<_Comment>() };
            //获取问题对应评论
            DataSet ds = CommentBLL.Instance.GetCommentsByType(mainID, userID, mainType, sbiMainType, priseType, page.StartIndex, page.EndIndex);
            int recordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            if (recordCount > 0)
            {
                page.RecordCount = recordCount;
                comments.Comments = ModelConvertHelper<_Comment>.ConvertToList(ds.Tables[1]);
                //获取评论的回复
                comments.Comments.ForEach(item =>
                {
                    //(非回答作者)如果该回答需要付费并且没有付过费，则先不加载评论的回复
                    if (item.CommentUserID == userID || item.IsHideOrFeeToSee != 1 || item.IsFeeAnswer)
                    {
                        item.ReplyList = ModelConvertHelper<_ReplyComment>.ConvertToList(dal.GetReplyList(item.CommentId, userID, mainType, priseType));
                    }
                });
            }
            comments.CommentPage = page;
            return comments;
        }

        public long GetAuthorID(long id, int mainType)
        {
            string authorid = dal.GetAuthorID(id, mainType);
            return authorid.IsNotNullOrEmpty() ? Convert.ToInt64(authorid) : 0;
        }

        /// <summary>
        /// 根据ID获取评论的内容及评论对应的 主文章/帖子 的标题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mainType"></param>
        /// <returns></returns>
        public DataTable GetCommentInfoAndMainTitleByMainID(long id, int mainType, string sql)
        {
            return dal.GetCommentInfoAndMainTitleByMainID(id, mainType, sql);
        }

        public long FindUserIDByAnswerID(long id)
        {
            return dal.FindUserIDByAnswerID(id);
        }

        /// <summary>
        /// 根据用户ID获取最佳评论/回答
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="priseEnumType"></param>
        /// <param name="bestornice">1最佳解答 2最优解答</param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public MyCommentsViewModel GetBestAnswersByUserID(long userID, PriseEnumType priseEnumType, int bestornice, Paging paging)
        {
            MyCommentsViewModel model = new MyCommentsViewModel();
            DataSet ds = dal.GetBestAnswersByUserID(userID, priseEnumType.GetHashCode(), bestornice, paging.StartIndex, paging.EndIndex);
            int recordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            if (recordCount > 0)
            {
                paging.RecordCount = recordCount;
                model.CommentPage = paging;
                model.CommentList = ModelConvertHelper<_MyComments>.ConvertToList(ds.Tables[1]);
                model.CommentHeadUrl = ds.Tables[2].Rows[0][0].ToString();
            }
            return model;
        }

        /// <summary>
        /// 根据用户ID获取评论/回答
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="answerPage"></param>
        /// <returns></returns>
        public MyCommentsViewModel GetCommentListByUserid(long userID, CommentEnumType commentEnumType, Paging answerPage, long currentLoginUserID)
        {
            MyCommentsViewModel amodel = new MyCommentsViewModel();
            DataSet ds = dal.GetCommentListByUserid(userID, commentEnumType.GetHashCode(), answerPage.StartIndex, answerPage.EndIndex, currentLoginUserID, UserBaseBLL.Instance.IsMaster);
            DataTable answerdt = ds.Tables[1];
            if (answerdt.IsNotNullAndRowCount())
            {
                answerPage.RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                amodel.CommentPage = answerPage;
                amodel.CommentList = ModelConvertHelper<_MyComments>.ConvertToList(answerdt);
                amodel.CommentHeadUrl = ds.Tables[2].Rows[0][0].ToString();
            }
            return amodel;
        }

        public bool EditComment(int mainType, long id, string content)
        {
            return dal.EditComment(mainType, id, content);
        }

        public bool DeleteComment(int mainType, long id)
        {
            return dal.DeleteComment(mainType, id);
        }
    }
}

