using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Common
{
    public interface ICombineKeyParser
    {
        string CreateCombinedKey(IDictionary<string, object> items);
        IDictionary<string, string> ParseCombinedKey(string combinedKey);
    }

    public class CombineKeyParser : ICombineKeyParser
    {
        private CombineKeyParser()
        {
            
        }

        public string CreateCombinedKey(IDictionary<string, object> items)
        {
            if (items == null || items.Count == 0)
            {
                return string.Empty;
            }

            var segments = from item in items
                select string.Format("{0}={1}", item.Key, item.Value);
            return string.Join("&", segments);
        }

        public IDictionary<string, string> ParseCombinedKey(string combinedKey)
        {
            var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(combinedKey))
            {
                return dic;
            }

            if (string.IsNullOrWhiteSpace(combinedKey))
            {
                return dic;
            }

            foreach (string itemKeyValue in Regex.Split(combinedKey, "&"))
            {
                string[] singlePair = Regex.Split(itemKeyValue, "=");
                var key = singlePair[0];
                var value = singlePair.Length == 2 ? singlePair[1] : string.Empty;
                dic.Add(key, value);
            }
            return dic;
        }
        
        #region for ut & di extensions

        public static CombineKeyParser NewForTest()
        {
            return new CombineKeyParser();
        }

        public static Func<ICombineKeyParser> Resolve { get; } = SimpleLazyFactory<ICombineKeyParser>.Instance.Default(() => new CombineKeyParser()).Resolve;

        #endregion
    }

    public static class CombineKeyParserExtensions
    {
        public static string AutoGetCombinedKey(this ICombineKeyParser parser, IDictionary<string, object> items, params string[] includeKeys)
        {
            var includedItems = SelectIncludeItems(items, includeKeys);
            return parser.CreateCombinedKey(includedItems);
        }
        public static string AutoGetCombinedKey(this ICombineKeyParser parser, object model, params string[] includeKeys)
        {
            if (model == null)
            {
                return null;
            }

            var propertiesDic = GetGetPropertiesDic(model);
            return parser.AutoGetCombinedKey(propertiesDic, includeKeys);
        }

        public static IDictionary<string, object> AutoSetCombinedKey(this ICombineKeyParser parser, IDictionary<string, object> model, IDictionary<string, object> items, params string[] includeKeys)
        {
            if (model == null)
            {
                return null;
            }
            var includedItems = SelectIncludeItems(items, includeKeys);
            foreach (var includedItem in includedItems)
            {
                if (model.ContainsKey(includedItem.Key))
                {
                    model[includedItem.Key] = includedItem.Value;
                }
            }
            return model;
        }
        public static object AutoSetCombinedKey(this ICombineKeyParser parser, object model, IDictionary<string, object> items, params string[] includeKeys)
        {
            if (model == null)
            {
                return null;
            }
            var includedItems = SelectIncludeItems(items, includeKeys);
            foreach (var includedItem in includedItems)
            {
                SetProperty(model, includedItem.Key, includedItem.Value);
            }
            return model;
        }

        public static IDictionary<string, object> AutoSetCombinedKey(this ICombineKeyParser parser, IDictionary<string, object> model, string key, object value)
        {
            if (model == null)
            {
                return null;
            }

            var theKey = model.Keys.SingleOrDefault(x => x.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (theKey != null)
            {
                model[theKey] = value;
            }

            return model;
        }
        public static object AutoSetCombinedKey(this ICombineKeyParser parser, object model, string key, object value)
        {
            if (model == null)
            {
                return null;
            }
            SetProperty(model, key, value);
            return model;
        }

        private static IDictionary<string, object> GetGetPropertiesDic(object model)
        {
            return CombineKeyHelper.GetGetPropertiesDic(model);
        }
        private static IDictionary<string, T> SelectIncludeItems<T>(IDictionary<string, T> items, params string[] includeKeys)
        {
            return CombineKeyHelper.SelectIncludeItems(items, includeKeys);
        }
        private static bool SetProperty(object model, string key, object value)
        {
            return CombineKeyHelper.SetProperty(model, key, value);
        }
    }

    public interface IHasCombineKey<T> where T : IHasCombineKey<T>
    {
        string[] GetIncludePropertyNames();
        string GetCombinedKeyName();
    }

    public static class HasCombineKeyExtensions
    {
        public static T SetPropertiesByCombinedKey<T>(this T model, ICombineKeyParser parser = null) where T : IHasCombineKey<T>
        {
            if (model == null)
            {
                return default(T);
            }

            var includeKeyNames = model.GetIncludePropertyNames();
            var combineKeyName = model.GetCombinedKeyName();
            return SetPropertiesByCombinedKey(model, combineKeyName, includeKeyNames, parser);
        }

        public static T SetCombinedKeyByProperties<T>(this T model, ICombineKeyParser parser = null) where T : IHasCombineKey<T>
        {
            if (model == null)
            {
                return default(T);
            }

            var includeKeyNames = model.GetIncludePropertyNames();
            var combineKeyName = model.GetCombinedKeyName();
            return SetCombinedKeyByProperties(model, combineKeyName, includeKeyNames, parser);
        }
        
        public static T SetPropertiesByCombinedKey<T>(this T model, string combinedKeyName, string[] includeKeyNames, ICombineKeyParser parser = null)
        {
            if (model == null)
            {
                return default(T);
            }
            
            if (includeKeyNames == null || includeKeyNames.Length == 0)
            {
                return model;
            }

            if (string.IsNullOrWhiteSpace(combinedKeyName))
            {
                return model;
            }

            var combineKeyValue = GetProperty(model, combinedKeyName);
            if (combineKeyValue == null)
            {
                return model;
            }

            var combineKeyParser = parser ?? CombineKeyParser.Resolve();
            var includeKeys = combineKeyParser.ParseCombinedKey(combineKeyValue.ToString());
            foreach (var includeKey in includeKeys)
            {
                SetProperty(model, includeKey.Key, includeKey.Value);
            }
            return model;
        }
        
        public static T SetCombinedKeyByProperties<T>(this T model, string combinedKeyName, string[] includeKeyNames, ICombineKeyParser parser = null) 
        {
            if (model == null)
            {
                return default(T);
            }

            if (includeKeyNames == null || includeKeyNames.Length == 0)
            {
                return model;
            }
            if (string.IsNullOrWhiteSpace(combinedKeyName))
            {
                return model;
            }

            var combineKeyParser = parser ?? CombineKeyParser.Resolve();
            var combinedKeyValue = combineKeyParser.AutoGetCombinedKey(model, includeKeyNames);
            SetProperty(model, combinedKeyName, combinedKeyValue);
            return model;
        }
        
        private static bool SetProperty(object model, string key, object value)
        {
            return CombineKeyHelper.SetProperty(model, key, value);
        }
        private static object GetProperty(object model, string key)
        {
            return CombineKeyHelper.GetProperty(model, key);
        }
    }

    internal static class CombineKeyHelper
    {
        internal static IDictionary<string, object> GetGetPropertiesDic(object model)
        {
            var result = new Dictionary<string, object>();
            if (model == null)
            {
                return result;
            }

            var theType = model.GetType();
            var propertyInfos = theType.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                result.Add(propertyInfo.Name, propertyInfo.GetValue(model, null));
            }
            return result;
        }
        internal static IDictionary<string, T> SelectIncludeItems<T>(IDictionary<string, T> items, params string[] includeKeys)
        {
            var result = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            if (items == null || items.Count == 0)
            {
                return result;
            }

            if (includeKeys == null || includeKeys.Length == 0)
            {
                return items;
            }


            foreach (var includeKey in includeKeys)
            {
                foreach (var item in items)
                {
                    if (item.Key.Equals(includeKey, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(includeKey, item.Value);
                    }
                }
            }

            return result;
        }
        internal static bool SetProperty(object model, string key, object value)
        {
            var result = false;
            if (model != null && !string.IsNullOrEmpty(key) && value != null)
            {
                //获取类型信息
                var theType = model.GetType();
                var propertyInfos = theType.GetProperties();

                foreach (var propertyInfo in propertyInfos)
                {
                    if (propertyInfo.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        var theValue = value;
                        if (value.GetType() != propertyInfo.PropertyType)
                        {
                            theValue = Convert.ChangeType(value, propertyInfo.PropertyType);
                        }
                        propertyInfo.SetValue(model, theValue, null);
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
        internal static object GetProperty(object model, string key)
        {
            if (model != null && !string.IsNullOrEmpty(key))
            {
                //获取类型信息
                var theType = model.GetType();
                var propertyInfos = theType.GetProperties();

                foreach (var propertyInfo in propertyInfos)
                {
                    if (propertyInfo.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        var value = propertyInfo.GetValue(model, null);
                        return value;
                    }
                }
            }
            return null;
        }
    }
}
