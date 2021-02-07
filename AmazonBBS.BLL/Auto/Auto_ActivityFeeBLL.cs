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
    /// 活动费用表
    /// </summary>
    public class Auto_ActivityFeeBLL
    {
        Auto_ActivityFeeDAL dal = new Auto_ActivityFeeDAL();

        #region add
        /// <summary>
        /// 添加一条记录，没有任何逻辑
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public int Add(ActivityFee model, SqlTransaction tran = null)
        {
            return dal.Add(model, tran);
        }
        #endregion



        #region update
        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public bool Edit(ActivityFee model, SqlTransaction tran = null)
        {
            return dal.Update(model, tran);
        }
        #endregion

        #region delete
        /// <summary>
        /// 删除一条记录，没有任何逻辑
        /// </summary>
        /// <param name="Id">主键</param>
        /// <returns></returns>
        public bool DeleteByID(long Id)
        {
            return dal.Delete(Id);
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public ActivityFee GetItem(long id, SqlTransaction tran = null)
        {
            DataTable dt = dal.GetModel(id, tran);

            return ModelConvertHelper<ActivityFee>.ConvertToList(dt).FirstOrDefault();
        }
        #endregion

        #region query

        /// <summary>
        /// 记录总数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return dal.Count();
        }


        /// <summary>
        /// 查询所有记录
        /// </summary>
        /// <returns></returns>
        public List<ActivityFee> SearchAll()
        {
            DataTable dt = dal.GetList();
            return ModelConvertHelper<ActivityFee>.ConvertToList(dt);
        }

        /// <summary>
        /// 查询最新N条记录
        /// </summary>
        /// <param name="top">N条</param>
        /// <returns></returns>
        public List<ActivityFee> SearchAll(int top)
        {
            DataTable dt = dal.GetList(top);
            return ModelConvertHelper<ActivityFee>.ConvertToList(dt);
        }

        ///// <summary>
        ///// 分页显示内容
        ///// </summary>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <returns></returns>
        //public List<ActivityFee> Query(int pageIndex, int pageSize)
        //{
        //    DataTable dt = dal.Query(pageIndex, pageSize);
        //    return ModelConvertHelper<ActivityFee>.ConvertToList(dt);
        //}

        /// <summary>
        /// 分页显示内容
        /// </summary>
        /// <param name="startIndex">开始码</param>
        /// <param name="endIndex">结束码</param>
        /// <returns></returns>      
        public List<ActivityFee> SearchByRows(int startIndex, int endIndex)
        {
            DataTable dt = dal.SearchByRows(startIndex, endIndex);
            return ModelConvertHelper<ActivityFee>.ConvertToList(dt);
        }

        #endregion

    }
}

