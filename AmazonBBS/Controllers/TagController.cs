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
    [LOGIN]
    public class TagController : BaseController
    {
        public ActionResult Index()
        {
            var list = GetTags(TagsSortTypeEnum.Sort_All);
            return View(list);
        }

        public ActionResult Tag(string sort)
        {
            var sortType = EnumHelper.ToEnum<TagsSortTypeEnum>(sort);
            var model = GetTags(sortType);
            return PartialView("_LoadTagList", model);
        }

        private List<HomeTagsViewModel> GetTags(TagsSortTypeEnum tagsSortTypeEnum)
        {
            return TagBLL.Instance.GetAllTags(tagsSortTypeEnum);
        }

        public ActionResult Category(long id = 0)
        {
            if (id > 0)
            {
                var tag = TagBLL.Instance.GetModel(id);
                if (tag != null)
                {
                    var model = GetCategorys(id);
                    model.TagName = tag.TagName;
                    model.TagID = id;
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index", "Tag");
                }
            }
            else
            {
                return RedirectToAction("Index", "Tag");
            }
        }

        public ActionResult LoadTag(long id = 0)
        {
            if (id > 0)
            {
                var tag = TagBLL.Instance.GetModel(id);
                if (tag != null)
                {
                    return PartialView("_LoadCategorys", GetCategorys(id));
                }
                else
                {
                    return RedirectToAction("Index", "Tag");
                }
            }
            else
            {
                return RedirectToAction("Index", "Tag");
            }
        }

        private TagCategoryVM GetCategorys(long id)
        {
            int pageSize = 10;
            //TagCategoryVM model = new TagCategoryVM()
            //{
            //    QuestionTags = new BaseListViewModel<_QuestionInfo>() { Page = InitPage() }
            //};
            //model.QuestionTags = QuestionBLL.Instance.GetQuestionList()
            //return View();

            TagCategoryVM model = new TagCategoryVM()
            {
                Articles = new List<_Article>(),
                QuestionInfos = new List<_QuestionInfo>(),
                //Page = InitPage(pageSize)
            };

            int[] count = MenuBelongTagBLL.Instance.Count(id);
            //int bbsCount = count[0], articleCount = count[1];
            ////显示规则，优先显示帖子，最后显示文章，帖子数量不足用文章补充
            ////如果帖子条数超过10条，则显示帖子条数，否则补上文章条数
            //if (bbsCount > 0)
            //{
            //model.QuestionInfos = QuestionBLL.Instance.GetQuestionList(0, model.Page, tagId: id);
            //int selectedCount = model.Page.
            //}

            //已跳过的条数
            int skipCount = pageSize * (GetRequest("pi", 1) - 1);
            //如果 已跳过的条数 大于帖子总条数，则直接筛选文章的
            if (skipCount < count[0])
            {
                //先获取标签对应的帖子
                var bbsPage = InitPage(pageSize);
                model.QuestionInfos = QuestionBLL.Instance.GetQuestionList(0, bbsPage, tagId: id);
                //如果帖子 当前筛选总数 不总 每页显示数量，则以文章补充
                if (model.QuestionInfos.Count < pageSize)
                {
                    if (count[1] > 0)
                    {
                        //如果每页10条数据，已查出结尾的3条帖子数据，则要以7条文章补全，如果不足7条，则显示全部，并且不可再往后分页
                        var articlePage = InitPage();
                        articlePage.PageSize = pageSize - model.QuestionInfos.Count;
                        articlePage.PageIndex = 1;
                        model.Articles = ArticleBLL.Instance.GetAllArticles(articlePage, tagid: id);
                    }
                }
                bbsPage.RecordCount = count[0] + count[1];
                model.Page = bbsPage;
            }
            else
            {
                model.Page = InitPage(pageSize);
                int startIndex = GetRequest("pi", 1) * pageSize - count[0];
                model.Articles = ArticleBLL.Instance.GetAllArticles(startIndex - pageSize + 1, startIndex, tagid: id);
                model.Page.RecordCount = count[0] + count[1];
            }
            return model;
        }

        public ActionResult B(long id)
        {
            var tag = TagBLL.Instance.GetModel(id);
            if (tag != null)
            {
                BBSListViewModel model = new BBSListViewModel()
                {
                    QuestionPage = InitPage()
                };
                model.QuestionList = QuestionBLL.Instance.GetQuestionList(0, model.QuestionPage, null, tagId: id);
                ViewBag.HasTopicMaster = false;// UserBaseBLL.Instance.IsRoot;//无权编辑页面，除了超管
                ViewBag.Name = tag.TagName;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Tag");
            }
        }

        public ActionResult A(long id)
        {
            var tag = TagBLL.Instance.GetModel(id);
            if (tag != null)
            {
                ArticleViewModel model = new ArticleViewModel();
                model.ARticlePage = InitPage();
                model.Articles = ArticleBLL.Instance.GetAllArticles(model.ARticlePage, tagid: id);
                ViewBag.HasTopicMaster = false;// UserBaseBLL.Instance.IsRoot;//无权编辑页面，除了超管
                ViewBag.Name = tag.TagName;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Tag");
            }
        }
    }
}