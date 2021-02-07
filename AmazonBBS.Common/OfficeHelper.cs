//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Data;
//using Word = Microsoft.Office.Interop.Word;
//using System.IO;
//using System.Runtime.Remoting.Contexts;

//namespace AmazonBBS.Common
//{
//    /// <summary>
//    /// 使用替换模板进行到处word文件
//    /// </summary>
//    public class WordUtility
//    {
//        private object tempFile = null;
//        private object saveFile = null;
//        private static Word._Document wDoc = null; //word文档
//        private static Word._Application wApp = null; //word进程
//        private object missing = System.Reflection.Missing.Value;

//        public WordUtility(string tempFile, string saveFile)
//        {
//            tempFile = System.Web.HttpContext.Current.Server.MapPath(tempFile);
//            saveFile = System.Web.HttpContext.Current.Server.MapPath(saveFile);
//            this.tempFile = Path.Combine(Application.StartupPath, @tempFile);
//            this.saveFile = Path.Combine(Application.StartupPath, @saveFile);
//        }

//        /// <summary>
//        /// 模版包含头部信息和表格，表格重复使用
//        /// </summary>
//        /// <param name="dt">重复表格的数据</param>
//        /// <param name="expPairColumn">word中要替换的表达式和表格字段的对应关系</param>
//        /// <param name="simpleExpPairValue">简单的非重复型数据</param>
//        public bool GenerateWord(DataTable dt, Dictionary<string, string> expPairColumn, Dictionary<string, string> simpleExpPairValue)
//        {
//            if (!File.Exists(tempFile.ToString()))
//            {

//                return false;
//            }
//            try
//            {
//                wApp = new Word.Application();

//                wApp.Visible = false;

//                wDoc = wApp.Documents.Add(ref tempFile, ref missing, ref missing, ref missing);

//                wDoc.Activate();// 当前文档置前

//                bool isGenerate = false;

//                if (simpleExpPairValue != null && simpleExpPairValue.Count > 0)
//                    isGenerate = ReplaceAllRang(simpleExpPairValue);

//                // 表格有重复
//                if (dt != null && dt.Rows.Count > 0 && expPairColumn != null && expPairColumn.Count > 0)
//                    isGenerate = GenerateTable(dt, expPairColumn);

//                if (isGenerate)
//                    wDoc.SaveAs(ref saveFile, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
//                        ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);

//                DisposeWord();

//                return true;
//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// 单个替换 模版没有重复使用的表格
//        /// </summary>
//        /// <param name="dc">要替换的</param>
//        public bool GenerateWord(Dictionary<string, string> dc)
//        {
//            return GenerateWord(null, null, dc);
//        }


//        private bool GenerateTable(DataTable dt, Dictionary<string, string> expPairColumn)
//        {
//            try
//            {
//                int tableNums = dt.Rows.Count;

//                Word.Table tb = wDoc.Tables[1];

//                tb.Range.Copy();

//                Dictionary<string, object> dc = new Dictionary<string, object>();
//                for (int i = 0; i < tableNums; i++)
//                {
//                    dc.Clear();

//                    if (i == 0)
//                    {
//                        foreach (string key in expPairColumn.Keys)
//                        {
//                            string column = expPairColumn[key];
//                            object value = null;
//                            value = dt.Rows[i][column];
//                            dc.Add(key, value);
//                        }

//                        ReplaceTableRang(wDoc.Tables[1], dc);
//                        continue;
//                    }

//                    wDoc.Paragraphs.Last.Range.Paste();

//                    foreach (string key in expPairColumn.Keys)
//                    {
//                        string column = expPairColumn[key];
//                        object value = null;
//                        value = dt.Rows[i][column];
//                        dc.Add(key, value);
//                    }

//                    ReplaceTableRang(wDoc.Tables[1], dc);
//                }


//                return true;
//            }
//            catch (Exception ex)
//            {
//                DisposeWord();
//                return false;
//            }
//        }

//        private bool ReplaceTableRang(Word.Table table, Dictionary<string, object> dc)
//        {
//            try
//            {
//                object replaceArea = Word.WdReplace.wdReplaceAll;

//                foreach (string item in dc.Keys)
//                {
//                    object replaceKey = item;
//                    object replaceValue = dc[item];
//                    table.Range.Find.Execute(ref replaceKey, ref missing, ref missing, ref missing,
//                      ref missing, ref missing, ref missing, ref missing, ref missing,
//                      ref replaceValue, ref replaceArea, ref missing, ref missing, ref missing,
//                      ref missing);
//                }
//                return true;
//            }
//            catch (Exception ex)
//            {
//                DisposeWord();

//                return false;
//            }
//        }

//        private bool ReplaceAllRang(Dictionary<string, string> dc)
//        {
//            try
//            {
//                object replaceArea = Word.WdReplace.wdReplaceAll;

//                foreach (string item in dc.Keys)
//                {
//                    object replaceKey = item;
//                    object replaceValue = dc[item];
//                    wApp.Selection.Find.Execute(ref replaceKey, ref missing, ref missing, ref missing,
//                      ref missing, ref missing, ref missing, ref missing, ref missing,
//                      ref replaceValue, ref replaceArea, ref missing, ref missing, ref missing,
//                      ref missing);
//                }
//                return true;
//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
//        }

//        private void DisposeWord()
//        {
//            object saveOption = Word.WdSaveOptions.wdSaveChanges;

//            wDoc.Close(ref saveOption, ref missing, ref missing);

//            saveOption = Word.WdSaveOptions.wdDoNotSaveChanges;

//            wApp.Quit(ref saveOption, ref missing, ref missing); //关闭Word进程
//        }
//    }
//}