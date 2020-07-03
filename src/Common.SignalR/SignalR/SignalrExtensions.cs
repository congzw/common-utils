using Common.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR
{
    public static class SignalRExtensions
    {
        public static HttpContext TryGetHttpContext(this Hub hub)
        {
            return hub?.Context?.GetHttpContext();
        }

        public static string TryGetScopeId(this Hub hub)
        {
            return hub?.TryGetHttpContext().TryGetQueryParameterValue(HubConst.Args_ScopeId, HubConst.DefaultScopeId);
        }

        public static string TryGetClientId(this Hub hub)
        {
            return hub?.TryGetHttpContext().TryGetQueryParameterValue(HubConst.Args_ClientId, string.Empty);
        }

        public static string TryGetConnectionId(this Hub hub)
        {
            return hub?.TryGetHttpContext().TryGetQueryParameterValue(HubConst.Args_Id, string.Empty);
        }
    }
}
