using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using AmazonBBS.Common;
using AmazonBBS.DAL;
using AmazonBBS.Model;

namespace AmazonBBS.BLL
{

    /// <summary>
    /// 文章表
    /// </summary>
    public class ArticleBLL : Auto_ArticleBLL
    {
        public static ArticleBLL Instance
        {
            get { return SingleHepler<ArticleBLL>.Instance; }
        }
        ArticleDAL dal = new ArticleDAL();

        public List<_Article> GetAllArticles(Paging aRticlePage, string keyWord = null, long tagid = 0)
        {
            DataSet ds = dal.GetAllArticles(aRticlePage.StartIndex, aRticlePage.EndIndex, keyWord, tagid);
            List<_Article> list = ModelConvertHelper<_Article>.ConvertToList(ds.Tables[1]);
            aRticlePage.RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

            return list;
        }

        public List<_Article> GetAllArticles(int startIndex, int endIndex, long tagid = 0)
        {
            DataSet ds = dal.GetAllArticles(startIndex, endIndex, tagId: tagid);
            List<_Article> list = ModelConvertHelper<_Article>.ConvertToList(ds.Tables[1]);
            return list;
        }


        public List<_Article> GetCheckList()
        {
            DataTable ds = dal.GetCheckList();
            List<_Article> list = new List<_Article>();
            list.AddRange(ModelConvertHelper<_Article>.ConvertToList(ds).OrderByDescending(a => Convert.ToDateTime(a.CreateTime).ToString("yyyy-MM-dd")).ThenByDescending(a => a.PVCount).ToList());
            return list;
        }

        public _Article GetArticleDetail(long articleid, long userid, Paging page)
        {
            //获取文章 基本信息
            DataTable dt = dal.GetArticleDetail(articleid, userid);
            _Article amodel = ModelConvertHelper<_Article>.ConverToModel(dt);
            if (amodel != null)
            {
                amodel.Comments = CommentBLL.Instance.GetCommentCallBack(articleid, CommentEnumType.Article.GetHashCode(), PriseEnumType.ArticleComment.GetHashCode(), userid, page, ScoreBeloneMainEnumType.Comment_Article.GetHashCode());
                amodel.Tags = TagBLL.Instance.GetTagByMainID(CommentEnumType.Article.GetHashCode(), articleid);
                return amodel;
            }
            return null;
        }

        public List<_Article> GetAllArticleList(string order, Paging page)
        {
            DataSet ds = dal.GetAllArticleList(order, page.StartIndex, page.EndIndex);
            page.RecordCount = ds.Tables[0].Rows[0][0].ToString().ToInt32();
            return ModelConvertHelper<_Article>.ConvertToList(ds.Tables[1]);
        }


        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(Article model, SqlTransaction tran)
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
        public ResultInfo Update(Article model, SqlTransaction tran = null)
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

        public MyArticleViewModel GetArticleListByUserid(long userID, Paging articlePage)
        {
            MyArticleViewModel amodel = new MyArticleViewModel();

            DataSet ds = dal.GetArticleListByUserid(userID, articlePage.StartIndex, articlePage.EndIndex, UserBaseBLL.Instance.IsMaster);
            DataTable adt = ds.Tables[1];
            if (adt.IsNotNullAndRowCount())
            {
                articlePage.RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                amodel.ArticlePage = articlePage;
                amodel.Articles = ModelConvertHelper<_Article>.ConvertToList(adt);
                amodel.ArticleHeadUrl = ds.Tables[2].Rows[0][0].ToString();
            }
            return amodel;
        }

        public bool UpdataPV(long articleId)
        {
            return dal.UpdatePV(articleId);
        }
        #endregion



        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public Article GetModel(long id, SqlTransaction tran = null)
        {
            return GetItem(id, tran);
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
        //    List<Article> li = ModelConvertHelper<Article>.ConvertToList(tab);
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
                var li = ModelConvertHelper<Article>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;


        }
        #endregion


    }
}

