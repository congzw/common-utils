using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.ClientMonitors.ClientGroups
{
    public class AddToGroupEvent : BaseHubCrossEvent
    {
        public AddToGroup Args { get; set; }

        public AddToGroupEvent(Hub raiseHub, AddToGroup args) : base(raiseHub)
        {
            Args = args;
        }

        public AddToGroupEvent(HubContextWrapper context, AddToGroup args) : base(context)
        {
            Args = args;
        }
    }

    public class AddToGroup
    {
        public AddToGroup()
        {
            Items = new List<ScopeClientGroupLocate>();
        }
        public IList<ScopeClientGroupLocate> Items { get; set; }
    }
}