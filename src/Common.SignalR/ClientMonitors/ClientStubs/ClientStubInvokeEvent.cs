using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Common.SignalR.Refs;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.ClientMonitors.ClientStubs
{
    //代表埋在客户端的一个方法桩子，供从服务器方主动调用
    public interface IClientStub : IHaveBags
    {
        string Method { get; set; }
    }

    public interface IInvokeClientStub
    {
        //todo add some spec by need
    }

    public class InvokeClientStub : IInvokeClientStub, IHaveBags
    {
        public InvokeClientStub()
        {
            Bags = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public string Method { get; set; }
        public IDictionary<string, object> Bags { get; set; }

        public static InvokeClientStub Create(string method)
        {
            return new InvokeClientStub() { Method = method };
        }
    }

    public class InvokeClientStubEvent : BaseHubCrossEvent
    {
        public InvokeClientStub Args { get; set; }

        public InvokeClientStubEvent(Hub raiseHub, InvokeClientStub args) : base(raiseHub)
        {
            Args = args;
        }

        public InvokeClientStubEvent(HubContextWrapper context, InvokeClientStub args) : base(context)
        {
            Args = args;
        }
    }
}