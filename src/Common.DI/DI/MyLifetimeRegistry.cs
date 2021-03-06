﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

        public void AutoRegister(IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            var lifetimeType = typeof(IMyLifetime);
            var lifetimeSingletonType = typeof(IMySingleton);
            var lifetimeScopedType = typeof(IMyScoped);
            var lifetimeTransientType = typeof(IMyTransient);
            var lifetimeIgnoreType = typeof(IMyLifetimeIgnore);

            var autoBindTypes = assemblies.SelectMany(x =>
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
                    services.AddSingleton(autoBindType);
                    AddIfNotExist(autoBindType, autoBindType);

                    foreach (var serviceInterface in serviceInterfaces)
                    {
                        if (!IgnoreServiceInterfaces.Contains(serviceInterface))
                        {
                            services.AddSingleton(serviceInterface, sp => sp.GetService(autoBindType));
                            AddIfNotExist(autoBindType, serviceInterface);
                        }
                    }
                    continue;
                }

                if (lifetimeScopedType.IsAssignableFrom(autoBindType))
                {
                    //bind self
                    services.AddScoped(autoBindType);
                    AddIfNotExist(autoBindType, autoBindType);

                    foreach (var serviceInterface in serviceInterfaces)
                    {
                        if (!IgnoreServiceInterfaces.Contains(serviceInterface))
                        {
                            services.AddScoped(serviceInterface, sp => sp.GetService(autoBindType));
                            AddIfNotExist(autoBindType, serviceInterface);
                        }
                    }
                    continue;
                }

                //"IMyTransient" and "IMyLifetime" will use IMyTransient
                //bind self
                services.AddTransient(autoBindType);
                AddIfNotExist(autoBindType, autoBindType);

                foreach (var serviceInterface in serviceInterfaces)
                {
                    if (!IgnoreServiceInterfaces.Contains(serviceInterface))
                    {
                        services.AddTransient(serviceInterface, autoBindType);
                        AddIfNotExist(autoBindType, serviceInterface);
                    }
                }
            }
        }

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

        public static MyLifetimeRegistry Instance = new MyLifetimeRegistry();
    }
}
