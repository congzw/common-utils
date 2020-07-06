using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common.SignalR
{
    public class ScopeContext : IScopeKey, IHaveBags
    {
        public string ScopeId { get; set; }
        public IDictionary<string, object> Bags { get; set; } = BagsHelper.Create();

        public object GetItem(string key, object defaultValue = null)
        {
            return !this.Bags.ContainsKey(key) ? defaultValue : this.Bags[key];
        }

        public ScopeContext SetItem(string key, object value)
        {
            this.Bags[key] = value;
            return this;
        }

        #region for ut & di extensions

        public static ScopeContext GetScopeContext(string scopeId, bool createIfNotExist = true)
        {
            var scopeService = Resolve();
            return scopeService.GetScopeContext(scopeId, createIfNotExist);
        }

        private static readonly Lazy<IScopeRepository> LazyInstance = new Lazy<IScopeRepository>(() => new ScopeRepository());
        public static Func<IScopeRepository> Resolve { get; set; } = () => LazyInstance.Value;

        #endregion
    }

    #region for extensions

    public interface IScopeRepository
    {
        ScopeContext GetScopeContext(string scopeId, bool createIfNotExist);
        void SetScopeContext(ScopeContext scopeContext);
        void RemoveScopeContext(string scopeId);
        void ClearAll();
    }
    public class ScopeRepository : IScopeRepository
    {
        //default use memory dictionary impl, can also be replaced by other impl such as database source...
        public IDictionary<string, ScopeContext> Contexts { get; set; } = new ConcurrentDictionary<string, ScopeContext>(StringComparer.OrdinalIgnoreCase);

        public ScopeContext GetScopeContext(string scopeId, bool createIfNotExist)
        {
            if (!Contexts.ContainsKey(scopeId))
            {
                if (!createIfNotExist)
                {
                    return null;
                }

                var scopeContext = new ScopeContext { ScopeId = scopeId };
                Contexts[scopeId] = scopeContext;
            }
            return Contexts[scopeId];
        }

        public void SetScopeContext(ScopeContext scopeContext)
        {
            if (scopeContext == null)
            {
                throw new ArgumentNullException(nameof(scopeContext));
            }
            Contexts[scopeContext.ScopeId] = scopeContext;
        }

        public void RemoveScopeContext(string scopeId)
        {
            if (!Contexts.ContainsKey(scopeId))
            {
                return;
            }
            Contexts.Remove(scopeId);
        }

        public void ClearAll()
        {
            Contexts.Clear();
        }
    }
    public static class ScopeExtensions
    {
        public static T GetItemAs<T>(this ScopeContext ctx, string key, T defaultValue = default(T))
        {
            var value = ctx.GetItem(key, defaultValue);
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }

    #endregion
}