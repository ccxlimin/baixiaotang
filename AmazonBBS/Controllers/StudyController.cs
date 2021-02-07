using AmazonBBS.Model;
using AmazonBBS.Common;
using AmazonBBS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityFramework.Extensions;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AmazonBBS.Controllers
{
    [LOGIN]
    public class StudyController : BaseController
    {
        public ActionResult Index()
        {
            if (IsSign)
            {
                //判断是否专属会员
                if (DB.UserExt.FirstOrDefault(a => a.UserID == UserID).OnlyLevelName.HasValue)
                {
                    var userStudyCenterVM = DB.StudyUnit.Where(a => a.IsDelete == 0).OrderBy(a => a.SortIndex).Select(a => new UserStudyCenterVM
                    {
                        StudyUnit = a,
                        StudyClasses = DB.StudyClass.Where(c => c.StudyUnitId == a.StudyUnitId).OrderBy(c => c.SortIndex).Select(c => new StudyClassVM
                        {
                            StudyClass = c,
                            StudyStatus = DB.UserStudy.FirstOrDefault(ustudy => ustudy.StudyUnitId == a.StudyUnitId && ustudy.StudyClassId == c.StudyClassId && ustudy.UserID == UserID).IsStudyed
                        }).ToList()
                    }).ToList();

                    return View(userStudyCenterVM);
                }
                else
                {
                    return Redirect("/");
                }
            }
            else
            {
                return Redirect("/");
            }
        }

        public ActionResult Detail(Guid id)
        {
            if (id != Guid.Empty)
            {
                if (DB.UserExt.FirstOrDefault(a => a.UserID == UserID).OnlyLevelName.HasValue)
                {
                    var c = DB.StudyClass.FirstOrDefault(a => a.StudyClassId == id);
                    if (c == null)
                    {
                        return RedirectToAction("index", "study");
                    }
                    else
                    {
                        return View(c);
                    }
                }
                else
                {
                    return Redirect("/");
                }
            }
            else
            {
                return RedirectToAction("index", "study");
            }
        }

        [HttpPost]
        public ActionResult Study(Guid id)
        {
            ResultInfo ri = new ResultInfo();

            if (IsSign)
            {
                if (id != Guid.Empty)
                {
                    if (DB.UserExt.FirstOrDefault(a => a.UserID == UserID).OnlyLevelName.HasValue)
                    {
                        var c = DB.StudyClass.FirstOrDefault(a => a.StudyClassId == id);
                        if (c != null)
                        {
                            var unit = DB.StudyUnit.FirstOrDefault(a => a.StudyUnitId == c.StudyUnitId);
                            if (unit != null)
                            {
                                //是否在学习中
                                var studyInfo = DB.UserStudy.FirstOrDefault(a => a.StudyClassId == id && a.StudyUnitId == c.StudyUnitId && a.UserID == UserID);
                                if (studyInfo == null || !studyInfo.IsStudyed)
                                {
                                    DB.UserStudy.Add(new UserStudy
                                    {
                                        CreateTime = DateTime.Now,
                                        IsStudyed = false,
                                        StudyClassId = id,
                                        StudyUnitId = c.StudyUnitId,
                                        UserID = UserID,
                                        UserStudyId = Guid.NewGuid(),
                                    });
                                }
                                else
                                {
                                    studyInfo.IsStudyed = false;
                                }

                                DB.UserStudy.Where(a => a.UserID == UserID && !a.IsStudyed).Update(a => new UserStudy { IsStudyed = true });

                                DB.SaveChanges();
                                ri.Ok = true;
                            }
                            else
                            {
                                ri.Msg = "课程对应章节不存在，请重新学习";
                            }
                        }
                        else
                        {
                            ri.Msg = "课程不存在,请重新学习";
                        }
                    }
                    else
                    {
                        ri.Msg = "你不是标签会员，无法学习";
                    }
                }
                else
                {
                    ri.Msg = "学习失败";
                }
            }
            else
            {
                ri.Msg = "今天尚未签到，请先签到后再学习";
            }
            return Result(ri);
        }

        /// <summary>
        /// 分享
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Share(long id, Guid cid)
        {
            if (id == UserID)
            {
                var userStudy = DB.UserStudy.FirstOrDefault(a => a.UserID == UserID && a.StudyClassId == cid);
                if (userStudy != null)
                {
                    //判断当前学习课程 在 今天是否已生成分享
                    var today = DateTime.Now.Date;

                    var siteImage = DB.StudyShareImgSet.FirstOrDefault(a => a.IsUse);//获取网站后台设置的打卡分享图片
                    string url = siteImage?.Url;
                    string imageUrl = string.Empty;
                    bool canshare = false;
                    if (siteImage == null)
                    {
                        canshare = true;
                    }
                    else
                    {
                        var shareInfo = DB.UserShareLog.FirstOrDefault(a => !a.IsDelete && a.ShareTime == today && a.ClassId == cid && a.UseImage == siteImage.StudyShareImgSetId);
                        if (shareInfo == null)
                        {
                            canshare = true;
                        }
                        else
                        {
                            imageUrl = shareInfo.Url;
                        }
                    }

                    if (canshare)
                    {
                        //创建图
                        //生成图片进行分享
                        var image = CreateImage(url == null ? null : Image.FromFile(UploadHelper.GetMapPath(url)), userStudy);
                        var relativePath = "/content/createImage/{0}".FormatWith(UserID);
                        var localPath = UploadHelper.GetMapPath(relativePath);
                        if (!Directory.Exists(localPath))
                        {
                            Directory.CreateDirectory(localPath);
                        }
                        var imageName = "/share{0}{1}.png".FormatWith(UserID, DateTime.Now.ToString(9));
                        image.Save(localPath + imageName, ImageFormat.Png);
                        ViewBag.Url = relativePath + imageName;

                        //添加建图记录
                        DB.UserShareLog.Add(new UserShareLog
                        {
                            Url = relativePath + imageName,
                            ClassId = cid,
                            CreateTime = DateTime.Now,
                            IsDelete = false,
                            ShareTime = today,
                            UseImage = siteImage?.StudyShareImgSetId
                        });
                        DB.SaveChanges();
                    }
                    else
                    {
                        ViewBag.Url = imageUrl;
                    }
                    return View();
                }
                else
                {
                    return Redirect("/");
                }
            }
            else
            {
                return Redirect("/");
            }
        }

        private Image CreateImage(Image imageSite, UserStudy studyInfo)
        {
            int widthSite = 320, heightSite = 500;
            var coinSource = CoinSourceEnum.Sign.GetHashCode();
            if (imageSite != null)
            {
                //获取默认图片的尺寸宽高
                widthSite = imageSite.Width;
                heightSite = imageSite.Height;
            }
            //获取学习中的课程信息
            var classInfo = DB.StudyClass.FirstOrDefault(a => a.StudyClassId == studyInfo.StudyClassId && a.IsDelete == 0);

            string text1 = "会员：" + UserInfo.UserName.ToString();
            string text2 = "总签到天数：" + DB.ScoreCoinLog.Count(a => a.CoinSource == coinSource && a.UserID == UserID);
            string text3 = "学习内容：《{0}》".FormatWith(classInfo.Name);
            string text4 = "打卡时间：" + studyInfo.CreateTime.ToString(2);

            //int height = 600;

            int fontsize = widthSite / 50;
            Font font = new Font("Arial", fontsize, FontStyle.Regular);

            SolidBrush brush = new SolidBrush(Color.Black);
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            Bitmap image = new Bitmap(widthSite, heightSite);//先根据初始图宽高创建画布
            Graphics g = Graphics.FromImage(image);
            SizeF sizeF = g.MeasureString(text1, font, PointF.Empty, format);//得到文本的宽高
            int width_ = (int)sizeF.Width;
            int height_ = (int)sizeF.Height;
            image.Dispose();
            image = new Bitmap(widthSite, height_ * 4);//重新绘制画布宽高
            g = Graphics.FromImage(image);
            g.Clear(Color.White);//透明

            var _width = (widthSite - sizeF.Width) / 2;
            _width = _width < 180 ? 0 : _width;
            RectangleF rectangleF1 = new RectangleF(_width, 0, widthSite, height_ - 180);
            RectangleF rectangleF2 = new RectangleF(_width, height_ * 1, widthSite, height_ - 180);
            RectangleF rectangleF3 = new RectangleF(_width, height_ * 2, widthSite, height_ - 180);
            RectangleF rectangleF4 = new RectangleF(_width, height_ * 3, widthSite, height_ - 180);
            //绘制图片
            g.DrawString(text1, font, brush, rectangleF1);
            g.DrawString(text2, font, brush, rectangleF2);
            g.DrawString(text3, font, brush, rectangleF3);
            g.DrawString(text4, font, brush, rectangleF4);
            g.Dispose();

            if (imageSite != null)
            {
                #region 开始拼接图片
                var heightNew = heightSite + height_ * 4;
                Bitmap bitMapNew = new Bitmap(widthSite, heightNew);
                Graphics g1 = Graphics.FromImage(bitMapNew);
                g1.FillRectangle(Brushes.White, new Rectangle(0, 0, widthSite, heightNew));
                g1.DrawImage(imageSite, 0, 0, widthSite, heightNew);
                g1.DrawImage(image, 0, heightSite, widthSite, height_ * 4);
                Image imageNew = bitMapNew;
                g1.Dispose();
                #endregion

                return imageNew;
            }
            else
            {
                return image;
            }
        }

        /// <summary>
        /// 学习完成 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult studyfinish(Guid id)
        {
            ResultInfo ri = new ResultInfo();

            if (id != Guid.Empty)
            {
                var model = DB.UserStudy.FirstOrDefault(a => a.StudyClassId == id && a.UserID == UserID);
                if (model != null)
                {
                    var classinfo = DB.StudyClass.FirstOrDefault(a => a.StudyClassId == id);
                    if (classinfo != null)
                    {
                        model.IsStudyed = true;
                        DB.SaveChanges();
                        ri.Ok = true;
                    }
                }
            }

            return Result(ri);
        }
    }
}