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
    /// 行业
    /// </summary>
	public class JobTradeBLL : Auto_JobTradeBLL
    {
		public static JobTradeBLL Instance
        {
            get { return SingleHepler<JobTradeBLL>.Instance; }
        }
        JobTradeDAL dal = new JobTradeDAL();


		#region add
		/// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
		public ResultInfo Create(JobTrade model, SqlTransaction tran = null)
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
        public ResultInfo Update(JobTrade model, SqlTransaction tran = null)
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
        public JobTrade GetModel<T>(T id)
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
        //    List<JobTrade> li = ModelConvertHelper<JobTrade>.ConvertToList(tab);
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
                var li = ModelConvertHelper<JobTrade>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;
        }
		#endregion
    }
}
