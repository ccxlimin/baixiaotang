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
    /// 用户扩展表
    /// </summary>
    public class UserExtBLL : Auto_UserExtBLL
    {
        public static UserExtBLL Instance
        {
            get { return SingleHepler<UserExtBLL>.Instance; }
        }

        UserExtDAL dal = new UserExtDAL();

        /// <summary>
        /// 根据用户ID获取相关信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UserExt GetExtInfo(long userID)
        {
            return ModelConvertHelper<UserExt>.ConverToModel(dal.GetExtInfo(userID));
        }

        /// <summary>
        /// 头衔签到次数、头衔名称、专属头衔名称、展示头衔或专属头衔、头衔图标
        /// </summary>
        /// <param name="userid"></param>
        /// <returns>[头衔签到次数, 头衔名称, 专属头衔名称, 展示头衔或专属头衔, 头衔图标]</returns>
        public string[] GetLevelNameForUser(long userid)
        {
            DataTable dt = dal.GetLevelNameForUser(userid);
            List<string> names = new List<string>();
            if (dt.IsNotNullAndRowCount())
            {
                var c0 = dt.Rows[0][0];
                if (c0 != null)
                {
                    names.Add(c0.ToString());
                }
                var c1 = dt.Rows[0][1];
                if (c1 != null)
                {
                    names.Add(c1.ToString());
                }
                else
                {
                    names.Add(string.Empty);
                }
                var c2 = dt.Rows[0][2];
                if (c2 != null
                    )
                {
                    names.Add(c2.ToString());
                }
                else
                {
                    names.Add(string.Empty);
                }
                names.Add(dt.Rows[0][3].ToString());
                names.Add(dt.Rows[0][4].ToString());
            }
            return names.ToArray();
        }

        /// <summary>
        /// 判断用户是否拥有足够的积分/VIP分
        /// </summary>
        /// <param name="type">1积分 2金钱/VIP分</param>
        /// <param name="coin">数量</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public bool HasEnoughCoin(int type, int coin, long userID)
        {
            UserExt ext = GetExtInfo(userID);
            if (type == 1)
            {
                return ext.TotalScore >= coin;
            }
            else
            {
                return ext.TotalCoin >= coin;
            }
        }

        /// <summary>
        /// 用户是否已进行法人认证
        /// </summary>
        /// <param name="userv"></param>
        /// <returns></returns>
        public bool IsFaRenOauth(int userv)
        {
            return userv >= 4;
        }

        public bool UpdateSkin(long userID, string skin)
        {
            return dal.UpdateSkin(userID, skin);
        }

        /// <summary>
        /// 解除帐号封禁
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool ReSetCloseAccount(long userID)
        {
            return dal.ReSetCloseAccount(userID);
        }

        /// <summary>
        /// 找出最近即将解封的账号
        /// </summary>
        /// <returns></returns>
        public UserExt FindNearCloseAccount()
        {
            return ModelConvertHelper<UserExt>.ConverToModel(dal.FindNearCloseAccount());
        }

        public List<UserInfoViewModel> GetFaRenRenZheng(Paging page)
        {
            List<UserInfoViewModel> list = new List<UserInfoViewModel>();

            DataSet ds = dal.GetFaRenRenZheng(page.StartIndex, page.EndIndex);
            DataTable dt = ds.Tables[1];
            if (dt.IsNotNullAndRowCount())
            {
                page.RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                list = ModelConvertHelper<UserInfoViewModel>.ConvertToList(dt);
            }
            return list;
        }

        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(UserExt model)
        {
            ResultInfo ri = new ResultInfo();

            if (model == null) return ri;

            int result = Add(model, null);

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
        public ResultInfo Update(UserExt model, SqlTransaction tran = null)
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

            var model = GetModel(id, null);
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
        public UserExt GetModel(long id, SqlTransaction tran)
        {
            return GetItem(id, tran);
        }

        /// <summary>
        /// 封号（非永久）
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="closetime"></param>
        /// <returns></returns>
        public bool CloseAccount(long userid, DateTime closetime)
        {
            return dal.CloseAccount(userid, closetime);
        }

        /// <summary>
        /// 设置用户发表文章、BBS、活动是否需要审核(取消设置)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CheckBBS(long id, int type)
        {
            return dal.CheckBBS(id, type);
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
        //    List<UserExt> li = ModelConvertHelper<UserExt>.ConvertToList(tab);
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
                var li = ModelConvertHelper<UserExt>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;


        }

        #endregion

        public bool Exist(long userid)
        {
            return Convert.ToInt32(dal.Exist(userid)) > 0;
        }


        /// <summary>
        /// 更新用户头衔
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="levelName"></param>
        /// <param name="only"></param>
        /// <returns></returns>
        public bool UpdateLevelName(long userid, int levelName, bool only)
        {
            return dal.UpdateLevelName(userid, levelName, only);
        }

        /// <summary>
        /// 增加相应积分/金钱
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="coin"></param>
        /// <param name="type">1 积分  2金钱</param>
        /// <returns></returns>
        public bool AddScore(long? userid, int coin, int type, SqlTransaction tran = null)
        {
            return dal.AddScoreCoin(Convert.ToInt64(userid), coin, type, tran);
        }

        /// <summary>
        /// 减去相应积分/金钱
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="num"></param>
        /// <param name="type">1 积分  2金钱</param>
        /// <returns></returns>
        public bool SubScore(long userid, int num, int type, SqlTransaction tran)
        {
            return dal.SubScore(userid, num, type, tran);
        }

        /// <summary>
        /// 获取红人用户(红人榜按照积分等级进行排名选前20)
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<UserViewModel> GetHotUsers()
        {
            int count = Convert.ToInt32(ConfigHelper.AppSettings("HotUsersCount"));
            DataTable dt = dal.GetHotUsers(count);
            return ModelConvertHelper<UserViewModel>.ConvertToList(dt);
        }

        /// <summary>
        /// 获取新用户(30天内注册的)
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<UserViewModel> GetNewUsers()
        {
            int days = Convert.ToInt32(ConfigHelper.AppSettings("NewUsersDays"));
            DataTable dt = dal.GetNewUsers(days);
            return ModelConvertHelper<UserViewModel>.ConvertToList(dt);
        }

        /// <summary>
        /// 获取老用户（180天以上的）
        /// </summary>
        /// <returns></returns>
        public List<UserViewModel> GetOldUsers()
        {
            int days = Convert.ToInt32(ConfigHelper.AppSettings("NewUsersDays"));
            int count = Convert.ToInt32(ConfigHelper.AppSettings("HotUsersCount"));
            DataTable dt = dal.GetOldUsers(days, count);
            var model = ModelConvertHelper<UserViewModel>.ConvertToList(dt);

            return model.OrderByDescending(a => { return a.SignCount + a.CommentCount; }).ToList();
        }

        public List<SignCountAndLevel> GetALLSignAndLevel(SqlTransaction tran)
        {
            return ModelConvertHelper<SignCountAndLevel>.ConvertToList(dal.GetALLSignAndLevel(tran));
        }

        /// <summary>
        /// 获取签到次数和头衔级别
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public SignCountAndLevel GetSignAndLevelByUserID(SqlTransaction tran, long userID)
        {
            return ModelConvertHelper<SignCountAndLevel>.ConverToModel(dal.GetSignAndLevelByUserID(tran, userID));
        }

        /// <summary>
        /// 拒绝用户法人认证请求
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool RejectUserAuth(long userId)
        {
            return dal.RejectUserAuth(userId);
        }
    }
}

