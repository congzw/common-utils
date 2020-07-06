using System.Collections.Generic;

namespace Common.SignalR
{
    public class ScopeContext : IHaveBags, IScopeKey
    {
        public string ScopeId { get; set; }
        public IDictionary<string, object> Bags { get; set; } = BagsHelper.Create();
    }
}