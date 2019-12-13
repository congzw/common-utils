using System;
using System.Collections.Generic;
using System.Reflection;

// ReSharper disable CheckNamespace

// change list:
// 20191213 add json support 2.1.0
// 20191024 use reflection 2.0.0
// 20191016 first release 1.0.0
namespace Common
{
    public interface IShouldHaveBags
    {
        //IDictionary<string, object> Bags { get; set; }
    }

    public interface IHaveBags : IShouldHaveBags
    {
        IDictionary<string, object> Bags { get; set; }
    }

    public static class ShouldHaveBagsExtensions
    {
        public static string GetBagsMethod = "GetBagsPropertyName";

        public static IDictionary<string, object> TryGetBags(this object instance, bool bagsNotExistThrows)
        {
            IDictionary<string, object> bags = null;
            var bagName = string.Empty;
            var exMessage = string.Empty;
            try
            {
                bagName = GuessBagsPropertyName(instance);
                bags = GetProperty(instance, bagName) as IDictionary<string, object>;
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }

            if (bags == null)
            {
                if (bagsNotExistThrows)
                {
                    throw new InvalidOperationException(string.Format("没有找到名为{0}的Bags属性。{1}", bagName, exMessage));
                }
            }

            return bags;
        }

        public static string GuessBagsPropertyName(object model)
        {
            var theType = model.GetType();
            var methodInfo = theType.GetMethod(GetBagsMethod, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (methodInfo == null)
            {
                return "Bags";
            }
            return methodInfo.Invoke(model, null) as string;
        }

        private static object GetProperty(object model, string name)
        {
            var theType = model.GetType();
            var propInfo = theType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (propInfo == null)
            {
                return null;
            }
            return propInfo.GetValue(model, null);
        }
    }

    public static class HaveBagsExtensions
    {
        public static T GetBagValue<T>(this IShouldHaveBags haveBags, string itemKey, T defaultItemValue = default(T))
        {
            var bags = haveBags.TryGetBags(true);
            if (!bags.ContainsKey(itemKey))
            {
                return defaultItemValue;
            }

            var bagItemValue = bags[itemKey];
            var bagItemConvertTo = SafeConvertTo<T>(bagItemValue);
            return bagItemConvertTo;
        }

        public static THaveBags SetBagValue<THaveBags, T>(this THaveBags haveBags, string itemKey, T itemValue) where THaveBags : IShouldHaveBags
        {
            var bags = haveBags.TryGetBags(true);
            bags[itemKey] = itemValue;
            return haveBags;
        }

        private static T SafeConvertTo<T>(object bagItemValue)
        {
            if (bagItemValue is T itemValue)
            {
                return itemValue;
            }
            
            if (SimpleConvert.Current == null)
            {
                throw new InvalidOperationException("无法完成转换:" + typeof(T).Name);
            }
            return SimpleConvert.Current.SafeConvertTo<T>(bagItemValue);
        }
    }

    #region for extensions

    //public interface ISafeConvert
    //{
    //    T SafeConvertTo<T>(object value);
    //}

    //public static class SimpleConvert
    //{
    //    private static Func<ISafeConvert> _safeConvertFunc;

    //    public static ISafeConvert Current => _safeConvertFunc?.Invoke();

    //    public static void Initialize(Func<ISafeConvert> safeConvertFunc)
    //    {
    //        _safeConvertFunc = safeConvertFunc ?? throw new ArgumentNullException(nameof(safeConvertFunc));
    //    }
    //}

    ////how to use:
    ////SimpleConvert.Initialize(() => SafeConvert.Instance);
    //public class SafeConvert : ISafeConvert
    //{
    //    public T SafeConvertTo<T>(object value)
    //    {
    //        if (value is T modelValue)
    //        {
    //            return modelValue;
    //        }
    //        //处理网络序列化
    //        if (value is JObject theJObject)
    //        {
    //            return theJObject.ToObject<T>();
    //        }
    //        var json = JsonConvert.SerializeObject(value);
    //        var argsT = JsonConvert.DeserializeObject<T>(json);
    //        return argsT;
    //    }

    //    public static SafeConvert Instance = new SafeConvert();
    //}

    #endregion
}
