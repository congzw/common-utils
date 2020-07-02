using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.ClientMonitors.ClientStubs
{
    public class InvokeClientStubEvent : BaseHubCrossEvent
    {
        public InvokeClientStub Args { get; set; }

        public InvokeClientStubEvent(Hub raiseHub, InvokeClientStub args) : base(raiseHub)
        {
            Args = args;
        }

        public InvokeClientStubEvent(HubContextWrapper context, InvokeClientStub args) : base(context)
        {
            Args = args;
        }
    }
    public class InvokeClientStubEventHandler : IHubCrossEventHandler
    {
        public float HandleOrder { get; set; }
        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is InvokeClientStubEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            if (!ShouldHandle(@event))
            {
                return;
            }

            var theEvent = (InvokeClientStubEvent)@event;
            //await _clientMethodInvokeBus.Process(theEvent.Args).ConfigureAwait(false);

            //todo const
            await theEvent.RaiseHub.Clients.All.SendAsync("InvokeClientStub", theEvent.Args);
        }
    }
}