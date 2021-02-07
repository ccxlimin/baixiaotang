using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AmazonBBS.BLL;
using AmazonBBS.Model;
using AmazonBBS.Common;
using System.Threading;

namespace AmazonBBS
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutofacConfig.Register();

            //Chat();
            if (Convert.ToBoolean(ConfigHelper.AppSettings("OpenGlobalRun")))
            {
                ReSetCloseAccount();
                NiceAnswer();
            }
        }

        /// <summary>
        /// 处理每个帖子的优秀答题者（按最多点赞来）(---每24小时刷新一次---)
        /// </summary>
        private void NiceAnswer()
        {
            Thread t = new Thread(() =>
            {
                //计划每天凌晨3点执行吧
                DateTime now = DateTime.Now;
                int hours = 0;
                if (now.Hour > 3)
                {
                    hours = 27 - now.Hour;
                    Thread.Sleep(hours * 60 * 60 * 1000);
                }

                var coinSource = CoinSourceEnum.NiceAnswer.GetHashCode();
                while (true)
                {
                    List<_QuestionInfo> questions = QuestionBLL.Instance.GetALLQuestion();
                    questions.ForEach(question =>
                    {
                        if (question.CommentCount > 1)
                        {
                            _Comment niceComment = QuestionBLL.Instance.GetNiceComment(question.QuestionId, question.BestAnswerId);
                            if (niceComment != null)
                            {
                                if (question.NiceAnswerId != niceComment.CommentId)
                                {
                                    TranHelper tranHelper = new TranHelper();
                                    try
                                    {
                                        if (SqlHelper.ExecuteSql(tranHelper.Tran, System.Data.CommandType.Text, "update Question set NiceAnswerId={0} where QuestionId={1}".FormatWith(niceComment.CommentId, question.QuestionId)) > 0)
                                        {
                                            int coin = Convert.ToInt32(question.Coin);
                                            int coinType = Convert.ToInt32(question.CoinType);
                                            long commentUserId = Convert.ToInt64(niceComment.CommentUserID);
                                            if (coinType != 0)
                                            {
                                                //判断是否已赠送过积分
                                                if (!ScoreCoinLogBLL.Instance.HasGiveScore(commentUserId, question.QuestionId, coinType, coinSource, tranHelper.Tran))
                                                {
                                                    //未赠送过，则赠送积分
                                                    if (UserExtBLL.Instance.AddScore(commentUserId, coin * 7 / 10, coinType, tranHelper.Tran))
                                                    {
                                                        if (ScoreCoinLogBLL.Instance.Log(coin * 7 / 10, coinType, CoinSourceEnum.NiceAnswer, commentUserId, niceComment.UserName, tranHelper.Tran, question.QuestionId))
                                                        {
                                                            tranHelper.Commit();
                                                        }
                                                        else
                                                        {
                                                            tranHelper.RollBack();
                                                        }
                                                        tranHelper.Dispose();
                                                    }
                                                    else
                                                    {
                                                        tranHelper.RollBack();
                                                        tranHelper.Dispose();
                                                    }
                                                }
                                                else
                                                {
                                                    tranHelper.RollBack();
                                                    tranHelper.Dispose();
                                                }
                                            }
                                            else
                                            {
                                                tranHelper.Commit();
                                            }
                                        }
                                        else
                                        {
                                            tranHelper.RollBack();
                                            tranHelper.Dispose();
                                        }
                                    }
                                    catch
                                    {
                                        tranHelper.RollBack();
                                        tranHelper.Dispose();
                                    }
                                    finally
                                    {
                                        //Thread.Sleep(1 * 10 * 1000);
                                    }
                                }
                            }
                        }
                    });
                    Thread.Sleep(24 * 60 * 60 * 1000);
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// 重置封禁的账号
        /// </summary>
        private void ReSetCloseAccount()
        {
            #region MyRegion
            //string[] sendEmailAccount = ConfigHelper.AppSettings("Emailor").Split('|');
            //EmailHelper email = new EmailHelper();

            //email.From = sendEmailAccount[0];
            //email.FromName = "百晓堂";

            ////string[] receiveMails = ConfigHelper.AppSettings("ReceiveEmail").Split(',');

            //email.Recipients = new List<EmailHelper.RecipientClass>();
            //email.Recipients.Add(new EmailHelper.RecipientClass()
            //{
            //    Recipient = "1003243618@qq.com",
            //    RecipientName = "Staton"
            //});

            //email.Subject = "Web应用程序开始启动.重置封禁的账号";
            //email.Body = "Web应用程序开始启动.重置封禁的账号";
            //email.IsBodyHtml = false;
            //string emailhost = ConfigHelper.AppSettings("EmailHost");
            //email.ServerHost = emailhost.IsNotNullOrEmpty() ? emailhost : "smtp.{0}".FormatWith(sendEmailAccount[0].Split('@')[1]);
            //email.ServerPort = 465;
            //email.Username = sendEmailAccount[0];
            //email.Password = sendEmailAccount[1];

            //bool ok = email.Send2();
            #endregion
            UserExtBLL ubll = UserExtBLL.Instance;
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    UserExt u = ubll.FindNearCloseAccount();
                    if (u != null)
                    {
                        int time = (int)Convert.ToDateTime(u.CloseTime).Subtract(DateTime.Now).TotalSeconds;
                        if (time > 0)
                        {
                            Thread.Sleep((time + 1) * 1000);
                        }
                        ubll.ReSetCloseAccount((long)u.UserID);
                    }
                    else
                    {
                        Thread.Sleep(10 * 60 * 1000);
                    }
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        protected void Application_Error()
        {
            if (true)
            {
                //在出现未处理的错误时运行的代码 
                Exception ex = Server.GetLastError().GetBaseException();
                //StringBuilder str = new StringBuilder();
                //str.Append("\r\n" + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"));
                //str.Append("\r\n.客户信息：");
                //str.Append("\r\n\t浏览器:" + Request.Browser.Browser.ToString());
                //str.Append("\r\n\t浏览器版本:" + Request.Browser.MajorVersion.ToString());
                //str.Append("\r\n\t操作系统:" + Request.Browser.Platform.ToString());
                //str.Append("\r\n.错误信息：");
                //str.Append("\r\n\t页面：" + Request.Url.ToString());
                //str.Append("\r\n\t错误信息：" + ex.Message);
                //str.Append("\r\n\t错误源：" + ex.Source);
                //str.Append("\r\n\t异常方法：" + ex.TargetSite);
                //str.Append("\r\n\t堆栈信息：" + ex.StackTrace);
                //str.Append("\r\n-------------------------------------------------------------------------------");
                ErrorBLL.Instance.Log(ex.ToString());
                //跳转至出错页面 
                Response.Redirect("/");
            }
        }

    }
}
