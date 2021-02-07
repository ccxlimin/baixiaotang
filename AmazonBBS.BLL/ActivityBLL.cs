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
    /// 活动表
    /// </summary>
    public class ActivityBLL : Auto_ActivityBLL
    {
        public static ActivityBLL Instance
        {
            get { return SingleHepler<ActivityBLL>.Instance; }
        }
        ActivityDAL dal = new ActivityDAL();

        public List<_Activity> GetAllActivits(Paging activityPage, string key = null)
        {
            List<_Activity> list = null;
            DataSet ds = dal.GetAllActivits(activityPage.StartIndex, activityPage.EndIndex, key);
            DataTable adt = ds.Tables[1];
            if (adt.IsNotNullAndRowCount())
            {
                list = ModelConvertHelper<_Activity>.ConvertToList(adt);
                activityPage.RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            return list;
        }

        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(Activity model)
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


        public List<_Activity> GetCheckList()
        {
            DataTable ds = dal.GetCheckList();
            List<_Activity> list = new List<_Activity>();
            list.AddRange(ModelConvertHelper<_Activity>.ConvertToList(ds).OrderByDescending(a => Convert.ToDateTime(a.ActivityCreateTIme).ToString("yyyy-MM-dd")).ThenByDescending(a => a.PVCount).ToList());
            return list;
        }
        #endregion

        #region update
        /// <summary>
        /// 修改 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultInfo Update(Activity model, SqlTransaction tran = null)
        {
            ResultInfo ri = new ResultInfo();
            if (Edit(model, tran))
            {
                ri.Ok = true;
                ri.Msg = "修改成功";
            }

            return ri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="searchMyJoinItem">是否搜索我购买的活动</param>
        /// <returns></returns>
        public _Activity GetDetail(long id, long userid, bool searchMyJoinItem)
        {
            DataSet ds = dal.GetDetail(id, userid, searchMyJoinItem);
            _Activity model = new _Activity();
            model = ModelConvertHelper<_Activity>.ConverToModel(ds.Tables[0]);
            model.FeeList = ModelConvertHelper<ActivityFee>.ConvertToList(ds.Tables[1]);
            model.JoinQuestions = ModelConvertHelper<JoinItemQuestionExt>.ConvertToList(ds.Tables[2]);
            if (searchMyJoinItem)
            {
                model.JoinFeeList = ModelConvertHelper<UserBuyPartyListInfo>.ConvertToList(ds.Tables[3]);
            }
            return model;
        }

        public _Activity GetPartyDetail(long id, long userid, Paging page)
        {
            //获取活动 基本信息
            _Activity amodel = GetDetail(id, userid, true);
            if (amodel != null)
            {
                amodel.Comments = CommentBLL.Instance.GetCommentCallBack(id, CommentEnumType.Party.GetHashCode(), PriseEnumType.PartyComment.GetHashCode(), userid, page);
                return amodel;
            }
            return null;
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
        public Activity GetModel(long id, SqlTransaction tran = null)
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
        //    List<Activity> li = ModelConvertHelper<Activity>.ConvertToList(tab);
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
                var li = ModelConvertHelper<Activity>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;


        }

        /// <summary>
        /// 判断活动是否能够报名
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public ResultInfo CanJoinParty(Activity party)
        {
            ResultInfo ri = new ResultInfo();
            if (DateTime.Now < party.EndTime)
            {
                //判断活动是否能报名
                if (party.CanJoinOnBegin == 2)
                {
                    if (DateTime.Now < party.JoinBeginTime)
                    {
                        ri.Msg = "活动报名未开始！活动报名时间为：{0}".FormatWith(party.JoinBeginTime.ToString());
                        return ri;
                    }
                    else if (DateTime.Now > party.JoinEndTime)
                    {
                        ri.Msg = "活动报名已截止！";
                        return ri;
                    }
                }
                if (DateTime.Now > party.BeginTime)
                {
                    //判断活动有无开始
                    ri.Msg = "活动已开始！不能再报名！";
                    return ri;
                }
                else
                {
                    ri.Ok = true;
                    return ri;
                }
            }
            else
            {
                ri.Msg = "活动已结束！";
                return ri;
            }
        }

        public bool PVCount(long activityId)
        {
            return dal.PVCount(activityId);
        }

        public PartyCreateViewModel GetEditDetail(long id)
        {
            var ds = dal.GetEditDetail(id);
            PartyCreateViewModel model = new PartyCreateViewModel()
            {
                Party = ModelConvertHelper<Activity>.ConverToModel(ds.Tables[0]),
                PartyFee = ModelConvertHelper<ActivityFee>.ConvertToList(ds.Tables[1]),
                JoinItemQues = ModelConvertHelper<JoinItemQuestionExt>.ConvertToList(ds.Tables[2])
            };
            return model;
        }
        #endregion


    }
}

