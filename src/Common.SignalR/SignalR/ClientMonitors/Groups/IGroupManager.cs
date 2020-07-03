using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.SignalR.ClientMonitors.Groups
{
    public interface IScopeGroupManager
    {
        Task<IList<ScopeGroup>> GetGroups(IScopeGroupLocate args);
        Task<ScopeGroup> GetGroup(IScopeGroupLocate args);
        Task AddGroup(AddGroup args);
        Task RemoveGroup(RemoveGroup args);
    }
    
    public class ScopeGroup : IScopeGroupLocate, IHaveBags
    {
        public ScopeGroup()
        {
            Bags = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public string ScopeId { get; set; }
        public string Group { get; set; }
        public IDictionary<string, object> Bags { get; set; }
    }

    public class AddGroup
    {
        public AddGroup()
        {
            Items = new List<ScopeGroup>();
        }
        public IList<ScopeGroup> Items { get; set; }
    }

    public class RemoveGroup
    {
        public RemoveGroup()
        {
            Items = new List<ScopeGroupLocate>();
        }
        public IList<ScopeGroupLocate> Items { get; set; }
    }
}