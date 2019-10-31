using System;
using System.Collections.Generic;
using System.Reflection;

namespace Common.SignalR.Refs
{
    public interface IShouldHaveBags
    {
    }

    public interface IHaveBags : IShouldHaveBags
    {
        IDictionary<string, object> Bags { get; set; }
    }

    public static class HaveBagsExtensions
    {
        public static string GetBagsMethod = "GetBagsPropertyName";

        public static T SetBagValue<T>(this T instance, string key, object value, bool bagsNotExistThrows = true) where T : IShouldHaveBags
        {
            if (instance == null)
            {
                return instance;
            }

            var bags = TryGetBags(instance, bagsNotExistThrows);
            if (bags == null)
            {
                return instance;
            }
            bags[key] = value;
            return instance;
        }

        public static TValue GetBagValue<T, TValue>(this T instance, string key, TValue defaultValue, bool bagsNotExistThrows = true) where T : IShouldHaveBags
        {
            var bags = TryGetBags(instance, bagsNotExistThrows);
            if (bags == null || !bags.ContainsKey(key))
            {
                return defaultValue;
            }

            return (TValue)bags[key];
        }

        private static IDictionary<string, object> TryGetBags<T>(T instance, bool bagsNotExistThrows) where T : IShouldHaveBags
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

        private static string GuessBagsPropertyName(object model)
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
}
