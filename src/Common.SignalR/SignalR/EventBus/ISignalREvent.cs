using System;

// ReSharper disable once CheckNamespace
namespace Common.SignalR
{
    public interface ISignalREvent
    {
        /// <summary>
        /// 触发事件的时间
        /// </summary>
        DateTime RaiseAt { get; }
    }
}