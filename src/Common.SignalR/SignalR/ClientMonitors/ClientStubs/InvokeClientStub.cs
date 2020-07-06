using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common.SignalR.ClientMonitors.ClientStubs
{
    /// <summary>
    /// 代表埋在客户端的一个方法桩子，供从服务器方主动调用
    /// </summary>
    public interface IClientStub : IHaveBags
    {
        string Method { get; set; }
    }


    public class InvokeClientStub : IScopeKey, IHaveBags
    {
        public InvokeClientStub()
        {
            Bags = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public string ScopeId { get; set; }
        public string Method { get; set; }
        public object MethodArgs { get; set; }
        public IDictionary<string, object> Bags { get; set; }

        public static InvokeClientStub Create(string method, object methodArgs)
        {
            return new InvokeClientStub() { Method = method, MethodArgs = methodArgs};
        }

    }
}