using System.Threading.Tasks;

namespace Common.SignalR.ClientMonitors.ClientMethods
{
    public interface IClientMethodManager
    {
        Task ClientMethodInvoke(ClientMethodInvokeEvent theEvent);
    }
}