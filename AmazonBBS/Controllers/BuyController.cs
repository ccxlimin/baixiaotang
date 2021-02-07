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
    public class BuyController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IScoreService _scoreService;
        public INoticeService noticeService { get; set; }
        public AmazonBBSDBContext amazonBBSDBContext { get; set; }

        public BuyController(IUserService userService, IScoreService scoreService)
        {
            _userService = userService;
            _scoreService = scoreService;
        }

        public ActionResult Index(int feetype, int fee, int mid, int maintype)
        {
            var ri = new ResultInfo<string>();
            var e = (ContentFeeMainEnumType)maintype;
            //获取金额
            long authorId = 0;
            string mainTitle = string.Empty;
            string content = string.Empty;
            bool canBuy = false;
            if (e == ContentFeeMainEnumType.BBS)
            {
                var model = DB.Question.FirstOrDefault(a => a.QuestionId == mid);
                content = model.Body;
                authorId = model.UserID.Value;
                mainTitle = model.Title;
                if (fee == model.ContentFee)
                {
                    canBuy = true;
                    fee = model.ContentFee.Value;
                    feetype = model.ContentFeeType.Value;
                }
            }
            else
            {
                var model = DB.Article.FirstOrDefault(a => a.ArticleId == mid);
                content = model.Body;
                mainTitle = model.Title;
                authorId = model.UserID.Value;
                if (fee == model.ContentFee)
                {
                    canBuy = true;
                    fee = model.ContentFee.Value;
                    feetype = model.ContentFeeType.Value;
                }
            }
            if (UserID != authorId)
            {
                if (canBuy)
                {
                    //判断是否已购买
                    if (DB.ContentBuyLog.FirstOrDefault(a => a.BuyerId == UserID && a.MainID == mid && a.MainType == maintype) == null)
                    {
                        //开启事务
                        var tran = DB.Database.BeginTransaction();
                        try
                        {
                            // 判断资金是否足够
                            var enough = _scoreService.HasEnoughCoinAndSubCoin(feetype == 10 ? 1 : 2, fee, UserID, CoinSourceEnum.BuyContent);
                            if (enough.Item1)
                            {
                                DateTime now = DateTime.Now;
                                //添加购买记录
                                DB.ContentBuyLog.Add(new ContentBuyLog
                                {
                                    BuyerId = UserID,
                                    CreateTime = now,
                                    Fee = fee,
                                    FeeType = feetype,
                                    MainID = mid,
                                    MainType = maintype
                                });

                                _scoreService.AddScoreOrCoin(authorId, feetype == 10 ? 1 : 2, fee, CoinSourceEnum.UserBuyContent);

                                DB.SaveChanges();
                                tran.Commit();

                                //通知购买用户 通知作者
                                noticeService.OnUserBuy_Content_Success_For_BBS_Arcitle_Notice_BuyerAndAuthor(UserInfo, authorId, mid, mainTitle, e, fee, feetype, now);

                                ri.Ok = true;
                                ri.Msg = "购买成功";
                                ri.Data = content;
                            }
                            else
                            {
                                ri.Msg = enough.Item2;
                                tran.Rollback();
                            }
                        }
                        catch
                        {
                            tran.Rollback();
                            ri.Msg = "购买失败";
                        }
                    }
                    else
                    {
                        ri.Ok = true;
                        ri.Data = content;
                        ri.Msg = "你已购买过该主题内容了";
                    }
                }
                else
                {
                    ri.Msg = "信息错误，请刷新页面重新购买！";
                }
            }
            else
            {
                ri.Msg = "自己不能购买自己的";
            }

            return Result(ri);
        }
    }
}