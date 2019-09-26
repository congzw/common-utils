using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
// ReSharper disable CheckNamespace

namespace Common.SignalR.Scoped
{
    public class OnUpdateBagsEvent : BaseHubEvent
    {
        public IDictionary<string, object> Bags { get; }

        public OnUpdateBagsEvent(Hub raiseHub, IDictionary<string, object> bags) : base(raiseHub)
        {
            Bags = bags;
        }
    }

    public class OnUpdateBagsHubContextEvent : BaseHubContextEvent
    {
        public string ClientId { get; set; }
        public string ScopeGroupId { get; set; }
        public IDictionary<string, object> Bags { get; }

        public OnUpdateBagsHubContextEvent(MyHubContext hubContext, string clientId, string scopeGroupId,
            IDictionary<string, object> bags) : base(hubContext)
        {
            ClientId = clientId;
            ScopeGroupId = scopeGroupId;
            Bags = bags ?? new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
    }

    public static class OnDisconnectedEventExtensions
    {
        public static string _UpdateScopedConnectionBags = "UpdateScopedConnectionBags";
        public static string _ScopedConnectionsUpdated = "ScopedConnectionsUpdated";

        public static string UpdateScopedConnectionBags(this ScopedConstForServer server)
        {
            return _UpdateScopedConnectionBags;
        }

        public static string ScopedConnectionsUpdated(this ScopedConstForClient client)
        {
            return _ScopedConnectionsUpdated;
        }
    }

    public class OnUpdateBagsEventHandler : IHubEventHandler, IHubContextEventHandler
    {
        private readonly ScopedConnectionManager _scopedConnectionManager;

        public OnUpdateBagsEventHandler(ScopedConnectionManager scopedConnectionManager)
        {
            _scopedConnectionManager = scopedConnectionManager;
            HandleOrder = HubEventHandleOrders.Instance.Forward();
        }

        public float HandleOrder { get; set; }
        
        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is OnUpdateBagsEvent || @event is OnUpdateBagsHubContextEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            if (!ShouldHandle(@event))
            {
                return;
            }

            if (@event is OnUpdateBagsHubContextEvent hubContextEvent)
            {
                await _scopedConnectionManager.UpdateScopedConnectionBagsOutSideHub(hubContextEvent).ConfigureAwait(false);
            }
            if (@event is OnUpdateBagsEvent hubEvent)
            {
                await _scopedConnectionManager.UpdateScopedConnectionBags(hubEvent).ConfigureAwait(false);
            }
        }
    }
}
