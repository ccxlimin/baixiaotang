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
    /// 产品服务
    /// </summary>
    public class ProductBLL : Auto_ProductBLL
    {
        public static ProductBLL Instance
        {
            get { return SingleHepler<ProductBLL>.Instance; }
        }

        public ProductViewModel FindAll(Paging page, string key = null)
        {
            ProductViewModel model = new ProductViewModel();
            var ds = dal.SearchByRows(page.StartIndex, page.EndIndex, key);
            int count = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            page.RecordCount = count;
            model.ProductList = ModelConvertHelper<_Product>.ConvertToList(ds.Tables[1]);
            model.ProductPage = page;
            return model;
        }

        ProductDAL dal = new ProductDAL();

        #region add
        /// <summary>
        /// 保存 (可能有其他业务逻辑检查)
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public ResultInfo Create(Product model, SqlTransaction tran)
        {
            ResultInfo ri = new ResultInfo();

            if (model == null) return ri;

            int result = Add(model, tran);

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
        public ResultInfo Update(Product model)
        {
            ResultInfo ri = new ResultInfo();
            if (Edit(model))
            {
                ri.Ok = true;
                ri.Msg = "修改成功";
            }
            return ri;
        }

        public _Product GetProductDetail(int id, long userID, Paging page)
        {
            _Product model = ModelConvertHelper<_Product>.ConverToModel(dal.GetModel(id));
            if (model != null)
            {
                model.Comments = CommentBLL.Instance.GetCommentCallBack(id, CommentEnumType.Product.GetHashCode(), PriseEnumType.ProductComment.GetHashCode(), userID, page);
                return model;
            }
            return null;
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
        public Product GetModel(long id, SqlTransaction tran = null)
        {
            return GetItem(id, tran);
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
        //    List<Product> li = ModelConvertHelper<Product>.ConvertToList(tab);
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
                var li = ModelConvertHelper<Product>.ConvertToList(tab);
            }
            return paging;


        }
        #endregion

        #region 筛选
        public ProductViewModel SelectByCondition(Paging page, string search_cname, string search_pname, string search_price_min, string search_price_max, string search_endTime)
        {
            StringBuilder sb = new StringBuilder();
            if (IsSafe(search_cname))
            {
                sb.Append(@" and a.CompanyName like '%{0}%' ".FormatWith(search_cname));
            }
            if (IsSafe(search_pname))
            {
                sb.Append(" and a.PTitle like '%{0}%' ".FormatWith(search_pname));
            }
            if (IsSafe(search_price_min) && IsSafe(search_price_max))
            {
                sb.Append(" and ISNULL(a.PPrice,0) between {0} and {1} ".FormatWith(search_price_min, search_price_max));
            }
            if (IsSafe(search_endTime))
            {
                sb.Append(" and a.CreateTime < {0} ".FormatWith(Convert.ToDateTime(search_endTime).AddDays(-30)));
            }
            var model = new ProductViewModel();
            var ds = dal.SelectByCondition(page.StartIndex, page.EndIndex, sb.ToString());
            int count = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            page.RecordCount = count;
            model.ProductList = ModelConvertHelper<_Product>.ConvertToList(ds.Tables[1]);
            model.ProductPage = page;
            return model;
        }

        private bool IsSafe(string condition)
        {
            if ("delete,update,select".IndexOf(condition.ToLower()) > -1)
            {
                return false;
            }
            else
            {
                if (condition.IsNotNullOrEmpty())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Product GetProductBuyPay(long mainid)
        {
            return ModelConvertHelper<Product>.ConverToModel(dal.GetProductBuyPay(mainid));
        }

        public BaseListViewModel<Product> GetProductList(long userID, Paging paging)
        {
            var model = new BaseListViewModel<Product>();
            DataSet ds = dal.GetProductList(userID, paging.StartIndex, paging.EndIndex);
            model.Page = paging;
            model.Page.RecordCount = ds.Tables[0].Rows[0][0].ToString().ToInt32();
            model.DataList = ModelConvertHelper<Product>.ConvertToList(ds.Tables[1]);
            return model;
        }
        #endregion
    }
}

