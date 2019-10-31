using System.Threading.Tasks;

namespace Common.SignalR.ClientMonitors.Groups
{
    public interface IClientGroupManager
    {
        Task AddToGroup(AddToGroup args);
        Task RemoveFromGroup(RemoveFromGroup args);
    }
}