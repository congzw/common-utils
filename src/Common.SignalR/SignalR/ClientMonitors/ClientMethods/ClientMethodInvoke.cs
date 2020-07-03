using System.Collections.Generic;

namespace Common.SignalR.ClientMonitors.ClientMethods
{
    public interface IClientMethod
    {
        string Method { get; set; }
        object[] MethodArgs { get; set; }
        IDictionary<string, object> Bags { get; set; }
    }

    public interface IClientMethodInvoke : IClientMethod, IScopeClientLocate
    {
    }

    public class ClientMethodInvoke : IClientMethodInvoke
    {
        public ClientMethodInvoke()
        {
            MethodArgs = new List<object>().ToArray();
            Bags = BagsHelper.Create();
        }

        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Method { get; set; }
        public object[] MethodArgs { get; set; }
        public IDictionary<string, object> Bags { get; set; }
    }
}