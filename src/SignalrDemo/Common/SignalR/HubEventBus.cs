using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

// ReSharper disable CheckNamespace

namespace Common.SignalR
{
    public interface IHubEvent
    {
        DateTime RaiseAt { get; }

        Hub RaiseHub { get; }

        HubEventContext Context { get; set; }
    }

    public class HubEventContext
    {
        public HubEventContext()
        {
            Items = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
        public IDictionary<string, object> Items { get; set; }
    }

    public interface IHubEventHandler
    {
        float HandleOrder { set; get; }
        bool ShouldHandle(IHubEvent hubEvent);
        Task HandleAsync(IHubEvent hubEvent);
    }

    public class HubEventBus
    {
        public IEnumerable<IHubEventHandler> HubEventHandlers { get; }

        public HubEventBus(IEnumerable<IHubEventHandler> hubEventHandlers)
        {
            HubEventHandlers = hubEventHandlers;
        }
        
        public async Task Raise(IHubEvent hubEvent)
        {
            var hubEventHandlers = ResolveHubEventHandlers()
                .Where(x => x.ShouldHandle(hubEvent))
                .OrderBy(x => x.HandleOrder)
                .ToList();

            foreach (var hubEventHandler in hubEventHandlers)
            {
                await hubEventHandler.HandleAsync(hubEvent).ConfigureAwait(false);
            }
        }

        protected IEnumerable<IHubEventHandler> ResolveHubEventHandlers()
        {
            if (HubEventHandlers == null)
            {
                return Enumerable.Empty<IHubEventHandler>();
            }
            return HubEventHandlers;
        }
    }

    public abstract class BaseHubEvent : IHubEvent
    {
        protected BaseHubEvent(Hub raiseHub)
        {
            RaiseAt = DateTime.Now;
            RaiseHub = raiseHub;
        }

        public DateTime RaiseAt { get; private set; }
        public Hub RaiseHub { get; private set; }
        public HubEventContext Context { get; set; }
    }

    public class HubEventHandleOrders
    {
        public float Forward()
        {
            return -100;
        }

        public float Middle()
        {
            return 0;
        }

        public float Backward()
        {
            return 100;
        }

        public float Between(float one, float two)
        {
            return (one + two) / 2;
        }

        public static HubEventHandleOrders Instance = new HubEventHandleOrders();
    }
}
