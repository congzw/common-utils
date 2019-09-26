using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

// ReSharper disable CheckNamespace

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
        Hub RaiseHub { get; }
    }
    public interface IHubContextEvent : ISignalREvent
    {
        MyHubContext Context { get; }
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
        protected BaseHubContextEvent(MyHubContext hubContext)
        {
            RaiseAt = DateHelper.Instance.GetDateNow();
            Context = hubContext;
        }
        public DateTime RaiseAt { get; private set; }
        public MyHubContext Context { get; private set; }
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

    public class MyHubContext
    {
        public IHubClients Clients { get; set; }
        public IGroupManager Groups { get; set; }
    }

    public static class HubContextExtensions
    {
        public static MyHubContext AsMyHubContext<THub>(this IHubContext<THub> context) where THub : Hub
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var hubContext = new MyHubContext();
            hubContext.Clients = context.Clients;
            hubContext.Groups = context.Groups;
            return hubContext;
        }
    }

    public class HubEventHandleOrders
    {
        public float Forward()
        {
            return -100;
        }

        public float Middle()
        {
            return 0;
        }

        public float Backward()
        {
            return 100;
        }

        public float Between(float one, float two)
        {
            return (one + two) / 2;
        }

        public static HubEventHandleOrders Instance = new HubEventHandleOrders();
    }
}
