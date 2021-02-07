using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Model
{
    /// <summary>
    /// 整合UserBase 和 UserExt
    /// </summary>
    public class UserInfoViewModel
    {
        /// <summary>
        /// 邮箱验证码
        /// </summary>
        public int ValidateCode
        {
            get; set;
        }

        #region UserBase
        /// <summary>
        /// 
        /// </summary>
        public long UserID
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string UserName
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Account
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Password
        {
            get;
            set;
        }
        /// <summary>
        /// 注册来源（1QQ登录 2邮箱注册 3微信登录)
        /// </summary>
        public int? Source
        {
            get;
            set;
        }

        /// <summary>
        /// 第三方登录ID
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string CreateUser
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string UpdateUser
        {
            get;
            set;
        }
        /// <summary>
        /// 是否删除（0有效 1已删除 2永久冻结 3临时封号）
        /// </summary>
        public int? IsDelete
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Sign
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string HeadUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime? LoginTime
        {
            get;
            set;
        }
        /// <summary>
        /// 登录IP地址
        /// </summary>
        public string LoginIP
        {
            get;
            set;
        }
        /// <summary>
        /// 性别(1男2女)
        /// </summary>
        public int? Gender
        {
            get;
            set;
        }
        /// <summary>
        /// 所在省
        /// </summary>
        public string Province
        {
            get;
            set;
        }
        /// <summary>
        /// 所在市
        /// </summary>
        public string City
        {
            get;
            set;
        }
        /// <summary>
        /// 所在县
        /// </summary>
        public string County
        {
            get;
            set;
        }
        /// <summary>
        /// 出生年月日
        /// </summary>
        public string Birth
        {
            get;
            set;
        }
        /// <summary>
        /// 岗位
        /// </summary>
        public string Work
        {
            get;
            set;
        }
        /// <summary>
        /// 经营类目
        /// </summary>
        public string JingYing
        {
            get;
            set;
        }
        /// <summary>
        /// 工作年限
        /// </summary>
        public string WorkYear
        {
            get;
            set;
        }

        #endregion

        #region UserExt
        /// <summary>
        /// 
        /// </summary>
        public long UserExtId
        {
            get;
            set;
        }
        /// <summary>
        /// 剩余总积分
        /// </summary>
        public int? TotalScore
        {
            get;
            set;
        }
        /// <summary>
        /// 剩余总金钱
        /// </summary>
        public int? TotalCoin
        {
            get;
            set;
        }

        /// <summary>
        /// 用户头衔
        /// </summary>
        public int? LevelName { get; set; }

        /// <summary>
        /// 用户专属头衔
        /// </summary>
        public int? OnlyLevelName { get; set; }

        /// <summary>
        /// 账号封禁到期时间
        /// </summary>
        public DateTime? CloseTime { get; set; }

        /// <summary>
        /// 用户认证
        /// (0:未认证 1:1级认证，2:2级认证 3：3级认证 4：1级和2级认证 5：2级和3级认证 6：1级和3级认证)
        /// 1=红人认证
        /// 2=牛人认证
        /// 3=法人认证
        /// </summary>
        public int UserV { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string CardID { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 公司联系电话
        /// </summary>
        public string CompanyTel { get; set; }

        /// <summary>
        /// 身份证（照片）
        /// </summary>
        public string CardPic { get; set; }
        /// <summary>
        /// 法人执照
        /// </summary>
        public string FaRenPic { get; set; }

        /// <summary>
        /// VIP等级（1，2，3……等级无上限）
        /// </summary>
        public int VIP { get; set; }

        /// <summary>
        /// VIP有效时间
        /// </summary>
        public DateTime? VIPExpiryTime { get; set; }

        /// <summary>
        /// 头衔显示类型(1头衔(默认)   2专属头衔)
        /// </summary>
        public int HeadNameShowType { get; set; }

        /// <summary>
        /// 发贴需要审核(0不需要审核 1需要审核)
        /// </summary>
        public int CheckBBS { get; set; }
        #endregion
    }
}
