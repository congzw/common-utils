using System;
using System.Collections.Generic;

namespace Common.Scopes
{
    public class ScopeContext : IScopeId
    {
        public string ScopeId { get; set; }

        public IDictionary<string, object> Items { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        
        public object GetItem(string key, object defaultValue = null)
        {
            return !this.Items.ContainsKey(key) ? defaultValue : this.Items[key];
        }

        public ScopeContext SetItem(string key, object value)
        {
            this.Items[key] = value;
            return this;
        }
        
        #region for ut & di extensions

        public static ScopeContext GetScopeContext(string scopeId, bool createIfNotExist = true)
        {
            var scopeService = Resolve();
            return scopeService.GetScopeContext(scopeId, createIfNotExist);
        }

        private static readonly Lazy<IScopeService> LazyInstance = new Lazy<IScopeService>(() => new ScopeService());
        public static Func<IScopeService> Resolve { get; set; } = () => LazyInstance.Value;

        #endregion
    }
}
