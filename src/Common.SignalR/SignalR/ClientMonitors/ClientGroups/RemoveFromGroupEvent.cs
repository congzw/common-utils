using System.Collections.Generic;
using Common.SignalR.EventBus;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.ClientMonitors.ClientGroups
{
    public class RemoveFromGroupEvent : ScopedHubEvent
    {
        public RemoveFromGroup Args { get; set; }

        public RemoveFromGroupEvent(Hub raiseHub, RemoveFromGroup args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }
    }

    public class RemoveFromGroup : IScopeKey
    {
        public RemoveFromGroup()
        {
            Items = new List<ScopeClientGroupLocate>();
        }
        public string ScopeId { get; set; }
        public IList<ScopeClientGroupLocate> Items { get; set; }
    }
}
