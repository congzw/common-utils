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

    public class OnUpdateBagsEventHandler : IHubEventHandler
    {
        private readonly ScopedConnectionManager _scopedConnectionManager;

        public OnUpdateBagsEventHandler(ScopedConnectionManager scopedConnectionManager)
        {
            _scopedConnectionManager = scopedConnectionManager;
            HandleOrder = HubEventHandleOrders.Instance.Forward();
        }
        
        public float HandleOrder { get; set; }

        public bool ShouldHandle(IHubEvent hubEvent)
        {
            return hubEvent is OnUpdateBagsEvent;
        }

        public async Task HandleAsync(IHubEvent hubEvent)
        {
            if (!ShouldHandle(hubEvent))
            {
                return;
            }
            var theEvent = (OnUpdateBagsEvent)hubEvent;
            await _scopedConnectionManager.UpdateScopedConnectionBags(theEvent.RaiseHub, theEvent.Bags).ConfigureAwait(false);
        }
    }
}
