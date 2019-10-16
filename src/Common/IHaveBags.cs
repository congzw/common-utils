using System;
using System.Collections.Generic;
// ReSharper disable CheckNamespace

// change list:
// 20191016 first release 0.0.1
namespace Common
{
    public interface IHaveBags
    {
        //IDictionary<string, object> Bags { get; set; }
    }

    public interface IHaveBagsProperty : IHaveBags
    {
        string GetBagsPropertyName();
    }

    public static class HaveBagsExtensions
    {
        //public static T SetBagValue<T>(this T instance, string key, object value)
        //{
        //    if (instance == null)
        //    {
        //        return instance;
        //    }

        //    var bags = GetBagsRef(instance);
        //    bags[key] = value;
        //    return instance;
        //}

        //public static TValue GetBagValue<T, TValue>(this T instance, string key, TValue defaultValue)
        //{
        //    var bags = GetBagsRef(instance);

        //    if (!bags.ContainsKey(key))
        //    {
        //        return defaultValue;
        //    }

        //    return (TValue)bags[key];
        //}

        //private static IDictionary<string, object> GetBagsRef<T>(T instance)
        //{
        //    IDictionary<string, object> bags = null;
        //    var bagName = string.Empty;
        //    try
        //    {
        //        bagName = instance is IHaveBagsProperty ? ((IHaveBagsProperty)instance).GetBagsPropertyName() : "Bags";
        //        bags = GetProperty(instance, bagName) as IDictionary<string, object>;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //    if (bags == null)
        //    {
        //        throw new InvalidOperationException(string.Format("没有找到名为{0}的Bags属性", bagName));
        //    }

        //    return bags;
        //}

        public static T SetBagValue<T>(this T instance, string key, object value) where T : IHaveBags
        {
            if (instance == null)
            {
                return instance;
            }

            var bags = GetBags(instance);
            bags[key] = value;
            return instance;
        }

        public static TValue GetBagValue<T, TValue>(this T instance, string key, TValue defaultValue) where T : IHaveBags
        {
            var bags = GetBags(instance);

            if (!bags.ContainsKey(key))
            {
                return defaultValue;
            }

            return (TValue)bags[key];
        }

        private static IDictionary<string, object> GetBags<T>(T instance) where T : IHaveBags
        {
            IDictionary<string, object> bags = null;
            var bagName = string.Empty;
            try
            {
                bagName = instance is IHaveBagsProperty ? ((IHaveBagsProperty)instance).GetBagsPropertyName() : "Bags";
                bags = GetProperty(instance, bagName) as IDictionary<string, object>;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            if (bags == null)
            {
                throw new InvalidOperationException(string.Format("没有找到名为{0}的Bags属性", bagName));
            }

            return bags;
        }


        private static object GetProperty(object target, string name)
        {
            var site = System.Runtime.CompilerServices.
                CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>
                .Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(),
                    new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }
    }
}
