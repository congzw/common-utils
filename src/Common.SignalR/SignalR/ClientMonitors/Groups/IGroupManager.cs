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

        //todo 双屏： 独立，同步，交替，协作？
        //todo 双屏.独立，双屏.同步，双屏.交替，双屏.协作？
        //todo 双屏.独立，双屏.PPT，双屏.协作 + [同步，交替]？
        //todo 控制： 正常，锁定
        //todo 白板： 正常，锁定
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

    public class KnownGroup
    {
        //双屏组
        public static string DoubleScreens = "DoubleScreens";
        //遥控组
        public static string RemoteControl = "RemoteControl";
        //分享屏幕组
        public static string ShareScreen = "ShareScreen";
        //标注
        public static string Mark = "Mark";

        //for extensions
        public static KnownGroup Ext = new KnownGroup();
    }
}