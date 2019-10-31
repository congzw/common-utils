using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.SignalR.ClientMonitors.Groups
{
    public interface IGroupManager
    {
        Task<IList<ScopeGroup>> GetGroups(IScopeGroupLocate args);
        Task<ScopeGroup> GetGroup(IScopeGroupLocate args);
        Task AddGroup(AddGroup args);
        Task RemoveGroup(RemoveGroup args);
    }
    
    public class ScopeGroup : IScopeGroupLocate
    {
        public string ScopeId { get; set; }
        public string Group { get; set; }

        //todo 双屏： 独立，同步，交替，协作？
        //todo 双屏.独立，双屏.同步，双屏.交替，双屏.协作？
        //todo 双屏.独立，双屏.PPT，双屏.协作 + [同步，交替]？
        //todo 控制： 正常，锁定
        //todo 白板： 正常，锁定
        public string State { get; set; }
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