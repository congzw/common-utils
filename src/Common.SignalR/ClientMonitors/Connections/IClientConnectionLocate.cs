using System.Collections.Generic;
using System.Linq;
using Common.SignalR.Refs;

namespace Common.SignalR.ClientMonitors.Connections
{
    public interface IClientConnectionLocate : IScopeClientKey
    {
        //ScopeId, ClientId => ConnectionId
        string ConnectionId { get; set; }
    }
    public static class ClientConnectionLocateExtensions
    {
        public static bool SameLocateKey(this IClientConnectionLocate locate, IClientConnectionLocate locate2)
        {
            if (locate == null || locate2 == null)
            {
                return false;
            }

            return locate.ScopeId.MyEquals(locate2.ScopeId)
                   && locate.ClientId.MyEquals(locate2.ClientId)
                   && locate.ConnectionId.MyEquals(locate2.ConnectionId);
        }

        public static T Locate<T>(this IEnumerable<T> locates, IClientConnectionLocate locate) where T : IClientConnectionLocate
        {
            if (locates == null || locate == null)
            {
                return default(T);
            }

            if (!string.IsNullOrWhiteSpace(locate.ConnectionId))
            {
                return locates.SingleOrDefault(x => locate.ConnectionId.MyEquals(x.ConnectionId));
            }
            return locates.FirstOrDefault(x => locate.ScopeId.MyEquals(x.ScopeId) && locate.ClientId.MyEquals(x.ClientId));
        }

        public static T WithConnectionId<T>(this T instance, string connectionId) where T : IClientConnectionLocate
        {
            if (instance == null)
            {
                return instance;
            }
            instance.ConnectionId = connectionId;
            return instance;
        }
    }
    public class ClientConnectionLocate : IClientConnectionLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string ConnectionId { get; set; }

        public static ClientConnectionLocate Create()
        {
            return new ClientConnectionLocate();
        }
    }
}