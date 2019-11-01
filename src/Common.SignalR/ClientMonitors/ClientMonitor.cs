using System.Threading.Tasks;
using Common.SignalR.ClientMonitors.Connections;

namespace Common.SignalR.ClientMonitors
{
    public class ClientMonitor : IClientMonitor
    {
        private readonly IClientConnectionRepository _clientConnectionRepository;

        public ClientMonitor(IClientConnectionRepository clientConnectionRepository)
        {
            _clientConnectionRepository = clientConnectionRepository;
        }

        public Task OnConnected(OnConnectedEvent theEvent)
        {
            return Task.CompletedTask;
        }

        public Task OnDisconnected(OnDisconnectedEvent theEvent)
        {
            return Task.CompletedTask;
        }

        public Task KickClient(KickClientEvent theEvent)
        {
            return Task.CompletedTask;
        }
    }
}
