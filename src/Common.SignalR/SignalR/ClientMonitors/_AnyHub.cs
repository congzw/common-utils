using System;
using System.Threading.Tasks;
using Common.SignalR.ClientMonitors.ClientMethods;
using Common.SignalR.ClientMonitors.ClientStubs;
using Common.SignalR.ClientMonitors.Connections;
using Common.SignalR.ClientMonitors.Groups;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR.ClientMonitors
{
    //a demo for how to use ClientMonitors
    public class _AnyHub : Hub
    {
        private readonly HubEventBus _hubEventBus;

        public _AnyHub(HubEventBus hubEventBus)
        {
            _hubEventBus = hubEventBus;
        }

        //连接
        public override async Task OnConnectedAsync()
        {
            await _hubEventBus.Raise(new OnConnectedEvent(this)).ConfigureAwait(false);
            await base.OnConnectedAsync().ConfigureAwait(false);
        }

        //断开
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var reason = exception == null ? "" : exception.Message;
            await _hubEventBus.Raise(new OnDisconnectedEvent(this, reason)).ConfigureAwait(false);
            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }
        
        //踢掉（管理场景）
        public async Task KickClient(KickClient args)
        {
            await _hubEventBus.Raise(new KickClientEvent(this, args)).ConfigureAwait(false);
            await base.OnConnectedAsync().ConfigureAwait(false);
        }
        
        //加入组成员
        public Task AddToGroup(AddToGroup args)
        {
            return _hubEventBus.Raise(new AddToGroupEvent(this, args));
        }

        //移除组成员
        public Task RemoveFromGroup(RemoveFromGroup args)
        {
            return _hubEventBus.Raise(new RemoveFromGroupEvent(this, args));
        }

        //Scope的上下文切换
        public Task ChangeScope(ChangeScope args)
        {
            return _hubEventBus.Raise(new ChangeScopeEvent(this, args));
        }
        
        //代表客户端的方法调用，供同步页面等场景使用
        public Task ClientMethodInvoke(ClientMethodInvoke args)
        {
            //this.Clients.All.SendAsync("", null);
            //this.Clients.All.SendCoreAsync("", null);
            return _hubEventBus.Raise(new ClientMethodInvokeEvent(this, args));
        }

        //代表从服务器端的方法调用，供数据通知等场景使用
        public Task InvokeClientStub(InvokeClientStub args)
        {
            return _hubEventBus.Raise(new InvokeClientStubEvent(this, args));
        }
    }
}
