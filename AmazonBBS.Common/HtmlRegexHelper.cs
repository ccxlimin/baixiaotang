using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AmazonBBS.Common
{
    public class HtmlRegexHelper
    {
        /// <summary>   
        /// 取得HTML中所有图片的 URL。   
        /// </summary>   
        /// <param name="sHtmlText">HTML代码</param>   
        /// <returns>图片的URL列表</returns>   
        public static string[] GetHtmlImageUrlList(string sHtmlText)
        {
            return GetHtmlImg(sHtmlText, false).ToArray();
        }

        /// <summary>   
        /// 取得HTML中所有图片的 URL。   
        /// </summary>   
        /// <param name="sHtmlText">HTML代码</param>   
        /// <param name="isRemoveAllImg">是否将html代码里的所有img标签全移除</param>   
        /// <returns>图片的URL列表</returns>   
        public static string[] GetHtmlImageUrlList(string sHtmlText, out string htmlText)
        {
            var list = GetHtmlImg(sHtmlText, true);
            htmlText = list.Last();
            list.Remove(htmlText);
            return list.ToArray();
        }

        /// <summary>   
        /// 取得HTML中所有图片的 URL。   
        /// </summary>   
        /// <param name="sHtmlText">HTML代码</param>   
        /// <param name="isRemoveAllImg">是否将html代码里的所有img标签全移除</param>   
        /// <returns>图片的URL列表</returns>   
        private static List<string> GetHtmlImg(string htmlText, bool isRemoveAllImg)
        {
            // 定义正则表达式用来匹配 img 标签   
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串   
            MatchCollection matches = regImg.Matches(htmlText);
            List<string> sUrlList = new List<string>();

            // 取得匹配项列表   
            foreach (Match match in matches)
            {
                var groups = match.Groups;
                sUrlList.Add(groups["imgUrl"].Value);
                if (isRemoveAllImg)
                {
                    htmlText = htmlText.Replace(groups[0].Value, string.Empty);
                }
            }
            if (isRemoveAllImg)
            {
                sUrlList.Add(htmlText);
            }
            return sUrlList;
        }

        public static string GetHtmlRegexP(string htmlText)
        {
            //Regex regP = new Regex("<p>[^<]*?</p>", RegexOptions.IgnoreCase);
            //Regex regP = new Regex("<p><br/></p>", RegexOptions.IgnoreCase);
            Regex regP = new Regex("<p></p>", RegexOptions.IgnoreCase);
            Regex regP1 = new Regex("<p><br/></p>", RegexOptions.IgnoreCase);
            Regex regP2 = new Regex("<p><br /></p>", RegexOptions.IgnoreCase);
            Regex regP3 = new Regex("<p><br></p>", RegexOptions.IgnoreCase);
            return regP3.Replace(regP2.Replace(regP1.Replace(regP.Replace(htmlText, string.Empty), string.Empty), string.Empty), string.Empty);
        }

        /// <summary>
        /// 转换 Html 内容为纯文本内容
        /// </summary>
        /// <param name="HTML">HTML 内容</param>
        /// <returns>转换后的纯文本内容</returns>
        public static string ToText(string HTML)
        {
            string br = "<br/>";
            int count = br.Count();
            string output = Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(HTML, @"(?m)<script[^>]*>(\w|\W)*?</script[^>]*>", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase), @"(?m)<style[^>]*>(\w|\W)*?</style[^>]*>", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase), @"(?m)<select[^>]*>(\w|\W)*?</select[^>]*>", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase), @"(?m)<a[^>]*>(\w|\W)*?</a[^>]*>", br, RegexOptions.Multiline | RegexOptions.IgnoreCase), "(<[^>]+?>)| ", br, RegexOptions.Multiline | RegexOptions.IgnoreCase), @"(\s)+", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            output = Regex.Replace(output, "(&nbsp;)+", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            output = Regex.Replace(output, "(<br/>)+", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            output = Regex.Replace(output, "( )+", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (output.StartsWith(br))
            {
                output = output.Remove(0, count);
            }
            if (output.EndsWith(br))
            {
                output = output.Remove(output.Length - count, count);
            }
            return output;
        }
    }
}
