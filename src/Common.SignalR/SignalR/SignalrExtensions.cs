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
    }
}
