using System;
using Common.SignalR.Refs;
// ReSharper disable CheckNamespace

namespace Common.SignalR.ClientMonitors
{
    public class ScopeContext : IScopeKey
    {
        public string ScopeId { get; set; }

        public static ScopeContext Current
        {
            get
            {
                var hubScopeService = ScopeContextService.Resolve();
                return hubScopeService.Current;
            }
        }
    }

    public static class HubScopeExtensions
    {
        public static bool IsEmpty(this ScopeContext hubScope)
        {
            return hubScope == null || string.IsNullOrWhiteSpace(hubScope.ScopeId);
        }
    }

    #region for extensions support

    public interface IScopeContextService
    {
        ScopeContext Current { get; set; }
    }

    public class ScopeContextService : IScopeContextService
    {
        private static readonly ScopeContext Empty = new ScopeContext() { ScopeId = string.Empty };

        public ScopeContextService()
        {
            Current = Empty;
        }

        public ScopeContext Current { get; set; }

        #region for ut & di extensions

        public static Func<IScopeContextService> Resolve { get; } = SimpleLazyFactory<IScopeContextService>.Instance.Default(
            () => new ScopeContextService()).Resolve;

        #endregion
    }

    #endregion
}
