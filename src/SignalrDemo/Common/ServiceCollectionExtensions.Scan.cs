using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable CheckNamespace

namespace Common
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAllImpl<InterfaceType>(this IServiceCollection services, ServiceLifetime lifetime, params Type[] ignoreImplTypes)
        {
            var parentType = typeof(InterfaceType);
            return AddAllImpl(services, parentType, lifetime, ignoreImplTypes);
        }
        public static IServiceCollection AddAllImpl(this IServiceCollection services, Type parentType, ServiceLifetime lifetime, params Type[] ignoreImplTypes)
        {
            var implTypes = AppDomain.CurrentDomain.GetImplementingTypes(parentType).ToList();
            foreach (var implType in implTypes)
            {
                if (ignoreImplTypes.Contains(implType))
                {
                    continue;
                }
                services.Add(new ServiceDescriptor(parentType, implType, lifetime));
            }
            return services;
        }
    }
}
