using Microsoft.Extensions.DependencyInjection;

// ReSharper disable CheckNamespace

namespace Common.SignalR.Scoped
{
    public static class ScopedHubExtensions
    {
        public static IServiceCollection AddScopedHub(this IServiceCollection services)
        {
            services.AddScoped<HubEventBus>();
            services.AddAllImpl<IHubEventHandler>(ServiceLifetime.Scoped);

            services.AddSingleton<IScopedConnectionRepository, MemoryScopedConnectionRepository>();
            services.AddSingleton<ScopedConnectionManager>();
            return services;
        }

        //不支持继承，不能这么用！
        //public static void WrapHub<THub, THubWrap>(this IServiceCollection services)
        //    where THub : Hub
        //    where THubWrap : THub
        //{
        //    services.AddScoped<THubWrap>();
        //    services.Replace(ServiceDescriptor.Scoped<THub>(sp =>
        //    {
        //        var anyHubWrap = sp.GetService<THubWrap>();
        //        return anyHubWrap;
        //    }));
        //}
    }
}
