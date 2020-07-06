using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.SignalR.ClientMonitors;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.EventBus
{
    public interface ISignalREvent : IScopeKey, IHaveBags
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

    #region HubContext Wrapper
    
    public class HubContextWrapper
    {
        public IHubClients<IClientProxy> Clients { get; set; }
        public IGroupManager Groups { get; set; }
    }

    public class HubContextWrapper<THub> : HubContextWrapper where THub : Hub
    {
        public IHubContext<THub> HubContext { get; set; }
    }

    public static class HubContextExtensions
    {
        public static HubContextWrapper<THub> AsHubContextWrapper<THub>(this IHubContext<THub> context) where THub : Hub
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var hubContext = new HubContextWrapper<THub>();
            hubContext.Clients = context.Clients;
            hubContext.Groups = context.Groups;
            hubContext.HubContext = context;
            return hubContext;
        }
    }

    #endregion

    #region base events

    public class ScopedHubEvent : IHubEvent, IHubContextEvent
    {
        #region ctors

        public ScopedHubEvent(Hub raiseHub, string scopeId)
        {
            //来自HUB内部的请求，自动修正ScopeId
            if (string.IsNullOrWhiteSpace(scopeId))
            {
                scopeId = raiseHub.GetCurrentScope();
                if (string.IsNullOrWhiteSpace(scopeId))
                {
                    throw new ArgumentNullException(nameof(scopeId));
                }
            }
            ScopeId = scopeId;
            RaiseAt = DateHelper.Instance.GetDateNow();
            RaiseHub = raiseHub;
        }

        public ScopedHubEvent(HubContextWrapper context, string scopeId)
        {
            if (string.IsNullOrWhiteSpace(scopeId))
            {
                throw new ArgumentNullException(nameof(scopeId));
            }
            RaiseAt = DateHelper.Instance.GetDateNow();
            Context = context;
        }

        #endregion

        public string ScopeId { get; set; }
        public IDictionary<string, object> Bags { get; set; }
        public DateTime RaiseAt { get; }
        public Hub RaiseHub { get; }
        public HubContextWrapper Context { get; }

        public bool IsCalledFromHub()
        {
            return RaiseHub != null;
        }
        public bool IsCalledOutsideHub()
        {
            return !IsCalledFromHub();
        }
        public IHubClients<IClientProxy> TryGetHubCallerClients()
        {
            if (RaiseHub != null)
            {
                return RaiseHub.Clients;
            }
            return Context.Clients;
        }
    }
    
    #endregion

    #region event context
    
    public static class SignalREventExtensions
    {
        public static string ShouldWaitComplete = "ShouldWaitComplete";
        public static bool GetShouldWaitComplete(this ISignalREvent @event, ISignalREventHandler handler, bool defaultValue)
        {
            return @event.GetAs(handler, ShouldWaitComplete, defaultValue);
        }
        public static void SetShouldWaitComplete(this ISignalREvent @event, ISignalREventHandler handler, bool shouldWait)
        {
            @event.Set(handler, ShouldWaitComplete, shouldWait);
        }

        public static string ShouldThrowWhenException = "ShouldThrowWhenException";
        public static bool GetShouldThrowWhenException(this ISignalREvent @event, ISignalREventHandler handler, bool defaultValue)
        {
            return @event.GetAs(handler, ShouldThrowWhenException, defaultValue);
        }
        public static void SetShouldThrowWhenException(this ISignalREvent @event, ISignalREventHandler handler, bool shouldThrow)
        {
            @event.Set(handler, ShouldThrowWhenException, shouldThrow);
        }

        public static string ShouldStopSend = "StopSend";
        public static bool GetStopSend(this ISignalREvent @event, ISignalREventHandler handler, bool defaultValue)
        {
            return @event.GetAs(handler, ShouldStopSend, defaultValue);
        }
        public static void SetStopSend(this ISignalREvent @event, ISignalREventHandler handler, bool shouldStop)
        {
            @event.Set(handler, ShouldStopSend, shouldStop);
        }

        //helpers
        private static void Set(this ISignalREvent @event, ISignalREventHandler handler, string key, object value)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            var theKey = string.Format("{0}_{1}", key, handler.GetType().FullName);
            @event.Bags[theKey] = value;
        }
        private static object Get(this ISignalREvent @event, ISignalREventHandler handler, string key, object defaultValue)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var theKey = string.Format("{0}_{1}", key, handler.GetType().FullName);
            if (!@event.Bags.ContainsKey(theKey))
            {
                return defaultValue;
            }
            return @event.Bags[theKey];
        }
        private static T GetAs<T>(this ISignalREvent @event, ISignalREventHandler handler, string key, T defaultValue)
        {
            var theValue = @event.Get(handler, key, defaultValue);
            if (theValue == null)
            {
                return defaultValue;
            }
            return (T)Convert.ChangeType(theValue, typeof(T));
        }
    }
    
    #endregion

    public interface ISignalREventHandler
    {
        float HandleOrder { set; get; }
        bool ShouldHandle(ISignalREvent @event);
        Task HandleAsync(ISignalREvent @event);
    }
}