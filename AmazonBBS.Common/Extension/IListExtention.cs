using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AmazonBBS.Common
{
    /// <summary>
    /// IList扩展方法
    /// </summary>
    public static class IListExtention
    {
        /// <summary>
        /// 判断List集合为null或没有数据
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        /// <summary>
        /// 判断List集合不为null且至少有一条数据
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty<T>(this IList<T> list)
        {
            return !(list == null || list.Count == 0);
        }

        private static List<T> IList2List<T>(IList list)
        {
            if (list == null)
            {
                return new List<T>();
            }

            T[] array = new T[list.Count];
            list.CopyTo(array, 0);
            return new List<T>(array);
        }

        /// <summary>
        /// IList转为指定类型的List
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="list">IList集合</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IList<T> list)
        {
            return IList2List<T>((IList)list);
        }

        /// <summary>
        /// IList转为指定类型的List
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="list">IList集合</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IList list)
        {
            return IList2List<T>(list);
        }

        private static string _Join<T>(IList<T> list, string speater)
        {
            if (!list.IsNotNullOrEmpty())
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (T item in list)
            {
                sb.AppendFormat("{0}{1}", item.ToString(), speater);
            }
            if (speater.IsNotNullOrEmpty())
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将数组通过指定的分隔符拼接成一个字符串
        /// </summary>
        /// <typeparam name="T">类型(int,long,double...)</typeparam>
        /// <param name="list">需要拼接的集合</param>
        /// <param name="speater">分隔符</param>
        /// <returns></returns>
        public static string Join<T>(this IList<T> list, string speater) where T : struct
        {
            return _Join(list, speater);
        }

        /// <summary>
        /// 将数组通过指定的分隔符拼接成一个字符串
        /// </summary>
        /// <param name="list">需要拼接的集合(string)</param>
        /// <param name="speater">分隔符</param>
        /// <returns></returns>
        public static string Join(this IList<string> list, string speater)
        {
            return _Join(list, speater);
        }

        /// <summary>
        /// 遍历数组
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="action">对集合每行数据进行action委托的执行回调</param>
        public static void ForEach<T>(this IList<T> list, Action<T> action)
        {
            if (list.IsNotNullOrEmpty())
            {
                foreach (T item in list)
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// 遍历数组
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="action">
        /// 对集合每行数据进行action委托的执行回调
        /// T泛型类型，int:当前遍历循环下标
        /// </param>
        public static void ForEach<T>(this IList<T> list, Action<T, int> action)
        {
            if (list.IsNotNullOrEmpty())
            {
                for (int i = 0, length = list.Count; i < length; i++)
                {
                    action(list[i], i);
                }
            }
        }

        /// <summary>
        /// 将泛类型集合List类转换成DataTable
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="list">集合</param>
        /// <param name="exportColumns">
        /// 可选参数
        /// 指定转换的属性列
        /// 以,分隔(属性1,属性2,属性3)
        /// </param>
        /// <returns>表</returns>
        public static DataTable ToDataTable<T>(this IList<T> list, string exportColumns = null) where T : class, new()
        {
            if (list.IsNullOrEmpty())
            {
                throw new Exception("转换的集合不能为null");
            }
            else
            {
                bool isExportCol = exportColumns.IsNotNullOrEmpty();
                string[] exportCols = isExportCol ? exportColumns.Split(',') : null;

                //取出第一个实体的所有类型
                Type type = list[0].GetType();
                PropertyInfo[] propertyInfos = type.GetProperties();
                int length = propertyInfos.Length;

                DataTable dt = new DataTable(type.Name);
                list.ForEach((item, i) =>
                {
                    if (i != 0 && item.GetType() != type)
                    {
                        //检查所有的实体类型都相同
                        throw new Exception("要转换的集合元素类型不一致");
                    }

                    DataRow dr = dt.NewRow();
                    propertyInfos.ForEach(p =>
                    {
                        string columnName = p.Name;
                        if (!isExportCol || (isExportCol && exportCols.Contains(columnName)))
                        {
                            Type _ptype = p.PropertyType;
                            bool isGenericType = _ptype.IsGenericType && _ptype.GetGenericTypeDefinition() == typeof(Nullable<>);
                            Type columnType = isGenericType ? _ptype.GenericTypeArguments[0] : _ptype;

                            if (i == 0)
                            {
                                dt.Columns.Add(new DataColumn(columnName, columnType));
                            }

                            object value = p.GetValue(item);
                            dr[columnName] = value == null ? DBNull.Value : value;
                        }
                    });
                    dt.Rows.Add(dr);
                });
                return dt;
            }
        }
    }
}