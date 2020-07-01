using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common.Scopes
{
    public class MockRepository : IScopeContextRepository
    {
        public IDictionary<string, ScopeContext> Contexts { get; set; } = new ConcurrentDictionary<string, ScopeContext>(StringComparer.OrdinalIgnoreCase);
        public MockRepository(bool seed)
        {
            if (seed)
            {
                var scopeContext = new ScopeContext();
                Contexts["001"] = scopeContext;
                scopeContext.Items["foo"] = "FOO";
                scopeContext.Items["isFoo"] = true;
            }
        }


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
}