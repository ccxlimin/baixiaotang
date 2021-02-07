using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using AmazonBBS.Common;
using AmazonBBS.DAL;
using AmazonBBS.Model;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace AmazonBBS.BLL
{

    /// <summary>
    /// 用户基本表
    /// </summary>
    public class Auto_UserBaseBLL
    {
        Auto_UserBaseDAL dal = new Auto_UserBaseDAL();

        #region add
        /// <summary>
        /// 添加一条记录，没有任何逻辑
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public int Add(UserBase model, SqlTransaction tran)
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
        public bool Edit(UserBase model, SqlTransaction tran)
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
        public bool DeleteByID(long Id, SqlTransaction tran)
        {
            return dal.Delete(Id, tran);
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public UserBase GetItem(long id, SqlTransaction tran)
        {
            DataTable dt = dal.GetModel(id, tran);

            return ModelConvertHelper<UserBase>.ConvertToList(dt).FirstOrDefault();
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
        public List<UserBase> SearchAll()
        {
            DataTable dt = dal.GetList();
            return ModelConvertHelper<UserBase>.ConvertToList(dt);
        }

        /// <summary>
        /// 查询最新N条记录
        /// </summary>
        /// <param name="top">N条</param>
        /// <returns></returns>
        public List<UserBase> SearchAll(int top)
        {
            DataTable dt = dal.GetList(top);
            return ModelConvertHelper<UserBase>.ConvertToList(dt);
        }

        ///// <summary>
        ///// 分页显示内容
        ///// </summary>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <returns></returns>
        //public List<UserBase> Query(int pageIndex, int pageSize)
        //{
        //    DataTable dt = dal.Query(pageIndex, pageSize);
        //    return ModelConvertHelper<UserBase>.ConvertToList(dt);
        //}

        /// <summary>
        /// 分页显示内容
        /// </summary>
        /// <param name="startIndex">开始码</param>
        /// <param name="endIndex">结束码</param>
        /// <returns></returns>      
        public List<UserBase> SearchByRows(int startIndex, int endIndex)
        {
            DataTable dt = dal.SearchByRows(startIndex, endIndex);
            return ModelConvertHelper<UserBase>.ConvertToList(dt);
        }

        #endregion

    }
}

