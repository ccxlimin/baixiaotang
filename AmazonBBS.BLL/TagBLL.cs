using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using AmazonBBS.Common;
using AmazonBBS.DAL;
using AmazonBBS.Model;

namespace AmazonBBS.BLL
{

    /// <summary>
    /// 标签
    /// </summary>
    public class TagBLL : Auto_TagBLL
    {
        public static TagBLL Instance
        {
            get { return SingleHepler<TagBLL>.Instance; }
        }

        TagDAL dal = new TagDAL();

        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(Tag model, SqlTransaction tran = null)
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

        public bool ExistTag(string name, int pageModel = 0)
        {
            return dal.ExistTag(name, pageModel).ToInt32() > 0;
        }

        /// <summary>
        /// 如果不是从后台添加的标签，那么判断用户当月是否已添加过3次了，超过3次不能再添加
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool CanAddTag(long userID)
        {
            if (UserBaseBLL.Instance.IsMaster) { return true; }
            return dal.CanAddTag(userID, DateTime.Now.AddDays(-DateTime.Now.Day + 1).Date).ToInt32() < ConfigHelper.AppSettings("UserAddTagCount").ToInt32();
        }
        #endregion

        #region update
        /// <summary>
        /// 修改 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultInfo Update(Tag model, SqlTransaction tran = null)
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
        public Tag GetModel(long id)
        {
            return GetItem(id);
        }

        /// <summary>
        /// 标签匹配
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="matchKey"></param>
        /// <returns></returns>
        public List<Tag> MatchTags(string matchKey, string queryType = null)
        {
            return ModelConvertHelper<Tag>.ConvertToList(dal.MatchTags(matchKey, queryType));
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
        //    List<Tag> li = ModelConvertHelper<Tag>.ConvertToList(tab);
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
                var li = ModelConvertHelper<Tag>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;
        }

        /// <summary>
        /// 获取使用最多的3个标签
        /// </summary>
        /// <param name="tagType"></param>
        /// <returns></returns>
        public List<Tag> GetTop3Tag(int tagType, SqlTransaction tran)
        {
            return ModelConvertHelper<Tag>.ConvertToList(dal.GetTop3Tag(tagType, tran));
        }

        /// <summary>
        /// 根据 所属页面模块类型 分页获取标签
        /// </summary>
        /// <param name="type"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<_Tag> GetTags(int type, Paging page)
        {
            var ds = dal.GetTags(type, page.StartIndex, page.EndIndex);
            page.RecordCount = ds.Tables[0].Rows[0][0].ToString().ToInt32();
            return ModelConvertHelper<_Tag>.ConvertToList(ds.Tables[1]);
        }
        #endregion

        #region 随机获取N个标签(前8个标签按创建时间，后面的标签按量的多少排序)
        public List<List<HomeTagsViewModel>> GetRandomTag()
        {
            int newcount = CacheBLL.Instance.Get_TagFixedNumber();
            int count = CacheBLL.Instance.Get_TagRandomNumber();
            var ds = dal.GetRandomTag(newcount, count);
            var list = new List<List<HomeTagsViewModel>>();
            list.Add(ModelConvertHelper<HomeTagsViewModel>.ConvertToList(ds.Tables[0]));
            list.Add(ModelConvertHelper<HomeTagsViewModel>.ConvertToList(ds.Tables[1]));
            return list;
        }
        #endregion

        #region 获取所有的标签
        public List<HomeTagsViewModel> GetAllTags(TagsSortTypeEnum tagsSortTypeEnum)
        {
            return ModelConvertHelper<HomeTagsViewModel>.ConvertToList(dal.GetAllTags(tagsSortTypeEnum));
        }

        /// <summary>
        /// 根据对象内容（文章，问题）ID获取所属标签
        /// </summary>
        /// <param name="mainType"></param>
        /// <param name="mainId"></param>
        /// <returns></returns>
        public List<Tag> GetTagByMainID(int mainType, long mainId)
        {
            return ModelConvertHelper<Tag>.ConvertToList(dal.GetTagByMainID(mainType, mainId));
        }
        #endregion
    }
}
