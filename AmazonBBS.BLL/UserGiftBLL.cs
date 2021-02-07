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
    /// 用户兑换礼物表
    /// </summary>
    public class UserGiftBLL : Auto_UserGiftBLL
    {
        public static UserGiftBLL Instance
        {
            get { return SingleHepler<UserGiftBLL>.Instance; }
        }
        UserGiftDAL dal = new UserGiftDAL();


        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(UserGift model, SqlTransaction tran = null)
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
        public ResultInfo Update(UserGift model, SqlTransaction tran)
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
        public UserGift GetModel(long id, SqlTransaction tran = null)
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
        //    List<UserGift> li = ModelConvertHelper<UserGift>.ConvertToList(tab);
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
                var li = ModelConvertHelper<UserGift>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;


        }
        #endregion

        #region 判断当前用户是否参加该活动
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

        public List<_UserBaseInfo> GetBuyUsers(long id)
        {
            return ModelConvertHelper<_UserBaseInfo>.ConvertToList(dal.GetBuyUsers(id));
        }

        public GiftBuyerManageViewModel GetBuyerList(long id, JoinItemTypeEnum joinItemType)
        {
            var ds = dal.GetBuyerList(id, joinItemType.GetHashCode());

            var model = new GiftBuyerManageViewModel() { };
            if (ds.Tables[0].IsNotNullAndRowCount())
            {
                model.GiftInfo = ModelConvertHelper<Gift>.ConverToModel(ds.Tables[0]);
                model.BuyList = ModelConvertHelper<_BuyManageInfo>.ConvertToList(ds.Tables[1]);
                model.JoinQuestions = ModelConvertHelper<JoinItemQuestionExt>.ConvertToList(ds.Tables[2]);
                model.JoinAnswers = ModelConvertHelper<JoinItemAnswerExt>.ConvertToList(ds.Tables[3]);
            }
            return model;
        }

        /// <summary>
        /// 更新用户付款信息
        /// </summary>
        /// <param name="v"></param>
        /// <param name="value"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public bool UpdateUserBuyInfo(long userid, long itemID, SqlTransaction tran)
        {
            return dal.UpdateUserBuyInfo(userid, itemID, tran);
        }

        public UserGift GetDetailBuyInfo(long userid, long itemID, SqlTransaction tran)
        {
            return ModelConvertHelper<UserGift>.ConverToModel(dal.GetDetailJoinInfo(userid, itemID, tran));
        }

        public long GetUserBuyItemID(decimal? fee, int? buyCount, string createUser, SqlTransaction tran = null)
        {
            var result = dal.GetUserBuyItemID(fee, buyCount, createUser, tran);
            return result.IsNullOrEmpty() ? 0 : Convert.ToInt64(result);
        }
        #endregion
    }
}

