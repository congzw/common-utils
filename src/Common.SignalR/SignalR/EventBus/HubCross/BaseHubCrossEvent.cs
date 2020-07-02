using System;
using Microsoft.AspNetCore.SignalR;

// ReSharper disable once CheckNamespace
namespace Common.SignalR
{
    public abstract class BaseHubCrossEvent : IHubCrossEvent
    {
        protected BaseHubCrossEvent(Hub raiseHub)
        {
            RaiseAt = DateHelper.Instance.GetDateNow();
            RaiseHub = raiseHub;
        }
        protected BaseHubCrossEvent(HubContextWrapper context)
        {
            RaiseAt = DateHelper.Instance.GetDateNow();
            Context = context;
        }

        public DateTime RaiseAt { get; }
        public Hub RaiseHub { get; }
        public HubContextWrapper Context { get; }
    }
}