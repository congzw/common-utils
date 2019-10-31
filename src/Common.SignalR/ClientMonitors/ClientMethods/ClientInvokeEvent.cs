using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Common.SignalR.ClientMonitors.Connections;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;

namespace Common.SignalR.ClientMonitors.ClientMethods
{
    public class ClientMethodInvokeEvent : BaseHubCrossEvent
    {
        public ClientMethodInvoke Args { get; }

        public ClientMethodInvokeEvent(Hub raiseHub, ClientMethodInvoke args) : base(raiseHub)
        {
            Args = args;
        }

        public ClientMethodInvokeEvent(HubContextWrapper context, ClientMethodInvoke args) : base(context)
        {
            Args = args;
        }
    }
    
    public interface IClientMethod
    {
        string Method { get; set; }
        IDictionary<string, object> Bags { get; set; }
    }

    public interface IClientMethodInvoke : IClientMethod, IClientLocate
    {
    }

    public class ClientMethodInvoke : IClientMethodInvoke
    {
        public ClientMethodInvoke()
        {
            Bags = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Method { get; set; }
        public JObject MethodArgs { get; set; }
        public IDictionary<string, object> Bags { get; set; }
    }
}
