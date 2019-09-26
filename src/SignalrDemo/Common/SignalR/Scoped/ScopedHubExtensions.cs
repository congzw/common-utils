using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Common;

// ReSharper disable CheckNamespace

namespace Common.SignalR.Scoped
{
    public static class ScopedHubExtensions
    {
        public static IServiceCollection AddScopedHub(this IServiceCollection services)
        {
            services.AddScoped<HubEventBus>();
            services.AddAllImpl<ISignalREventHandler>(ServiceLifetime.Scoped, typeof(SignalREventHandlerDecorator));
            //todo config
            //var traceEventBus = false;
            //if (traceEventBus)
            //{
            //    services.Decorate<ISignalREventHandler, SignalREventHandlerDecorator>();
            //}

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

    public class SignalREventHandlerDecorator : ISignalREventHandler
    {
        private readonly ISignalREventHandler _signalREventHandler;

        public SignalREventHandlerDecorator(ISignalREventHandler signalREventHandler)
        {
            _signalREventHandler = signalREventHandler;
        }

        public float HandleOrder
        {
            get => _signalREventHandler.HandleOrder;
            set => _signalREventHandler.HandleOrder = value;
        }

        public bool ShouldHandle(ISignalREvent @event)
        {
            return _signalREventHandler.ShouldHandle(@event);
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            //todo trace
            await _signalREventHandler.HandleAsync(@event).ConfigureAwait(false);
        }
    }
}
