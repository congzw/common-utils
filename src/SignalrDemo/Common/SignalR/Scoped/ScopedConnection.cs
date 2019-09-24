using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

// ReSharper disable CheckNamespace

namespace Common.SignalR.Scoped
{
    public class ScopedConnection
    {
        public ScopedConnection()
        {
            ScopeGroupId = ScopedConst.ForOther.DefaultScopeGroup();
            var now = DateHelper.Instance.GetDateNow();
            CreateAt = now;
            LastUpdateAt = now;
            Bags = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public string ScopeGroupId { get; set; }
        public string ClientId { get; set; }
        public string ConnectionId { get; set; }

        public DateTime CreateAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string Desc { get; set; }

        public IDictionary<string, object> Bags { get; set; }

        public string CreateDesc()
        {
            return this.AsIniString(new string[] { nameof(Desc), nameof(Bags) });
        }
        public ScopedConnection UpdateDesc()
        {
            this.Desc = CreateDesc();
            var bagIni = this.Bags.AsIniString(null);
            if (!string.IsNullOrWhiteSpace(bagIni))
            {
                this.Desc += ";";
                this.Desc += bagIni;
            }
            return this;
        }
    }
}
