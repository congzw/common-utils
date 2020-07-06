using Common.SignalR.EventBus;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.ClientMonitors.Connections
{
    public class KickClientEvent : ScopedHubEvent
    {
        public KickClient Args { get; }

        public KickClientEvent(Hub raiseHub, KickClient args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public KickClientEvent(HubContextWrapper context, KickClient args) : base(context, args.ScopeId)
        {
            Args = args;
        }
    }

    public class KickClient : IScopeClientLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
    }
}
