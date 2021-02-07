using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using AmazonBBS.BLL;
using AmazonBBS.Model;
using AmazonBBS.Common;
using System.Data.SqlClient;
using System.Text;
using EntityFramework.Extensions;

namespace AmazonBBS.Controllers
{
    public class CController : BaseController
    {
        #region 发送聊天消息
        [HttpPost]
        [LOGIN]
        public ActionResult Talk(long id, string message)
        {
            ResultInfo<Chat> ri = new ResultInfo<Chat>();
            if (id > 0)
            {
                if (id != UserID)
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        int sendCount = ConfigHelper.AppSettings("SendMsgCount").ToInt32();//用户每天最大发送消息数
                        //判断是否超次数(管理员不受此限制)
                        bool canSend = UserBaseBLL.Instance.IsMaster;
                        if (!canSend)
                        {
                            canSend = sendCount > ChatBLL.Instance.SendMsgCount(UserID) || UserID == 10009;
                        }
                        if (canSend)
                        {
                            string toUserName = UserBaseBLL.Instance.GetUserNameByUserID(id);
                            if (!string.IsNullOrEmpty(toUserName))
                            {
                                Chat chat = new Chat();
                                chat.ToID = id;
                                chat.ToUserName = toUserName;
                                chat.FromID = UserID;
                                chat.FromUserName = UserInfo.UserName;
                                chat.IsRead = 0;
                                chat.Message = HttpUtility.UrlDecode(message);
                                chat.SendTime = DateTime.Now;
                                chat.Batch = false;

                                if (ChatBLL.Instance.Add(chat) > 0)
                                {
                                    ri.Ok = true;
                                    ri.Msg = "消息发送成功";
                                    ri.Data = new ChatBox()
                                    {
                                        FromID = UserID,
                                        FromUserName = UserInfo.UserName,
                                        Message = chat.Message,
                                        Head = UserInfo.HeadUrl
                                    };
                                }
                                else
                                {
                                    ri.Msg = "消息发送失败";
                                }
                            }
                            else
                            {
                                ri.Msg = "目标用户不存在";
                            }
                        }
                        else
                        {
                            ri.Msg = "您今日发送次数已达上限";
                        }
                    }
                    else
                    {
                        ri.Msg = "消息内容不能为空";
                    }
                }
                else
                {
                    ri.Msg = "不能给自己发送消息";
                }
            }
            else
            {
                ri.Msg = "接收人不存在!";
            }
            return Result(ri);
        }
        #endregion

        #region (循环监听)获取当前登录人所有未读消息 及 通知
        /// <summary>
        /// 获取当前登录人所有未读消息 及 通知
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        public ActionResult MSG()
        {
            ResultInfo<ListenChatAndNotice> ri = new ResultInfo<ListenChatAndNotice>() { Data = new ListenChatAndNotice() };
            ri.Data = ChatBLL.Instance.GetALlUnReadMsg(UserID);
            ri.Ok = true;
            return Result(ri);
        }
        #endregion

        #region 根据发送消息人获取所有未读消息
        [ActionName("Listen")]
        public ActionResult GetMessageByFromID(long id)
        {
            ResultInfo<List<ChatBox>> ri = new ResultInfo<List<ChatBox>>();
            if (id > 0)
            {
                List<ChatBox> list = ChatBLL.Instance.GetALlUnReadMessageByFromID(id, UserID);
                if (list.Count > 0)
                {
                    ri.Ok = true;
                    ri.Data = list;
                }
            }
            return Result(ri);
        }
        #endregion

        #region 置为已读
        /// <summary>
        /// 置为已读
        /// </summary>
        /// <param name="msgid"></param>
        /// <returns></returns>
        public ActionResult Read(string msgid)
        {
            ResultInfo ri = new ResultInfo();
            ri.Ok = ChatBLL.Instance.Read(msgid);
            return Result(ri);
        }
        #endregion

        #region 管理员批量发送消息
        [IsMaster]
        [LOGIN]
        [HttpPost]
        public ActionResult TalkBatch(List<long> ids, string message)
        {
            ResultInfo ri = new ResultInfo();
            ids.Remove(UserID);
            if (ids.Count > 0)
            {
                if (message.IsNotNullOrEmpty())
                {
                    var users = DB.UserBase.Where(a => ids.Contains(a.UserID) && a.IsDelete == 0).ToList();
                    if (users.Count > 0)
                    {
                        string fromUserName = UserInfo.UserName;
                        long fromoUserId = UserID;
                        string msg = HttpUtility.UrlDecode(message);
                        List<Chat> chats = new List<Chat>();
                        var now = DateTime.Now;
                        users.ForEach(user =>
                        {
                            chats.Add(new Chat
                            {
                                ToID = user.UserID,
                                ToUserName = user.UserName,
                                FromID = fromoUserId,
                                FromUserName = fromUserName,
                                IsRead = 0,
                                Message = msg,
                                SendTime = now,
                                Batch = true,
                            });
                        });
                        SqlHelper.SqlBulkCopyByDatatable(SqlHelper.DefaultConnectionString, "Chat", chats.ToDataTable());
                        ri.Ok = true;
                        ri.Msg = "消息发送成功";
                    }
                    else
                    {
                        ri.Msg = "没有有效用户，无法发送消息";
                    }
                }
                else
                {
                    ri.Msg = "消息内容不能为空！";
                }
            }
            else
            {
                ri.Msg = "接收人不能为空!";
            }
            return Result(ri);
        }
        #endregion

        #region 给全站用户发送消息 
        [LOGIN]
        [IsMaster]
        [HttpPost]
        public ActionResult All(string message)
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                if (message.IsNotNullOrEmpty())
                {
                    var users = DB.UserBase.Where(a => a.IsDelete == 0 && a.UserID != UserID).ToList();
                    if (users.Count > 0)
                    {
                        string fromUserName = UserInfo.UserName;
                        long fromoUserId = UserID;
                        string msg = HttpUtility.UrlDecode(message);
                        List<Chat> chats = new List<Chat>();
                        var now = DateTime.Now;
                        users.ForEach(user =>
                        {
                            chats.Add(new Chat
                            {
                                ToID = user.UserID,
                                ToUserName = user.UserName,
                                FromID = fromoUserId,
                                FromUserName = fromUserName,
                                IsRead = 0,
                                Message = msg,
                                SendTime = now,
                                Batch = true,
                            });
                        });
                        SqlHelper.SqlBulkCopyByDatatable(SqlHelper.DefaultConnectionString, "Chat", chats.ToDataTable());

                        ri.Ok = true;
                        ri.Msg = "消息发送成功";
                    }
                    else
                    {
                        ri.Msg = "没有有效用户，无法发送消息";
                    }
                }
                else
                {
                    ri.Msg = "消息内容不能为空！";
                }
            }
            catch (Exception e)
            {
                ErrorBLL.Instance.Log(e.ToString());
            }
            return Result(ri);
        }
        #endregion
    }
}