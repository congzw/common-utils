using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Common
{
    public class MyModelHelper
    {
        private static IList<Type> _notProcessPerpertyBaseTypes = new List<Type>()
        {
            typeof (DynamicObject),
            typeof (Object),
            //typeof (BaseViewModel),
            //typeof (BaseViewModel<>),
            //typeof (Expando)
        };
        /// <summary>
        /// 在这些类型中声明的属性不处理
        /// </summary>
        public static IList<Type> NotProcessPerpertyBaseTypes
        {
            get => _notProcessPerpertyBaseTypes;
            set => _notProcessPerpertyBaseTypes = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static void TryCopyProperties(Object updatingObj, Object collectedObj, string[] excludeProperties = null)
        {
            if (collectedObj != null && updatingObj != null)
            {
                //获取类型信息
                Type updatingObjType = updatingObj.GetType();
                PropertyInfo[] updatingObjPropertyInfos = updatingObjType.GetProperties();

                Type collectedObjType = collectedObj.GetType();
                PropertyInfo[] collectedObjPropertyInfos = collectedObjType.GetProperties();

                string[] fixedExPropertites = excludeProperties ?? new string[] { };

                foreach (PropertyInfo updatingObjPropertyInfo in updatingObjPropertyInfos)
                {
                    foreach (PropertyInfo collectedObjPropertyInfo in collectedObjPropertyInfos)
                    {
                        if (updatingObjPropertyInfo.Name == collectedObjPropertyInfo.Name)
                        {
                            if (fixedExPropertites.Contains(updatingObjPropertyInfo.Name, StringComparer.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            //do not process complex property
                            var isSimpleType = IsSimpleType(collectedObjPropertyInfo.PropertyType);
                            if (!isSimpleType)
                            {
                                continue;
                            }

                            //fix dynamic problems: System.Reflection.TargetParameterCountException
                            var declaringType = collectedObjPropertyInfo.DeclaringType;
                            if (declaringType != null && declaringType != collectedObjType)
                            {
                                //do not process base class dynamic property
                                if (NotProcessPerpertyBaseTypes.Contains(declaringType))
                                {
                                    continue;
                                }
                            }

                            object value = collectedObjPropertyInfo.GetValue(collectedObj, null);
                            if (updatingObjPropertyInfo.CanWrite)
                            {
                                //do not process read only property
                                updatingObjPropertyInfo.SetValue(updatingObj, value, null);
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 是否是简单类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimpleType(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimpleType(typeInfo.GetGenericArguments()[0]);
            }
            return typeInfo.IsPrimitive
                   || typeInfo.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal)
                   //|| type == typeof(Guid)
                   //|| type == typeof(DateTime)
                   || type.IsSubclassOf(typeof(ValueType)); //Guid, Datetime, etc...
        }

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

    public static class ModelExtensions
    {
        public static T WithProperties<T>(this T instance, object copyFrom)
        {
            if (copyFrom == null)
            {
                return instance;
            }
            var items = MyModelHelper.GetKeyValueDictionary(copyFrom);
            var propertyNames = MyModelHelper.GetPropertyNames(copyFrom.GetType());
            foreach (var propertyName in propertyNames)
            {
                MyModelHelper.SetProperty(instance, propertyName, items[propertyName]);
            }
            return instance;
        }
    }
}
