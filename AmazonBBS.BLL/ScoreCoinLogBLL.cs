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
    /// 积分金钱流水表
    /// </summary>
    public class ScoreCoinLogBLL : Auto_ScoreCoinLogBLL
    {
        public static ScoreCoinLogBLL Instance
        {
            get { return SingleHepler<ScoreCoinLogBLL>.Instance; }
        }

        #region 判断用户今日是否签到
        /// <summary>
        /// 判断用户今日是否签到
        /// </summary>
        /// <returns></returns>
        public bool GetSignStatus(long userid)
        {
            DataTable dt = dal.IsSignToday(userid);
            if (dt.IsNotNullAndRowCount())
            {
                DateTime signTime = Convert.ToDateTime(dt.Rows[0][0]);
                if (signTime.ToShortDateString() == DateTime.Now.ToShortDateString())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        ScoreCoinLogDAL dal = new ScoreCoinLogDAL();

        /// <summary>
        /// 记录消费积分/VIP分的流水
        /// </summary>
        public bool Log(int coin, int coinType, CoinSourceEnum coinSource, long feeUserID, string feeUserName, SqlTransaction tran, long createUser = 0)
        {
            ScoreCoinLog model = new ScoreCoinLog()
            {
                Coin = coin,
                UserID = feeUserID,
                CoinSource = coinSource.GetHashCode(),
                CoinTime = DateTime.Now,
                CoinType = coinType,
                CreateUser = createUser == 0 ? feeUserID.ToString() : createUser.ToString(),
                UserName = feeUserName
            };
            return ScoreCoinLogBLL.Instance.Add(model, tran) > 0;
        }

        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(ScoreCoinLog model, SqlTransaction tran = null)
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

        public bool HasGiveScore(long? commentUserID, long mainID, int coinSource, int coinType, SqlTransaction tran)
        {
            return Convert.ToInt32(dal.HasGiveScore(commentUserID, mainID, coinSource, coinType, tran)) > 0;
        }
        #endregion

        #region update
        /// <summary>
        /// 修改 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultInfo Update(ScoreCoinLog model, SqlTransaction tran = null)
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
        public ScoreCoinLog GetModel(long id)
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
        //    List<ScoreCoinLog> li = ModelConvertHelper<ScoreCoinLog>.ConvertToList(tab);
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
                var li = ModelConvertHelper<ScoreCoinLog>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;
        }
        #endregion
    }
}

