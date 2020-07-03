using System.Collections.Generic;

namespace Common.SignalR.ClientMonitors.ClientGroups
{
    public interface IScopeClientGroupRepository
    {
        IEnumerable<ScopeClientGroup> QueryScopeClientGroups(IScopeClientGroupLocate args);
        ScopeClientGroup GetScopeClientGroup(IScopeClientGroupLocate args);
        void Add(ScopeClientGroup scopeClientGroup);
        void Remove(ScopeClientGroup scopeClientGroup);
    }
}