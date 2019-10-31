using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.SignalR.ClientMonitors.Groups
{
    public interface IClientGroupManager
    {
        Task AddToGroup(AddToGroup args);
        Task RemoveFromGroup(RemoveFromGroup args);
        Task<IList<ScopeClientGroup>> GetGroups(IScopeClientGroupLocate args);
    }

    public class ScopeClientGroup : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
    }

    public interface IScopeClientGroupRepository
    {
        IEnumerable<ScopeClientGroup> QueryScopeClientGroups(IScopeClientGroupLocate args);
        ScopeClientGroup GetScopeClientGroup(IScopeClientGroupLocate args);
        void Add(ScopeClientGroup scopeClientGroup);
        void Remove(ScopeClientGroup scopeClientGroup);
    }

    public class GetScopeClientGroupsArgs : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
    }
}