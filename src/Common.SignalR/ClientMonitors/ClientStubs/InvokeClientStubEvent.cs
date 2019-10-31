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
}