using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AmazonBBS.Common
{
    public class UploadHelper
    {
        #region 删除上传的文件
        /// <summary>
        /// 删除上传的文件
        /// </summary>
        /// <param name="delFilePath"></param>
        public static void DeleteUpFile(string delFilePath)
        {
            if (delFilePath.IsNotNullOrEmpty())
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
        }
        #endregion

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

        #region 判断上传图片是否合法

        #endregion
    }
}
