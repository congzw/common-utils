using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Common.SignalR.ClientMonitors.Connections;

namespace Common.SignalR.ClientMonitors.ClientMethods
{
    public interface IClientMethod
    {
        string Method { get; set; }
        object[] MethodArgs { get; set; }
        IDictionary<string, object> Bags { get; set; }
    }

    public interface IClientMethodInvoke : IClientMethod, IClientLocate
    {
    }

    public class ClientMethodInvoke : IClientMethodInvoke
    {
        public ClientMethodInvoke()
        {
            MethodArgs = new List<object>().ToArray();
            Bags = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Method { get; set; }
        public object[] MethodArgs { get; set; }
        public IDictionary<string, object> Bags { get; set; }
    }
}