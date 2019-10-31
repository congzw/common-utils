using System.Collections.Generic;
using System.Linq;
using Common.SignalR.Refs;
using Common.SignalR.Refs.Http;
using Microsoft.AspNetCore.Http;

namespace Common.SignalR.ClientMonitors.Connections
{
    public interface IClientLocate : IScopeClientKey
    {
        //todo add more specific prop by need
    }
    public class ClientLocate : IClientLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }

        public static ClientLocate Create()
        {
            return new ClientLocate();
        }
    }
    public static class ClientLocateExtensions
    {
        public static bool SameLocateKey(this IClientLocate locate, IClientLocate locate2)
        {
            if (locate == null || locate2 == null)
            {
                return false;
            }

            return locate.ScopeId.MyEquals(locate2.ScopeId)
                   && locate.ClientId.MyEquals(locate2.ClientId);
        }

        public static T Locate<T>(this IEnumerable<T> locates, IClientLocate locate) where T : IClientLocate
        {
            if (locates == null || locate == null)
            {
                return default(T);
            }
            return locates.FirstOrDefault(x => locate.ScopeId.MyEquals(x.ScopeId) && locate.ClientId.MyEquals(x.ClientId));
        }

        public static T WithScopeId<T>(this T scopeKey, string scopeId) where T : IScopeKey
        {
            if (scopeKey == null)
            {
                //throw new ArgumentNullException(nameof(scopedClientKey));
                return default(T);
            }
            scopeKey.ScopeId = scopeId;
            return scopeKey;
        }
        public static T WithClientId<T>(this T clientKey, string clientId) where T : IClientKey
        {
            if (clientKey == null)
            {
                //throw new ArgumentNullException(nameof(scopedClientKey));
                return default(T);
            }
            clientKey.ClientId = clientId;
            return clientKey;
        }

        public static T TryAutoSetScopeId<T>(this HttpContext httpContext, T scopedClientKey) where T : IScopeClientKey
        {
            if (httpContext == null)
            {
                //throw new ArgumentNullException(nameof(scopedClientKey));
                return scopedClientKey;
            }

            var scopeGroupId = httpContext.TryGetQueryParameterValue(nameof(scopedClientKey.ScopeId), string.Empty);
            if (!string.IsNullOrWhiteSpace(scopeGroupId))
            {
                scopedClientKey.WithScopeId(scopeGroupId);
            }
            return scopedClientKey;
        }
        public static T TryAutoSetClientId<T>(this HttpContext httpContext, T scopedClientKey) where T : IClientKey
        {
            if (httpContext == null)
            {
                //throw new ArgumentNullException(nameof(scopedClientKey));
                return scopedClientKey;
            }

            var clientId = httpContext.TryGetQueryParameterValue(nameof(scopedClientKey.ClientId), string.Empty);
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                scopedClientKey.WithClientId(clientId);
            }
            return scopedClientKey;
        }
        public static T TryAutoSetScopeClientKey<T>(this HttpContext httpContext, T scopedClientKey) where T : IScopeClientKey
        {
            if (httpContext == null)
            {
                //throw new ArgumentNullException(nameof(scopedClientKey));
                return scopedClientKey;
            }

            httpContext.TryAutoSetClientId(scopedClientKey);
            httpContext.TryAutoSetScopeId(scopedClientKey);
            return scopedClientKey;
        }
    }
}
