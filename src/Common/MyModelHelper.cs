using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable CheckNamespace
namespace Common
{
    public class MyModelHelper
    {
        public static IList<string> GetPropertyNames<T>()
        {
            return GetPropertyNames(typeof(T));
        }

        public static IList<string> GetPropertyNames(Type theType)
        {
            var result = new List<string>();
            var propertyInfos = theType.GetProperties();
            foreach (var var in propertyInfos)
            {
                result.Add(var.Name);
            }
            return result;
        }

        public static IDictionary<string, object> GetKeyValueDictionary(object model)
        {
            var result = new Dictionary<string, object>();
            if (model == null)
            {
                return result;
            }

            var theType = model.GetType();
            var propertyInfos = theType.GetProperties();
            foreach (PropertyInfo var in propertyInfos)
            {
                result.Add(var.Name, var.GetValue(model, null));
            }
            return result;
        }

        public static void SetPropertiesWithDictionary(IDictionary<string, object> items, object toBeUpdated, string keyPrefix = null)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            if (toBeUpdated == null)
            {
                throw new ArgumentNullException(nameof(toBeUpdated));
            }

            var theType = toBeUpdated.GetType();
            var propertyInfos = theType.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                foreach (var theKey in items.Keys)
                {
                    if (!theKey.Equals(keyPrefix + propertyInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var theValue = items[theKey];
                    if (theValue == null)
                    {
                        continue;
                    }

                    if (theValue.GetType() != propertyInfo.PropertyType)
                    {
                        theValue = Convert.ChangeType(theValue, propertyInfo.PropertyType);
                    }
                    propertyInfo.SetValue(toBeUpdated, theValue, null);
                }
            }

        }

        public static void SetProperties(object toBeUpdated, object getFrom, string[] excludeProperties = null)
        {
            if (toBeUpdated == null)
            {
                throw new ArgumentNullException(nameof(toBeUpdated));
            }
            if (getFrom == null)
            {
                throw new ArgumentNullException(nameof(getFrom));
            }

            //获取类型信息
            Type toBeUpdatedType = toBeUpdated.GetType();
            PropertyInfo[] propertyInfos = toBeUpdatedType.GetProperties();
            IList<string> fixedProperties = excludeProperties ?? new string[] { };

            var propertyValues = GetKeyValueDictionary(getFrom);
            foreach (var excludeProperty in fixedProperties)
            {
                if (propertyValues.ContainsKey(excludeProperty))
                {
                    propertyValues.Remove(excludeProperty);
                }
            }


            foreach (var propertyValue in propertyValues)
            {
                var propertyInfo = propertyInfos.SingleOrDefault(x => propertyValue.Key.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(toBeUpdated, propertyValue.Value, null);
                }
            }
        }

        public static bool SetProperty(object model, string key, object value)
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
    }
}
