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
    /// 人才求职
    /// </summary>
    public class QiuZhiBLL : Auto_QiuZhiBLL
    {
        public static QiuZhiBLL Instance
        {
            get { return SingleHepler<QiuZhiBLL>.Instance; }
        }

        public QiuZhiViewModel FindAll(Paging page, string key = null)
        {
            QiuZhiViewModel model = new QiuZhiViewModel();
            var ds = dal.SearchByRows(page.StartIndex, page.EndIndex, key);
            int count = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            page.RecordCount = count;
            model.QiuZhiList = ModelConvertHelper<_QiuZhi>.ConvertToList(ds.Tables[1]);
            model.QiuZhiPage = page;
            return model;
        }

        QiuZhiDAL dal = new QiuZhiDAL();

        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(QiuZhi model)
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
        public ResultInfo Update(QiuZhi model)
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
        public QiuZhi GetModel(long id)
        {
            return GetItem(id);
        }

        public _QiuZhi GetQiuZhiDetail(long id, long userID, Paging page)
        {
            _QiuZhi model = ModelConvertHelper<_QiuZhi>.ConverToModel(dal.GetQiuZhiDetail(id, userID));
            if (model != null)
            {
                model.Comments = CommentBLL.Instance.GetCommentCallBack(id, CommentEnumType.QiuZhi.GetHashCode(), PriseEnumType.QiuZhiComment.GetHashCode(), userID, page);
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
        //    List<QiuZhi> li = ModelConvertHelper<QiuZhi>.ConvertToList(tab);
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
                var li = ModelConvertHelper<QiuZhi>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;


        }
        #endregion

        #region 筛选
        public QiuZhiViewModel SelectByCondition(Paging page, string search_jobTrade, string search_job, string search_WorkYear, string search_workPlace, string search_money, string search_study, string search_worktype)
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
            if (IsSafe(search_WorkYear))
            {
                sb.Append(" and a.WorkYear = {0}' ".FormatWith(search_WorkYear));
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
                sb.Append(" and a.WorkType={0}".FormatWith(search_worktype));
            }

            var model = new QiuZhiViewModel();
            var ds = dal.SelectByCondition(page.StartIndex, page.EndIndex, sb.ToString());
            int count = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            page.RecordCount = count;
            model.QiuZhiList = ModelConvertHelper<_QiuZhi>.ConvertToList(ds.Tables[1]);
            model.QiuZhiPage = page;
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

        public BaseListViewModel<QiuZhi> GetQiuZhiList(long userID, Paging paging)
        {
            var model = new BaseListViewModel<QiuZhi>();
            DataSet ds = dal.GetQiuZhiList(userID, paging.StartIndex, paging.EndIndex);
            model.Page = paging;
            model.Page.RecordCount = ds.Tables[0].Rows[0][0].ToString().ToInt32();
            model.DataList = ModelConvertHelper<QiuZhi>.ConvertToList(ds.Tables[1]);
            return model;
        }
        #endregion

        public QiuZhi GetQiuZhiBuyPay(long mainid)
        {
            return ModelConvertHelper<QiuZhi>.ConverToModel(dal.GetQiuZhiBuyPay(mainid));
        }
    }
}

