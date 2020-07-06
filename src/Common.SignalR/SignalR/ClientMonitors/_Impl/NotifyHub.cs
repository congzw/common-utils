using System;
using System.Linq;
using System.Threading.Tasks;
using Common.SignalR.ClientMonitors.ClientMethods;
using Common.SignalR.ClientMonitors.ClientStubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Common.SignalR.ClientMonitors._Impl
{
    /// <summary>
    /// 服务端接口
    /// </summary>
    public interface IScopedHubServer
    {
    }

    /// <summary>
    /// 客户端使用的接口
    /// </summary>
    public interface IScopedHubClient
    {
        Task ClientMethodInvoke(ClientMethodInvoke args);
        Task InvokeClientStub(InvokeClientStub args);

        //= > all client stub call from InvokeClientStub?
        //Task UpdateState(object argsTodo);
        //Task Notify(object argsTodo);
    }
    public class NotifyHub : Hub<IScopedHubClient>, IScopedHubServer
    {
        private readonly ILogger<NotifyHub> _logger;

        public NotifyHub(ILogger<NotifyHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            //await Clients.All.OnNotify(new { UserId= Context.User.Identity.Name, Name=Context.User.Identity.Name, ConnectId = Context.ConnectionId });
            var userId = Context.User.Identity.Name;
            var groups = Context.GetHttpContext().Request.Query["group"].FirstOrDefault();
            _logger.LogDebug($"OnConnectedAsync----userId:{userId},groups:{groups},connectionId:{ Context.ConnectionId}");
            //if (!string.IsNullOrWhiteSpace(userId))
            //{
            //    await _signalrRedisHelper.AddConnectForUserAsync(userId, Context.ConnectionId);
            //    await JoinToGroup(userId, Context.ConnectionId, groups?.Split(','));
            //    await DealOnLineNotify(userId, Context.ConnectionId);
            //}
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //var userId = Context.User.Identity.Name;
            //var groups = Context.GetHttpContext().Request.Query["group"].FirstOrDefault();
            //_logger.LogDebug($"OnDisconnectedAsync----userId:{userId},groups:{groups},connectionId:{ Context.ConnectionId}");
            //if (!string.IsNullOrWhiteSpace(userId))
            //{
            //    await _signalrRedisHelper.RemoveConnectForUserAsync(userId, Context.ConnectionId);
            //    await DealOffLineNotify(userId, Context.ConnectionId);
            //}
            //await LeaveFromGroup(Context.ConnectionId, groups?.Split(','));
            await base.OnDisconnectedAsync(exception);
        }

        ///// <summary>
        ///// 加入组
        ///// </summary>
        ///// <param name="groupName"></param>
        ///// <returns></returns>
        //private async Task JoinToGroup(string userId, string connectionId, params string[] groups)
        //{
        //    if (!string.IsNullOrWhiteSpace(userId) && groups != null && groups.Length > 0)
        //    {
        //        foreach (var group in groups)
        //        {
        //            await Groups.AddToGroupAsync(connectionId, group);
        //            await _signalrRedisHelper.AddUserForGroupAsync(group, connectionId, userId);

        //            // await Clients.Group(group).OnJoinGroup(new { ConnectId = connectionId, UserId = userId, GroupName = group });
        //        }
        //    }
        //}

        ///// <summary>
        ///// 从组中移除
        ///// </summary>
        ///// <param name="groupName"></param>
        ///// <returns></returns>
        //private async Task LeaveFromGroup(string connectionId, params string[] groups)
        //{
        //    if (groups != null && groups.Length > 0)
        //    {
        //        foreach (var group in groups)
        //        {
        //            await Groups.RemoveFromGroupAsync(connectionId, group);
        //            await _signalrRedisHelper.RemoveConnectFromGroupAsync(group, connectionId);
        //            // await Clients.Group(group).OnLeaveGroup(new { ConnectId = connectionId, GroupName = group });
        //        }
        //    }
        //}

        ///// <summary>
        ///// 处理上线通知(只有用户第一个连接才通知)
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="connectionId"></param>
        ///// <returns></returns>
        //private async Task DealOnLineNotify(string userId, string connectionId)
        //{
        //    var userConnectCount = await _signalrRedisHelper.GetConnectsCountByUserAsync(userId);
        //    await Clients.All.OnLine(new OnLineData()
        //    {
        //        UserId = userId,
        //        ConnectionId = connectionId,
        //        IsFirst = userConnectCount == 1
        //    });
        //}

        ///// <summary>
        ///// 处理下线通知(只有当用户一个连接都没了 才算下线)
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="connectionId"></param>
        ///// <returns></returns>
        //private async Task DealOffLineNotify(string userId, string connectionId)
        //{
        //    var userConnectCount = await _signalrRedisHelper.GetConnectsCountByUserAsync(userId);
        //    await Clients.All.OffLine(new OffLineData()
        //    {
        //        UserId = userId,
        //        ConnectionId = connectionId,
        //        IsLast = userConnectCount == 0
        //    });
        //}
    }

}
