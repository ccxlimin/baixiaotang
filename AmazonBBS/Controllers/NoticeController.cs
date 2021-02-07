using AmazonBBS.BLL;
using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Controllers
{
    /// <summary>
    /// 新动态通知
    /// </summary>
    public class NoticeController : BaseController
    {
        #region 退订
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionid">mainID</param>
        /// <param name="actionid2">sign</param>
        /// <param name="actionid3">mainType</param>
        /// <returns></returns>
        [HttpGet]
        public void TD(long actionid, string actionid2, string actionid3 = "-1")
        {
            string msg = string.Empty;
            try
            {
                int mainType = Convert.ToInt32(actionid3);
                long mainID = actionid;
                long authorID = 0;
                string title = string.Empty;

                if (mainType != -1)
                {
                    bool ok = false;
                    if (mainType == 1)
                    {
                        Question qmodel = QuestionBLL.Instance.GetModel(actionid);
                        if (qmodel == null)
                        {
                            msg = "邮件校验不成功，退订不成功";
                        }
                        else
                        {
                            authorID = Convert.ToInt64(qmodel.UserID);
                            title = qmodel.Title;
                            ok = true;
                        }
                    }
                    else if (mainType == 5)
                    {
                        ZhaoPin zmodel = ZhaoPinBLL.Instance.GetModel(mainID);
                        if (zmodel == null)
                        {
                            msg = "邮件校验不成功，退订不成功";
                        }
                        else
                        {
                            authorID = Convert.ToInt64(zmodel.Publisher);
                            title = zmodel.Gangwei;
                            ok = true;
                        }
                    }
                    if (ok)
                    {
                        EmailNotice emodel = EmailNoticeBLL.Instance.GetModelByAuthor(authorID, mainType, actionid);
                        if (emodel == null)
                        {
                            msg = "邮件退订状态不正常";
                        }
                        else
                        {
                            //比较签名
                            if (emodel.MD5Sign == actionid2)
                            {
                                if (emodel.EmailNoticeAuthor == 0)
                                {
                                    msg = "您已退订";
                                }
                                else
                                {
                                    emodel.EmailNoticeAuthor = 0;
                                    emodel.CreateTime = DateTime.Now;
                                    if (EmailNoticeBLL.Instance.Update(emodel).Ok)
                                    {
                                        msg = "退订成功";
                                    }
                                }
                            }
                            else
                            {
                                msg = "邮件校验不成功，退订不成功";
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            Response.Write(msg);
        }
        #endregion

        #region 恢复
        [HttpGet]
        public void HF(long actionid, string actionid2, string actionid3 = "-1")
        {
            string msg = string.Empty;
            try
            {
                int mainType = Convert.ToInt32(actionid3);
                long mainID = actionid;
                long authorID = 0;
                string title = string.Empty;

                if (mainType != -1)
                {
                    bool ok = false;
                    if (mainType == 1)
                    {
                        Question qmodel = QuestionBLL.Instance.GetModel(actionid);
                        if (qmodel == null)
                        {
                            msg = "邮件校验不成功，恢复不成功";
                        }
                        else
                        {
                            authorID = Convert.ToInt64(qmodel.UserID);
                            title = qmodel.Title;
                            ok = true;
                        }
                    }
                    else if (mainType == 5)
                    {
                        ZhaoPin zmodel = ZhaoPinBLL.Instance.GetModel(mainID);
                        if (zmodel == null)
                        {
                            msg = "邮件校验不成功，恢复不成功";
                        }
                        else
                        {
                            authorID = Convert.ToInt64(zmodel.Publisher);
                            title = zmodel.Gangwei;
                            ok = true;
                        }
                    }
                    if (ok)
                    {
                        EmailNotice emodel = EmailNoticeBLL.Instance.GetModelByAuthor(authorID, mainType, actionid);
                        if (emodel == null)
                        {
                            msg = "邮件恢复状态不正常";
                        }
                        else
                        {
                            //比较签名
                            //string _sign = DESEncryptHelper.Encrypt(authorID.ToString() + actionid.ToString() + title, emodel.MD5Key);
                            if (emodel.MD5Sign == actionid2)
                            {
                                if (emodel.EmailNoticeAuthor == 1)
                                {
                                    msg = "您已恢复";
                                }
                                else
                                {
                                    emodel.EmailNoticeAuthor = 1;
                                    emodel.CreateTime = DateTime.Now;
                                    if (EmailNoticeBLL.Instance.Update(emodel).Ok)
                                    {
                                        msg = "恢复成功";
                                    }
                                }
                            }
                            else
                            {
                                msg = "邮件校验不成功，恢复不成功";
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            Response.Write(msg);
        }
        #endregion

        #region 通知置已读
        [LOGIN]
        public void Read()
        {
            if (IsLogin)
            {
                NoticeBLL.Instance.Read(UserID, DateTime.Now);
            }
        }
        #endregion
    }
}