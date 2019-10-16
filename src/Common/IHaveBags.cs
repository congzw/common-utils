using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

// ReSharper disable CheckNamespace

// change list:
// 20191016 first release 1.0.0
namespace Common
{
    public interface IShouldHaveBags
    {
    }

    public interface IHaveBagsProperty : IShouldHaveBags
    {
        string GetBagsPropertyName();
    }

    public interface IHaveBags : IShouldHaveBags
    {
        IDictionary<string, object> Bags { get; set; }
    }

    public static class HaveBagsExtensions
    {
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
                bagName = instance is IHaveBagsProperty ? ((IHaveBagsProperty)instance).GetBagsPropertyName() : "Bags";
                bags = GetProperty(instance, bagName) as IDictionary<string, object>;
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            if (bags == null && bagsNotExistThrows)
            {
                throw new InvalidOperationException(string.Format("没有找到名为{0}的Bags属性。{1}", bagName, exMessage));
            }

            return bags;
        }
        
        private static object GetProperty(object target, string name)
        {
            var site = CallSite<Func<CallSite, object, object>>
                .Create(Binder.GetMember(0, name, target.GetType(),
                    new[] { CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }
    }
}
