using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using AmazonBBS.Common;
using AmazonBBS.DAL;
using AmazonBBS.Model;

namespace AmazonBBS.BLL
{

    /// <summary>
    /// 枚举
    /// </summary>
    public class BBSEnumBLL : Auto_BBSEnumBLL
    {
        public static BBSEnumBLL Instance
        {
            get { return SingleHepler<BBSEnumBLL>.Instance; }
        }
        BBSEnumDAL dal = new BBSEnumDAL();

        public BBSEnum GetInfo(string id)
        {
            DataTable dt = dal.GetInfo(id);
            return ModelConvertHelper<BBSEnum>.ConverToModel(dt);
        }

        public List<BBSEnum> GetBBSMenus()
        {
            DataTable dt = dal.GetBBSMenus();
            return ModelConvertHelper<BBSEnum>.ConvertToList(dt);
        }

        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(BBSEnum model)
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
        public ResultInfo Update(BBSEnum model)
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
        public BBSEnum GetModel(long id)
        {
            return GetItem(id);
        }

        /// <summary>
        /// 分享链接对应奖励
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<ShareCoinAndCount> GetShareCoinList(long userID, ref int count)
        {
            DataSet ds = dal.GetShareCoinList(userID);
            count = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            return ModelConvertHelper<ShareCoinAndCount>.ConvertToList(ds.Tables[1]);
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
        //    List<BBSEnum> li = ModelConvertHelper<BBSEnum>.ConvertToList(tab);
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
                var li = ModelConvertHelper<BBSEnum>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;


        }
        #endregion

        /// <summary>
        /// 按类目查询所有条目
        /// </summary>
        /// <param name="enumType">类型</param>
        /// <param name="sort">是否排序</param>
        /// <returns></returns>
        public List<BBSEnum> Query(int enumType, bool sort = false)
        {
            string key = "MenuCache_{0}".FormatWith(enumType);
            //缓存设置
            var cachevalue = CSharpCacheHelper.Get(key);
            if (cachevalue == null)
            {
                DataTable dt = dal.Query(enumType, sort);
                var list = ModelConvertHelper<BBSEnum>.ConvertToList(dt);
                CSharpCacheHelper.Set(key, list, 120);
                return list.OrderBy(a => a.SortIndex).ToList();
            }
            else
            {
                return (cachevalue as List<BBSEnum>).OrderBy(a => a.SortIndex).ToList();
            }
        }

        /// <summary>
        /// 获取能够发表文章的信息
        /// </summary>
        /// <returns></returns>
        public BBSEnum GetSetArticleRol()
        {
            return ModelConvertHelper<BBSEnum>.ConverToModel(dal.GetSetArticleRol());
        }

        public bool Exist(string cn, int type, int id)
        {
            return Convert.ToInt32(dal.Exist(cn, type, id)) > 0;
        }
    }
}

