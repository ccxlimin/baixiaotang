using System.Web;
using System.Web.Optimization;

namespace AmazonBBS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //BundleTable.EnableOptimizations = false;
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //            "~/Scripts/bootstrap.js",
            //            "~/Scripts/respond.js",
            //            //"~/Content/summernote/dist/summernote.js",
            //            //"~/Content/summernote/dist/summernote.min.js",
            //            "~/Content/site.js"
            //          ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/summernote/dist/summernote.css",
                      "~/Scripts/layer-1.8.5/skin/default/layer.css",
                      "~/Content/site.css"
                      //"~/Content/summernote/dist/summernote.css"
                      ));

            bundles.Add(new ScriptBundle("~/Content/main").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/respond.js",
                        "~/Scripts/bootstrap.js",
                        "~/Content/summernote/dist/summernote.js",
                        "~/Scripts/layer-1.8.5/layer.min.js",
                        "~/Scripts/layer-1.8.5/CommonLayer.js",
                        "~/Content/site.js",//业务主要js尽量放在各依赖js后面
                        "~/Scripts/modernizr-*"
                        ));

            #region BBS 和article用
            bundles.Add(new StyleBundle("~/Content/css2").Include(
                    "~/Content/bbscss/discuz.common.css",
                    "~/Content/bbscss/discuz.css",
                    "~/Content/share/dist/css/share.min.css"
                        ));
            bundles.Add(new ScriptBundle("~/Content/main2").Include(
                        "~/Content/share/dist/js/social-share.min.js",
                        "~/Scripts/clipboard.min.js"
                        ));
            #endregion
        }
    }
}
