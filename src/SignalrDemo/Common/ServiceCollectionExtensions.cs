using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable CheckNamespace

namespace Common
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAllImpl<InterfaceType>(this IServiceCollection services,  ServiceLifetime lifetime)
        {
            var parentType = typeof(InterfaceType);
            return AddAllImpl(services, parentType, lifetime);
        }
        public static IServiceCollection AddAllImpl(this IServiceCollection services, Type parentType, ServiceLifetime lifetime)
        {
            var implTypes = AppDomain.CurrentDomain.GetImplementingTypes(parentType).ToList();
            foreach (var implType in implTypes)
            {
                services.Add(new ServiceDescriptor(parentType, implType, lifetime));
            }
            return services;
        }
    }
}