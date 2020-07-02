using System;

// ReSharper disable once CheckNamespace
namespace Common.SignalR
{
    public abstract class BaseHubContextEvent : IHubContextEvent
    {
        protected BaseHubContextEvent(HubContextWrapper hubContext)
        {
            RaiseAt = DateHelper.Instance.GetDateNow();
            Context = hubContext;
        }
        public DateTime RaiseAt { get; private set; }
        public string ClientId { get; set; }
        public HubContextWrapper Context { get; private set; }
    }
}