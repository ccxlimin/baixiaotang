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
    public class ProductController : BaseController
    {
        // GET: Product
        public ActionResult Index()
        {
            return View(GetProducts());
        }

        private ProductViewModel GetProducts()
        {
            Paging page = InitPage(20);
            ProductViewModel model = ProductBLL.Instance.FindAll(page);
            return model;
        }

        [HttpGet]
        public ActionResult Products()
        {
            var model = GetProducts();
            return PartialView("_Search", model);
        }

        #region 发布产品
        [LOGIN]
        public ActionResult Publish()
        {
            return View();
        }

        [LOGIN]
        [HttpPost]
        public ActionResult Publish(Product model)
        {
            ResultInfo ri = UpLoadImg("ProductPic", "/Content/Job/CP", beforeSaveFile: (save, resultinfo) =>
            {
                try
                {
                    BeginTran();
                    //判断VIP分是否足够
                    int publishScore = Convert.ToInt32(ConfigHelper.AppSettings("PUBLISH_PRODUCT"));
                    int type = 2;
                    if (UserExtBLL.Instance.HasEnoughCoin(type, publishScore, UserID))
                    {
                        //扣除相应积分
                        bool logOk = UserExtBLL.Instance.SubScore(UserID, publishScore, type, Tran);
                        if (logOk)
                        {
                            if (ScoreCoinLogBLL.Instance.Log(-publishScore, type, CoinSourceEnum.PublishProduct, UserID, UserInfo.UserName, Tran))
                            {
                                resultinfo = save();
                                if (resultinfo.Ok)
                                {
                                    model.CreateTime = DateTime.Now;
                                    model.CreateUser = UserID.ToString();
                                    model.IsDelete = 0;
                                    model.PLogo = UserInfo.HeadUrl;
                                    model.ProductPic = resultinfo.Url;
                                    model.IsTop = 0;
                                    model.IsJinghua = 0;
                                    model.IsRemen = 0;
                                    model.PVCount = 0;
                                    model.UpdateTime = DateTime.Now;
                                    model.UpdateUser = UserID.ToString();
                                    model.ValidTime = DateTime.Now.AddDays(GetRequest<int>("deadTime", 30));

                                    int result = ProductBLL.Instance.Add(model, Tran);

                                    if (result > 0)
                                    {
                                        var uri = ConfigHelper.AppSettings("ProductDetail").FormatWith(result);
                                        resultinfo.Url = uri;
                                        resultinfo.Ok = true;
                                        Commit();

                                        //通知作者
                                        NoticeBLL.Instance.OnPayPublish_Notice_Author(UserID, DateTime.Now, GetDomainName + uri, model.PTitle, 20, publishScore.ToString(), NoticeTypeEnum.Product_Pay_Publish);
                                        //通知关注作者的用户
                                        NoticeBLL.Instance.OnAdd_Notice_Liker(UserInfo.UserName, UserID, uri, model.PTitle, NoticeTypeEnum.Product_Add, GetDomainName);
                                    }
                                    else
                                    {
                                        RollBack();
                                        resultinfo.Msg = "发布失败";
                                    }
                                }
                            }
                            else
                            {
                                RollBack();
                                resultinfo.Msg = "发布失败";
                            }
                        }
                        else
                        {
                            RollBack();
                            resultinfo.Msg = "发布失败";
                        }
                    }
                    else
                    {
                        resultinfo.Msg = "VIP分不足，请去个人中心充值";
                    }
                }
                catch
                {
                    resultinfo.Msg = "发布异常 ";
                    RollBack();
                    UploadHelper.DeleteUpFile(resultinfo.Url);
                }
            });
            return Result(ri);
        }
        #endregion

        public ActionResult Detail(int id = 0)
        {
            if (id > 0)
            {
                Paging page = InitPage();
                _Product model = ProductBLL.Instance.GetProductDetail(id, UserID, page);
                if (model == null)
                {
                    return RedirectToAction("Index", "Product");
                }
                ViewBag.canSeeContact = UserInfo != null && (model.CreateUser == UserID.ToString() || FeeHRBLL.Instance.IsPayContact(UserID, id, 3));

                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        #region 删除
        [HttpPost]
        public ActionResult Delete(long id)
        {
            ResultInfo ri = new ResultInfo();
            if (UserBaseBLL.Instance.IsRoot)
            {
                if (id > 0)
                {
                    Product model = ProductBLL.Instance.GetModel(id);
                    if (model != null)
                    {
                        if (model.IsDelete == 1)
                        {
                            ri.Msg = "该产品已被删除";
                        }
                        else
                        {
                            model.IsDelete = 1;
                            ri = ProductBLL.Instance.Update(model);
                        }
                    }
                    else
                    {
                        ri.Msg = "异常";
                    }
                }
                else
                {
                    ri.Msg = "异常";
                }
            }
            else
            {
                ri.Msg = "你没有权限进行此操作";
            }

            return Result(ri);
        }
        #endregion

        #region 编辑
        //[IsMaster]
        [LOGIN]
        public ActionResult Edit(long id)
        {
            if (id > 0)
            {
                var product = ProductBLL.Instance.GetModel(id);
                return View(product);
            }
            else
            {
                return RedirectToAction("Index", "Product");
            }
        }

        //[IsMaster]
        [LOGIN]
        [HttpPost]
        public ActionResult Edit(Product model)
        {
            ResultInfo ri = new ResultInfo();
            if (ModelState.IsValid)
            {
                Product _model = ProductBLL.Instance.GetModel(model.ProductID);
                if (_model == null)
                {
                    ri.Msg = "该产品已被删除";
                }
                else
                {
                    Action<string> action = (imagePath) =>
                    {
                        _model.CompanyName = model.CompanyName;
                        _model.PTitle = model.PTitle;
                        _model.PLocation = model.PLocation;
                        _model.PDesc = model.PDesc;
                        _model.PFunction = model.PFunction;
                        _model.PPrice = model.PPrice;
                        _model.PSize = model.PSize;
                        _model.Contact = model.Contact;
                        _model.ProductPic = imagePath;
                        _model.PUnit = model.PUnit;
                        _model.PWeight = model.PWeight;
                        _model.SendDay = model.SendDay;
                        _model.UpdateUser = UserID.ToString();
                        _model.UpdateTime = DateTime.Now;

                        ri = ProductBLL.Instance.Update(_model);
                        if (ri.Ok)
                        {
                            ri.Url = ConfigHelper.AppSettings("ProductDetail").FormatWith(_model.ProductID);
                        }
                    };
                    //是否修改上传照片
                    if (GetRequest<bool>("ischange"))
                    {
                        string oldImagePath = _model.ProductPic;
                        ri = UpLoadImg("ProductPic", "/Content/Job/CP");
                        if (ri.Ok)
                        {
                            action(ri.Url);
                            UploadHelper.DeleteUpFile(oldImagePath);
                        }
                    }
                    else
                    {
                        action(_model.ProductPic);
                    }
                }
            }
            return Result(ri);
        }
        #endregion

        #region 导航到word
        //[HttpPost]
        //public ActionResult ExportWord()
        //{

        //}
        #endregion

        #region 搜索
        [HttpGet]
        public ActionResult Search(string key)
        {
            ResultInfo ri = new ResultInfo();

            if (key.IsNotNullOrEmpty())
            {
                Paging page = InitPage(20);
                ProductViewModel model = ProductBLL.Instance.FindAll(page, key);
                return PartialView("_Search", model);
            }
            else
            {
                ri.Msg = "请输入关键词";
            }
            return Result(ri);
        }
        #endregion

        #region 条件筛选
        [HttpGet]
        public ActionResult Select(string search_cname, string search_pname, string search_price, string search_endTime)
        {
            ResultInfo ri = new ResultInfo();
            Paging page = InitPage(20);
            string search_price_min = string.Empty;
            string search_price_max = string.Empty;
            switch (search_price)
            {
                case "1":
                    search_price_min = "0";
                    search_price_max = "500";
                    break;
                case "2":
                    search_price_min = "500";
                    search_price_max = "1000";
                    break;
                case "3":
                    search_price_min = "1000";
                    search_price_max = "2000";
                    break;
                case "4":
                    search_price_min = "2000";
                    search_price_max = "3000";
                    break;
                case "5":
                    search_price_min = "3000";
                    search_price_max = "5000";
                    break;
                case "6":
                    search_price_min = "5000";
                    search_price_max = "999999";
                    break;
            }
            ProductViewModel model = ProductBLL.Instance.SelectByCondition(page, search_cname, search_pname, search_price_min, search_price_max, search_endTime);
            return PartialView("_Search", model);
        }
        #endregion

        #region 预约购买
        [LOGIN]
        public ActionResult OrderBuy(long id = 0)
        {
            ResultInfo ri = new ResultInfo();
            if (id > 0)
            {
                var model = ProductBLL.Instance.GetModel(id);
                if (model != null)
                {
                    if (model.IsDelete == 0)
                    {
                        ProductBuy buymodel = new ProductBuy()
                        {
                            BuyUserID = UserID,
                            IsDelete = 0,
                            CreateTime = DateTime.Now,
                            //LinkName=string.Empty,
                            //LinkTel
                            ProductID = model.ProductID
                        };
                        if (ProductBuyBLL.Instance.Add(buymodel) > 0)
                        {
                            ri.Ok = true;
                            //通知商家
                            var salerInfo = UserBaseBLL.Instance.GetUserInfo(model.CreateUser.ToInt64());
                            NoticeBLL.Instance.OnOrderBuyProduct_Notice_Saler(salerInfo, model.ProductID.ToString(), model.PTitle, UserInfo.UserName, UserID, GetDomainName, NoticeTypeEnum.OrderBuyProduct);
                        }
                    }
                    else
                    {
                        ri.Msg = "产品信息已被删除，不能预约！";
                    }
                }
                else
                {
                    ri.Msg = "产品信息不存在！";
                }
            }
            else
            {
                ri.Msg = "预约信息源错误！";
            }
            return Result(ri);
        }
        #endregion

        #region 擦亮
        public ActionResult Light(long id = 0)
        {
            ResultInfo<DateTime> ri = new ResultInfo<DateTime>();
            if (id > 0)
            {
                Product model = ProductBLL.Instance.GetModel(id);
                if (model != null)
                {
                    if (DateTime.Now >= model.ValidTime)
                    {
                        DateTime time = DateTime.Now.AddDays(30);
                        model.ValidTime = time;
                        if (ProductBLL.Instance.Update(model).Ok)
                        {
                            ri.Ok = true;
                            ri.Msg = "成功擦亮，有效时间延长30天！";
                            ri.Data = time;
                        }
                    }
                    else
                    {
                        ri.Msg = "招聘信息未失效，无需擦亮！";
                    }
                }
                else
                {
                    ri.Msg = "招聘信息不存在！";
                }
            }
            else
            {
                ri.Msg = "招聘信息不存在！";
            }
            return Result(ri);
        }
        #endregion
    }
}