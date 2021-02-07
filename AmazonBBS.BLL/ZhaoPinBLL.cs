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
    /// 人才招聘
    /// </summary>
    public class ZhaoPinBLL : Auto_ZhaoPinBLL
    {
        public static ZhaoPinBLL Instance
        {
            get { return SingleHepler<ZhaoPinBLL>.Instance; }
        }
        ZhaoPinDAL dal = new ZhaoPinDAL();

        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(ZhaoPin model, SqlTransaction tran)
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
        public ResultInfo Update(ZhaoPin model)
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
        public ZhaoPin GetModel(long id)
        {
            return GetItem(id);
        }

        public _ZhaoPin GetZhaoPinDetail(long id, long userid, Paging page)
        {
            _ZhaoPin model = ModelConvertHelper<_ZhaoPin>.ConverToModel(dal.GetDetail(id, userid));
            if (model != null)
            {
                model.Comments = CommentBLL.Instance.GetCommentCallBack(id, CommentEnumType.ZhaoPin.GetHashCode(), PriseEnumType.ZhaoPinComment.GetHashCode(), userid, page);
                return model;
            }
            return null;
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
        //    List<ZhaoPin> li = ModelConvertHelper<ZhaoPin>.ConvertToList(tab);
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
                var li = ModelConvertHelper<ZhaoPin>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;


        }
        #endregion

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ZhaoPinViewModel FindAll(Paging page, string key = null)
        {
            ZhaoPinViewModel model = new ZhaoPinViewModel();
            var ds = dal.SearchByRows(page.StartIndex, page.EndIndex, key);
            int count = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            page.RecordCount = count;
            model.ZhaoPinList = ModelConvertHelper<_ZhaoPin>.ConvertToList(ds.Tables[1]);
            model.ZhaoPinPage = page;
            return model;
        }

        public ZhaoPinViewModel SelectByCondition(Paging page, string search_jobTrade, string search_job, string search_companyName, string search_workPlace, string search_money, string search_study, string search_worktype)
        {
            StringBuilder sb = new StringBuilder();
            if (IsSafe(search_jobTrade))
            {
                sb.Append(@" and a.BelongJobTrade='{0}' ".FormatWith(search_jobTrade));
            }
            if (IsSafe(search_job))
            {
                sb.Append(" and a.BelongJob = '{0}' ".FormatWith(search_job));
            }
            if (IsSafe(search_companyName))
            {
                sb.Append(" and a.CName like '%{0}%' ".FormatWith(search_companyName));
            }
            if (IsSafe(search_workPlace))
            {
                search_workPlace = search_workPlace.IndexOf("市") > -1 ? search_workPlace.Replace("市", string.Empty) : search_workPlace;
                sb.Append(" and a.WorkPlace like '%{0}%'".FormatWith(search_workPlace));
            }
            if (IsSafe(search_money) && MatchHelper.IsNum.IsMatch(search_money))
            {
                sb.Append(" and a.[Money]={0}".FormatWith(search_money));
            }
            if (IsSafe(search_study) && MatchHelper.IsNum.IsMatch(search_study))
            {
                sb.Append(" and a.Study={0}".FormatWith(search_study));
            }
            if (IsSafe(search_worktype) && MatchHelper.IsNum.IsMatch(search_worktype))
            {
                sb.Append(" and a.WorkeType={0}".FormatWith(search_worktype));
            }

            var model = new ZhaoPinViewModel();
            var ds = dal.SelectByCondition(page.StartIndex, page.EndIndex, sb.ToString());
            int count = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            page.RecordCount = count;
            model.ZhaoPinList = ModelConvertHelper<_ZhaoPin>.ConvertToList(ds.Tables[1]);
            model.ZhaoPinPage = page;
            return model;
        }

        private bool IsSafe(string condition)
        {
            if ("delete,update,select".IndexOf(condition.ToLower()) > -1)
            {
                return false;
            }
            else
            {
                if (condition.IsNotNullOrEmpty())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 获取用户的所有招聘信息，并排除已邀请的求职ID对应的招聘信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<ZhaoPin> GetZhaoPinInfos(long id, long userid)
        {
            return ModelConvertHelper<ZhaoPin>.ConvertToList(dal.GetZhaoPinInfos(id, userid));
        }

        public BaseListViewModel<ZhaoPin> GetZhaoPinList(long userID, Paging page)
        {
            var model = new BaseListViewModel<ZhaoPin>();
            DataSet ds = dal.GetZhaoPinList(userID, page.StartIndex, page.EndIndex);
            model.Page = page;
            model.Page.RecordCount = ds.Tables[0].Rows[0][0].ToString().ToInt32();
            model.DataList = ModelConvertHelper<ZhaoPin>.ConvertToList(ds.Tables[1]);
            return model;
        }

        public ZhaoPin GetZhaoPinBuyPay(long mainid)
        {
            return ModelConvertHelper<ZhaoPin>.ConverToModel(dal.GetZhaoPinBuyPay(mainid));
        }
    }
}

