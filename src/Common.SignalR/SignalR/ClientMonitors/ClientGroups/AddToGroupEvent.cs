using System.Collections.Generic;
using Common.SignalR.EventBus;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.ClientMonitors.ClientGroups
{
    public class AddToGroupEvent : ScopedHubEvent
    {
        public AddToGroup Args { get; set; }

        public AddToGroupEvent(Hub raiseHub, AddToGroup args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public AddToGroupEvent(HubContextWrapper context, AddToGroup args) : base(context, args.ScopeId)
        {
            Args = args;
        }
    }

    public class AddToGroup : IScopeKey
    {
        public AddToGroup()
        {
            Items = new List<ScopeClientGroupLocate>();
        }
        public string ScopeId { get; set; }
        public IList<ScopeClientGroupLocate> Items { get; set; }
    }
}