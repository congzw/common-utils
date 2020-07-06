using System.Threading.Tasks;
using Common.SignalR.EventBus;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.ClientMonitors.Scopes
{
    public interface IScopeManager
    {
        Task ScopeUpdate(ScopeUpdateEvent theEvent);
        Task ScopeReset(ScopeResetEvent theEvent);
    }

    #region events
    
    //clear & update
    public class ScopeResetEvent : ScopedHubEvent
    {
        public ScopeContext Args { get; }

        public ScopeResetEvent(Hub raiseHub, ScopeContext args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public ScopeResetEvent(HubContextWrapper context, ScopeContext args) : base(context, args.ScopeId)
        {
            Args = args;
        }
    }

    //update
    public class ScopeUpdateEvent : ScopedHubEvent
    {
        public ScopeContext Args { get; set; }

        public ScopeUpdateEvent(Hub raiseHub, ScopeContext args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public ScopeUpdateEvent(HubContextWrapper context, ScopeContext args) : base(context, args.ScopeId)
        {
        }
    }

    #endregion
}
