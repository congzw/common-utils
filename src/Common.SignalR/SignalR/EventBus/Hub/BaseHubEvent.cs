using System;
using Microsoft.AspNetCore.SignalR;

// ReSharper disable once CheckNamespace
namespace Common.SignalR
{
    public abstract class BaseHubEvent : IHubEvent
    {
        protected BaseHubEvent(Hub raiseHub)
        {
            RaiseAt = DateHelper.Instance.GetDateNow();
            RaiseHub = raiseHub;
        }

        public DateTime RaiseAt { get; private set; }
        public Hub RaiseHub { get; private set; }
    }
}