using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable CheckNamespace

namespace Common
{
    public static class ReflectExtensions
    {
        private static IEnumerable<Type> _allTypes = null;
        public static IEnumerable<Type> GetAllTypes(this AppDomain appDomain, bool useCache = true)
        {
            if (useCache)
            {
                return _allTypes ?? (_allTypes = appDomain.GetAssemblies()
                           .SelectMany(assembly => assembly.GetTypes()).ToList());
            }
            return appDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).ToList();
        }

        public static IEnumerable<Type> GetAllTypes(this Assembly assembly)
        {
            return assembly.GetTypes();
        }

        public static IEnumerable<Type> GetImplementingTypes(this AppDomain appDomain, Type desiredType)
        {
            var assemblies = appDomain.GetAssemblies();
            var types = assemblies.SelectMany(GetAllTypes);
            return types.Where(type 
                => DoesTypeSupportInterface(type, desiredType)
                && type.IsInstantiable());
        }

        public static bool DoesTypeSupportInterface(this Type testType, Type desiredType)
        {
            if (desiredType.IsAssignableFrom(testType))
                return true;
            if (testType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == desiredType))
                return true;
            return false;
        }
        public static bool IsInstantiable(this Type type)
        {
            return !(type.IsAbstract || type.IsGenericTypeDefinition || type.IsInterface);
        }
    }
}
