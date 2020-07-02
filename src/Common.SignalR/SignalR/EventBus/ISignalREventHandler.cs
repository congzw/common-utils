using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Common.SignalR
{
    public interface ISignalREventHandler
    {
        float HandleOrder { set; get; }
        bool ShouldHandle(ISignalREvent @event);
        Task HandleAsync(ISignalREvent @event);
    }
}