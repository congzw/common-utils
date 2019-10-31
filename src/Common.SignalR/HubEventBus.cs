using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DateHelper = Common.SignalR.Refs.DateHelper;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR
{
    #region events

    public interface ISignalREvent
    {
        /// <summary>
        /// 触发事件的时间
        /// </summary>
        DateTime RaiseAt { get; }
    }
    public interface IHubEvent : ISignalREvent
    {
        //hub内部使用
        Hub RaiseHub { get; }
    }
    public interface IHubContextEvent : ISignalREvent
    {
        //hub外部使用
        HubContextWrapper Context { get; }
    }

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

    public interface IHubCrossEvent : ISignalREvent, IHubEvent, IHubContextEvent
    {
    }

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

    public static class HubCrossEventExtensions
    {
        public static bool IsOutsideHub(this IHubCrossEvent theEvent)
        {
            return theEvent.Context != null;
        }
    }


    #endregion

    #region handlers

    public interface ISignalREventHandler
    {
        float HandleOrder { set; get; }
        bool ShouldHandle(ISignalREvent @event);
        Task HandleAsync(ISignalREvent @event);
    }

    public interface IHubEventHandler : ISignalREventHandler
    {
    }

    public interface IHubContextEventHandler : ISignalREventHandler
    {
    }

    public interface IHubCrossEventHandler : IHubEventHandler, IHubContextEventHandler, ISignalREventHandler
    {
    }

    #endregion

    public class HubEventBus
    {
        public IEnumerable<ISignalREventHandler> Handlers { get; }

        public HubEventBus(IEnumerable<ISignalREventHandler> signalREventHandlers)
        {
            Handlers = signalREventHandlers ?? Enumerable.Empty<ISignalREventHandler>();
        }

        public async Task Raise(ISignalREvent hubEvent)
        {
            var sortedHandlers = Handlers
                .Where(x => x.ShouldHandle(hubEvent))
                .OrderBy(x => x.HandleOrder)
                .ToList();

            foreach (var handler in sortedHandlers)
            {
                try
                {
                    //todo trace log
                    await handler.HandleAsync(hubEvent).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    //todo log
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
