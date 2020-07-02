using System.Threading.Tasks;

namespace Common.SignalR.ClientMonitors.ClientStubs
{
    public interface IClientStubManager
    {
        Task InvokeClientStub(InvokeClientStubEvent theEvent);
    }
}