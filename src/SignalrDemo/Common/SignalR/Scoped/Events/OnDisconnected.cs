using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
// ReSharper disable CheckNamespace

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

        public bool ShouldHandle(ISignalREvent hubEvent)
        {
            return hubEvent is OnDisconnectedEvent;
        }

        public Task HandleAsync(ISignalREvent hubEvent)
        {
            if (!ShouldHandle(hubEvent))
            {
                return Task.CompletedTask;
            }
            var theEvent = (OnDisconnectedEvent)hubEvent;
            return _scopedConnectionManager.OnDisconnected(theEvent);
        }
    }
}
