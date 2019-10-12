using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace Common
{
    /// <summary>
    /// Url工具类
    /// </summary>
    public interface ISimpleQueryString
    {
        string CreateQueryString(NameValueCollection nvc, bool autoAppendQuestionMark);
        NameValueCollection ParseQueryString(string queryString);
    }

    /// <summary>
    /// UrlHelper
    /// </summary>
    public class SimpleQueryString : ISimpleQueryString
    {
        public string CreateQueryString(NameValueCollection nvc, bool autoAppendQuestionMark)
        {
            var result = string.Empty;
            if (nvc == null || nvc.Count == 0)
            {
                return result;
            }

            // Concat all key/value pairs into a string separated by ampersand
            IEnumerable<string> segments = from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format("{0}={1}",key, value);
            if (autoAppendQuestionMark)
            {
                return "?" + string.Join("&", segments);
            }
            return string.Join("&", segments);
        }

        public NameValueCollection ParseQueryString(string queryString)
        {
            var nvc = new NameValueCollection();
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return nvc;
            }

            // remove anything other than query string from url
            if (queryString.Contains("?"))
            {
                queryString = queryString.Substring(queryString.IndexOf('?') + 1);
            }

            foreach (string vp in Regex.Split(queryString, "&"))
            {
                string[] singlePair = Regex.Split(vp, "=");
                nvc.Add(singlePair[0], singlePair.Length == 2 ? singlePair[1] : string.Empty);
            }
            return nvc;
        }

        #region for di extensions

        public static Func<ISimpleQueryString> Resolve { get; } = SimpleLazyFactory<ISimpleQueryString>.Instance.Default(() => new SimpleQueryString()).Resolve;

        #endregion
    }

    public static class SimpleQueryStringExtensions
    {
        public static string CreateQueryStringFromDic(this ISimpleQueryString simpleQueryString, IDictionary<string, string> items, bool autoAppendQuestionMark)
        {
            var nvc = new NameValueCollection();
            if (items != null)
            {
                foreach (var item in items)
                {
                    nvc.Add(item.Key, item.Value);
                }
            }
            return simpleQueryString.CreateQueryString(nvc, autoAppendQuestionMark);
        }
        
        public static IDictionary<string, string> ParseQueryStringToDic(this ISimpleQueryString simpleQueryString, string queryString)
        {
            var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var nvc = simpleQueryString.ParseQueryString(queryString);
            if (nvc == null || nvc.Count == 0)
            {
                return dic;
            }

            foreach (var key in nvc.AllKeys)
            {
                dic.Add(key, nvc[key]);
            }
            return dic;
        }
        
        public static string CreateQueryStringFromObject(this ISimpleQueryString simpleQueryString, object obj, bool autoAppendQuestionMark)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (obj.GetType().IsPrimitive)
            {
                //简单类型
                throw new ArgumentException("不支持简单类型");
            }

            var nvc = new NameValueCollection();
            var properties = obj.GetType().GetProperties()
                .Where(x => x.CanRead)
                .ToDictionary(x => x.Name, x => x.GetValue(obj, null));

            // Get names for all IEnumerable properties (excl. string)
            var multiValueKeys = properties
                .Where(x => !(x.Value is string) && x.Value is IEnumerable)
                .Select(x => x.Key)
                .ToList();

            foreach (var property in properties)
            {
                if (multiValueKeys.Contains(property.Key))
                {
                    AddMultiValues(property, properties, nvc);
                }
                else
                {
                    nvc.Add(property.Key, property.Value == null ? string.Empty : property.Value.ToString());
                }
            }
            return simpleQueryString.CreateQueryString(nvc, autoAppendQuestionMark);
        }

        private static void AddMultiValues(KeyValuePair<string, object> property, Dictionary<string, object> properties, NameValueCollection nvc)
        {
            var key = property.Key;
            var valueType = properties[key].GetType();
            var valueElemType = valueType.IsGenericType
                ? valueType.GetGenericArguments()[0]
                : valueType.GetElementType();
            if (valueElemType.IsPrimitive || valueElemType == typeof(string))
            {
                var enumerable = properties[key] as IEnumerable;
                foreach (var item in enumerable)
                {
                    nvc.Add(key, item == null ? string.Empty : item.ToString());
                }
            }
            else
            {
                var propertyValue = properties[key];
                nvc.Add(key, propertyValue == null ? string.Empty : propertyValue.ToString());
            }
        }
    }
}
