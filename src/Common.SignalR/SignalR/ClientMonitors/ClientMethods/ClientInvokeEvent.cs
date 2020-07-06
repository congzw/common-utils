using Common.SignalR.EventBus;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.ClientMonitors.ClientMethods
{
    public class ClientMethodInvokeEvent : ScopedHubEvent
    {
        public ClientMethodInvoke Args { get; }

        public ClientMethodInvokeEvent(Hub raiseHub, ClientMethodInvoke args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public ClientMethodInvokeEvent(HubContextWrapper context, ClientMethodInvoke args) : base(context, args.ScopeId)
        {
            Args = args;
        }
    }
}
