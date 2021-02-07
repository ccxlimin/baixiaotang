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
    public class ChatBLL : Auto_ChatBLL
    {
        public static ChatBLL Instance
        {
            get { return SingleHepler<ChatBLL>.Instance; }
        }
        ChatDAL dal = new ChatDAL();


        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(Chat model)
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
        public ResultInfo Update(Chat model)
        {
            ResultInfo ri = new ResultInfo();
            if (Edit(model))
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

        public ListenChatAndNotice GetALlUnReadMsg(long userID)
        {
            var ds = dal.GetAllUnReadMsg(userID);
            var model = new ListenChatAndNotice();
            model.Chats = ModelConvertHelper<ChatViewModel>.ConvertToList(ds.Tables[0]);
            model.Notices = ds.Tables[1].Rows[0][0].ToString().ToInt32();
            //return ModelConvertHelper<ChatViewModel>.ConvertToList(dal.GetAllUnReadMsg(userID));
            return model;
        }

        public List<ChatBox> GetALlUnReadMessageByFromID(long id, long currentUserid)
        {
            return ModelConvertHelper<ChatBox>.ConvertToList(dal.GetALlUnReadMessageByFromID(id, currentUserid));
        }

        /// <summary>
        /// 根据聊天对象ID获取对话记录
        /// </summary>
        /// <param name="chatPage"></param>
        /// <param name="userID"></param>
        /// <param name="toUserID"></param>
        /// <returns></returns>
        public List<_MsgBox> GetDialogByUserID(Paging chatPage, long userID, long toUserID)
        {
            DataSet ds = dal.GetDialogByUserID(chatPage.StartIndex, chatPage.EndIndex, userID, toUserID);
            chatPage.RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            return ModelConvertHelper<_MsgBox>.ConvertToList(ds.Tables[1]);
        }

        /// <summary>
        /// 根据当前用户ID获取所有对话记录 的 最新一条
        /// </summary>
        /// <param name="page"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<_MsgBox> GetMyMessage(Paging page, long userId)
        {
            DataSet ds = dal.GetMyMessage(page.StartIndex, page.EndIndex, userId);
            page.RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            return ModelConvertHelper<_MsgBox>.ConvertToList(ds.Tables[1]);
        }
        #endregion



        #region getmodel
        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public Chat GetModel(long id)
        {
            return GetItem(id);
        }

        public bool Read(string msgid)
        {
            return dal.Read(msgid);
        }

        public bool Read(long fromuserid, long touserid)
        {
            return dal.Read(fromuserid, touserid);
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
        //    List<Chat> li = ModelConvertHelper<Chat>.ConvertToList(tab);
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
                var li = ModelConvertHelper<Chat>.ConvertToList(tab);
                //paging.SetDataSource(li);
            }
            return paging;


        }
        #endregion


    }
}

