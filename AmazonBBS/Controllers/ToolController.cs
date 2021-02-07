using AmazonBBS.BLL;
using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections;
using EntityFramework.Extensions;

namespace AmazonBBS.Controllers
{
    public class ToolController : BaseController
    {
        #region 问题页浏览量
        [HttpPost]
        public void PV(long id)
        {
            if (id > 0)
            {
                //判断ID问题是否存在 
                if (QuestionBLL.Instance.GetItem(id) != null)
                {
                    QuestionBLL.Instance.PVUpdate(id);
                }
            }
        }
        #endregion

        #region 随机数
        /// <summary>
        /// 随机数
        /// </summary>
        /// <returns></returns>
        protected int RandCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }
        #endregion

        #region 发送邮件
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="from">来源：true：第三方绑定注册 false:常规注册</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RegistEmail(string mail, bool from)
        {
            return Result(SendEmail(mail, from ? 2 : 1));
        }

        [HttpPost]
        public ActionResult ShareRegistEmail(string mail)
        {
            return Result(SendEmail(mail, 3));
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="fromType">注册类型（1常规注册 ）</param>
        /// <returns></returns>
        private ResultInfo SendEmail(string mail, int fromType)
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                if (!UserBaseBLL.Instance.ExistAccount(mail))
                {
                    string sessionEmailCodeName = string.Empty;
                    string sessionEmailName = string.Empty;
                    if (fromType == 1)
                    {
                        sessionEmailCodeName = "SendEmailCode";
                        sessionEmailName = "RegisterEmail";
                    }
                    else if (fromType == 2)
                    {
                        sessionEmailCodeName = "SendOauthEmailCode";
                        sessionEmailName = "RegistOauthEmail";
                    }
                    else if (fromType == 3)
                    {
                        sessionEmailCodeName = "SendShareEmailCode";
                        sessionEmailName = "RegistShareEmail";
                    }
                    string role = "^[a-z0-9]+([._\\-]*[a-z0-9])*@([a-z0-9]+[-a-z0-9]*[a-z0-9]+.){1,63}[a-z0-9]+$";
                    if (Regex.IsMatch(mail, role))
                    {
                        int code = RandCode();
                        Session[sessionEmailCodeName] = code;
                        #region MyRegion
                        //bool ok = new EmailHelper().Send("邮箱验证码(新用户注册-百晓堂)", $@"百晓堂:尊敬的用户，你的验证码为[{code}]，百晓堂官方不会以任何形式向您索要验证码，不要告诉任何人", new List<EmailHelper.RecipientClass>()
                        //{
                        //    new EmailHelper.RecipientClass()
                        //    {
                        //         Recipient = mail,
                        //         RecipientName = mail
                        //    }
                        //}); 
                        #endregion
                        #region MyRegion
                        string[] sendEmailAccount = ConfigHelper.AppSettings("Emailor").Split('|');
                        EmailHelper email = new EmailHelper();

                        email.From = sendEmailAccount[0];
                        email.FromName = "百晓堂";

                        //string[] receiveMails = ConfigHelper.AppSettings("ReceiveEmail").Split(',');

                        email.Recipients = new List<EmailHelper.RecipientClass>();
                        email.Recipients.Add(new EmailHelper.RecipientClass()
                        {
                            Recipient = mail,
                            RecipientName = mail
                        });

                        email.Subject = "邮箱验证码(新用户注册-百晓堂)";
                        email.Body = $@"百晓堂:尊敬的用户，您的验证码为[{code}]，百晓堂官方不会以任何形式向您索要验证码，不要告诉任何人！";
                        email.IsBodyHtml = false;
                        //email.ServerHost = "smtp.{0}".FormatWith(sendEmailAccount[0].Split('@')[1]);
                        string emailhost = ConfigHelper.AppSettings("EmailHost");
                        email.ServerHost = emailhost.IsNotNullOrEmpty() ? emailhost : "smtp.{0}".FormatWith(sendEmailAccount[0].Split('@')[1]);
                        email.ServerPort = 465;
                        email.Username = sendEmailAccount[0];
                        email.Password = sendEmailAccount[1];

                        bool ok = email.Send2();
                        #endregion
                        if (ok)
                        {
                            ri.Ok = true;
                            ri.Msg = "验证码发送成功";
                            //保存注册的邮箱
                            Session[sessionEmailName] = mail;
                        }
                        else
                        {
                            SessionHelper.Remove(sessionEmailCodeName);
                            ri.Msg = "验证码发送失败";
                        }
                    }
                    else
                    {
                        ri.Msg = "请确认邮箱格式正确";
                    }
                }
                else
                {
                    ri.Msg = "邮箱已被注册!";
                }
            }
            catch (Exception e)
            {
                ri.Msg = e.Message;
            }
            return ri;
        }
        #endregion

        #region 上传
        private readonly string headurlPath = "/Content/U/Head";
        private readonly string vipheadurlPath = "/Content/U/Head/VIP";
        private readonly string QuestionPath = "/Content/U/Ques";
        private readonly string StudyPath = "/Content/Study";
        private readonly string giftPath = "/Content/U/Gift";
        private readonly string icoPath = "/";
        private readonly string jinghuaPath = "/Content/U/Site";
        private readonly string aboutPath = "/Content/About";
        private readonly string newsPath = "/Content/News";
        private readonly string commentPath = "/Content/Comment";
        private readonly string adPath = "/Content/AD";
        private readonly string userVPath = "/Content/U/UserV";

        private readonly string activeTemplatePath = "/Content/Active/ActiveTemplate";
        private readonly string activeFinalImgPath = "/Content/Active/ActiveFinalImg";

        private readonly string studysharePath = "/Content/site/studyshare/base";//上传学习分享图片目录

        #region 上传学习分享图片
        [IsMaster]
        [HttpPost]
        public ActionResult studyshare()
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                HttpPostedFileBase upfile = Request.Files["file"];
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
                        string localPath = UploadHelper.GetMapPath("/" + studysharePath);
                        if (!Directory.Exists(localPath))
                        {
                            Directory.CreateDirectory(localPath);
                        }

                        string filename = $"studyshare{DateTime.Now.ToString(9)}.png";
                        upfile.SaveAs(localPath + "/" + filename);

                        DB.StudyShareImgSet.Where(a => a.IsUse).Update(a => new StudyShareImgSet { IsUse = false });

                        DB.StudyShareImgSet.Add(new StudyShareImgSet
                        {
                            CreateTime = DateTime.Now,
                            IsUse = true,
                            Url = studysharePath + "/" + filename
                        });
                        DB.SaveChanges();

                        ri.Ok = true;
                    }
                }
            }
            catch (Exception e)
            {
                ri.Ok = false;
                ri.Msg = "上传图片失败";
            }
            return Result(ri);
        }
        #endregion

        #region 活动编辑时最终确定性的图片
        //[IsMaster]
        /// <summary>
        /// 活动编辑时最终确定性的图片
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult ActiveFinalImg()
        {
            return Result(UpLoadFile(activeFinalImgPath, ImgExtTypeEnum.jpg));
        }
        #endregion

        #region 活动临时图片
        //[IsMaster]
        /// <summary>
        /// 活动临时图片
        /// </summary>
        /// <returns></returns>
        [LOGIN]
        [HttpPost]
        public ActionResult ActiveTemplateImg()
        {
            return Result(UpLoadFile(activeTemplatePath, ImgExtTypeEnum.jpg));
        }

        #endregion

        #region 上传用户认证logo
        /// <summary>
        /// 上传用户认证logo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [IsMaster]
        public ActionResult UserV(int id)
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                HttpPostedFileBase upfile = Request.Files["file"];
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
                            string localPath = UploadHelper.GetMapPath("/" + userVPath);
                            if (!Directory.Exists(localPath))
                            {
                                Directory.CreateDirectory(localPath);
                            }

                            string filename = id.ToString() + ".png";
                            upfile.SaveAs(localPath + "/" + filename);
                            ri.Ok = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ri.Ok = false;
                ri.Msg = "上传图片失败";
            }
            return Result(ri);
        }
        #endregion

        #region 精华LOGO
        /// <summary>
        /// 精华LOGO
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [IsMaster]
        [HttpPost]
        public ActionResult Jinghua(int id)
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                HttpPostedFileBase upfile = Request.Files["file"];
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
                            string localPath = UploadHelper.GetMapPath("/" + jinghuaPath);
                            if (!Directory.Exists(localPath))
                            {
                                Directory.CreateDirectory(localPath);
                            }

                            string filename = id == 1 ? "jinghuatie.png" : "rementie.png";
                            upfile.SaveAs(localPath + "/" + filename);
                            ri.Ok = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ri.Ok = false;
                ri.Msg = "上传图片失败";
            }
            return Result(ri);
        }
        #endregion

        #region 课程顾问形象
        /// <summary>
        /// 课程顾问形象
        /// </summary>
        /// <returns></returns>
        [IsMaster]
        [HttpPost]
        public ActionResult KEGUID()
        {
            ResultInfo ri = new ResultInfo();
            HttpPostedFileBase upfile = Request.Files["file"];
            if (upfile == null)
            {
                ri.Msg = "请选择要上传的文件";
            }
            else
            {
                string fullPath = "/Content/img/kechengguid/invitehead.jpg";
                decimal maxlength = 0.2M;
                if (upfile.ContentLength > maxlength * 1024 * 1024)
                {
                    ri.Msg = "上传文件最大只能是{0}M".FormatWith(maxlength);
                }
                else
                {
                    upfile.SaveAs(UploadHelper.GetMapPath(fullPath));
                    ri.Ok = true;
                }
            }
            return Result(ri);
        }
        #endregion

        #region 网站ICO
        [HttpPost]
        [IsMaster]
        public ActionResult ICO(int type)
        {
            ResultInfo ri = new ResultInfo();

            HttpPostedFileBase upfile = Request.Files["file"];
            if (upfile == null)
            {
                ri.Msg = "请选择要上传的文件";
            }
            else
            {
                string fullPath = string.Empty;
                switch (type)
                {
                    case 1: fullPath = "/favicon.ico"; break;
                    case 2: fullPath = "/Content/U/Site/levelname.png"; break;
                    case 3: fullPath = "/Content/U/Site/onlylevelname.png"; break;
                    case 4: fullPath = "/Content/U/Site/master.png"; break;
                }
                int maxlength = Convert.ToInt32(ConfigHelper.AppSettings("UploadFileMax"));//最大上传大小/M
                if (upfile.ContentLength > maxlength * 1024 * 1024)
                {
                    ri.Msg = "上传文件最大只能是{0}M".FormatWith(maxlength);
                }
                else
                {
                    upfile.SaveAs(UploadHelper.GetMapPath(fullPath));
                    ri.Ok = true;
                }
            }
            return Result(ri);
        }
        #endregion

        #region 上传头像
        /// <summary>
        /// 上传头像
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [LOGIN]
        public ActionResult UploadHead()
        {
            ResultInfo result = UpLoadFile(headurlPath, ImgExtTypeEnum.jpg, true);
            if (result.Ok)
            {
                var user = UserBaseBLL.Instance.GetUserInfo(UserID);

                if (user.HeadUrl.IsNotNullOrEmpty())
                {
                    string oldHeadUrl = user.HeadUrl;
                    string oldThumbHeadUrl = oldHeadUrl.Replace(UserID.ToString(), "thumb/{0}".FormatWith(UserID));
                    UploadHelper.DeleteUpFile(oldHeadUrl);
                    UploadHelper.DeleteUpFile(oldThumbHeadUrl);
                }
                //更新头像
                result.Ok = UserBaseBLL.Instance.UploadHeadUrl(result.Data.ToString(), UserID);
            }

            var model = UserBaseBLL.Instance.GetModel(UserInfo.UserID);
            SetLogin(model);

            return Result(result);
        }
        #endregion

        #region VIP头像
        [HttpPost]
        [LOGIN]
        public ActionResult UploadVipHead()
        {
            ResultInfo ri = UpLoadFile(vipheadurlPath, ImgExtTypeEnum.gif, false, false);
            if (ri.Ok)
            {
                //管理员可以直接免审核
                if (UserBaseBLL.Instance.IsMaster)
                {
                    var model = UserBaseBLL.Instance.GetModel(UserInfo.UserID);
                    if (model.HeadUrl.IsNotNullOrEmpty())
                    {
                        string oldHeadUrl = model.HeadUrl;
                        string oldThumbHeadUrl = oldHeadUrl.Replace(UserID.ToString(), "thumb/{0}".FormatWith(UserID));
                        UploadHelper.DeleteUpFile(oldHeadUrl);
                        UploadHelper.DeleteUpFile(oldThumbHeadUrl);
                    }
                    //更新头像
                    UserBaseBLL.Instance.UploadHeadUrl(ri.Url, UserID);
                    model.HeadUrl = ri.Url;
                    SetLogin(model);
                }
                else
                {
                    //待审核
                    VIPHead model = new VIPHead()
                    {
                        CreateTime = DateTime.Now,
                        HeadUrl = ri.Url,
                        IsChecked = 0,
                        IsDelete = 0,
                        UserID = UserID,
                    };
                    ri = new ResultInfo();
                    if (VIPHeadBLL.Instance.Add(model) > 0)
                    {
                        ri.Ok = true;
                    }
                }
            }
            return Result(ri);
        }
        #endregion

        #region 广告图片
        [HttpPost]
        [IsMaster]
        public bool ADIMG()
        {
            return UpLoadFile(adPath, ImgExtTypeEnum.jpg).Ok;
        }
        #endregion

        #region 礼物、数据、课程 图片
        [HttpPost]
        [IsMaster]
        public ActionResult UploadGiftImg()
        {
            ResultInfo ri = UpLoadFile(giftPath, ImgExtTypeEnum.jpg);
            return Result(ri);
        }
        #endregion

        #region 上传图片
        [LOGIN]
        [HttpPost]
        public ActionResult UpLoadIMG()
        {
            ResultInfo ri = UpLoadFile(QuestionPath, ImgExtTypeEnum.jpg);
            return Result(ri);
        }
        #endregion

        #region 上传课程图片
        public ActionResult UploadStudyImage()
        {
            ResultInfo ri = UpLoadFile(StudyPath, ImgExtTypeEnum.jpg);
            return Result(ri);
        }
        #endregion

        #region 关于主页 上传图片
        [LOGIN]
        [HttpPost]
        public ActionResult UpLoadAboutIMG()
        {
            return Result(UpLoadFile(aboutPath, ImgExtTypeEnum.jpg));
        }
        #endregion

        #region 上传新闻图片
        [LOGIN]
        [HttpPost]
        public ActionResult UpLoadNewsIMG()
        {
            return Result(UpLoadFile(newsPath, ImgExtTypeEnum.jpg));
        }
        #endregion

        #region 上传评论图片
        [LOGIN]
        [HttpPost]
        public ActionResult UpLoadCommentIMG()
        {
            return Result(UpLoadFile(commentPath, ImgExtTypeEnum.jpg));
        }
        #endregion

        #region 上传文件处理
        private ResultInfo UpLoadFile(string uploadPath, ImgExtTypeEnum imgExt, bool isThumbnail = false, bool size1M = true)
        {
            ResultInfo ri = new ResultInfo();
            string delFilePath = Request.Params["DelFilePath"];
            string type = Request.Params["itype"];
            HttpPostedFileBase upfile = Request.Files["file"];

            if (upfile == null)
            {
                ri.Msg = "请选择要上传的文件";
                return ri;
            }

            int aid = IsLogin ? (int)UserID : 0;
            ri = FileSaveAs(upfile, imgExt, isThumbnail, aid, uploadPath, type, size1M);

            //删除已存在的旧文件，旧文件不为空且应是上次文件，防止跨目录删除
            if (!string.IsNullOrEmpty(delFilePath) && delFilePath.IndexOf("../", StringComparison.Ordinal) == -1)
            {
                UploadHelper.DeleteUpFile(delFilePath);
            }

            return ri;
        }

        #endregion

        #region 图片上传辅助方法

        /// <summary>
        /// 文件上传方法
        /// </summary>
        /// <param name="upfile">文件流</param>
        /// <param name="isthumbnail">是否缩略图</param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ResultInfo FileSaveAs(HttpPostedFileBase upfile, ImgExtTypeEnum imgExt, bool isthumbnail, int aid, string uploadPath, string itype, bool size1M = true)
        {
            ResultInfo ri = new ResultInfo();
            string fileExt = Path.GetExtension(upfile.FileName);//文件扩展名

            //检查文件扩展名是否合法
            if (!CheckFileExt(fileExt))
            {
                ri.Msg = $"不允许上传{fileExt}类型的文件！";
                return ri;
            }
            //检查文件大小是否合法
            //if (size1M)
            //{
            //    int maxlength = Convert.ToInt32(ConfigHelper.AppSettings("UploadFileMax"));//最大上传大小/M
            //    if (!UserBaseBLL.Instance.IsMaster && upfile.ContentLength > maxlength * 1024 * 1024)
            //    {
            //        ri.Msg = "上传文件最大只能是{0}M".FormatWith(maxlength);
            //        return ri;
            //    }
            //}

            int fileSize = upfile.ContentLength;//获取文件的大小，以字节为单位
            string fileName = Path.GetFileNameWithoutExtension(upfile.FileName);//文件名
            string newFileName = "{0}{1}".FormatWith(DateTimeHelper.GetToday(9), fileExt.Contains(ImgExtTypeEnum.gif.ToString()) ? fileExt : "." + imgExt.ToString());//随机生成新的文件名
            string upLoadPath = GetUploadPath(uploadPath, aid.ToString());//上传目录的相对路径
            string fullUploadPath = UploadHelper.GetMapPath(upLoadPath);//上传目录的物理路径
            string newFilePath = upLoadPath + newFileName;//上传后的路径

            //检查上传的物理路径是否存在，不存在则创建
            if (!Directory.Exists(fullUploadPath))
            {
                Directory.CreateDirectory(fullUploadPath);
            }
            //保存文件
            upfile.SaveAs(fullUploadPath + newFileName);

            //生成缩略图
            string thumbFullName = string.Empty;
            if (isthumbnail)
            {
                using (Image image = Image.FromStream(upfile.InputStream))
                {
                    string thumbPath = GetUploadPath(GetUploadPath(uploadPath, "thumb"), aid.ToString());
                    string thumbFullPath = UploadHelper.GetMapPath(thumbPath);
                    if (!Directory.Exists(thumbFullPath))
                    {
                        Directory.CreateDirectory(thumbFullPath);
                    }
                    thumbFullName = thumbPath + "thumb_{0}".FormatWith(newFileName);
                    GetThumbnail(image, 100, 100).Save(Server.MapPath(thumbFullName));
                }
            }

            //处理完毕，返回JOSN格式的文件信息
            ri.Ok = true;
            ri.Msg = "上传文件成功！";
            ri.Url = newFilePath;
            ri.Data = thumbFullName;
            return ri;
            //return new { status = 1, msg = "上传文件成功！", path = newFilePath, thumbName = thumbFullName, size = fileSize, ext = fileExt };
        }

        /// <summary>
        /// 为图片生成缩略图  
        /// </summary>
        /// <param name="phyPath">原图片的路径</param>
        /// <param name="width">缩略图宽</param>
        /// <param name="height">缩略图高</param>
        /// <returns></returns>
        public Bitmap GetThumbnail(Image image, int width, int height)
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

        /// <summary>
        /// 检查后缀是否合法
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static bool CheckUploadFileType(string fileName)
        {
            //允许的文件类型，如"rar,txt"
            var allowUploadFileType = ConfigHelper.AppSettings("AllowUploadFileType");

            //得到后缀，如rar
            var fileType = fileName.Substring(fileName.LastIndexOf(".") + 1);
            var typeList = allowUploadFileType.Split(',');

            return typeList.Contains(fileType) ? true : false;
        }

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
        /// 检查文件大小是否合法
        /// </summary>
        /// <param name="fileExt">文件扩展名，不含“.”</param>
        /// <param name="fileSize">文件大小(B)</param>
        private bool CheckFileSize(string fileExt, int fileSize)
        {
            if (IsImage(fileExt))//如果是图片文件,最大上传大小为1M
            {
                if (fileSize > 1024 * 1024)
                {
                    return false;
                }
            }
            else//其他文件最大上传文件大小为10M
            {
                if (fileSize > 10240 * 1024)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否为图片文件
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        private bool IsImage(string _fileExt)
        {
            ArrayList al = new ArrayList { "bmp", "jpeg", "jpg", "gif", "png" };
            return al.Contains(_fileExt.ToLower());
        }
        #endregion
        #endregion

        #region 图形验证码 
        public ActionResult VerificationImage(string t)
        {
            var codeHelper = new CodeHelper(4);
            string code = codeHelper.GenerateCode();
            Session["ForgetPWD"] = code;
            //Response.Cookies.Add(new HttpCookie("VerificationCode", code));
            MemoryStream ms = codeHelper.GeneratePicture(code);
            return File(ms.ToArray(), "image/jpeg");
        }

        public ActionResult VerificationExchangeImage(string t)
        {
            var codeHelper = new CodeHelper(4);
            string code = codeHelper.GenerateCode();
            Session["ExchangePWD"] = code;
            //Response.Cookies.Add(new HttpCookie("VerificationCode", code));
            MemoryStream ms = codeHelper.GeneratePicture(code);
            return File(ms.ToArray(), "image/jpeg");
        }
        #endregion
    }
}