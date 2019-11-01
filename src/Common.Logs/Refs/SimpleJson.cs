using System;
using Newtonsoft.Json;

namespace Common.Logs.Refs
{
    public interface ISimpleJson
    {
        string SerializeObject(object value, bool indented);
        object DeserializeObject(string json, object defaultValue);
        T DeserializeObject<T>(string json);
        T DeserializeObject<T>(string json, T defaultValue);
    }

    public class SimpleJson : ISimpleJson
    {
        public string SerializeObject(object value, bool indented)
        {
            return JsonConvert.SerializeObject(value, indented ? Formatting.Indented : Formatting.None);
        }

        public T DeserializeObject<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }

        public T DeserializeObject<T>(string json, T defaultValue)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return defaultValue;
            }
            return JsonConvert.DeserializeObject<T>(json);
        }

        public object DeserializeObject(string json, object defaultValue)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return defaultValue;
            }
            return JsonConvert.DeserializeObject(json);
        }

        #region for di extensions

        private static readonly Lazy<SimpleJson> Instance = new Lazy<SimpleJson>(() => new SimpleJson());
        public static Func<ISimpleJson> Resolve { get; set; } = () => Instance.Value;

        #endregion

    }
}
