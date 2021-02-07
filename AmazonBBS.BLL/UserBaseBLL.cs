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
    public class UserBaseBLL : Auto_UserBaseBLL
    {
        private readonly AmazonBBSDBContext _amazonBBSDBContext;

        public static UserBaseBLL Instance
        {
            get
            {
                return SingleHepler<UserBaseBLL>.Instance;
            }
        }
        UserBaseDAL dal = new UserBaseDAL();

        public UserViewModel GetUserDetail<T>(T id, long currentUserId)
        {
            return ModelConvertHelper<UserViewModel>.ConverToModel(dal.GetUserDetail(id, currentUserId));
        }

        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(UserBase model)
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
        public ResultInfo Update(UserBase model, SqlTransaction tran = null)
        {
            ResultInfo ri = new ResultInfo();
            if (Edit(model, tran))
            {
                ri.Ok = true;
                ri.Msg = "修改成功";
            }

            return ri;
        }

        public UserManageViewModel QueryAllUserInfo(Paging page, long me)
        {
            DataSet ds = dal.QueryAllUserInfo(page.StartIndex, page.EndIndex, me);
            DataTable dt = ds.Tables[1];
            UserManageViewModel model = new UserManageViewModel();
            if (dt.IsNotNullAndRowCount())
            {
                page.RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                //model.UserBaseList = ModelConvertHelper<UserBase>.ConvertToList(dt);
                //model.UserExtList = ModelConvertHelper<UserExt>.ConvertToList(dt);
                model.UserInfoList = ModelConvertHelper<UserInfoViewModel>.ConvertToList(dt);
            }
            return model;
        }

        /// <summary>
        /// 更新用户性别
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public bool UpdateGender(long userID, int gender)
        {
            return dal.UpdateGender(UserID, gender);
        }

        public bool UpdateBirth(long userID, string birth)
        {
            return dal.UpdateBirth(UserID, birth);
        }

        public UserBase GetUserInfo(string account)
        {
            return ModelConvertHelper<UserBase>.ConverToModel(dal.GetUserInfo(account));
        }

        public UserBase GetUserInfo(long uid)
        {
            return ModelConvertHelper<UserBase>.ConverToModel(dal.GetUserInfo(uid));
        }

        public UserInfoViewModel GetUserInfoALL(long uid)
        {
            return ModelConvertHelper<UserInfoViewModel>.ConverToModel(dal.GetUserInfoALL(uid));
        }

        public bool UpdateAreas(long userID, string province, string city, string county)
        {
            return dal.UpdateAreas(UserID, province, city, county);
        }
        #endregion

        #region delete
        /// <summary>
        /// 删除 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public ResultInfo Delete(long id, SqlTransaction tran)
        {
            ResultInfo ri = new ResultInfo();

            var model = GetModel(id, tran);
            if (model == null)
            {
                ri.Msg = "删除的信息不存在";
                return ri;
            }
            if (DeleteByID(id, tran))
            {
                ri.Ok = true;
            }
            else
            {
                ri.Msg = "删除记录时候出错了";
            }
            return ri;

        }

        public bool UpdateSign(long userID, string sign)
        {
            return dal.UpdateSign(userID, sign);
        }
        #endregion

        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public UserBase GetModel(long id, SqlTransaction tran = null)
        {
            return GetItem(id, tran);
        }

        public UserManageViewModel GetUserByKey(string key)
        {
            var dt = dal.GetUserByKey(key);
            UserManageViewModel model = new UserManageViewModel();
            if (dt.IsNotNullAndRowCount())
            {
                model.UserInfoList = ModelConvertHelper<UserInfoViewModel>.ConvertToList(dt);
            }
            return model;
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
        //    List<UserBase> li = ModelConvertHelper<UserBase>.ConvertToList(tab);
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
                var li = ModelConvertHelper<UserBase>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;


        }

        public bool UploadHeadUrl(string url, long userId, SqlTransaction tran = null)
        {
            return dal.UploadHeadUrl(url, userId, tran);
        }
        #endregion

        public bool ExistAccount(string account)
        {
            string code = dal.ExistAccount(account);
            return Convert.ToInt32(code) > 0;
        }

        /// <summary>
        /// 永久封号
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool CloseAccount(long userid)
        {
            return dal.CloseAccount(userid);
        }

        public long ExistUserName(string username)
        {
            string code = dal.ExistUserName(username);
            return string.IsNullOrEmpty(code) ? 0 : Convert.ToInt64(code);
        }

        public UserBase GetUserInfo(string account, string password)
        {
            DataTable dt = dal.GetUserInfo(account, password);
            return ModelConvertHelper<UserBase>.ConverToModel(dt);
        }

        public string GetUserNameByUserID(long userID)
        {
            return dal.GetUserNameByUserID(userID);
        }

        /// <summary>
        /// 判断用户是否能够发表文章
        /// </summary>
        /// <returns></returns>
        public bool CanPublishArticle()
        {
            if (IsLogin)
            {
                BBSEnum levels = BBSEnumBLL.Instance.GetSetArticleRol();
                if (levels != null)
                {
                    return UserExtBLL.Instance.HasEnoughCoin(1, (int)levels.SortIndex, UserInfo.UserID);
                }
            }
            return false;
        }

        //private UserBase _userinfo2 = null;

        /// <summary>
        /// 用户信息
        /// </summary>
        public UserBase UserInfo
        {
            get
            {
                string cookie = CookieHelper.GetCookie("uinfo");
                if (string.IsNullOrEmpty(cookie))
                {
                    return null;
                }
                else
                {
                    string user = DESEncryptHelper.Decrypt(cookie, ConfigHelper.AppSettings("LoginDesKey"));
                    return (UserBase)JsonConvert.DeserializeObject<UserBase>(user);
                }
            }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserID
        {
            get
            {
                UserBase u = UserInfo;
                if (u == null)
                {
                    return 0;
                }
                else
                {
                    return u.UserID;
                }
            }
        }

        /// <summary>
        /// 判断用户是否登录
        /// </summary>
        public bool IsLogin
        {
            get
            {
                return UserInfo != null;
            }
        }

        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool IsMaster
        {
            get
            {
                if (UserInfo != null)
                {
                    var masterCache = "masterCache" + UserInfo.UserID;
                    var cache = CSharpCacheHelper.Get(masterCache);
                    if (cache == null)
                    {
                        Master info = MasterBLL.Instance.Find(UserInfo.UserID);
                        CSharpCacheHelper.Set(masterCache, info != null, APPConst.ExpriseTime.Week1);
                        return info != null;
                    }
                    else
                    {
                        return (bool)cache;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public bool IsRoot
        {
            get
            {
                if (UserInfo == null) return false;
                var masterCache = "rootCache" + UserInfo.UserID;
                var cache = CSharpCacheHelper.Get(masterCache);
                if (cache == null)
                {
                    Master info = MasterBLL.Instance.Find(UserInfo.UserID);
                    bool isroot = info == null ? false : info.IsRoot == 1 ? true : false;
                    CSharpCacheHelper.Set(masterCache, isroot, APPConst.ExpriseTime.Week1);
                    return isroot;
                }
                return (bool)cache;
            }
        }

        /// <summary>
        /// 网站普通管理员
        /// </summary>
        public bool IsNormalMaster
        {
            get
            {
                if (UserInfo == null) return false;
                var masterCache = "normalMasterCache" + UserInfo.UserID;
                var cache = CSharpCacheHelper.Get(masterCache);
                if (cache == null)
                {
                    Master info = MasterBLL.Instance.Find(UserInfo.UserID);
                    bool isNormalMaster = info == null ? false : info.IsRoot == 0 ? true : false;
                    CSharpCacheHelper.Set(masterCache, isNormalMaster, APPConst.ExpriseTime.Week1);
                    return isNormalMaster;
                }
                return (bool)cache;
            }
        }

        ///// <summary>
        ///// 当前第三方登录帐号是否已绑定邮箱帐号
        ///// </summary>
        //public bool IsBindAccount { get; set; }

        /// <summary>
        /// 判断管理员是否拥有某页面权限
        /// </summary>
        /// <param name="bbsEnumid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool HasTopicMaster(int bbsEnumid, long userid)
        {
            return Convert.ToInt32(dal.HasTopicMaster(bbsEnumid, userid)) > 0;
        }

        /// <summary>
        /// 退出当前登录
        /// </summary>
        public void UserLogOff()
        {
            if (UserInfo != null)
            {
                CookieHelper.RemoveCookie("uinfo");
            }
        }

        /// <summary>
        /// 修改昵称
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool UpdateUserName(string nickname, long userID)
        {
            return dal.UpdateUserName(nickname, userID);
        }

        /// <summary>
        /// 根据OPENID查找用户信息
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="weChatAuth"></param>
        /// <returns></returns>
        public UserBase FindUserInfoByOpenID(string openid, RegistEnumType sourceType)
        {
            return ModelConvertHelper<UserBase>.ConverToModel(dal.FindUserInfoByOpenID(openid, sourceType.GetHashCode()));
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public bool DeleteUser(long userId, SqlTransaction tran)
        {
            return dal.DeleteUser(userId, tran);
        }

        /// <summary>
        /// 恢复删除
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public bool ReCoverUser(long userId, SqlTransaction tran)
        {
            return dal.ReCoverUser(userId, tran);
        }
        /// <summary>
        /// 获取签到榜用户信息(今日签到用户)
        /// </summary>
        /// <returns></returns>
        public List<SignUserViewModel> GetSignUsersByToday()
        {
            var time = DateTime.Now;
            DataTable dt = dal.GetSignUsers(time.Date, time);
            return ModelConvertHelper<SignUserViewModel>.ConvertToList(dt);
        }

        /// <summary>
        /// 获取月度签到榜
        /// </summary>
        /// <returns></returns>
        public List<SignUserViewModel> GetSignUsersByMonth()
        {
            DateTime time = DateTime.Now;
            DataTable dt = dal.GetSignUsersByMonth(time.AddDays(-time.Day + 1).Date, time);
            return ModelConvertHelper<SignUserViewModel>.ConvertToList(dt);
        }

        /// <summary>
        /// 获取用户皮肤设置(缓存)
        /// </summary>
        /// <returns></returns>
        public string GetUserSkin(long userid)
        {
            string key = "skin-{0}".FormatWith(userid);
            object skin = CSharpCacheHelper.Get(key);
            if (skin == null)
            {
                string skin_ = UserExtBLL.Instance.GetExtInfo(userid).UserCenterSkin;
                if (skin_.IsNullOrEmpty())
                {
                    skin_ = "skin-default";
                }
                CSharpCacheHelper.Set(key, skin_, 240);
                return skin_;
            }
            return skin.ToString();
            //return "skin-default";
        }

        /// <summary>
        /// 判断用户是否为VIP标签
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool IsVIP(long userid)
        {
            string key = "isVip_" + userid;
            object value = CSharpCacheHelper.Get(key);
            if (value == null)
            {
                var ext = UserExtBLL.Instance.GetExtInfo(userid);
                if (ext != null)
                {
                    CSharpCacheHelper.Set(key, ext.VIP > 0, APPConst.ExpriseTime.Week1);
                    return ext.VIP > 0;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return (bool)value;
            }
        }

        /// <summary>
        /// 获取网站所有客服
        /// </summary>
        /// <returns></returns>
        public CustomerVM GetCustomers()
        {
            var list = new CustomerVM();
            var cache = CSharpCacheHelper.Get(APPConst.Customoer);
            if (cache == null)
            {
                var type1 = CustomerEnumType.QQ.GetHashCode();
                var type2 = CustomerEnumType.WeChar.GetHashCode();
                var type3 = CustomerEnumType.WeChar_GZH.GetHashCode();

                var dt = dal.GetCustomers();
                var request = ModelConvertHelper<Customer>.ConvertToList(dt).OrderByDescending(a => a.CreateTime).ToList();
                list.QQs = request.Where(a => a.Type == type1).ToList();
                list.WXs = request.Where(a => a.Type == type2).ToList();
                list.GZHs = request.Where(a => a.Type == type3).ToList();

                CSharpCacheHelper.Set(APPConst.Customoer, list, APPConst.ExpriseTime.Week1);
                return list;
            }
            else
            {
                return (CustomerVM)cache;
            }
        }
    }
}

