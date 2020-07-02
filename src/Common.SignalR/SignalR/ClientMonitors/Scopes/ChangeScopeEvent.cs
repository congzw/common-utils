using Microsoft.AspNetCore.SignalR;
// ReSharper disable CheckNamespace

namespace Common.SignalR.ClientMonitors
{
    public class ChangeScopeEvent : BaseHubCrossEvent
    {
        public ChangeScope Args { get; set; }

        public ChangeScopeEvent(Hub raiseHub, ChangeScope args) : base(raiseHub)
        {
            Args = args;
        }

        public ChangeScopeEvent(HubContextWrapper context) : base(context)
        {
        }
    }
    public class ChangeScope : IScopeKey
    {
        public string ScopeId { get; set; }
    }
}