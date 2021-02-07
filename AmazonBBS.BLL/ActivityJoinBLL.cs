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
    /// 活动报名表
    /// </summary>
    public class ActivityJoinBLL : Auto_ActivityJoinBLL
    {
        public static ActivityJoinBLL Instance
        {
            get { return SingleHepler<ActivityJoinBLL>.Instance; }
        }
        ActivityJoinDAL dal = new ActivityJoinDAL();


        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(ActivityJoin model, SqlTransaction tran)
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
        public ResultInfo Update(ActivityJoin model, SqlTransaction tran)
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
        public ActivityJoin GetModel(long id, SqlTransaction tran = null)
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
        //    List<ActivityJoin> li = ModelConvertHelper<ActivityJoin>.ConvertToList(tab);
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
                var li = ModelConvertHelper<ActivityJoin>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;


        }
        #endregion

        /// <summary>
        /// 查找参加活动人数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int JoinCount(long id)
        {
            return Convert.ToInt32(dal.JoinCount(id));
        }

        /// <summary>
        /// 判断当前用户是否参加该活动
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="aid"></param>
        /// <returns></returns>
        public bool IsJoined(long userId, long aid)
        {
            return Convert.ToInt32(dal.IsJoined(userId, aid)) > 0;
        }

        /// <summary>
        /// 获取所有参加活动的人员名单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<_UserBaseInfo> GetJoinedUsers(long id)
        {
            var dt = dal.GetJoinedUsers(id);
            return ModelConvertHelper<_UserBaseInfo>.ConvertToList(dt);
        }

        public PartyJoinManageViewModel GetJoinList(long id)
        {
            var ds = dal.GetJoinList(id);

            var model = new PartyJoinManageViewModel() { };
            if (ds.Tables[0].IsNotNullAndRowCount())
            {
                model.PartyInfo = ModelConvertHelper<Activity>.ConverToModel(ds.Tables[0]);
                model.JoinList = ModelConvertHelper<_JoinManageInfo>.ConvertToList(ds.Tables[1]);
                model.JoinQuestions = ModelConvertHelper<JoinItemQuestionExt>.ConvertToList(ds.Tables[2]);
                model.JoinAnswers = ModelConvertHelper<JoinItemAnswerExt>.ConvertToList(ds.Tables[3]);
            }
            return model;
        }

        /// <summary>
        /// 根据用户ID和主ID查找用户参加的活动信息
        /// </summary>
        /// <param name="v"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public ActivityJoin GetDetailJoinInfo(long userid, long itemID, SqlTransaction tran)
        {
            return ModelConvertHelper<ActivityJoin>.ConverToModel(dal.GetDetailJoinInfo(userid, itemID, tran));
        }
    }
}

