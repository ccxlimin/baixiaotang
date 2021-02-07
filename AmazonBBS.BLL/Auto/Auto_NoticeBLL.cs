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
    /// 系统通知
    /// </summary>
	public class Auto_NoticeBLL
    {
        Auto_NoticeDAL dal = new Auto_NoticeDAL();

		#region add
        /// <summary>
        /// 添加一条记录，没有任何逻辑
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public int Add(Notice model, SqlTransaction tran = null)
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
        public bool Edit(Notice model, SqlTransaction tran = null)
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
        public Notice GetItem(long id)
        {
            DataTable dt =dal.GetModel(id);

            return ModelConvertHelper<Notice>.ConvertToList(dt).FirstOrDefault();
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
        public List<Notice> SearchAll()
        {
            DataTable dt = dal.GetList();
            return ModelConvertHelper<Notice>.ConvertToList(dt);
        }
		
		/// <summary>
        /// 查询最新N条记录
        /// </summary>
        /// <param name="top">N条</param>
        /// <returns></returns>
        public List<Notice> SearchAll(int top)
        {
            DataTable dt =dal.GetList(top);
            return ModelConvertHelper<Notice>.ConvertToList(dt);
        }

        ///// <summary>
        ///// 分页显示内容
        ///// </summary>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <returns></returns>
        //public List<Notice> Query(int pageIndex, int pageSize)
        //{
        //    DataTable dt = dal.Query(pageIndex, pageSize);
        //    return ModelConvertHelper<Notice>.ConvertToList(dt);
        //}

		/// <summary>
        /// 分页显示内容
        /// </summary>
        /// <param name="startIndex">开始码</param>
        /// <param name="endIndex">结束码</param>
        /// <returns></returns>      
        public List<Notice> SearchByRows(int startIndex, int endIndex)
        {
            DataTable dt = dal.SearchByRows(startIndex, endIndex);
            return ModelConvertHelper<Notice>.ConvertToList(dt);
        }
		#endregion

    }
}
