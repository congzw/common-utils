using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Common.Http;
using Common.SignalR.ClientMonitors.ClientGroups;
using Common.SignalR.ClientMonitors.ClientMethods;
using Common.SignalR.ClientMonitors.ClientStubs;
using Common.SignalR.ClientMonitors.Connections;
using Common.SignalR.EventBus;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Common.SignalR.ClientMonitors
{
    //a demo for how to use ClientMonitors

    public static class HubExtensions
    {
        public static string GetCurrentScope<THub>(this THub hub) where THub : Hub
        {
            if (hub == null)
            {
                throw new ArgumentNullException(nameof(hub));
            }

            //query >= items >= default
            var scope = hub.TryGetHttpContext().TryGetQueryParameterValue(HubConst.Args_ScopeId, (string)null);
            if (scope != null)
            {
                hub.Context.Items[HubConst.Args_ScopeId] = scope;
                return scope;
            }

            if (hub.Context.Items.ContainsKey(HubConst.Args_ScopeId))
            {
                return hub.Context.Items[HubConst.Args_ScopeId] as string;
            }

            hub.Context.Items[HubConst.Args_ScopeId] = HubConst.DefaultScopeId;
            return hub.Context.Items[HubConst.Args_ScopeId] as string;
        }

        public static THub FixScopeIdForContext<THub>(this THub hub, string scopeId = null) where THub : Hub
        {
            if (hub == null)
            {
                throw new ArgumentNullException(nameof(hub));
            }

            if (string.IsNullOrWhiteSpace(scopeId))
            {
                scopeId = hub.TryGetHttpContext().TryGetQueryParameterValue(HubConst.Args_ScopeId, HubConst.DefaultScopeId);
            }
            hub.Context.Items[HubConst.Args_ScopeId] = scopeId;
            return hub;
        }

        public static THub FixScopeIdForArgs<THub>(this THub hub, IScopeKey args) where THub : Hub
        {
            if (string.IsNullOrWhiteSpace(args.ScopeId))
            {
                args.ScopeId = hub.GetCurrentScope();
            }
            return hub;
        }

    }

    public class _AnyHub : Hub
    {
        private readonly SignalREventBus _hubEventBus;

        public _AnyHub(SignalREventBus hubEventBus)
        {
            Trace.WriteLine(string.Format("[_AnyHub] {0} >>>>>>>> {1}", "CTOR", string.Empty));
            _hubEventBus = hubEventBus;
        }

        //连接
        public override async Task OnConnectedAsync()
        {
            this.FixScopeIdForContext();
            TraceHubContext("OnConnectedAsync");
            //[13704] [_AnyHub] OnConnectedAsync >>>>>>>> ?scopeId=s1&clientId=c2&id=gMop-YWYX7zbRWdqJOhyig

            await _hubEventBus.Raise(new OnConnectedEvent(this)).ConfigureAwait(false);
            await base.OnConnectedAsync().ConfigureAwait(false);
        }

        //断开
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            this.FixScopeIdForContext();
            TraceHubContext("OnDisconnectedAsync");
            //[13704][_AnyHub] OnDisconnectedAsync >>>>>>>> ?scopeId=s1&clientId=c2&id=gMop-YWYX7zbRWdqJOhyig

            var reason = exception == null ? "" : exception.Message;
            await _hubEventBus.Raise(new OnDisconnectedEvent(this, reason)).ConfigureAwait(false);
            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }
        
        //踢掉（管理场景）
        public async Task KickClient(KickClient args)
        {
            this.FixScopeIdForContext().FixScopeIdForArgs(args);

            TraceHubContext("KickClient");
            await _hubEventBus.Raise(new KickClientEvent(this, args)).ConfigureAwait(false);
            await base.OnConnectedAsync().ConfigureAwait(false);
        }
        
        //加入组成员
        public Task AddToGroup(AddToGroup args)
        {
            this.FixScopeIdForContext().FixScopeIdForArgs(args);
            return _hubEventBus.Raise(new AddToGroupEvent(this, args));
        }

        //移除组成员
        public Task RemoveFromGroup(RemoveFromGroup args)
        {
            return _hubEventBus.Raise(new RemoveFromGroupEvent(this, args));
        }

        ////Scope的上下文切换
        //public Task ChangeScope(ChangeScopeArgs args)
        //{
        //    return _hubEventBus.Raise(new ChangeScopeEvent(this, args));
        //}
        
        //代表客户端的方法调用，供同步页面等场景使用
        public Task ClientMethodInvoke(ClientMethodInvoke args)
        {
            this.FixScopeIdForContext().FixScopeIdForArgs(args);
            TraceHubContext("ClientMethodInvoke");
            return _hubEventBus.Raise(new ClientMethodInvokeEvent(this, args));
        }

        //代表从服务器端的方法调用，供数据通知等场景使用
        public Task InvokeClientStub(InvokeClientStub args)
        {
            this.FixScopeIdForContext().FixScopeIdForArgs(args);
            TraceHubContext("InvokeClientStub");
            return _hubEventBus.Raise(new InvokeClientStubEvent(this, args));
        }

        private void TraceHubContext(string method)
        {
            var invokeCountKey = "_InvokeCount_";
            var invokeCount = 1;
            if (Context.Items.ContainsKey(invokeCountKey))
            {
                invokeCount = (int)Context.Items[invokeCountKey];
                invokeCount++;
            }
            this.Context.Items[invokeCountKey] = invokeCount;
            
            //基于hub的所有方法调用，都能拿到基于当前的上下文信息参数Query
            //[10664] [_AnyHub] InvokeClientStub >>>>>>>> ?scopeId=s1&clientId=c1&id=hB1kQwNfvF9bu-Tp_cSaig
            //[10664] [_AnyHub] KickClient >>>>>>>> ?scopeId=s1&clientId=c1&id=hB1kQwNfvF9bu-Tp_cSaig
            
            //Trace.WriteLine(string.Format("[_AnyHub] {0} >>>>>>>> {1}", method, this.Context.ConnectionId));
            Trace.WriteLine(string.Format("[_AnyHub] {0} >>>>>>>> {1}", method, this.TryGetHttpContext().Request.QueryString));
            //Trace.WriteLine(string.Format("[_AnyHub] {0} >>>>>>>> {1}", method, JsonConvert.SerializeObject(this.Context.Items, Formatting.None)));
        }
    }
}
