// ReSharper disable CheckNamespace

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.Scoped
{
    public class OnDisconnectedEvent : BaseHubEvent
    {
        public Exception Exception { get; set; }

        public OnDisconnectedEvent(Hub raiseHub, Exception exception) : base(raiseHub)
        {
            Exception = exception;
        }
    }

    public class OnDisconnectedEventHandler : IHubEventHandler
    {
        private readonly ScopedConnectionManager _scopedConnectionManager;

        public OnDisconnectedEventHandler(ScopedConnectionManager scopedConnectionManager)
        {
            _scopedConnectionManager = scopedConnectionManager;
            HandleOrder = HubEventHandleOrders.Instance.Forward();
        }

        public float HandleOrder { get; set; }

        public bool ShouldHandle(IHubEvent hubEvent)
        {
            return hubEvent is OnDisconnectedEvent;
        }

        public Task HandleAsync(IHubEvent hubEvent)
        {
            if (!ShouldHandle(hubEvent))
            {
                return Task.CompletedTask;
            }
            var theEvent = (OnDisconnectedEvent)hubEvent;
            return _scopedConnectionManager.OnDisconnected(theEvent.RaiseHub, theEvent.Exception);
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
}
