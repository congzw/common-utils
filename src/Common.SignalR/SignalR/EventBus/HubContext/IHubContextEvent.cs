// ReSharper disable once CheckNamespace
namespace Common.SignalR
{
    public interface IHubContextEvent : ISignalREvent
    {
        //hub外部使用
        HubContextWrapper Context { get; }
    }
}