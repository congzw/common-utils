using System;
// ReSharper disable CheckNamespace

namespace Common
{
    public interface ISafeConvert
    {
        T SafeConvertTo<T>(object value);
    }

    public static class SimpleConvert
    {
        private static Func<ISafeConvert> _safeConvertFunc;

        public static ISafeConvert Current => _safeConvertFunc?.Invoke();

        public static void Initialize(Func<ISafeConvert> safeConvertFunc)
        {
            _safeConvertFunc = safeConvertFunc ?? throw new ArgumentNullException(nameof(safeConvertFunc));
        }
    }

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
}