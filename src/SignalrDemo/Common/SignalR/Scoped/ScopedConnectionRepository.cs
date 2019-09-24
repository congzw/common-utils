using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable CheckNamespace

namespace Common.SignalR.Scoped
{
    public interface IScopedConnectionRepository
    {
        ScopedConnection GetScopedConnectionByScopedClientKey(ScopedClientKey scopedClientKey);
        ScopedConnection GetScopedConnection(string connectionId);
        IEnumerable<ScopedConnection> GetScopedConnections(string scopeGroupId);
        void AddOrUpdateScopedConnection(ScopedConnection conn);
        void RemoveScopedConnection(string connectionId);
    }

    public class ScopedClientKey
    {
        public string ScopeGroupId { get; set; }
        public string ClientId { get; set; }

        public string ToOneKey()
        {
            return ClientId + "," + ScopeGroupId;
        }
        public ScopedClientKey WithScopeGroupId(string scopeGroupId)
        {
            ScopeGroupId = scopeGroupId;
            return this;
        }
        public ScopedClientKey WithClientId(string clientId)
        {
            ClientId = clientId;
            return this;
        }
        public static ScopedClientKey Create()
        {
            return new ScopedClientKey();
        }
    }

    public class MemoryScopedConnectionRepository : IScopedConnectionRepository
    {
        public MemoryScopedConnectionRepository()
        {
            Scopes = new ConcurrentDictionary<string, IDictionary<string, ScopedConnection>>(StringComparer.OrdinalIgnoreCase);
            ConnectionScopes = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, string> ConnectionScopes { get; set; }
        public IDictionary<string, IDictionary<string, ScopedConnection>> Scopes { get; set; }

        public ScopedConnection GetScopedConnectionByScopedClientKey(ScopedClientKey scopedClientKey)
        {
            if (scopedClientKey == null)
            {
                return null;
            }
            Scopes.TryGetValue(scopedClientKey.ScopeGroupId, out var connDic);
            var theOne = connDic?.Values.SingleOrDefault(x => x.ClientId.Equals(scopedClientKey.ClientId, StringComparison.OrdinalIgnoreCase));
            return theOne;
        }

        public ScopedConnection GetScopedConnection(string connectionId)
        {
            ConnectionScopes.TryGetValue(connectionId, out var scopeGroupId);
            if (scopeGroupId == null)
            {
                //find no scopeGroup, should never enter here
                return null;
            }

            Scopes.TryGetValue(scopeGroupId, out var connDic);
            if (connDic == null)
            {
                //find no connDic, should never enter here
                return null;
            }

            connDic.TryGetValue(connectionId, out var conn);
            return conn;
        }

        public IEnumerable<ScopedConnection> GetScopedConnections(string scopeGroupId)
        {
            Scopes.TryGetValue(scopeGroupId, out var connDic);
            if (connDic == null)
            {
                //find no connDic, should never enter here
                return new List<ScopedConnection>();
            }
            var connections = connDic.Values.AsEnumerable();
            return connections;
        }

        public void AddOrUpdateScopedConnection(ScopedConnection conn)
        {
            if (conn == null)
            {
                return;
            }

            Scopes.TryGetValue(conn.ScopeGroupId, out var connDic);
            if (connDic == null)
            {
                connDic = new ConcurrentDictionary<string, ScopedConnection>(StringComparer.OrdinalIgnoreCase);
                Scopes[conn.ScopeGroupId] = connDic;
            }

            connDic[conn.ConnectionId] = conn;
            ConnectionScopes[conn.ConnectionId] = conn.ScopeGroupId;
            conn.LastUpdateAt = DateHelper.Instance.GetDateNow();
            conn.UpdateDesc();
        }

        public void RemoveScopedConnection(string connectionId)
        {
            if (string.IsNullOrWhiteSpace(connectionId))
            {
                return;
            }

            ConnectionScopes.TryGetValue(connectionId, out var scopeGroupId);
            if (scopeGroupId == null)
            {
                //find no scopeGroup, should never enter here
                return;
            }

            Scopes.TryGetValue(scopeGroupId, out var connDic);
            connDic?.Remove(connectionId);
        }
    }
}
