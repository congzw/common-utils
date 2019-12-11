﻿using System.Threading.Tasks;
using Common.SignalR.ClientMonitors;
using Common.SignalR.ClientMonitors.ClientMethods;
using Common.SignalR.ClientMonitors.Connections;
using Common.SignalR.Refs.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable CheckNamespace

namespace Common.SignalR.Scoped
{
    public static class ScopedExtensions
    {
        public static IServiceCollection AddClientMonitors(this IServiceCollection services)
        {
            services.AddScoped<HubEventBus>();
            services.AddAllImpl<ISignalREventHandler>(ServiceLifetime.Scoped, typeof(SignalREventHandlerDecorator));


            services.AddScoped<ClientMethodInvokeProcessBus>();
            services.AddAllImpl<IClientMethodInvokeProcess>(ServiceLifetime.Scoped);

            //todo by config
            //var traceEventBus = false;
            //if (traceEventBus)
            //{
            //    services.Decorate<ISignalREventHandler, SignalREventHandlerDecorator>();
            //}

            services.AddSingleton<IClientConnectionRepository, ClientConnectionRepository>();
            services.AddSingleton<IClientMonitor, ClientMonitor>();
            return services;
        }
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