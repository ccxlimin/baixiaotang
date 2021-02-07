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
    /// 页面模块对应标签
    /// </summary>
    public class MenuBelongTagBLL : Auto_MenuBelongTagBLL
    {
        public static MenuBelongTagBLL Instance
        {
            get { return SingleHepler<MenuBelongTagBLL>.Instance; }
        }
        MenuBelongTagDAL dal = new MenuBelongTagDAL();

        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(MenuBelongTag model, SqlTransaction tran = null)
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
        public ResultInfo Update(MenuBelongTag model, SqlTransaction tran = null)
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
        public MenuBelongTag GetModel(long id)
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
        //    List<MenuBelongTag> li = ModelConvertHelper<MenuBelongTag>.ConvertToList(tab);
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
                var li = ModelConvertHelper<MenuBelongTag>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;
        }
        #endregion

        #region 添加标签处理
        /// <summary>
        /// 添加标签处理
        /// </summary>
        /// <param name="mainId"></param>
        /// <param name="menuType"></param>
        /// <param name="tagType"></param>
        /// <param name="tags"></param>
        public bool HandleTags(long mainId, CommentEnumType mainType, int tagType, string[] tagArr, SqlTransaction tran = null)
        {
            List<string> tagList = new List<string>();
            if (tagArr == null || tagArr.Length == 0)
            {
                //用户没有选择标签时，由系统自动分配标签(取使用最多的前3个标签)
                var newList = TagBLL.Instance.GetTop3Tag(tagType, tran);
                newList.ForEach(tag => tagList.Add(tag.TagId.ToString()));
            }
            else
            {
                tagList = tagArr.ToList();
            }
            List<MenuBelongTag> list = new List<MenuBelongTag>();
            tagList.ForEach(tag =>
            {
                list.Add(new MenuBelongTag()
                {
                    MainId = mainId,
                    MainType = mainType.GetHashCode(),
                    TagId = tag.ToInt64()
                });
            });
            if (list.Count > 0)
            {
                return SqlHelper.SqlBulkCopyByDatatable("MenuBelongTag", list.ToDataTable<MenuBelongTag>(), tran);
            }
            else
            {
                return true;
            }
        }

        public bool RemoveAndAddNew(long mainId, int menuType, CommentEnumType mainType, string[] tags, SqlTransaction tran)
        {
            if (DeleteTagForMainID(mainId, mainType, tran))
            {
                if (HandleTags(mainId, mainType, menuType, tags, tran))
                {
                    return true;
                }
            }
            return false;
        }

        public bool DeleteTagForMainID(long mainId, CommentEnumType mainType, SqlTransaction tran)
        {
            if (dal.HasTagByMainId(mainId, mainType.GetHashCode()))
            {
                return dal.DeleteTagForMainID(mainId, mainType.GetHashCode(), tran);
            }
            else { return true; }
        }
        #endregion

        #region 获取标签下的帖子和文章数量
        public int[] Count(long tagId)
        {
            var ds = dal.Count(tagId);
            return new[] { ds.Tables[0].Rows[0][0].ToString().ToInt32(), ds.Tables[1].Rows[0][0].ToString().ToInt32() };
        }
        #endregion
    }
}
