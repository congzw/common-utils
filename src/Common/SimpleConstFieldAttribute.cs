using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

// ReSharper disable CheckNamespace

// 20200103 first release 1.0.0
namespace Common
{
    /// <summary>
    /// 通过反射查找系统内的所有常量Field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SimpleConstFieldAttribute : Attribute
    {
        public SimpleConstFieldAttribute()
        {

        }

        public SimpleConstFieldAttribute(string description)
        {
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }

    #region dto

    public class SimpleConstFieldValue
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string Description { get; set; }
    }

    #endregion

    public class SimpleConstFieldHelper
    {
        /// <summary>
        /// 反射查找并导出程序集里的注册信息
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static string ExportConstFieldContents(Assembly assembly, Func<Type, SimpleConstFieldValue, string> formatter)
        {
            if (formatter == null)
            {
                formatter = (t, x) => string.Format("{0}.{1} = \"{2}\"; //{3}", t?.FullName?.Replace("+", "."),
                    x.FieldName, x.FieldValue, x.Description);
            }

            var sb = new StringBuilder();
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var theFieldValues = GetConstFields(type);
                foreach (var theFieldValue in theFieldValues)
                {
                    var s = formatter(type, theFieldValue);
                    sb.AppendFormat("{0}\r\n", s);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 反射查找并导出程序集里的注册信息
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IList<SimpleConstFieldValue> ExportConstFields(Assembly assembly)
        {
            var nbConstFieldValues = new List<SimpleConstFieldValue>();
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var theFieldValues = GetConstFields(type);
                nbConstFieldValues.AddRange(theFieldValues);
            }
            return nbConstFieldValues;
        }

        /// <summary>
        /// 获取某个类型声明的所有的MyConstFieldAttribute字段的信息
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public static IList<SimpleConstFieldValue> GetConstFields(Type classType)
        {
            var list = new List<SimpleConstFieldValue>();

            if (classType.IsEnum)
            {
                var names = Enum.GetNames(classType);
                foreach (var name in names)
                {
                    var filedInfo = classType.GetField(name);
                    var attributes = filedInfo.GetCustomAttributes(typeof(SimpleConstFieldAttribute), false);
                    if (attributes.Length > 0)
                    {
                        var att = (SimpleConstFieldAttribute)attributes[0];
                        var enumFiledValue = new SimpleConstFieldValue { Description = att.Description };

                        enumFiledValue.FieldName = !string.IsNullOrEmpty(att.Name)
                            ? att.Name
                            : name;

                        enumFiledValue.FieldValue = name;
                        list.Add(enumFiledValue);
                    }
                }
            }
            else
            {
                var fieldInfos = classType.GetProperties();
                foreach (var fieldInfo in fieldInfos)
                {
                    var customAttributes = fieldInfo.GetCustomAttributes(typeof(SimpleConstFieldAttribute), false);
                    if (customAttributes.Length > 0)
                    {
                        var att = (SimpleConstFieldAttribute)customAttributes[0];
                        var fieldValue = SimpleConstFieldValue(classType, att, fieldInfo);

                        list.Add(fieldValue);
                    }
                }
            }

            return list;
        }

        private static SimpleConstFieldValue SimpleConstFieldValue(Type classType, SimpleConstFieldAttribute att, PropertyInfo fieldInfo)
        {
            var fieldValue = new SimpleConstFieldValue { Description = att.Description };

            fieldValue.FieldName = !string.IsNullOrEmpty(att.Name)
                ? att.Name
                : fieldInfo.Name;

            if (fieldInfo.IsStatic())
            {
                fieldValue.FieldValue = fieldInfo.GetValue(null).ToString();
            }
            else
            {
                var instance = Activator.CreateInstance(classType);
                fieldValue.FieldValue = fieldInfo.GetValue(instance).ToString();
            }

            return fieldValue;
        }

        /// <summary>
        /// 获取某个类型声明的所有的Const字段的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IList<SimpleConstFieldValue> GetConstFields<T>()
        {
            var classType = typeof(T);
            return GetConstFields(classType);
        }

        public static bool ContainsConstFiled<T>(string value)
        {
            var userTypeCodes = GetConstFields<T>()
                .Select(x => x.FieldValue).ToList();
            bool contains = userTypeCodes.Contains(value, StringComparer.OrdinalIgnoreCase);
            return contains;
        }

        private static T GetAttributeOfType<T>(int enumVal, Type enumType) where T : Attribute
        {
            var memInfo = enumType.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }
    }

    public static class AssemblyExtensionsConstField
    {
        /// <summary>
        ///  反射查找并导出程序集里的[MyConstFieldAttribute]注册信息的文本内容
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static string ExportConstFieldContents(this Assembly assembly, Func<Type, SimpleConstFieldValue, string> formatter = null)
        {
            return SimpleConstFieldHelper.ExportConstFieldContents(assembly, formatter);
        }

        /// <summary>
        /// 反射查找并导出程序集里的[MyConstFieldAttribute]注册信息
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IList<SimpleConstFieldValue> ExportConstFields(this Assembly assembly)
        {
            return SimpleConstFieldHelper.ExportConstFields(assembly);
        }
    }

    public static class PropertyInfoExtensions
    {
        public static bool IsStatic(this PropertyInfo source, bool nonPublic = false)
        {
            return source.GetAccessors(nonPublic).Any(x => x.IsStatic);
        }
    }
}
