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
    /// 关注列表
    /// </summary>
    public class UserLikeBLL : Auto_UserLikeBLL
    {
        public static UserLikeBLL Instance
        {
            get { return SingleHepler<UserLikeBLL>.Instance; }
        }
        UserLikeDAL dal = new UserLikeDAL();


        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(UserLike model)
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
        public ResultInfo Update(UserLike model)
        {
            ResultInfo ri = new ResultInfo();
            if (Edit(model))
            {
                ri.Ok = true;
                ri.Msg = "修改成功";
            }

            return ri;
        }

        public List<_MyLikeInfo> GetLikesByUserID(long userID, int type, Paging likePage = null)
        {
            List<_MyLikeInfo> list = null;
            DataSet ds;
            if (likePage == null)
            {
                ds = dal.GetLikesByUserID(userID, type);
            }
            else
            {
                ds = dal.GetLikesByUserID(userID, type, likePage.StartIndex, likePage.EndIndex);
            }
            DataTable likedt = ds.Tables[1];
            if (likedt.IsNotNullAndRowCount())
            {
                likePage.RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                list = ModelConvertHelper<_MyLikeInfo>.ConvertToList(likedt);
            }
            return list;
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
        public UserLike GetModel(long id)
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
        //    List<UserLike> li = ModelConvertHelper<UserLike>.ConvertToList(tab);
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
                var li = ModelConvertHelper<UserLike>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">目标ID</param>
        /// <param name="type">关注类型（1 问题、 2文章 3、用户）</param>
        /// <param name="userID">关注用户ID</param>
        /// <returns></returns>
        public int IsLiked(long id, int type, long userID)
        {
            string likeId = dal.IsLiked(id, type, userID);
            return string.IsNullOrEmpty(likeId) ? 0 : Convert.ToInt32(likeId);
        }

        public bool UnLike(int likeId)
        {
            return dal.UnLike(likeId);
        }

        public string FindUserIdByID(string queryTable, string uesrColumn, int id)
        {
            return dal.FindUserIdByID(queryTable, uesrColumn, id);
        }

        public List<UserBase> FindLikerListByLikeType(long likeTargetID, LikeTypeEnum likeType)
        {
            DataTable dt = dal.FindLikerListByLikeType(likeTargetID, likeType.GetHashCode());
            return ModelConvertHelper<UserBase>.ConvertToList(dt);
        }

        /// <summary>
        /// 获取用户的粉丝
        /// </summary>
        /// <param name="id"></param>
        /// <param name="likeType"></param>
        /// <returns></returns>
        public List<FansViewModel> GetFansByUserID(long userId, int likeType)
        {
            return ModelConvertHelper<FansViewModel>.ConvertToList(dal.GetFansByUserID(userId, likeType));
        }
        #endregion


    }
}

