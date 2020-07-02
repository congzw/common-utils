using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.ClientMonitors.Connections
{
    public class KickClientEvent : BaseHubCrossEvent
    {
        public KickClient Args { get; }

        public KickClientEvent(Hub raiseHub, KickClient args) : base(raiseHub)
        {
            Args = args;
        }

        public KickClientEvent(HubContextWrapper context, KickClient args) : base(context)
        {
            Args = args;
        }
    }

    public class KickClient
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
    }
}
