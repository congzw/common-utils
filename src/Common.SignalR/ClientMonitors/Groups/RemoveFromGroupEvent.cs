using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.ClientMonitors.Groups
{
    public class RemoveFromGroupEvent : BaseHubEvent
    {
        public RemoveFromGroup Args { get; set; }

        public RemoveFromGroupEvent(Hub raiseHub, RemoveFromGroup args) : base(raiseHub)
        {
            Args = args;
        }
    }

    public class RemoveFromGroup
    {
        public RemoveFromGroup()
        {
            Items = new List<ScopeClientGroupLocate>();
        }
        public IList<ScopeClientGroupLocate> Items { get; set; }
    }
}
