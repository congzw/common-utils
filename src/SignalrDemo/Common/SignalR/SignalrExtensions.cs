using Common.Http;
using Microsoft.AspNetCore.SignalR;

// ReSharper disable CheckNamespace

namespace Common.SignalR
{
    public static class SignalRExtensions
    {
        public static T TryGetQueryParameterValue<T>(this Hub hub, string queryParameterName, T defaultValue = default(T))
        {
            if (hub == null)
            {
                return defaultValue;
            }

            if (hub.Context == null)
            {
                return defaultValue;
            }

            var httpContext = hub.Context.GetHttpContext();
            if (httpContext == null)
            {
                return defaultValue;
            }

            return httpContext.TryGetQueryParameterValue(queryParameterName, defaultValue);
        }
    }
}
