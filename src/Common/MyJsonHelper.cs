using System;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Common
{
    public interface IMyJsonHelper
    {
        string SerializeObject(object value, bool indented);
        object DeserializeObject(string json, object defaultValue);
    }

    public class MyJsonHelper : IMyJsonHelper
    {
        public string SerializeObject(object value, bool indented)
        {
            if (value is string strValue)
            {
                return strValue;
            }
            var formatting = indented ? Formatting.Indented : Formatting.None;
            var serializeObject = JsonConvert.SerializeObject(value, formatting);
            return serializeObject;
        }

        public object DeserializeObject(string json, object defaultValue)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return defaultValue;
            }
            if (defaultValue is string strValue)
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    return strValue;
                }
                return json;
            }
            return JsonConvert.DeserializeObject(json);
        }

        #region for extensions

        private static readonly MyJsonHelper _instance = new MyJsonHelper();
        public static Func<IMyJsonHelper> Resolve = () => _instance;

        #endregion
    }

    public static class MyJsonHelperExtensions
    {
        public static T DeserializeObject<T>(this IMyJsonHelper helper, string json, T defaultValue)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return defaultValue;
            }
            if (typeof(T) == typeof(string))
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    return defaultValue;
                }
                return json.As<T>();
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
        
        public static dynamic ParseXElementAsDynamic(this IMyJsonHelper helper, XElement rootNode)
        {
            string jsonText = JsonConvert.SerializeXNode(rootNode, Formatting.Indented);
            var withoutATSign = Regex.Replace(jsonText,"(?<=\")(@)(?!.*\":\\s )", 
                string.Empty, RegexOptions.IgnoreCase);
            dynamic dyn = JsonConvert.DeserializeObject<ExpandoObject>(withoutATSign);
            return dyn;
        }

        public static dynamic ParseXmlAsDynamic(this IMyJsonHelper helper, string xmlContent)
        {
            var xDoc = XDocument.Parse(xmlContent);
            return helper.ParseXElementAsDynamic(xDoc.Elements().First());
        }

        public static dynamic ParseXmlFileAsDynamic(this IMyJsonHelper helper, string xmlFile)
        {
            var xDoc = XDocument.Load(xmlFile);
            return helper.ParseXElementAsDynamic(xDoc.Elements().First());
        }
        
        private static T As<T>(this string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
