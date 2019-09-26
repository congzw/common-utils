using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
// ReSharper disable CheckNamespace

namespace Common.SignalR.Scoped
{
    public class ScopedConnectionManager
    {
        private readonly IScopedConnectionRepository _repository;

        public ScopedConnectionManager(IScopedConnectionRepository repository)
        {
            _repository = repository;
            ScopedContexts = new ConcurrentDictionary<string, HubCallerContext>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, HubCallerContext> ScopedContexts { get; set; }

        public HubCallerContext TryGetHubCallerContext(ScopedClientKey scopedClientKey)
        {
            var oneKey = scopedClientKey.ToOneKey();
            ScopedContexts.TryGetValue(oneKey, out var theOne);
            return theOne;
        }

        public async Task OnConnected(OnConnectedEvent theEvent)
        {
            if (theEvent?.RaiseHub == null)
            {
                return;
            }

            var hub = theEvent.RaiseHub;

            var conn = new ScopedConnection();
            var connectionId = hub.Context.ConnectionId;
            conn.ConnectionId = connectionId;
            var now = DateHelper.Instance.GetDateNow();
            conn.CreateAt = now;
            conn.LastUpdateAt = now;

            conn.ScopeGroupId = hub.TryGetQueryParameterValue(nameof(conn.ScopeGroupId), string.Empty);
            conn.ClientId = hub.TryGetQueryParameterValue(nameof(conn.ClientId), string.Empty);
            conn.UpdateDesc();

            _repository.AddOrUpdateScopedConnection(conn);

            if (!string.IsNullOrWhiteSpace(conn.ClientId))
            {
                //scoped clients with same clientId, old should be kicked off
                var scopedClientKey = ScopedClientKey.Create().WithClientId(conn.ClientId).WithScopeGroupId(conn.ScopeGroupId);
                var oneKey = scopedClientKey.ToOneKey();
                if (ScopedContexts.TryGetValue(oneKey, out var oldClientHub))
                {
                    await KickSameScopedClient(hub, oldClientHub, scopedClientKey).ConfigureAwait(false);
                }
                ScopedContexts[oneKey] = hub.Context;
            }

            await hub.Groups.AddToGroupAsync(conn.ConnectionId, conn.ScopeGroupId).ConfigureAwait(false);

            var scopedConnections = _repository
                .GetScopedConnections(conn.ScopeGroupId)
                .OrderBy(x => x.CreateAt).ToList();
            var clientProxy = hub.Clients.Group(conn.ScopeGroupId);
            await clientProxy.SendAsync(ScopedConst.ForClient.ScopedConnectionsUpdated(), scopedConnections).ConfigureAwait(false);
        }

        public async Task OnDisconnected(OnDisconnectedEvent theEvent)
        {
            if (theEvent?.RaiseHub == null)
            {
                return;
            }

            var hub = theEvent.RaiseHub;
            var exception = theEvent.Exception;

            var connectionId = hub.Context.ConnectionId;
            var now = DateHelper.Instance.GetDateNow();

            var conn = _repository.GetScopedConnection(connectionId);
            if (conn == null)
            {
                //find no conn, should never enter here
                return;
            }

            conn.LastUpdateAt = now;
            conn.UpdateDesc();

            if (exception != null)
            {
                conn.Desc += ", " + exception.Message;
            }
            _repository.RemoveScopedConnection(connectionId);
            await hub.Groups.RemoveFromGroupAsync(connectionId, conn.ScopeGroupId).ConfigureAwait(false);

            var scopedConnections = _repository
                .GetScopedConnections(conn.ScopeGroupId)
                .OrderBy(x => x.CreateAt).ToList();
            var clientProxy = hub.Clients.Group(conn.ScopeGroupId);
            await clientProxy.SendAsync(ScopedConst.ForClient.ScopedConnectionsUpdated(), scopedConnections);
        }

        public Task UpdateScopedConnectionBags(OnUpdateBagsEvent theEvent)
        {
            if (theEvent == null || theEvent.Bags == null || theEvent.Bags.Count == 0)
            {
                return Task.FromResult(0);
            }

            var hub = theEvent.RaiseHub;
            var bags = theEvent.Bags;

            var connectionId = hub.Context.ConnectionId;
            var conn = _repository.GetScopedConnection(connectionId);
            if (conn == null)
            {
                return Task.FromResult(0);
            }

            foreach (var bag in bags)
            {
                conn.Bags[bag.Key] = bag.Value;
            }

            _repository.AddOrUpdateScopedConnection(conn);

            var scopedConnections = _repository.GetScopedConnections(conn.ScopeGroupId);
            var connections = scopedConnections.OrderBy(x => x.CreateAt).ToList();
            var clientProxy = hub.Clients.Group(conn.ScopeGroupId);
            return clientProxy.SendAsync(ScopedConst.ForClient.ScopedConnectionsUpdated(), connections);
        }

        public Task UpdateScopedConnectionBagsOutSideHub(OnUpdateBagsHubContextEvent theEvent)
        {
            if (theEvent == null)
            {
                throw new ArgumentNullException(nameof(theEvent));
            }

            var hubContext = theEvent.Context;
            if (hubContext == null)
            {
                throw new ArgumentNullException(nameof(theEvent.Context));
            }

            if (theEvent.Bags == null || theEvent.Bags.Count == 0)
            {
                return Task.CompletedTask;
            }


            var scopedClientKey = ScopedClientKey.Create().WithScopeGroupId(theEvent.ScopeGroupId).WithClientId(theEvent.ClientId);
            var hubCallerContext = TryGetHubCallerContext(scopedClientKey);
            if (hubCallerContext == null)
            {
                return Task.CompletedTask;
            }

            var bags = theEvent.Bags;

            var connectionId = hubCallerContext.ConnectionId;
            var conn = _repository.GetScopedConnection(connectionId);
            if (conn == null)
            {
                return Task.FromResult(0);
            }

            foreach (var bag in bags)
            {
                conn.Bags[bag.Key] = bag.Value;
            }

            _repository.AddOrUpdateScopedConnection(conn);

            var scopedConnections = _repository.GetScopedConnections(conn.ScopeGroupId);
            var connections = scopedConnections.OrderBy(x => x.CreateAt).ToList();
            var clientProxy = hubContext.Clients.Group(conn.ScopeGroupId);
            return clientProxy.SendAsync(ScopedConst.ForClient.ScopedConnectionsUpdated(), connections);
        }

        private async Task KickSameScopedClient(Hub hub, HubCallerContext oldClientHub, ScopedClientKey scopedClientKey)
        {
            if (oldClientHub == null)
            {
                return;
            }

            //already exist, should kick!
            var oldConnectionId = oldClientHub.ConnectionId;
            var theConn = _repository.GetScopedConnection(oldConnectionId);
            if (theConn != null)
            {
                var clientProxy = hub.Clients.Client(oldConnectionId);
                if (clientProxy != null)
                {
                    var scopedConnections = _repository.GetScopedConnections(scopedClientKey.ScopeGroupId);
                    var connections = scopedConnections.OrderBy(x => x.CreateAt).ToList();
                    var theOne = connections.SingleOrDefault(x => x.ConnectionId.Equals(oldConnectionId));
                    if (theOne != null)
                    {
                        var oneKey = scopedClientKey.ToOneKey();
                        var message = string.Format("{0} is kicked by another same scoped client!", oneKey);
                        theOne.Desc = message;
                    }
                    await clientProxy.SendAsync(ScopedConst.ForClient.ScopedConnectionsUpdated(), connections).ConfigureAwait(false);
                }
            }
            _repository.RemoveScopedConnection(oldConnectionId);
            oldClientHub.Abort();
        }
    }
}