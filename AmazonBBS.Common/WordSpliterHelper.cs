using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AmazonBBS.Common
{
    public class WordSpliterHelper
    {
        #region 属性
        //分隔符
        private static string SplitChar = " ";

        //用于移除停止词
        public static string[] stopWordsList = new string[] { "的", "我们", "要", "自己", "之", "将", "“", "”", "，", "（", "）", "后", "应", "到", "某", "后", "个", "是", "位", "新", "一", "两", "在", "中", "或", "有", "更", "好" };

        private static readonly Hashtable _stopwords = null;
        #endregion

        #region 静态构造函数
        static WordSpliterHelper()
        {
            if (_stopwords == null)
            {
                _stopwords = new Hashtable();
                double dummy = 0;
                foreach (string word in stopWordsList)
                {
                    AddElement(_stopwords, word, dummy);
                }
            }
        }
        #endregion

        #region 数据缓存函数
        /// <summary>
        /// 数据缓存函数
        /// </summary>
        /// <param name="key">索引键</param>
        /// <param name="val">缓存的数据</param>
        private static void SetCache(string key, object val)
        {
            if (val == null)
                val = " ";
            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application.Set(key, val);
            System.Web.HttpContext.Current.Application.UnLock();
            //System.Web.HttpContext.Current.Cache.Insert(key, val, null, DateTime.Now.AddSeconds(i), TimeSpan.Zero);
        }

        //private static void SetCache(string key, object val)
        //{
        //    SetCache(key, val, 120);
        //}
        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <param name="mykey"></param>
        /// <returns></returns>
        private static object GetCache(string key)
        {
            return System.Web.HttpContext.Current.Application.Get(key);
            //return System.Web.HttpContext.Current.Cache[key];
        }
        #endregion

        #region 读取文本
        private static SortedList ReadTxtFile(string FilePath)
        {
            if (GetCache("dict") == null)
            {
                Encoding encoding = Encoding.GetEncoding("utf-8");
                SortedList arrText = new SortedList();
                //
                try
                {
                    FilePath = System.Web.HttpContext.Current.Server.MapPath(FilePath);
                    if (!File.Exists(FilePath))
                    {
                        arrText.Add("0", "文件" + FilePath + "不存在...");
                    }
                    else
                    {
                        StreamReader objReader = new StreamReader(FilePath, encoding);
                        string sLine = "";
                        //ArrayList arrText = new ArrayList();


                        while (sLine != null)
                        {
                            sLine = objReader.ReadLine();
                            if (sLine != null)
                                arrText.Add(sLine, sLine);
                        }
                        //
                        objReader.Close();
                        objReader.Dispose();
                    }
                }
                catch (Exception) { }
                SetCache("dict", arrText);
                //return (string[])arrText.ToArray(typeof(string));
            }
            return (SortedList)GetCache("dict");
        }
        #endregion

        #region 写文本
        //public static void WriteTxtFile(string FilePath, string message)
        //{
        //    try
        //    {
        //        //写文本
        //        StreamWriter writer = null;
        //        //
        //        string filePath = System.Web.HttpContext.Current.Server.MapPath(FilePath);
        //        if (File.Exists(filePath))
        //        {
        //            writer = File.AppendText(filePath);
        //        }
        //        else
        //        {
        //            writer = File.CreateText(filePath);
        //        }
        //        writer.WriteLine(message);
        //        writer.Close();
        //        writer.Dispose();
        //    }
        //    catch (Exception) { }
        //}
        #endregion

        #region 载入词典
        private static SortedList LoadDict
        {
            get { return ReadTxtFile("~/bbs/Templates/default.dic"); }
        }
        #endregion

        #region 判断某字符串是否在指定字符数组中
        private static bool StrIsInArray(string[] StrArray, string val)
        {
            for (int i = 0; i < StrArray.Length; i++)
                if (StrArray[i] == val) return true;
            return false;
        }
        #endregion

        #region 正则检测
        private static bool IsMatch(string str, string reg)
        {
            return new Regex(reg).IsMatch(str);
        }
        #endregion

        #region 首先格式化字符串(粗分)
        private static string FormatStr(string val)
        {
            string result = "";
            if (val == null || val == "")
                return "";
            //
            char[] CharList = val.ToCharArray();
            //
            string Spc = SplitChar;//分隔符
            int StrLen = CharList.Length;
            int CharType = 0; //0-空白 1-英文 2-中文 3-符号
            //
            for (int i = 0; i < StrLen; i++)
            {
                string StrList = CharList[i].ToString();
                if (StrList == null || StrList == "")
                    continue;
                //
                if (CharList[i] < 0x81)
                {
                    #region
                    if (CharList[i] < 33)
                    {
                        if (CharType != 0 && StrList != "\n" && StrList != "\r")
                        {
                            result += " ";
                            CharType = 0;
                        }
                        continue;
                    }
                    else if (IsMatch(StrList, "[^0-9a-zA-Z@\\.%#:/\\&_-]"))//排除这些字符
                    {
                        if (CharType == 0)
                            result += StrList;
                        else
                            result += Spc + StrList;
                        CharType = 3;
                    }
                    else
                    {
                        if (CharType == 2 || CharType == 3)
                        {
                            result += Spc + StrList;
                            CharType = 1;
                        }
                        else
                        {
                            if (IsMatch(StrList, "[@%#:]"))
                            {
                                result += StrList;
                                CharType = 3;
                            }
                            else
                            {
                                result += StrList;
                                CharType = 1;
                            }//end if No.4
                        }//end if No.3
                    }//end if No.2
                    #endregion
                }//if No.1
                else
                {
                    //如果上一个字符为非中文和非空格，则加一个空格
                    if (CharType != 0 && CharType != 2)
                        result += Spc;
                    //如果是中文标点符号
                    if (!IsMatch(StrList, "^[\u4e00-\u9fa5]+$"))
                    {
                        if (CharType != 0)
                            result += Spc + StrList;
                        else
                            result += StrList;
                        CharType = 3;
                    }
                    else //中文
                    {
                        result += StrList;
                        CharType = 2;
                    }
                }
                //end if No.1


            }//exit for
            //
            return result;
        }
        #endregion

        #region 分词
        /// <summary>
        /// 分词
        /// </summary>
        /// <param name="key">关键词</param>
        /// <returns></returns>
        private static ArrayList StringSpliter(string[] key)
        {
            ArrayList List = new ArrayList();
            try
            {
                SortedList dict = LoadDict;//载入词典
                //
                for (int i = 0; i < key.Length; i++)
                {
                    if (IsMatch(key[i], @"^(?!^\.$)([a-zA-Z0-9\.\u4e00-\u9fa5]+)$")) //中文、英文、数字
                    {
                        if (IsMatch(key[i], "^[\u4e00-\u9fa5]+$"))//如果是纯中文
                        {
                            //if (!dict.Contains(key[i].GetHashCode()))
                            //    List.Add(key[i]);
                            //
                            int keyLen = key[i].Length;
                            if (keyLen < 2)
                                continue;
                            else if (keyLen <= 7)
                                List.Add(key[i]);
                            //
                            //开始分词
                            for (int x = 0; x < keyLen; x++)
                            {
                                //x：起始位置//y：结束位置
                                for (int y = x; y < keyLen; y++)
                                {
                                    string val = key[i].Substring(x, keyLen - y);
                                    if (val == null || val.Length < 2)
                                        break;
                                    else if (val.Length > 10)
                                        continue;
                                    if (dict.Contains(val))
                                        List.Add(val);
                                }
                                //
                            }
                            //
                        }
                        //else if (IsMatch(key[i], @"^([0-9]+(\.[0-9]+)*)|([a-zA-Z]+)$"))//纯数字、纯英文
                        //{
                        //    List.Add(key[i]);
                        //}
                        else if (!IsMatch(key[i], @"^(\.*)$"))//不全是小数点
                        {
                            List.Add(key[i]);
                        }
                        //else //中文、英文、数字的混合
                        //{
                        //    List.Add(key[i]);
                        //}
                        //
                    }
                }
            }
            catch (Exception) { }
            //
            return List;
            //return (string[])List.ToArray(typeof(string));
        }
        #endregion

        #region 得到分词结果
        /// <summary>
        /// 得到分词结果
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ArrayList DoSplit(string key)
        {
            ArrayList KeyList = StringSpliter(FormatStr(key).Split(SplitChar.ToCharArray()));
            //KeyList.Insert(0,key);
            //
            //去掉重复的关键词
            //for (int i = 0; i < KeyList.Count; i++)
            //{
            //    for (int j = 0; j < KeyList.Count; j++)
            //    {
            //        if (KeyList[i].ToString() == KeyList[j].ToString())
            //        {
            //            if (i != j)
            //            {
            //                KeyList.RemoveAt(j);j--;
            //            }
            //        }
            //        //
            //    }
            //}


            //去掉没用的词
            for (int i = 0; i < KeyList.Count; i++)
            {
                if (IsStopword(KeyList[i].ToString()))
                {
                    KeyList.RemoveAt(i);
                }
            }
            return KeyList;
        }


        /// <summary> 
        /// 把一个集合按重复次数排序 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="inputList"></param> 
        /// <returns></returns> 
        public static Dictionary<string, int> SortByDuplicateCount(ArrayList inputList)
        {
            //用于计算每个元素出现的次数，key是元素，value是出现次数 
            Dictionary<string, int> distinctDict = new Dictionary<string, int>();
            for (int i = 0; i < inputList.Count; i++)
            {

                //这里没用trygetvalue，会计算两次hash 
                if (distinctDict.ContainsKey(inputList[i].ToString()))
                    distinctDict[inputList[i].ToString()]++;
                else
                    distinctDict.Add(inputList[i].ToString(), 1);
            }

            Dictionary<string, int> sortByValueDict = GetSortByValueDict(distinctDict);
            return sortByValueDict;
        }



        /// <summary> 
        /// 把一个字典俺value的顺序排序 
        /// </summary> 
        /// <typeparam name="K"></typeparam> 
        /// <typeparam name="V"></typeparam> 
        /// <param name="distinctDict"></param> 
        /// <returns></returns> 
        public static Dictionary<K, V> GetSortByValueDict<K, V>(IDictionary<K, V> distinctDict)
        {
            //用于给tempDict.Values排序的临时数组 
            V[] tempSortList = new V[distinctDict.Count];
            distinctDict.Values.CopyTo(tempSortList, 0);
            Array.Sort(tempSortList); //给数据排序 
            Array.Reverse(tempSortList);//反转 

            //用于保存按value排序的字典 
            Dictionary<K, V> sortByValueDict = new Dictionary<K, V>(distinctDict.Count);
            for (int i = 0; i < tempSortList.Length; i++)
            {
                foreach (KeyValuePair<K, V> pair in distinctDict)
                {
                    //比较两个泛型是否相当要用Equals，不能用==操作符 
                    if (pair.Value.Equals(tempSortList[i]) && !sortByValueDict.ContainsKey(pair.Key))
                        sortByValueDict.Add(pair.Key, pair.Value);
                }
            }
            return sortByValueDict;
        }






        /// <summary>
        /// 得到分词关键字，以逗号隔开
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetKeyword(string key, bool getFirst = false, string defaultFirst = null)
        {
            string _value = "";
            ArrayList _key = DoSplit(key);
            Dictionary<string, int> distinctDict = SortByDuplicateCount(_key);
            if (getFirst)
            {
                var first = distinctDict.FirstOrDefault().Key;
                if (first == null)
                {
                    return defaultFirst;
                }
                return distinctDict.First().Key;
            }
            foreach (KeyValuePair<string, int> pair in distinctDict)
            {
                _value += pair.Key + ",";
            }
            return _value;
        }
        #endregion

        #region 移除停止词
        public static object AddElement(IDictionary collection, Object key, object newValue)
        {
            object element = collection[key];
            collection[key] = newValue;
            return element;
        }

        public static bool IsStopword(string str)
        {
            //int index=Array.BinarySearch(stopWordsList, str)
            return _stopwords.ContainsKey(str.ToLower());
        }
        #endregion
    }
}