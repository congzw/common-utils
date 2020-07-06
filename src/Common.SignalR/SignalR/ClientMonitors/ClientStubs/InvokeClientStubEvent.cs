using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

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

            var hubClients = theEvent.TryGetHubClients();
            //var proxy = hubClients.GetClientProxy("", new List<string>());
            //IClientProxy clientProxy = hubClients.Clients(new List<string>());
            //hubClients.Clients("");
            
            Trace.WriteLine(string.Format("[_AnyHub] {0} >>>>>>>> {1}", "InvokeClientStub", JsonConvert.SerializeObject(theEvent.Args, Formatting.None)));
            await hubClients.All.SendAsync("InvokeClientStub", theEvent.Args);

            ////await _clientMethodInvokeBus.Process(theEvent.Args).ConfigureAwait(false);
            ////todo
            //await theEvent.RaiseHub.Clients.All.SendAsync("InvokeClientStub", theEvent.Args);
        }
    }
}