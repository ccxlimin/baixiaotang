﻿using System;
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
    /// 报名购买选项问题扩展表
    /// </summary>
	public class Auto_JoinItemQuestionExtBLL
    {
        Auto_JoinItemQuestionExtDAL dal = new Auto_JoinItemQuestionExtDAL();

		#region add
        /// <summary>
        /// 添加一条记录，没有任何逻辑
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public int Add(JoinItemQuestionExt model, SqlTransaction tran = null)
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
        public bool Edit(JoinItemQuestionExt model, SqlTransaction tran = null)
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
        public JoinItemQuestionExt GetItem<T>(T id)
        {
            DataTable dt =dal.GetModel(id);

            return ModelConvertHelper<JoinItemQuestionExt>.ConvertToList(dt).FirstOrDefault();
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
        public List<JoinItemQuestionExt> SearchAll()
        {
            DataTable dt = dal.GetList();
            return ModelConvertHelper<JoinItemQuestionExt>.ConvertToList(dt);
        }
		
		/// <summary>
        /// 查询最新N条记录
        /// </summary>
        /// <param name="top">N条</param>
        /// <returns></returns>
        public List<JoinItemQuestionExt> SearchAll(int top)
        {
            DataTable dt =dal.GetList(top);
            return ModelConvertHelper<JoinItemQuestionExt>.ConvertToList(dt);
        }

        ///// <summary>
        ///// 分页显示内容
        ///// </summary>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <returns></returns>
        //public List<JoinItemQuestionExt> Query(int pageIndex, int pageSize)
        //{
        //    DataTable dt = dal.Query(pageIndex, pageSize);
        //    return ModelConvertHelper<JoinItemQuestionExt>.ConvertToList(dt);
        //}

		/// <summary>
        /// 分页显示内容
        /// </summary>
        /// <param name="startIndex">开始码</param>
        /// <param name="endIndex">结束码</param>
        /// <returns></returns>      
        public List<JoinItemQuestionExt> SearchByRows(int startIndex, int endIndex)
        {
            DataTable dt = dal.SearchByRows(startIndex, endIndex);
            return ModelConvertHelper<JoinItemQuestionExt>.ConvertToList(dt);
        }
		#endregion

    }
}
