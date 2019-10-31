using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.ClientMonitors.ClientMethods
{
    public class ClientMethodInvokeEvent : BaseHubCrossEvent
    {
        public ClientMethodInvoke Args { get; }

        public ClientMethodInvokeEvent(Hub raiseHub, ClientMethodInvoke args) : base(raiseHub)
        {
            Args = args;
        }

        public ClientMethodInvokeEvent(HubContextWrapper context, ClientMethodInvoke args) : base(context)
        {
            Args = args;
        }
    }
}
