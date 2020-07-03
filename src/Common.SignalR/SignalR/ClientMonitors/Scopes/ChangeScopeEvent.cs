using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
// ReSharper disable CheckNamespace

namespace Common.SignalR.ClientMonitors
{
    public class ChangeScopeEvent : BaseHubCrossEvent
    {
        public ChangeScopeArgs Args { get; set; }

        public ChangeScopeEvent(Hub raiseHub, ChangeScopeArgs args) : base(raiseHub)
        {
            Args = args;
        }

        public ChangeScopeEvent(HubContextWrapper context) : base(context)
        {
        }
    }

    public class ChangeScopeArgs : IScopeKey, IHaveBags
    {
        public string ScopeId { get; set; }
        public IDictionary<string, object> Bags { get; set; } = BagsHelper.Create();
    }
}