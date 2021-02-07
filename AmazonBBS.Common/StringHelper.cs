using System;
using System.Web;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace AmazonBBS.Common
{
    public class StringHelper
    {
        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
            else
                return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }



        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ReadFile(string filePath)
        {
            string fileValue = string.Empty;

            if (!IsFileExists(filePath))

                return fileValue;

            StreamReader rd = new StreamReader(filePath, Encoding.Default);

            fileValue = rd.ReadToEnd();

            rd.Close();

            return fileValue;
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFileExists(string path)
        {
            return System.IO.File.Exists(path);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="info">文件内容</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="path">保存路径</param>
        /// <param name="isConvert">是否覆盖文件</param>
        public static void SaveFile(string info, string fileName, string path)
        {
            string newPath = path + fileName;
            using (StreamWriter sw = new StreamWriter(newPath, false, Encoding.UTF8))
            {
                sw.WriteLine(info);
                sw.Flush();
                sw.Close();
            }
        }

        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="info">日志内容</param>
        public static void WriteLog(string info)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            if (File.Exists(file))
            {
                using (FileStream fs = new FileStream(file, FileMode.Append))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(info);
                        sw.Close();
                    }
                }
            }
            else
            {
                using (StreamWriter sw = File.CreateText(file))
                {
                    sw.WriteLine(info);
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// 请求远程网址
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetResponse(string url, string param, string method)
        {
            string reStr = "";
            try
            {
                if (string.IsNullOrEmpty(url)) throw new Exception("请求地址不能为空！");
                //if (param == null) throw new Exception("请求参数不能为空！");
                method = method ?? "GET";
                if (method.ToUpper() == "GET")
                {
                    if (param != "")
                    {
                        url = url + "?" + param;
                    }
                }
                HttpWebRequest hwrq = (HttpWebRequest)WebRequest.Create(url);
                hwrq.Method = method.ToUpper();
                hwrq.KeepAlive = true;
                hwrq.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                //hwrq.Headers.Add("appid", "693b47b6e4284483b049175a437c025c");
                //hwrq.Headers.Add("accessToken", Constants.Tokens.ToString());

                if (method.ToUpper() == "POST")
                {
                    ServicePointManager.Expect100Continue = false;
                    hwrq.ContentType = "application/x-www-form-urlencoded";
                    byte[] bs = Encoding.UTF8.GetBytes(param);
                    hwrq.ContentLength = bs.Length;
                    using (Stream reqStream = hwrq.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                    }
                }
                hwrq.Timeout = 60000;
                using (HttpWebResponse hwrp = (HttpWebResponse)hwrq.GetResponse())
                {
                    while (hwrp.StatusCode != HttpStatusCode.OK)
                    {
                        System.Threading.Thread.Sleep(500);
                    }
                    Stream st = hwrp.GetResponseStream();
                    reStr = new StreamReader(st, Encoding.UTF8).ReadToEnd();
                    st.Close();
                    st.Dispose();
                }
            }
            catch (Exception ex)
            {
                //Logger.Info("GetResponse Error:"+ex.Message);
                //WriteLog("Url:" + url);
            }
            return reStr;
        }


        /// <summary>
        /// 生成指定长度的随机数
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GetNickName(int len)
        {
            string str = "abcdefghijklmnopqrstuvwxyz";
            Random rd = new Random();
            string result = string.Empty;
            for (int i = 0; i < len; i++)
            {
                result += str[rd.Next(str.Length)];
            }
            return result;
        }


        /// <summary>
        /// md5 加密
        /// </summary>
        /// <param name="result">需要加密的字符</param>
        /// <returns></returns>
        public static string AddPass(string result)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(result, "MD5");
        }

        /// <summary>
        /// 根据KEY读取 web.config value值
        /// </summary>
        /// <param name="strKey">Key</param>
        /// <returns></returns>
        public static string ConfigValue(string strKey)
        {
            if (string.IsNullOrWhiteSpace(strKey)) return "";
            return ConfigurationManager.AppSettings[strKey].ToString();
        }



        /// <summary>
        /// 在字串中搜索数据头和数据尾，并返回数据头至数据尾之间的字符串
        /// </summary>
        /// <param name="strfull">被搜索的字符串</param>
        /// <param name="strhead">数据头字符串</param>
        /// <param name="strfoot">数据尾字符串</param>
        /// <returns>返回数据头至数据尾之间的字符串，否则返回空字符串</returns>
        public static string GetSubString(string strfull, string strhead, string strfoot)
        {
            int pos1, pos2;
            string result = "";
            pos1 = strfull.IndexOf(strhead, StringComparison.OrdinalIgnoreCase);
            if (pos1 > -1)
            {
                pos2 = strfull.IndexOf(strfoot, pos1 + strhead.Length, StringComparison.OrdinalIgnoreCase);
                if (pos2 > -1)
                {
                    result = strfull.Substring(pos1 + strhead.Length, pos2 - pos1 - strhead.Length);
                }
            }
            return result;
        }
        /// <summary>
        /// 在字串中搜索数据头，并返回数据头之后的strlen个字符
        /// </summary>
        /// <param name="strfull">被搜索的字符串</param>
        /// <param name="strhead">数据头字符串</param>
        /// <param name="strlen">返回数据长度</param>
        /// <returns></returns>
        string getSubString(string strfull, string strhead, int strlen)
        {
            int pos1;
            string result = "";
            pos1 = strfull.IndexOf(strhead, StringComparison.OrdinalIgnoreCase);
            if (pos1 > -1)
            {
                if (strfull.Length > pos1 + strhead.Length)
                {
                    result = strfull.Substring(pos1 + strhead.Length, strlen);
                }
            }
            return result;
        }
        /// <summary>
        /// 在字串中搜索数据头，并返回数据头之后的所有字符
        /// </summary>
        /// <param name="strfull"></param>
        /// <param name="strhead"></param>
        /// <returns></returns>
        string getSubString(string strfull, string strhead)
        {
            int pos1;
            string result = "";
            pos1 = strfull.IndexOf(strhead, StringComparison.OrdinalIgnoreCase);
            if (pos1 > -1)
            {
                if (strfull.Length > pos1 + strhead.Length)
                {
                    result = strfull.Substring(pos1 + strhead.Length);
                }
            }
            return result;
        }

        ///// <summary>
        ///// 交叉QQ号码格式
        ///// </summary>
        ///// <param name="num">QQ号码</param>
        ///// <returns></returns>
        //public static bool CheckNumber(string num)
        //{
        //    return System.Text.RegularExpressions.Regex.IsMatch(num, @"^[1-9][0-9]{4,11}$");
        //}

        /// <summary>
        /// 获取头像地址
        /// </summary>
        /// <param name="path">头像地址</param>
        /// <returns></returns>
        public static string GetHead(string path)
        {
            string result = string.IsNullOrWhiteSpace(path) ? "<img src='/img/common/head.jpg' />" : "<img src='" + path + "' />";

            return result;
        }


        /// <summary>     /// 取得HTML中所有图片的 URL。
        /// </summary>         /// <param name="sHtmlText">HTML代码</param>         
        /// /// <returns>图片的URL列表</returns>         
        /// 
        public static string[] GetHtmlImageUrlList(string sHtmlText)
        {
            // 定义正则表达式用来匹配 img 标签             
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串             

            MatchCollection matches = regImg.Matches(sHtmlText);

            int i = 0;
            string[] sUrlList = new string[matches.Count];

            // 取得匹配项列表             
            foreach (Match match in matches)
                sUrlList[i++] = match.Groups["imgUrl"].Value;

            return sUrlList;
        }
        /// <summary>
        /// 删除上传的文件
        /// </summary>
        /// <param name="delFilePath"></param>
        public static void DeleteUpFile(string delFilePath)
        {
            if (delFilePath.IndexOf(',') > -1)
            {
                delFilePath = delFilePath.Substring(delFilePath.IndexOf(',') + 1);
            }
            if (string.IsNullOrEmpty(delFilePath))
                return;


            string fullpath = GetMapPath(delFilePath); //原图
            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }
        }

        #region 获得当前绝对路径
        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {


            if (strPath.ToLower().StartsWith("http://"))
            {
                return strPath;
            }
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用
            {
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith("\\"))
                {
                    strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
                }
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }
        #endregion

        /// <summary>
        /// 将URL转换为在请求客户端可用的 URL（转换 ~/ 为绝对路径）
        /// </summary>
        /// <param name="relativeUrl">相对url</param>
        /// <returns> 返回绝对路径</returns>
        public static string ResolveUrl(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
            {
                return relativeUrl;
            }
            if (!relativeUrl.StartsWith("~/"))
            {
                return relativeUrl;
            }
            string[] strArray = relativeUrl.Split(new char[] { '?' });
            string str = VirtualPathUtility.ToAbsolute(strArray[0]);
            if (strArray.Length > 1)
            {
                str = str + "?" + strArray[1];
            }
            return str;
        }



        /// <summary>
        /// 返回文件扩展名，不含‘.’
        /// </summary>
        /// <param name="_filepath"></param>
        /// <returns></returns>
        public static string GetFileExt(string _filepath)
        {

            if (string.IsNullOrEmpty(_filepath))
            {
                return "";
            }
            if (_filepath.IndexOf(".", StringComparison.Ordinal) > 0)
            {
                return _filepath.Substring(_filepath.IndexOf(".", StringComparison.Ordinal) + 1);
            }


            return "";

        }



        /// <summary>
        /// 计算目标涨幅
        /// </summary>
        /// <param name="targetPrice">目标价</param>
        /// <param name="issuePrice">发布价</param>
        /// <returns></returns>
        public static decimal GetTargetZhangFu(decimal targetPrice, decimal issuePrice)
        {
            decimal a = targetPrice - issuePrice;

            try
            {
                if (a != 0)
                {
                    decimal r = Math.Round((a / issuePrice) * 100, 2);
                    return r;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {

                return 0;
            }

        }

        public static string FiterHTML(string Htmlstring)
        {
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "",
              RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "",
              RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

            return Htmlstring;
        }


        /// <summary>
        /// 移除html内的Elemtnts/Attributes及 ，超过charLimit个字符进行截断 
        /// </summary>
        /// <param name="rawHtml">待截字的html字符串</param>
        /// <param name="charLimit">最多允许返回的字符数</param>
        /// <returns></returns>
        public static string TrimHtml(string rawHtml, int charLimit)
        {
            if (string.IsNullOrEmpty(rawHtml))
            {
                return string.Empty;
            }
            string rawString = StripBBTags(StripHtml(rawHtml, true, false));
            if ((charLimit > 0) && (charLimit < rawString.Length))
            {
                return Trim(rawString, charLimit);
            }
            return rawString;
        }


        /// <summary>
        /// 清除UBB标签 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string StripBBTags(string content)
        {
            return Regex.Replace(content, @"\[[^\]]*?\]", string.Empty, RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 移除html标签
        /// </summary>
        /// <param name="rawString">是否移除Html实体</param>
        /// <param name="removeHtmlEntities">是否移除Html实体</param>
        /// <param name="enableMultiLine">是否保留换行符（会转换成换行符）</param>
        /// <returns></returns>
        public static string StripHtml(string rawString, bool removeHtmlEntities, bool enableMultiLine)
        {
            string input = rawString;
            if (enableMultiLine)
            {
                input = Regex.Replace(Regex.Replace(input, @"</p(?:\s*)>(?:\s*)<p(?:\s*)>", "\n\n", RegexOptions.Compiled | RegexOptions.IgnoreCase), @"<br(?:\s*)/>", "\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            input = input.Replace("\"", "''");
            if (removeHtmlEntities)
            {
                input = Regex.Replace(input, "&[^;]*;", string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            return Regex.Replace(input, "<[^>]+>", string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
        public static string Trim(string rawString, int charLimit)
        {
            return Trim(rawString, charLimit, "...");
        }



        /// <summary>
        /// 对字符串进行截字(区分单字节及双字节字符) 
        /// </summary>
        /// <param name="rawString">待截字的字符串</param>
        /// <param name="charLimit">截字的长度，按双字节计数</param>
        /// <param name="appendString">截去字的部分用替代字符串</param>
        /// <returns></returns>
        public static string Trim(string rawString, int charLimit, string appendString)
        {
            if (string.IsNullOrEmpty(rawString) || (rawString.Length <= charLimit))
            {
                return rawString;
            }
            if (Encoding.UTF8.GetBytes(rawString).Length <= (charLimit * 2))
            {
                return rawString;
            }
            charLimit = (charLimit * 2) - Encoding.UTF8.GetBytes(appendString).Length;
            StringBuilder builder = new StringBuilder();
            int num2 = 0;
            for (int i = 0; i < rawString.Length; i++)
            {
                char ch = rawString[i];
                builder.Append(ch);
                num2 += (ch > '\x0080') ? 2 : 1;
                if (num2 >= charLimit)
                {
                    break;
                }
            }
            return builder.Append(appendString).ToString();
        }

        #region 返回截取字符串
        /// <summary>
        /// 返回截取字符串
        /// </summary>
        /// <param name="sInString">源字符串</param>
        /// <param name="iCutLength">显示的长度</param>
        /// <returns>string</returns>
        public static string CutString(string sInString, int iCutLength)
        {

            string strCode = "gb2312";//字符串使用的字符集.日语:Shift_JIS.简体中文:GBK.世界语:unicode 

            if (sInString == null || sInString.Length == 0 || iCutLength <= 0)

                return "";

            int iCount = System.Text.Encoding.GetEncoding(strCode).GetByteCount(sInString);//获取字符串的个数

            if (iCount > iCutLength) //如果字符个数大于要截取的字符数.那么执行下面语句
            {

                int iLength = 0;

                for (int i = 0; i < sInString.Length; i++)
                {

                    int iCharLength = System.Text.Encoding.GetEncoding(strCode).GetByteCount(new char[] { sInString[i] });

                    iLength += iCharLength;

                    if (iLength == iCutLength)
                    {
                        sInString = sInString.Substring(0, i + 1);

                        //sInString = sInString + "..."; //加省略号

                        break;

                    }
                    else if (iLength > iCutLength)
                    {
                        sInString = sInString.Substring(0, i);
                        break;
                    }

                }

            }
            return sInString;
        }
        #endregion
    }
}
