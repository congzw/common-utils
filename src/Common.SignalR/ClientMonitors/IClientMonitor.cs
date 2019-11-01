using Common.SignalR.ClientMonitors.ClientMethods;
using Common.SignalR.ClientMonitors.ClientStubs;
using Common.SignalR.ClientMonitors.Connections;
using Common.SignalR.ClientMonitors.Groups;

namespace Common.SignalR.ClientMonitors
{
    public interface IClientMonitor : IClientConnectionManager, IScopeGroupManager, IClientGroupManager, IClientMethodManager, IClientStubManager
    {
    }
}