using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Common
{
    public interface IReflectHelper
    {
        object TryCreateInstance(string assemblyFile, string typeName, bool supportMultiLocationAssemblies = false);
    }

    public class ReflectHelper : IReflectHelper
    {
        public ReflectHelper()
        {
            LoadedAssemblies = new ConcurrentDictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, Assembly> LoadedAssemblies { get; set; }

        public object TryCreateInstance(string assemblyFile, string typeName, bool supportMultiLocationAssemblies = false)
        {
            if (!LoadedAssemblies.ContainsKey(assemblyFile))
            {
                LoadedAssemblies.Add(assemblyFile, LoadFrom(assemblyFile, supportMultiLocationAssemblies));
            }

            LoadedAssemblies.TryGetValue(assemblyFile, out var assembly);
            return assembly == null ? null : Create(assembly, typeName);
        }

        private Assembly LoadFrom(string assemblyFile, bool supportMultiLocationAssemblies = false)
        {
            if (supportMultiLocationAssemblies)
            {
                return Assembly.LoadFile(assemblyFile);
            }
            return Assembly.LoadFrom(assemblyFile);
        }
        private object Create(Assembly assembly, string typeName)
        {
            var theType = assembly.GetType(typeName);
            return Activator.CreateInstance(theType);
        }
        
        #region for di extensions

        private static readonly Lazy<IReflectHelper> LazyInstance = new Lazy<IReflectHelper>(() => new ReflectHelper());
        public static Func<IReflectHelper> Resolve { get; set; } = () => LazyInstance.Value;

        #endregion
    }
}
