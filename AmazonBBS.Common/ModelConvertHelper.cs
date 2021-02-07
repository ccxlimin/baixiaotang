using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Common
{
    public class ModelConvertHelper<T> where T : new()
    {
        /// <summary>
        /// DataTable 转换成list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertToList(DataTable dt)
        {
            // 定义集合
            List<T> ts = new List<T>();

            if (dt == null)
            {
                return ts;
            }

            string[] columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            foreach (DataRow dr in dt.Rows)
            {
                ts.Add(ConverToModel(dr, columnNames));
            }

            return ts;
        }

        /// <summary>
        /// Datatable 转换为Model
        /// 需要保证datatable里只有一条数据，如果是多条数据，会抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static T ConverToModel(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return default(T);
            }
            else
            {
                return ConverToModel(dt.Rows[0], dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray());
            }
        }

        private static T ConverToModel(DataRow row, string[] columnNames)
        {
            T t = new T();

            PropertyInfo[] propertys = typeof(T).GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                string tempName = pi.Name;
                // 检查DataTable是否包含此列
                if (columnNames.Contains(tempName))
                {
                    // 判断此属性是否有Setter
                    if (!pi.CanWrite)
                    {
                        continue;
                    }

                    Type pType = pi.PropertyType;
                    Type targetType = (pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(Nullable<>)) ?
                            pType.GetGenericArguments()[0] : pType;
                    object value;
                    try
                    {
                        if (row[tempName] != DBNull.Value)
                        {
                            value = Convert.ChangeType(row[tempName], targetType);
                        }
                        else
                        {
                            value = (pType.IsValueType && !pType.IsGenericType) ? Activator.CreateInstance(pType) : null;
                        }
                    }
                    catch (InvalidCastException)
                    {
                        value = (pType.IsValueType && !pType.IsGenericType) ? Activator.CreateInstance(pType) : null;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    pi.SetValue(t, value, null);
                }
            }
            return t;
        }
    }
}
