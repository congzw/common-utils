using System;
using System.Collections.Generic;

namespace Common
{
    public interface ISimpleEventBus<T> where T : ISimpleEvent
    {
        void Register(Action<T> eventAction);
        void ClearActions();
        void RemoveRegister(Action<T> eventAction);
        void Raise(T args);
        Func<T, bool> ShouldRaise { get; set; }
    }

    public class SimpleEventBus<T> : ISimpleEventBus<T> where T : ISimpleEvent
    {
        public SimpleEventBus()
        {
            Actions = new List<Action<T>>();
            ShouldRaise = ShouldRaiseThisEvent;
        }

        public void Register(Action<T> eventAction)
        {
            if (Actions.Contains(eventAction))
            {
                return;
            }
            Actions.Add(eventAction);
        }

        public void ClearActions()
        {
            Actions.Clear();
        }

        public void RemoveRegister(Action<T> eventAction)
        {
            if (!Actions.Contains(eventAction))
            {
                return;
            }
            Actions.Remove(eventAction);
        }

        public void Raise(T args)
        {
            if (!ShouldRaise(args))
            {
                return;
            }

            foreach (var action in Actions)
            {
                action(args);
            }
        }

        public IList<Action<T>> Actions { get; set; }

        public Func<T, bool> ShouldRaise { get; set; }

        private bool ShouldRaiseThisEvent(T theEvent)
        {
            if (theEvent == null)
            {
                return false;
            }
            return true;
        }
        
        #region for di extensions

        private static readonly Lazy<ISimpleEventBus<T>> LazyInstance = new Lazy<ISimpleEventBus<T>>(() => new SimpleEventBus<T>());
        public static Func<ISimpleEventBus<T>> Resolve { get; set; } = () => LazyInstance.Value;

        #endregion
    }

    public interface ISimpleEvent
    {
        /// <summary>
        /// 事件发生的时间
        /// </summary>
        DateTime DateTimeEventOccurred { get; }

        //暂不考虑诸如此类的复杂情况
        ///// <summary>
        ///// 是否支持取消
        ///// </summary>
        //bool SupportCancel { get; }
    }
    
    public abstract class BaseSimpleEvent : ISimpleEvent
    {
        public DateTime DateTimeEventOccurred { get; private set; }

        protected BaseSimpleEvent()
        {
            DateTimeEventOccurred = DateTime.Now;
        }
    }

    #region demo for async message event

    public class AsyncMessageEvent : BaseSimpleEvent
    {
        public AsyncMessageEvent(string message)
        {
            Message = message;
        }
        public string Message { get; set; }
    }

    #endregion
}
