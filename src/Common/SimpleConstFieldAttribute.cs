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
    public class DescriptionItemAttribute : Attribute
    {
        public DescriptionItemAttribute(string description)
        {
            Description = description;
        }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }
    }

    #region dto

    public class DescriptionItemValue
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string Description { get; set; }

        internal Type FromType { get; set; }
    }

    #endregion

    public class DescriptionItemHelper
    {
        /// <summary>
        /// 导出信息
        /// </summary>
        /// <param name="constFieldValues"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public string ExportDescriptionContents(IList<DescriptionItemValue> constFieldValues, Func<Type, DescriptionItemValue, string> formatter)
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
        public IList<DescriptionItemValue> GetDescriptionItems(Type classType)
        {
            var list = new List<DescriptionItemValue>();

            if (classType.IsEnum)
            {
                var names = Enum.GetNames(classType);
                foreach (var name in names)
                {
                    var filedInfo = classType.GetField(name);
                    var attributes = filedInfo.GetCustomAttributes(typeof(DescriptionItemAttribute), false);
                    if (attributes.Length > 0)
                    {
                        var att = (DescriptionItemAttribute)attributes[0];
                        var enumFiledValue = new DescriptionItemValue { Description = att.Description };

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
                    var customAttributes = memberInfo.GetCustomAttributes(typeof(DescriptionItemAttribute), false);
                    if (customAttributes.Length > 0)
                    {
                        var att = (DescriptionItemAttribute)customAttributes[0];
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

        /// <summary>
        /// 是否包含声明
        /// </summary>
        /// <param name="theType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsDescriptionItem(Type theType, string value)
        {
            var userTypeCodes = GetDescriptionItems(theType)
                .Select(x => x.FieldValue).ToList();
            bool contains = userTypeCodes.Contains(value, StringComparer.OrdinalIgnoreCase);
            return contains;
        }

        private static DescriptionItemValue SimpleConstFieldValue(Type classType, DescriptionItemAttribute att, PropertyInfo propInfo)
        {
            var fieldValue = new DescriptionItemValue { Description = att.Description, FromType = classType };

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
        private static DescriptionItemValue SimpleConstFieldValue(Type classType, DescriptionItemAttribute att, FieldInfo propInfo)
        {
            var fieldValue = new DescriptionItemValue { Description = att.Description, FromType = classType };

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

        public static DescriptionItemHelper Instance = new DescriptionItemHelper();
    }

    public static class AssemblyExtensionsConstField
    {
        /// <summary>
        /// 导出某个程序集的所有声明信息
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static string ExportDescriptionContents(this Assembly assembly, Func<Type, DescriptionItemValue, string> formatter = null)
        {
            var exportConstFields = assembly.GetDescriptionItems();
            return DescriptionItemHelper.Instance.ExportDescriptionContents(exportConstFields, formatter);
        }

        /// <summary>
        /// 获取某个程序集的所有声明信息
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IList<DescriptionItemValue> GetDescriptionItems(this Assembly assembly)
        {
            var nbConstFieldValues = new List<DescriptionItemValue>();
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var theFieldValues = DescriptionItemHelper.Instance.GetDescriptionItems(type);
                nbConstFieldValues.AddRange(theFieldValues);
            }

            return nbConstFieldValues;
        }

        /// <summary>
        /// 获取某个类型的所有声明信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IList<DescriptionItemValue> GetDescriptionItems<T>(this DescriptionItemHelper helper)
        {
            var classType = typeof(T);
            return helper.GetDescriptionItems(classType);
        }

        /// <summary>
        /// 检测是否包含声明
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="helper"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsDescriptionItem<T>(this DescriptionItemHelper helper, string value)
        {
            return helper.ContainsDescriptionItem(typeof(T), value);
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