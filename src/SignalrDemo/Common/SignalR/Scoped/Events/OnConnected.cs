using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
// ReSharper disable CheckNamespace

namespace Common.SignalR.Scoped
{
    public class OnConnectedEvent : BaseHubEvent
    {
        public OnConnectedEvent(Hub raiseHub) : base(raiseHub)
        {
        }
    }

    public class OnConnectedEventHandler : IHubEventHandler
    {
        private readonly ScopedConnectionManager _scopedConnectionManager;

        public OnConnectedEventHandler(ScopedConnectionManager scopedConnectionManager)
        {
            _scopedConnectionManager = scopedConnectionManager;
            HandleOrder = HubEventHandleOrders.Instance.Forward();
        }
        
        public float HandleOrder { get; set; }

        public bool ShouldHandle(IHubEvent hubEvent)
        {
            return hubEvent is OnConnectedEvent;
        }

        public async Task HandleAsync(IHubEvent hubEvent)
        {
            if (!ShouldHandle(hubEvent))
            {
                return;
            }
            var theEvent = (OnConnectedEvent)hubEvent;
            await _scopedConnectionManager.OnConnected(theEvent.RaiseHub).ConfigureAwait(false);
        }
    }
}
