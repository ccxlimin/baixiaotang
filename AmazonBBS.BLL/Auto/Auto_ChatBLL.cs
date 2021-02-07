using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using AmazonBBS.Common;
using AmazonBBS.DAL;
using AmazonBBS.Model;

namespace AmazonBBS.BLL
{

    /// <summary>
    /// 聊天记录
    /// </summary>
    public class Auto_ChatBLL
    {
        Auto_ChatDAL dal = new Auto_ChatDAL();

        #region add
        /// <summary>
        /// 添加一条记录，没有任何逻辑
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public int Add(Chat model)
        {
            return dal.Add(model);
        }
        #endregion



        #region update
        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public bool Edit(Chat model)
        {
            return dal.Update(model);
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
        public Chat GetItem(long id)
        {
            DataTable dt = dal.GetModel(id);

            return ModelConvertHelper<Chat>.ConvertToList(dt).FirstOrDefault();
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
        public List<Chat> SearchAll()
        {
            DataTable dt = dal.GetList();
            return ModelConvertHelper<Chat>.ConvertToList(dt);
        }

        /// <summary>
        /// 查询最新N条记录
        /// </summary>
        /// <param name="top">N条</param>
        /// <returns></returns>
        public List<Chat> SearchAll(int top)
        {
            DataTable dt = dal.GetList(top);
            return ModelConvertHelper<Chat>.ConvertToList(dt);
        }

        ///// <summary>
        ///// 分页显示内容
        ///// </summary>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <returns></returns>
        //public List<Chat> Query(int pageIndex, int pageSize)
        //{
        //    DataTable dt = dal.Query(pageIndex, pageSize);
        //    return ModelConvertHelper<Chat>.ConvertToList(dt);
        //}

        /// <summary>
        /// 分页显示内容
        /// </summary>
        /// <param name="startIndex">开始码</param>
        /// <param name="endIndex">结束码</param>
        /// <returns></returns>      
        public List<Chat> SearchByRows(int startIndex, int endIndex)
        {
            DataTable dt = dal.SearchByRows(startIndex, endIndex);
            return ModelConvertHelper<Chat>.ConvertToList(dt);
        }

        #endregion

        #region 获取今天发送消息次数
        public int SendMsgCount(long userID)
        {
            DateTime now = DateTime.Now;
            return dal.SendMsgCount(userID, now.Date, now).ToInt32();
        }
        #endregion
    }
}

