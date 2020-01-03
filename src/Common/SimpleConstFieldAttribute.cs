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

        internal Type FromType { get; set; }
    }

    #endregion

    public class SimpleConstFieldHelper
    {
        /// <summary>
        /// 导出信息
        /// </summary>
        /// <param name="constFieldValues"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public string ExportConstFieldContents(IList<SimpleConstFieldValue> constFieldValues, Func<Type, SimpleConstFieldValue, string> formatter)
        {
            if (formatter == null)
            {
                formatter = (t, x) => string.Format("{0}.{1} = \"{2}\"; //{3}",
                    t?.FullName?.Replace("+", "."),
                    x.FieldName,
                    x.FieldValue,
                    x.Description);
            }

            var sb = new StringBuilder();
            foreach (var constFieldValue in constFieldValues)
            {
                var s = formatter(constFieldValue.FromType, constFieldValue);
                sb.AppendFormat("{0}{1}", s, Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取某个类型声明的信息
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public IList<SimpleConstFieldValue> GetConstFields(Type classType)
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
                var memberInfos = classType.GetMembers(BindingFlags.Static
                                                        | BindingFlags.Instance
                                                        | BindingFlags.GetField
                                                        | BindingFlags.GetProperty
                                                        | BindingFlags.Public
                                                        | BindingFlags.NonPublic);

                foreach (var memberInfo in memberInfos)
                {
                    var customAttributes = memberInfo.GetCustomAttributes(typeof(SimpleConstFieldAttribute), false);
                    if (customAttributes.Length > 0)
                    {
                        var att = (SimpleConstFieldAttribute)customAttributes[0];
                        if (memberInfo is FieldInfo fieldInfo)
                        {
                            var fieldValue = SimpleConstFieldValue(classType, att, fieldInfo);
                            list.Add(fieldValue);
                        }
                        else if (memberInfo is PropertyInfo propInfo)
                        {
                            var fieldValue = SimpleConstFieldValue(classType, att, propInfo);
                            list.Add(fieldValue);
                        }
                    }
                }
            }

            return list;
        }

        public bool ContainsConstFiled(Type theType, string value)
        {
            var userTypeCodes = GetConstFields(theType)
                .Select(x => x.FieldValue).ToList();
            bool contains = userTypeCodes.Contains(value, StringComparer.OrdinalIgnoreCase);
            return contains;
        }

        private static SimpleConstFieldValue SimpleConstFieldValue(Type classType, SimpleConstFieldAttribute att, PropertyInfo propInfo)
        {
            var fieldValue = new SimpleConstFieldValue { Description = att.Description, FromType = classType };

            fieldValue.FieldName = !string.IsNullOrEmpty(att.Name)
                ? att.Name
                : propInfo.Name;

            if (propInfo.IsStatic())
            {
                fieldValue.FieldValue = propInfo.GetValue(null).ToString();
            }
            else
            {
                var instance = Activator.CreateInstance(classType);
                fieldValue.FieldValue = propInfo.GetValue(instance).ToString();
            }

            return fieldValue;
        }
        private static SimpleConstFieldValue SimpleConstFieldValue(Type classType, SimpleConstFieldAttribute att, FieldInfo propInfo)
        {
            var fieldValue = new SimpleConstFieldValue { Description = att.Description, FromType = classType };

            fieldValue.FieldName = !string.IsNullOrEmpty(att.Name)
                ? att.Name
                : propInfo.Name;

            if (propInfo.IsStatic)
            {
                fieldValue.FieldValue = propInfo.GetValue(null).ToString();
            }
            else
            {
                var instance = Activator.CreateInstance(classType);
                fieldValue.FieldValue = propInfo.GetValue(instance).ToString();
            }

            return fieldValue;
        }

        public static SimpleConstFieldHelper Instance = new SimpleConstFieldHelper();
    }

    public static class AssemblyExtensionsConstField
    {
        public static string ExportConstFieldContents(this Assembly assembly, Func<Type, SimpleConstFieldValue, string> formatter = null)
        {
            var exportConstFields = assembly.ExportConstFields();
            return SimpleConstFieldHelper.Instance.ExportConstFieldContents(exportConstFields, formatter);
        }

        public static IList<SimpleConstFieldValue> ExportConstFields(this Assembly assembly)
        {
            var nbConstFieldValues = new List<SimpleConstFieldValue>();
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var theFieldValues = SimpleConstFieldHelper.Instance.GetConstFields(type);
                nbConstFieldValues.AddRange(theFieldValues);
            }

            return nbConstFieldValues;
        }

        /// <summary>
        /// 获取某个类型声明的所有的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IList<SimpleConstFieldValue> GetConstFields<T>(this SimpleConstFieldHelper helper)
        {
            var classType = typeof(T);
            return helper.GetConstFields(classType);
        }

        public static bool ContainsConstFiled<T>(this SimpleConstFieldHelper helper, string value)
        {
            return helper.ContainsConstFiled(typeof(T), value);
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