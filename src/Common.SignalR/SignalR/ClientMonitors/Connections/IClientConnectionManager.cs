using System.Threading.Tasks;

namespace Common.SignalR.ClientMonitors.Connections
{
    public interface IClientConnectionManager
    {
        Task OnConnected(OnConnectedEvent theEvent);
        Task OnDisconnected(OnDisconnectedEvent theEvent);
        Task KickClient(KickClientEvent theEvent);
    }
}