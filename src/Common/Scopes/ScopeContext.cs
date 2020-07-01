using System;
using System.Collections.Generic;

namespace Common.Scopes
{
    public class ScopeContext : IScopeKey
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
            var repos = ReposFunc();
            return repos.GetScopeContext(scopeId, createIfNotExist);
        }

        private static readonly Lazy<IScopeContextRepository> LazyRepos = new Lazy<IScopeContextRepository>(() => new ScopeContextRepository());
        public static Func<IScopeContextRepository> ReposFunc { get; set; } = () => LazyRepos.Value;

        #endregion
    }
}
