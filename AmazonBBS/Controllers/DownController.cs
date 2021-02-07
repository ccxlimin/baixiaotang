using AmazonBBS.BLL;
using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmazonBBS.Controllers
{
    [LOGIN]
    public class DownController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IScoreService _scoreService;

        public DownController(IUserService userService, IScoreService scoreService)
        {
            _userService = userService;
            _scoreService = scoreService;
        }

        /// <summary>
        /// 下载附件
        /// </summary>
        /// <param name="mt">类型 1 帖子附件下载 2 文章附件下载</param>
        /// <param name="md">主ID 如帖子的ID 10000</param>
        /// <param name="ad">附件ID</param>
        /// <returns></returns>
        public ActionResult Index(int mt, long md, Guid ad)
        {
            var canDown = false;
            string fileName = string.Empty;
            string filepath = string.Empty;
            if (mt > 0 && md > 0 && ad != Guid.Empty)
            {
                long authorId = 0;
                //判断帖子是否存在
                if (mt == AttachEnumType.BBS.GetHashCode())
                {
                    var model = DB.Question.FirstOrDefault(a => a.IsDelete == 0 && a.QuestionId == md);
                    if (model != null)
                    {
                        canDown = true;
                        authorId = model.UserID.Value;
                    }
                }
                else
                {
                    var model = DB.Article.FirstOrDefault(a => a.IsDelete == 0 && a.ArticleId == md);
                    if (model != null)
                    {
                        canDown = true;
                        authorId = model.UserID.Value;
                    }
                }
                if (canDown)
                {
                    //判断附件是否存在
                    var attachInfo = DB.AttachMent.FirstOrDefault(a => a.AttachMentId == ad && a.MainId == md && a.MainType == mt);
                    if (attachInfo != null)
                    {
                        //判断是否需要付费下载 ,作者自己可以下载
                        if (attachInfo.IsFee && authorId != UserID)
                        {
                            //判断用户是否已付费
                            canDown = DB.AttachMentBuyLog.FirstOrDefault(a => a.AttachMentId == ad && a.BuyerId == UserID && a.MainID == md) != null;
                            //如果用户没有付费，则先付费，后下载
                            if (!canDown)
                            {
                                //开启事务
                                var tran = DB.Database.BeginTransaction();
                                try
                                {
                                    //扣除下载费用并记录
                                    var result = _scoreService.HasEnoughCoinAndSubCoin(attachInfo.FeeType == 10 ? 1 : 2, attachInfo.Fee, UserID, CoinSourceEnum.DownAttachMent);
                                    if (result.Item1)
                                    {
                                        canDown = true;
                                        //记录购买附件
                                        DB.AttachMentBuyLog.Add(new AttachMentBuyLog()
                                        {
                                            AttachMentId = ad,
                                            BuyerId = UserID,
                                            CreateTime = DateTime.Now,
                                            MainID = md,
                                            MainType = mt
                                        });

                                        //给作者增加 积分
                                        _scoreService.AddScoreOrCoin(authorId, attachInfo.FeeType == 10 ? 1 : 2, attachInfo.Fee, CoinSourceEnum.UserDownAttachMent);

                                        DB.SaveChanges();
                                        tran.Commit();
                                    }
                                    else
                                    {
                                        tran.Rollback();
                                        return Content(result.Item2);
                                    }
                                }
                                catch (Exception e)
                                {
                                    tran.Rollback();
                                    canDown = false;
                                    return Content("下载出错，请重试！");
                                }
                            }
                        }
                        if (canDown)
                        {
                            fileName = attachInfo.FileName;
                            filepath = attachInfo.FilePath;

                            //1天内重复下载不计次数
                            string key = $"down_{attachInfo.AttachMentId}_{UserID}";
                            var cache = CSharpCacheHelper.Get<bool>(key, false);
                            if (!cache)
                            {
                                CSharpCacheHelper.Set(key, true, APPConst.ExpriseTime.Day1);
                                attachInfo.DownCount++;
                                DB.SaveChanges();
                            }
                        }
                    }
                }
            }
            if (canDown)
            {
                return File(new FileStream(Server.MapPath(filepath), FileMode.Open), "text/plain", fileName);
            }
            else
            {
                return Content("你无法下载此资源！");
            }
        }
    }
}