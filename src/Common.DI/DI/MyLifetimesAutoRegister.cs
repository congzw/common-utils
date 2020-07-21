using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;

namespace Common.DI
{
    public static class MyLifetimesAutoRegister
    {
        public static void AutoBind(IServiceCollection services, IEnumerable<Assembly> specificAssemblies = null)
        {
            var lifetimeType = typeof(IMyLifetime);
            var lifetimeSingletonType = typeof(IMySingleton);
            var lifetimeScopedType = typeof(IMyScoped);
            var lifetimeTransientType = typeof(IMyTransient);

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

            var autoBindTypes = theAssemblies.SelectMany(x => x.ExportedTypes.Where(t => lifetimeType.IsAssignableFrom(t)))
                .Where(t => !t.IsAbstract && !t.IsInterface).ToList();

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
                    Type firstServiceInterface = null;
                    foreach (var serviceInterface in serviceInterfaces)
                    {
                        if (firstServiceInterface == null)
                        {
                            services.TryAddSingleton(serviceInterface, autoBindType);
                            firstServiceInterface = serviceInterface;
                        }
                        else
                        {
                            var copyType = firstServiceInterface;
                            services.TryAddSingleton(serviceInterface, sp => sp.GetService(copyType));
                        }
                    }
                    continue;
                }

                if (lifetimeScopedType.IsAssignableFrom(autoBindType))
                {
                    //bind self
                    services.TryAddScoped(autoBindType);
                    Type firstServiceInterface = null;
                    foreach (var serviceInterface in serviceInterfaces)
                    {
                        if (firstServiceInterface == null)
                        {
                            services.TryAddScoped(serviceInterface, autoBindType);
                            firstServiceInterface = serviceInterface;
                        }
                        else
                        {
                            var copyType = firstServiceInterface;
                            services.TryAddScoped(serviceInterface, sp => sp.GetService(copyType));
                        }
                    }
                    continue;
                }

                //default bind
                services.TryAddTransient(autoBindType);
                foreach (var serviceInterface in serviceInterfaces)
                {
                    services.TryAddTransient(serviceInterface, autoBindType);
                }
            }
        }
    }
}
