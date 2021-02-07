using AmazonBBS.Areas.Auto.Models;
using AmazonBBS.Areas.Auto.Utility;
using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace AmazonBBS.Areas.Auto.BLL
{
    public class AutoHelpBiz
    {
        AutoHelp ah = new AutoHelp();

        /// <summary>
        /// 获取表中所有字段
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public DataTable GetField(string tableName)
        {
            return ah.GetField(tableName);
        }

        /// <summary>
        /// 获取表中的主键
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public DataTable GetPk(string tableName)
        {
            return ah.GetPk(tableName);
        }

        /// <summary>
        /// 获取当前数据库所有表名称
        /// </summary>
        /// <returns></returns>
        public DataTable GetTable()
        {
            return ah.GetTable();
        }

        /// <summary>
        /// 获取表中字段备注信息
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public DataTable GetDescriptions(string tableName)
        {
            return ah.GetDescriptions(tableName);
        }


        /// <summary>
        /// 自定生成数据从文件
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public ResultInfo Create(AutoUI model)
        {
            ResultInfo ri = new ResultInfo();

            if (string.IsNullOrWhiteSpace(model.Tables))
            {
                ri.Msg = "请选择表";
                return ri;
            }

            if (!model.IsBiz && !model.IsDataAccess && !model.IsEntity && !model.IsControl)
            {
                ri.Msg = "请选择生成数据";
                return ri;
            }

            if (string.IsNullOrWhiteSpace(model.Methods) && (model.IsDataAccess || model.IsBiz))
            {
                ri.Msg = "请选择生成方法";
                return ri;
            }

            DataTable dt = GetTable();

            if (dt == null || dt.Rows.Count == 0)
            {
                ri.Msg = "数据库中没有创建表";
                return ri;
            }

            List<string> listTable = (from d in dt.AsEnumerable()
                                      select d.Field<string>("Name")).ToList<string>();

            StringBuilder errInfo = new StringBuilder();
            string[] method = null;

            if (!string.IsNullOrWhiteSpace(model.Methods))
            {
                method = model.Methods.Split(',');//需要生成的方法
            }

            foreach (string tableName in model.Tables.Split(','))
            {
                var queryTable = listTable.Contains(tableName);
                if (!queryTable)
                {
                    errInfo.AppendLine("表名(" + tableName + ")不存在");
                    continue;
                }

                DataTable field = GetField(tableName);

                if (field == null || field.Rows.Count == 0)
                {
                    errInfo.AppendLine("表(" + tableName + ")没有字段");
                    continue;
                }

                DataTable pk = GetPk(tableName);
                if (pk == null || pk.Rows.Count == 0)
                {
                    errInfo.AppendLine("表(" + tableName + ")没有主键");

                    continue;
                }

                DataTable tableContent = ah.GetTableDesc(tableName);

                string Conents = string.Empty;//表说明
                if (tableContent != null && tableContent.Rows.Count > 0)
                {
                    Conents = tableContent.Rows[0]["contents"].ToString();
                }

                DataTable desc = GetDescriptions(tableName);

                #region 是否生成实体层
                string entityPath = GetPath(System.AppDomain.CurrentDomain.BaseDirectory, "Auto\\Temp");

                string entityFile = tableName + ".cs";//实体层文件名称               

                if (model.IsEntity)
                {
                    bool bolModels = CreateEntity(tableName, field, desc, entityFile, entityPath, Conents);
                    errInfo.AppendLine(bolModels ? tableName + "实体层生成成功" : tableName + "实体层生成错误");
                }
                #endregion

                #region 是否生成数据层

                if (model.IsDataAccess)
                {

                    string dataPath = GetPath(System.AppDomain.CurrentDomain.BaseDirectory, "Auto\\Temp");

                    string dalFileName = "Auto_" + tableName + "DAL.cs";
                    bool bolDataAceess = CreateDataAccess(tableName, pk.Rows[0]["column_name"].ToString(), field, dataPath, dalFileName, method, Conents);
                    errInfo.AppendLine(bolDataAceess ? tableName + "数据层生成成功" : tableName + "数据层生成错误");
                }
                #endregion

                #region 是否生成业务层
                if (model.IsBiz)
                {
                    string bizPath = GetPath(System.AppDomain.CurrentDomain.BaseDirectory, "Auto\\Temp");//保存路径
                    string bizFile = "Auto_" + tableName + "BLL.cs";//业务层文件名称
                    bool bolBusiness = CreateBiz(pk.Rows[0]["column_name"].ToString(), tableName, bizFile, bizPath, method, Conents);
                    errInfo.AppendLine(bolBusiness ? tableName + "业务层生成成功" : tableName + "业务层生成错误");
                }
                #endregion

                #region 是否生成控制器


                if (model.IsControl)
                {
                    string webPath = System.AppDomain.CurrentDomain.BaseDirectory + "Controllers\\";

                    string webFile = tableName + "Controller.cs";//控制器文件名称

                    bool bolWeb = CreateControl(tableName, webPath, webFile);
                    errInfo.AppendLine(bolWeb ? tableName + "控制器生成成功" : tableName + "控制器生成错误");
                }
                #endregion

            }

            ri.Msg = errInfo.ToString();

            return ri;
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="tableName">标名称</param>
        /// <param name="field">表中所有字段</param>
        /// <param name="desc">字段备注信息</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="tableConent">表说明</param>
        /// <returns></returns>
        public bool CreateEntity(string tableName, DataTable field, DataTable desc, string fileName, string savePath, string tableConent)
        {
            bool bol = false;

            string basepath = System.AppDomain.CurrentDomain.BaseDirectory + "Auto\\model.txt";//实体层模板文件路径

            string contents = StringHelper.ReadFile(basepath);//模板文件内容

            if (!string.IsNullOrWhiteSpace(contents))
            {


                var query = from f in field.AsEnumerable()
                            from d in desc.AsEnumerable()
                            where f.Field<string>("COLUMN_NAME") == d.Field<string>("Column_Name")
                            select new
                            {
                                Name = f.Field<String>("COLUMN_NAME"),
                                Data_Type = f.Field<String>("Data_Type"),
                                IS_NULLABLE = f.Field<String>("IS_NULLABLE"),
                                Descriptions = d.Field<String>("Descriptions")
                            };

                StringBuilder sb = new StringBuilder();
                foreach (var item in query)
                {
                    sb.AppendLine("        /// <summary>");
                    sb.AppendLine("        /// " + item.Descriptions + "");
                    sb.AppendLine("        /// </summary>");
                    sb.AppendLine("        public " + GetDataType(item.Data_Type, item.IS_NULLABLE) + " " + item.Name);
                    sb.AppendLine("        {");
                    sb.AppendLine("            get;");
                    sb.AppendLine("            set;");
                    sb.AppendLine("        }");
                }

                contents = contents.Replace("$contents$", tableConent);
                contents = contents.Replace("$tables$", tableName);
                contents = contents.Replace("$fields$", sb.ToString());


                StringHelper.SaveFile(contents, fileName, savePath);

                bol = true;
            }

            return bol;
        }

        /// <summary>
        /// 设置自动类型
        /// </summary>
        /// <param name="Type">自动类型</param>
        /// <param name="IS_NULLABLE">是否为空</param>
        /// <returns></returns>
        string GetDataType(string Type, string IS_NULLABLE)
        {
            string result = string.Empty;
            switch (Type)
            {
                case "nvarchar":
                case "varchar":
                case "text":
                case "ntext":
                case "char":
                case "nchar":
                    result = "string";
                    break;

                case "int":
                    result = "int";
                    break;

                case "bigint":
                    result = "long";
                    break;

                case "smallint":
                    result = "short";
                    break;

                case "datetime":
                case "date":
                case "datetime2":
                case "smalldatetime":
                    result = "DateTime";
                    break;

                case "numeric":
                case "money":
                case "decimal":
                case "smallmoney":

                    result = "decimal";
                    break;

                case "float":
                    result = "double";
                    break;

                case "tinyint":
                    result = "byte";
                    break;

                case "uniqueidentifier":
                    result = "Guid";
                    break;

                case "bit":
                    result = "bool";
                    break;
            }

            if (result != "string" && IS_NULLABLE == "YES")
            {
                result = result + "?";
            }
            return result;
        }


        /// <summary>
        /// 获取新的路径
        /// </summary>
        /// <param name="oldPath">项目的根路径</param>
        /// <param name="newAccess">指定新的路径</param>
        /// <returns></returns>
        public string GetPath(string oldPath, string newAccess)
        {
            string[] arr = oldPath.Split('\\');

            string savePath = string.Empty;//文件保存路径保存路径
            for (int i = 0; i < arr.Length - 1; i++)
            {
                savePath += arr[i] + "\\";
            }

            savePath += newAccess + "\\";
            return savePath;
        }

        /// <summary>
        /// 生成业务层文件
        /// </summary>
        /// <param name="pk">主键</param>
        /// <param name="tableName">表名称</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="method">生成的方法</param>
        /// <param name="tableContent">表说明</param>
        /// <returns></returns>
        public bool CreateBiz(string pk, string tableName, string fileName, string savePath, string[] method, string tableContent)
        {
            bool bol = false;

            string contents = GetTmpContent("bllauto.txt", method);//模板文件内容

            if (!string.IsNullOrWhiteSpace(contents))
            {
                contents = contents.Replace("$table$", tableName);
                contents = contents.Replace("$contents$", tableContent);

                StringHelper.SaveFile(contents, fileName, savePath);

                string newName = fileName.Substring(5);

                if (!System.IO.File.Exists(savePath + newName))
                {
                    string customPath = System.AppDomain.CurrentDomain.BaseDirectory + "Auto\\bll.txt";//自定义业务层模板文件

                    //string customResult = StringHelper.ReadFile(customPath);
                    string customResult = GetTmpContent("bll.txt", method);//模板文件内容

                    if (!string.IsNullOrWhiteSpace(customResult))
                    {
                        customResult = customResult.Replace("$table$", tableName);
                        customResult = customResult.Replace("$pk$", pk);
                        customResult = customResult.Replace("$contents$", tableContent);

                        StringHelper.SaveFile(customResult, newName, savePath); //生成自定义的业务层文件
                    }
                }

                bol = true;
            }

            return bol;
        }


        /// <summary>
        /// 生成数据层文件
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="pk">主键</param>
        /// <param name="field">字段</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="method">生成的方法</param>
        /// <param name="tableConent">表说明</param>
        /// <returns></returns>
        public bool CreateDataAccess(string tableName, string pk, DataTable field, string savePath, string fileName, string[] method, string tableConent)
        {
            bool bol = false;

            List<string> source = new List<string>();
            source.Add(tableName);//表名称
            source.Add(pk);//表主键，不支持复合主键

            string fieldList = string.Empty;//字段列表
            string fieldPar = string.Empty;//字段参数化，在字段前面加上 "@"

            string arrSqlParameter = string.Empty;//SqlParameter

            string updateField = string.Empty;//修改时参数列表，set 后面字符串

            //判断主键是否为自增
            bool isIdentityPk = ah.IsIdentityField(tableName, pk);

            string hasCondition = string.Empty;//该表是否包含 表数据状态标识列（IsDelete）
            string orderByField = string.Empty;//排序字段

            field.ForEach(dr =>
            {
                var columnName = dr["column_name"].ToString();
                if (!isIdentityPk || pk != columnName)
                {
                    var str = "@" + columnName;
                    fieldList += columnName + ",";
                    fieldPar += str + ",";
                    updateField += "{0}=@{0},".FormatWith(columnName);
                    if (pk != columnName)
                    {
                        arrSqlParameter += "        new SqlParameter(\"" + str + "\", model." + columnName + ")," + "\r\n";
                    }
                }
                if (hasCondition.IsNullOrEmpty())
                {
                    if (columnName.ToLower().Equals("isdelete"))
                    {
                        hasCondition = columnName;
                    }
                }
                if (orderByField.IsNullOrEmpty())
                {
                    if (columnName.ToLower().Equals("createtime"))
                    {
                        orderByField = columnName;
                    }
                }
            });

            source.Add(fieldList.TrimEnd(','));
            source.Add(fieldPar.TrimEnd(','));
            source.Add(arrSqlParameter.TrimEnd(','));
            source.Add(updateField.TrimEnd(','));
            source.Add(tableConent);
            //source.Add(hasCondition);//添加查询条件，IsDelete=0的
            source.Add(orderByField.IsNullOrEmpty() ? pk : orderByField);

            string contents = GetTmpContent("dalauto.txt", method);

            if (!string.IsNullOrWhiteSpace(contents))
            {
                contents = contents.Replace("$table$", source[0]);
                contents = contents.Replace("$pk$", source[1]);
                contents = contents.Replace("$field$", source[2]);
                contents = contents.Replace("$fieldPar$", source[3]);
                contents = contents.Replace("$arrSqlParameter$", source[4]);
                contents = contents.Replace("$updateField$", source[5]);
                contents = contents.Replace("$contents$", source[6]);
                contents = contents.Replace("$condition$", hasCondition.IsNullOrEmpty() ? string.Empty : " where {0}=0 ".FormatWith(hasCondition));
                contents = contents.Replace("$orderByField$", source[7]);
                contents = contents.Replace("$condition2$", hasCondition.IsNullOrEmpty() ? string.Empty : " and {0}=0 ".FormatWith(hasCondition));

                contents = contents.Replace("$SelectIdentityID$", isIdentityPk ? "select @@IDENTITY" : string.Empty);
                contents = contents.Replace("$SelectIdentityType$", isIdentityPk ? "object" : "int");
                contents = contents.Replace("$SelectIdentityMethod$", isIdentityPk ? "GetSingle" : "ExecuteSql");
                contents = contents.Replace("$returnResult$", isIdentityPk ? "obj == null ? 0 : Convert.ToInt32(obj)" : "obj");

                contents = contents.Replace("$addPkID$", isIdentityPk ? string.Empty : "\r\n        new SqlParameter(\"@{0}\", model.{0})".FormatWith(pk));

                StringHelper.SaveFile(contents, fileName, savePath); //生成自动的数据层文件

                //生成自定义数据层文件(文件不存在则生成，文件名称不含有"Auto")

                string newFileName = fileName.Substring(5);

                if (!System.IO.File.Exists(savePath + newFileName))
                {
                    string customPath = System.AppDomain.CurrentDomain.BaseDirectory + "Auto\\dal.txt";//自定义数据层模板文件

                    string customResult = StringHelper.ReadFile(customPath);
                    if (!string.IsNullOrWhiteSpace(customResult))
                    {
                        customResult = customResult.Replace("$table$", source[0]);
                        customResult = customResult.Replace("$contents$", source[6]);
                        StringHelper.SaveFile(customResult, newFileName, savePath); //生成自定义的数据层文件
                    }
                }

                bol = true;

            }

            return bol;
        }

        /// <summary>
        /// 获取模板文件内容
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="method">需要生成的方法</param>
        /// <returns></returns>
        string GetTmpContent(string fileName, string[] method)
        {
            string basepath = System.AppDomain.CurrentDomain.BaseDirectory;

            string path = basepath + "Auto\\" + fileName;//数据层模板文件
            string contents = StringHelper.ReadFile(path);//模板文件内容

            #region 那些方法需要生成
            List<string> li = method.ToList();
            string begin = string.Empty, end = "#endregion";

            if (!li.Contains("add"))
            {
                begin = "#region add";//添加

                string add = StringHelper.GetSubString(contents, begin, end);
                contents = contents.Replace(begin + add + end, "");
            }

            if (!li.Contains("update"))
            {
                begin = "#region update";//更新记录
                contents = contents.Replace(begin + StringHelper.GetSubString(contents, begin, end) + end, "");
            }

            if (!li.Contains("delete"))
            {
                begin = "#region delete";//删除
                contents = contents.Replace(begin + StringHelper.GetSubString(contents, begin, end) + end, "");
            }

            if (!li.Contains("getmodel"))
            {
                begin = "#region getmodel";//根据主键查找单个记录
                contents = contents.Replace(begin + StringHelper.GetSubString(contents, begin, end) + end, "");
            }

            if (!li.Contains("query"))
            {
                begin = "#region query";//查询
                contents = contents.Replace(begin + StringHelper.GetSubString(contents, begin, end) + end, "");
            }
            #endregion

            return contents;
        }

        private bool CreateControl(string tableName, string webPath, string webFile)
        {

            bool bol = false;

            string basepath = System.AppDomain.CurrentDomain.BaseDirectory + "Auto\\control.txt";//业务层模板文件路径

            string contents = StringHelper.ReadFile(basepath);//模板文件内容

            if (!string.IsNullOrWhiteSpace(contents))
            {
                contents = contents.Replace("$table$", tableName);

                StringHelper.SaveFile(contents, webFile, webPath);

                bol = true;

            }

            return bol;
        }
    }
}