using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.ClientMonitors.Connections
{
    public class OnDisconnectedEvent : BaseHubEvent
    {
        public string Reason { get; set; }

        public OnDisconnectedEvent(Hub raiseHub, string reason) : base(raiseHub)
        {
            Reason = reason;
        }
    }

    public class OnDisconnectedEventHandler : IHubEventHandler
    {
        private readonly IClientMonitor _manager;

        public OnDisconnectedEventHandler(IClientMonitor manager)
        {
            _manager = manager;
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
            return _manager.OnDisconnected(theEvent);
        }
    }
}