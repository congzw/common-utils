using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;

namespace Common.DI
{
    public class MyLifetimeRegistry
    {
        public IList<Type> IgnoreServiceInterfaces { get; set; } = new List<Type>();
        public IDictionary<Type, IList<Type>> RegisteredTypes { get; set; } = new ConcurrentDictionary<Type, IList<Type>>();

        public MyLifetimeRegistry()
        {
            IgnoreServiceInterfaces.Add(typeof(IDisposable));
        }

        public void AutoRegister(IServiceCollection services, IEnumerable<Assembly> specificAssemblies = null)
        {
            var lifetimeType = typeof(IMyLifetime);
            var lifetimeSingletonType = typeof(IMySingleton);
            var lifetimeScopedType = typeof(IMyScoped);
            var lifetimeTransientType = typeof(IMyTransient);
            var lifetimeIgnoreType = typeof(IMyLifetimeIgnore);

            var theAssemblies = specificAssemblies;
            if (specificAssemblies == null)
            {
                var allLibs = DependencyContext.Default.CompileLibraries;
                var theLibs = allLibs.Where(x => !x.Name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase)
                                                      && !x.Name.StartsWith("System.", StringComparison.OrdinalIgnoreCase));
                theAssemblies = theLibs.Select(lib =>
                {
                    try
                    {
                        return AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    }
                    catch (Exception) { return null; }
                }
                ).Where(x => x != null);
            }

            var autoBindTypes = theAssemblies.SelectMany(x =>
                    x.ExportedTypes.Where(t =>
                        lifetimeType.IsAssignableFrom(t)
                        && !lifetimeIgnoreType.IsAssignableFrom(t)
                        && !t.IsAbstract
                        && !t.IsInterface)).ToList();

            foreach (var autoBindType in autoBindTypes)
            {
                var bindTypeImplInterfaces = autoBindType.GetInterfaces();
                var serviceInterfaces = bindTypeImplInterfaces.Where(t =>
                    t != lifetimeType
                    && t != lifetimeSingletonType
                    && t != lifetimeScopedType
                    && t != lifetimeTransientType).ToList();

                if (lifetimeSingletonType.IsAssignableFrom(autoBindType))
                {
                    //bind self
                    services.TryAddSingleton(autoBindType);
                    AddIfNotExist(autoBindType, autoBindType);

                    foreach (var serviceInterface in serviceInterfaces)
                    {
                        if (!IgnoreServiceInterfaces.Contains(serviceInterface))
                        {
                            services.TryAddSingleton(serviceInterface, sp => sp.GetService(autoBindType));
                            AddIfNotExist(autoBindType, serviceInterface);
                        }
                    }
                    continue;
                }

                if (lifetimeScopedType.IsAssignableFrom(autoBindType))
                {
                    //bind self
                    services.TryAddScoped(autoBindType);
                    AddIfNotExist(autoBindType, autoBindType);

                    foreach (var serviceInterface in serviceInterfaces)
                    {
                        if (!IgnoreServiceInterfaces.Contains(serviceInterface))
                        {
                            services.TryAddScoped(serviceInterface, sp => sp.GetService(autoBindType));
                            AddIfNotExist(autoBindType, serviceInterface);
                        }
                    }
                    continue;
                }

                //"IMyTransient" and "IMyLifetime" will use IMyTransient
                //bind self
                services.TryAddTransient(autoBindType);
                AddIfNotExist(autoBindType, autoBindType);

                foreach (var serviceInterface in serviceInterfaces)
                {
                    if (!IgnoreServiceInterfaces.Contains(serviceInterface))
                    {
                        services.TryAddTransient(serviceInterface, autoBindType);
                        AddIfNotExist(autoBindType, serviceInterface);
                    }
                }
            }
        }

        public static MyLifetimeRegistry Instance = new MyLifetimeRegistry();


        private void AddIfNotExist(Type autoBindType, Type serviceInterface)
        {
            if (!RegisteredTypes.ContainsKey(autoBindType))
            {
                RegisteredTypes.Add(autoBindType, new List<Type> { serviceInterface });
            }
            else
            {
                var serviceTypes = RegisteredTypes[autoBindType];
                if (!serviceTypes.Contains(serviceInterface))
                {
                    serviceTypes.Add(serviceInterface);
                }
            }
        }
    }
}
