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
    /// 礼物表
    /// </summary>
    public class GiftBLL : Auto_GiftBLL
    {
        public static GiftBLL Instance
        {
            get { return SingleHepler<GiftBLL>.Instance; }
        }
        GiftDAL dal = new GiftDAL();

        /// <summary>
        /// 根据类型获取所有兑换数据
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="type">礼物类型(1-礼物 2-数据分析 3-百晓堂课程)</param>
        /// <param name="key">关键词搜索</param>
        /// <returns></returns>
        public List<_Gift> GetAllGifts(Paging giftPage, int type, string key = null, string sortConfig = null)
        {
            List<_Gift> list = null;

            DataSet ds = dal.GetALLGifts(giftPage.StartIndex, giftPage.EndIndex, type, key, sortConfig);
            DataTable giftdt = ds.Tables[1];
            if (giftdt.IsNotNullAndRowCount())
            {
                giftPage.RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                list = ModelConvertHelper<_Gift>.ConvertToList(giftdt);
            }
            return list;
        }

        public _Gift GetDetail(long id, long userid, JoinItemTypeEnum joinItemType, bool searchMyJoinItem)
        {
            var ds = dal.GetGiftDetail(id, userid, joinItemType.GetHashCode(), searchMyJoinItem);
            _Gift amodel = ModelConvertHelper<_Gift>.ConverToModel(ds.Tables[0]);
            amodel.FeeList = ModelConvertHelper<GiftFee>.ConvertToList(ds.Tables[1]);
            amodel.JoinQuestions = ModelConvertHelper<JoinItemQuestionExt>.ConvertToList(ds.Tables[2]);
            if (searchMyJoinItem)
            {
                amodel.JoinFeeList = ModelConvertHelper<UserBuyGiftListInfo>.ConvertToList(ds.Tables[3]);
            }
            return amodel;
        }

        public _Gift GetGiftDetail(long id, long userid, Paging page, CommentEnumType commentEnumType, PriseEnumType priseEnumType, JoinItemTypeEnum joinItemType)
        {
            _Gift amodel = GetDetail(id, userid, joinItemType, true);
            amodel.Comments = CommentBLL.Instance.GetCommentCallBack(id, commentEnumType.GetHashCode(), priseEnumType.GetHashCode(), userid, page);
            return amodel;
        }

        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(Gift model)
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
        public ResultInfo Update(Gift model, SqlTransaction tran = null)
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

        public bool PVCount(long id)
        {
            return dal.PVCount(id);
        }
        #endregion



        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public Gift GetModel(long id, SqlTransaction tran = null)
        {
            return GetItem(id, tran);
        }

        public bool UpdateCount(int buyCount, long giftid, SqlTransaction tran = null)
        {
            return dal.UpdateCount(buyCount, giftid, tran);
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
        //    List<Gift> li = ModelConvertHelper<Gift>.ConvertToList(tab);
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
                var li = ModelConvertHelper<Gift>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;


        }

        public GiftCreateViewModel GetEditDetail(long id, JoinItemTypeEnum joinItemType)
        {
            var ds = dal.GetEditDetail(id, joinItemType.GetHashCode());
            GiftCreateViewModel model = new GiftCreateViewModel()
            {
                Gift = ModelConvertHelper<Gift>.ConverToModel(ds.Tables[0]),
                GiftFees = ModelConvertHelper<GiftFee>.ConvertToList(ds.Tables[1]),
                JoinItemQues = ModelConvertHelper<JoinItemQuestionExt>.ConvertToList(ds.Tables[2])
            };
            return model;
        }
        #endregion


    }
}

