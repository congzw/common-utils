using Microsoft.AspNetCore.SignalR;

// ReSharper disable once CheckNamespace
namespace Common.SignalR
{
    public interface IHubEvent : ISignalREvent
    {
        //hub内部使用
        Hub RaiseHub { get; }
    }
}