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

        public async Task OnConnected(Hub hub)
        {
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
            await UpdateScopedConnections(hub, conn.ScopeGroupId).ConfigureAwait(false);
        }

        public async Task OnDisconnected(Hub hub, Exception exception)
        {
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
            await UpdateScopedConnections(hub, conn.ScopeGroupId).ConfigureAwait(false);
        }
        
        public Task UpdateScopedConnectionBags(Hub hub, IDictionary<string, object> bags)
        {
            if (bags == null || bags.Count == 0)
            {
                return Task.FromResult(0);
            }

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
            return UpdateScopedConnections(hub, conn.ScopeGroupId);
        }

        public Task UpdateScopedConnections(Hub hub, string scopeGroupId)
        {
            var scopedConnections = _repository.GetScopedConnections(scopeGroupId);
            var connections = scopedConnections.OrderBy(x => x.CreateAt).ToList();
            return hub.Clients.Group(scopeGroupId).SendAsync(ScopedConst.ForClient.ScopedConnectionsUpdated(), connections);
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