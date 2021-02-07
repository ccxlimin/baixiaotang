using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AmazonBBS.Common
{
    public static class EnumHelper
    {
        /// <summary>  
        /// 扩展方法：根据枚举值得到相应的枚举定义字符串  
        /// </summary>  
        /// <param name="value"></param>  
        /// <returns></returns>  
        public static String ToEnumString<T>(this int value)
        {
            Type enumType = typeof(T);
            NameValueCollection nvc = GetEnumStringFromEnumValue(enumType);
            return nvc[value.ToString()];
        }

        /// <summary>  
        /// 根据枚举类型得到其所有的 值 与 枚举定义字符串 的集合  
        /// </summary>  
        /// <param name="enumType"></param>  
        /// <returns></returns>  
        public static NameValueCollection GetEnumStringFromEnumValue(Type enumType)
        {
            NameValueCollection nvc = new NameValueCollection();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            string strValue = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    strValue = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    nvc.Add(strValue, field.Name);
                }
            }
            return nvc;
        }

        /// <summary>  
        /// 扩展方法：根据枚举值得到属性Description中的描述, 如果没有定义此属性则返回空串  
        /// </summary>  
        /// <param name="value"></param>  
        /// <returns></returns>  
        public static String GetDescription<T>(int? value)
        {
            Type enumType = typeof(T);
            NameValueCollection nvc = GetNVCFromEnumValue(enumType);
            return nvc[value.ToString()];
        }

        /// <summary>  
        /// 根据枚举类型得到其所有的 值 与 枚举定义Description属性 的集合  
        /// </summary>  
        /// <param name="enumType"></param>  
        /// <returns></returns>  
        public static NameValueCollection GetNVCFromEnumValue(Type enumType)
        {
            NameValueCollection nvc = new NameValueCollection();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            string strValue = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    strValue = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = "";
                    }
                    nvc.Add(strValue, strText);
                }
            }
            return nvc;
        }

        /// <summary>
        /// 根据枚举值的字符串反推出枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumNameString"></param>
        /// <returns></returns>
        public static T ToEnum<T>(string enumNameString)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), enumNameString);

            }
            catch
            {
                return default(T);
            }
        }

        ///// <summary>
        ///// 枚举转换select
        ///// </summary>
        ///// <param name="enumType"></param>
        ///// <returns></returns>
        //public static IList<SelectListItem> EnumTypeToSelect(Type enumType)
        //{
        //    List<SelectListItem> li = new List<SelectListItem>();

        //    NameValueCollection list = GetNVCFromEnumValue(enumType);
        //    if (list != null)
        //    {
        //        foreach (string item in list.AllKeys)
        //        {
        //            li.Add(new SelectListItem() { 
        //                Text=list[item],
        //                Value = item
        //            });
        //        }
        //    }

        //    return li;

        //}


    }
}
