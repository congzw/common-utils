using Common.SignalR.EventBus;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.ClientMonitors.Connections
{
    public class OnConnectedEvent : ScopedHubEvent
    {
        public OnConnectedEvent(Hub raiseHub) : base(raiseHub, raiseHub.TryGetScopeId())
        {
        }
    }
}
