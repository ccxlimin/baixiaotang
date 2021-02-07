using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmazonBBS.BLL;
using AmazonBBS.Model;
using AmazonBBS.Common;
using Newtonsoft.Json;
using System.IO;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Configuration;

namespace AmazonBBS.Controllers
{
    public class BaseController : Controller, IController
    {
        public AmazonBBSDBContext DB { get; set; }
        private UserBase _userinfo = null;

        #region 用户信息
        /// <summary>
        /// 用户信息
        /// </summary>
        public UserBase UserInfo
        {
            get
            {
                if (_userinfo == null)
                {
                    string cookie = CookieHelper.GetCookie("uinfo");
                    if (string.IsNullOrEmpty(cookie))
                    {
                        return null;
                    }
                    string user = DESEncryptHelper.Decrypt(cookie, ConfigHelper.AppSettings("LoginDesKey"));
                    _userinfo = (UserBase)JsonConvert.DeserializeObject<UserBase>(user);
                    //判断登录时间
                    var now = DateTime.Now;
                    if (_userinfo.LoginTime.Value.Date != now.Date)
                    {
                        _userinfo.LoginTime = now;
                        var userinfo = DB.UserBase.FirstOrDefault(a => a.IsDelete == 0 && a.UserID == _userinfo.UserID);
                        userinfo.LoginTime = now;
                        DB.SaveChanges();
                        SetLogin(_userinfo);
                    }
                }
                return _userinfo;
            }
            set { _userinfo = value; }
        }
        #endregion

        #region 用户ID
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserID
        {
            get
            {
                UserBase u = UserInfo;
                if (u == null)
                {
                    return 0;
                }
                else
                {
                    return u.UserID;
                }
            }
        }
        #endregion

        #region 用户是否登录
        /// <summary>
        /// 用户是否登录
        /// </summary>
        protected bool IsLogin
        {
            get { return UserInfo != null; }
        }
        #endregion

        #region 今日是否已签到
        /// <summary>
        /// 今日是否已签到
        /// </summary>
        protected bool IsSign
        {
            get
            {
                if (!IsLogin) return false;

                var issigned = Session["isSigned"];

                if (issigned == null)
                {
                    Session["isSigned"] = ScoreCoinLogBLL.Instance.GetSignStatus(UserID);
                    return (bool)Session["isSigned"];
                }
                else
                {
                    return (bool)issigned;
                }
            }
            set
            {
                Session["isSigned"] = value;
            }
        }

        #endregion

        #region 保存登录信息
        /// <summary>
        /// 保存登录信息
        /// </summary>
        /// <param name="m"></param>
        protected void SetLogin(UserBase m)
        {
            _userinfo = m;
            string keyMsg = DESEncryptHelper.Encrypt("{0}".FormatWith(JsonConvert.SerializeObject(m)), ConfigHelper.AppSettings("LoginDesKey"));
            //ErrorBLL.Instance.Log("保存登录信息：{0}".FormatWith(keyMsg));
            CookieHelper.CreateCookie("uinfo", keyMsg);
        }
        #endregion

        #region 得到request值
        /// <summary>
        /// 得到request值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected T GetRequest<T>(string name, T defaultValue = default(T))
        {
            if (name.Length == 0)
            {
                return defaultValue;
            }
            string request = Request[name];
            if (string.IsNullOrEmpty(request))
            {
                return defaultValue;
            }
            else
            {
                return (T)Convert.ChangeType(request, typeof(T));
            }
        }

        protected T GetRequest<T>(string name)
        {
            if (name.Length == 0)
            {
                return default(T);
            }
            string request = Request[name];
            if (string.IsNullOrEmpty(request))
            {
                return default(T);
            }
            else
            {
                return (T)Convert.ChangeType(request, typeof(T));
            }
        }

        protected string GetRequest(string name)
        {
            if (name.Length == 0)
            {
                return string.Empty;
            }
            string request = Request[name];
            if (string.IsNullOrEmpty(request))
            {
                return string.Empty;
            }
            else
            {
                return request;
            }
        }
        #endregion

        #region 初始化查询页
        protected Paging InitPage(int defaultPageSize = 10, int defaultPageIndex = 1, int defaultMaxPageSize = 30)
        {
            int pi = GetRequest("pi", 1);
            int ps = GetRequest("ps", defaultPageSize);

            return new Paging()
            {
                PageIndex = pi,//页码
                PageSize = ps > defaultMaxPageSize ? defaultMaxPageSize : ps//页大小
            };
        }
        #endregion

        #region 操作结果集
        protected ActionResult Result(ResultInfo ri)
        {
            return Json(ri, JsonRequestBehavior.AllowGet);
        }
        protected ActionResult Result<T>(ResultInfo<T> ri)
        {
            return Json(ri, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 上传图片
        public ResultInfo UpLoadImg(string requestName, string savePath, ImgExtTypeEnum imgExt = ImgExtTypeEnum.jpg, Action<Func<ResultInfo>, ResultInfo> beforeSaveFile = null, string filename = null, bool onlyThumbImg = false, bool isNeedFile = true)
        {
            ResultInfo ri = new ResultInfo();
            HttpPostedFileBase upfile = Request.Files[requestName];
            if (isNeedFile)
            {
                if (upfile == null)
                {
                    ri.Msg = "请选择要上传的文件";
                }
                else
                {
                    string ext = Path.GetExtension(upfile.FileName);
                    if (!CheckFileExt(ext))
                    {
                        ri.Msg = $"不允许上传{ext}类型的文件！";
                    }
                    else
                    {
                        int maxlength = Convert.ToInt32(ConfigHelper.AppSettings("UploadFileMax"));//最大上传大小/M
                        if (upfile.ContentLength > maxlength * 1024 * 1024)
                        {
                            ri.Msg = "上传文件最大只能是{0}M".FormatWith(maxlength);
                        }
                        else
                        {
                            if (beforeSaveFile != null)
                            {
                                beforeSaveFile(() =>
                                {
                                    SaveFile(ri, upfile, savePath, ext, imgExt, filename, onlyThumbImg);
                                    return ri;
                                }, ri);
                            }
                            else
                            {
                                SaveFile(ri, upfile, savePath, ext, imgExt, filename, onlyThumbImg);
                            }
                        }
                    }
                }
            }
            else
            {
                beforeSaveFile(null, ri);
            }
            return ri;
        }

        private void SaveFile(ResultInfo ri, HttpPostedFileBase upfile, string savePath, string ext, ImgExtTypeEnum imgExt, string filename, bool onlyThumbImg)
        {
            string localPath = UploadHelper.GetMapPath(savePath);
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }

            ext = ext.ToLower().Contains("gif") ? ext : "." + imgExt.ToString();

            //如果文件名为空，则重新命名 并添加上扩展名
            if (filename.IsNullOrEmpty())
            {
                filename = DateTimeHelper.GetToday(9) + "_" + UserID + ext;
            }
            else
            {
                if (filename.IndexOf(".") == -1)
                {
                    //补全扩展名
                    filename += ext;
                }
            }
            if (onlyThumbImg)
            {
                using (Image image = Image.FromStream(upfile.InputStream))
                {
                    string thnumFilename = "thumb_{0}".FormatWith(filename);
                    string thumbFullName = localPath + "/" + thnumFilename;
                    GetThumbnail(image, 100, 100).Save(thumbFullName);
                    ri.Ok = true;
                    ri.Url = savePath + "/" + thnumFilename;
                }
            }
            else
            {
                string fullFilePath = localPath + "/" + filename;
                upfile.SaveAs(fullFilePath);
                string picpath = savePath + "/" + filename;

                ri.Ok = true;
                ri.Url = picpath;
            }
        }

        #region 为图片生成缩略图
        /// <summary>
        /// 为图片生成缩略图  
        /// </summary>
        /// <param name="phyPath">原图片的路径</param>
        /// <param name="width">缩略图宽</param>
        /// <param name="height">缩略图高</param>
        /// <returns></returns>
        private Bitmap GetThumbnail(Image image, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            //从Bitmap创建一个Graphics
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                //设置 
                gr.SmoothingMode = SmoothingMode.HighQuality;
                //下面这个也设成高质量
                gr.CompositingQuality = CompositingQuality.HighQuality;
                //下面这个设成High
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //把原始图像绘制成上面所设置宽高的缩小图
                Rectangle rectDestination = new Rectangle(0, 0, width, height);
                gr.DrawImage(image, new Rectangle(0, 0, 100, 100), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            return bmp;
        }
        #endregion

        #region 上传相关方法
        /// <summary>
        /// 获取上传目录的相对路径
        /// </summary>
        /// <returns></returns>
        private string GetUploadPath(string filePath, string extra)
        {
            if (!string.IsNullOrEmpty(extra))
            {
                filePath = $"{filePath}/{extra}";
            }
            return filePath + "/";
        }

        /// <summary>
        /// 检查文件扩展名是否合法
        /// </summary>
        /// <param name="fileExt"></param>
        /// <returns></returns>
        private bool CheckFileExt(string fileExt)
        {
            //检查危险文件
            string[] excExt = { "asp", "aspx", "ashx", "asa", "asmx", "asax", "php", "jsp", "htm", "html" };
            for (int i = 0; i < excExt.Length; i++)
            {
                if (excExt[i].ToLower() == fileExt.ToLower())
                {
                    return false;
                }
            }
            //检查合法文件
            return CheckUploadFileType(fileExt);
        }
        private static bool CheckUploadFileType(string fileName)
        {
            //允许的文件类型，如"rar,txt"
            var allowUploadFileType = ConfigHelper.AppSettings("AllowUploadFileType");

            //得到后缀，如rar
            var fileType = fileName.Substring(fileName.LastIndexOf(".") + 1);
            var typeList = allowUploadFileType.Split(',');

            return typeList.Contains(fileType) ? true : false;
        }
        #endregion
        #endregion

        #region 上传附件(非图片)
        public ResultInfo<AttachMent> UpLoadFile(HttpPostedFileBase upfile, string savePath, bool isNeedFile = false, bool needValidatFile = true, bool rename = true)
        {
            var ri = new ResultInfo<AttachMent>();
            if (upfile != null)
            {
                string fileExt = Path.GetExtension(upfile.FileName).ToLower();//文件扩展名
                int fileSize = upfile.ContentLength;//获取文件的大小，以字节为单位
                string fileName = Path.GetFileNameWithoutExtension(upfile.FileName);//文件名
                string newFileName = "{0}{1}".FormatWith(rename ? DateTimeHelper.GetToday(9) : fileName, fileExt);//是否重命名
                string upLoadPath = GetUploadPath(savePath, UserID.ToString());//上传目录的相对路径
                string fullUploadPath = UploadHelper.GetMapPath(upLoadPath);//上传目录的物理路径
                string newFilePath = upLoadPath + newFileName;//上传后的路径

                string ext = ".txt,.doc,.docx,.ppt,.pptx,.xls,.xlsx";
                if (needValidatFile && !ext.Contains(fileExt))
                {
                    ri.Msg = $"只能上传格式为{ext}的附件";
                }
                else
                {
                    //if (upfile.ContentLength > 2 * 1024 * 1024)
                    //{
                    //ri.Msg = "附件限制在2M以内！";
                    //}
                    //else
                    //{
                    //检查上传的物理路径是否存在，不存在则创建
                    if (!Directory.Exists(fullUploadPath))
                    {
                        Directory.CreateDirectory(fullUploadPath);
                    }
                    //保存文件
                    upfile.SaveAs(fullUploadPath + newFileName);
                    //处理完毕，返回JOSN格式的文件信息
                    ri.Ok = true;
                    ri.Msg = "上传文件成功！";
                    ri.Url = newFilePath;

                    string _filesize = fileSize < 1024 ? fileSize.ToString() + "B" : fileSize / 1024 < 1024 ? fileSize / 1024 + "KB" : fileSize / 1024 / 1024 + "MB";

                    ri.Data = new AttachMent()
                    {
                        FilePath = newFilePath,
                        FileName = newFileName,
                        FileSize = _filesize
                    };
                    //}
                }
            }
            else
            {
                if (isNeedFile)
                {
                    ri.Msg = "请添加附件";
                }
                else
                {
                    ri.Ok = true;
                }
            }
            return ri;
        }
        #endregion

        #region 异常处理
        protected override void OnException(ExceptionContext filterContext)
        {
            //标记异常已处理
            filterContext.ExceptionHandled = true;
            if (TranHelper != null && Tran != null)
            {
                TranHelper.RollBack();
            }

            #region 日志记录
            ErrorBLL.Instance.Log(filterContext.Exception.ToString());
            #endregion

            // 跳转到错误页
            //TempData["message"] = "异常：" + filterContext.Exception.Message + ";异常Trace:" + filterContext.Exception.StackTrace;
            //bool gettitle = Request.Url.AbsoluteUri.ToLower().Contains("gettitle=1");
            //if (!gettitle)
            //    filterContext.Result = new RedirectResult(Url.Action("Index", "ErrorPages"));
        }
        #endregion

        #region 事务
        private TranHelper TranHelper = null;
        /// <summary>
        /// 事务
        /// </summary>
        protected SqlTransaction Tran = null;

        protected SqlConnection Conn = null;
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        protected void BeginTran()
        {
            TranHelper = new TranHelper();
            Tran = TranHelper.Tran;
            Conn = TranHelper.Conn;
        }

        protected void BeginTranEF()
        {
            TranHelper = new TranHelper(ConfigurationManager.ConnectionStrings["AmazonBBSDBContext"].ConnectionString);
            Tran = TranHelper.Tran;
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        protected void RollBack()
        {
            if (TranHelper != null)
            {
                TranHelper.RollBack();
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        protected void Commit()
        {
            if (TranHelper != null)
            {
                TranHelper.Commit();
            }
        }
        #endregion

        #region 获取当前主域名
        /// <summary>
        /// 获取当前主域名
        /// </summary>
        protected string GetDomainName
        {
            get
            {
                Uri uri = Request.Url;
                return uri.AbsoluteUri.Replace(uri.PathAndQuery, string.Empty);
            }
        }
        #endregion
    }
}