using System.Threading.Tasks;

namespace Common.SignalR.ClientMonitors.Connections
{
    public interface IClientConnectionManager
    {
        Task OnConnected(OnConnectedEvent theEvent);
        Task OnDisconnected(OnDisconnectedEvent theEvent);
        Task KickClient(KickClientEvent theEvent);
        //Task ScopeReset(ScopeResetEvent theEvent);
        //Task ScopeUpdate(ScopeUpdateEvent theEvent);
    }

    #region events
    
    //public class ScopeResetEvent : BaseHubCrossEvent
    //{
    //    public ScopeContext Args { get; }

    //    public ScopeResetEvent(Hub raiseHub, ScopeContext args) : base(raiseHub, args.ScopeId)
    //    {
    //        Args = args;
    //    }

    //    public ScopeResetEvent(HubContextWrapper context, ScopeContext args) : base(context, args.ScopeId)
    //    {
    //        Args = args;
    //    }
    //}

    //public class ScopeUpdateEvent : BaseHubCrossEvent
    //{
    //    public ScopeContext Args { get; set; }

    //    public ScopeUpdateEvent(Hub raiseHub, ScopeContext args) : base(raiseHub, args.ScopeId)
    //    {
    //        Args = args;
    //    }

    //    public ScopeUpdateEvent(HubContextWrapper context, ScopeContext args) : base(context, args.ScopeId)
    //    {
    //    }
    //}

    #endregion
}