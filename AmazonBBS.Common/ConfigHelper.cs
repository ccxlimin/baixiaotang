using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;

namespace AmazonBBS.Common
{
    /// <summary>
    ///  Config配置文件 公共帮助类
    /// 版本：2.0
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// 根据Key取Value值
        /// </summary>
        /// <param name="key"></param>
        public static string AppSettings(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key]?.ToString()?.Trim();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 根据name取connectionString值
        /// </summary>
        /// <param name="name"></param>
        public static string ConnectionStrings(string name)
        {
            try
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString.Trim();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        ///// <summary>
        ///// 根据Key取Value值
        ///// </summary>
        ///// <param name="key"></param>
        //public static string GetConfigSetValue(string key)
        //{
        //    var keyzz = "ConfigSet" + key;
        //    var configsetvalue = CacheHelper.Get(keyzz);
        //    if (null != configsetvalue)
        //    {
        //        return configsetvalue as string;
        //    }
        //    else
        //    {
        //        StringBuilder strSql = new StringBuilder();
        //        strSql.Append("SELECT * FROM Base_ConfigSet ");
        //        strSql.Append(" WHERE ConfigSetKey=@ConfigSetKey ");
        //        List<DbParameter> parameter = new List<DbParameter>();
        //        parameter.Add(DbFactory.CreateDbParameter("@ConfigSetKey", key));
        //        var configSet = DataFactory.Database().FindTableBySql(strSql.ToString(), parameter.ToArray());
        //        if (configSet != null && configSet.Rows.Count > 0)
        //        {
        //            CacheHelper.Insert(keyzz, configSet.Rows[0]["ConfigSetValue"]);
        //            configsetvalue = configSet.Rows[0]["ConfigSetValue"];
        //        }
        //    }
        //    return configsetvalue as string;
        //}
        /// <summary>
        /// 根据Key修改Value
        /// </summary>
        /// <param name="key">要修改的Key</param>
        /// <param name="value">要修改为的值</param>
        public static void SetValue(string key, string value, string path)
        {
            System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
            xDoc.Load(HttpContext.Current.Server.MapPath(path));
            System.Xml.XmlNode xNode;
            System.Xml.XmlElement xElem1;
            System.Xml.XmlElement xElem2;
            xNode = xDoc.SelectSingleNode("//appSettings");

            xElem1 = (System.Xml.XmlElement)xNode.SelectSingleNode("//add[@key='" + key + "']");
            if (xElem1 != null) xElem1.SetAttribute("value", value);
            else
            {
                xElem2 = xDoc.CreateElement("add");
                xElem2.SetAttribute("key", key);
                xElem2.SetAttribute("value", value);
                xNode.AppendChild(xElem2);
            }
            xDoc.Save(HttpContext.Current.Server.MapPath(path));
        }
    }
}
