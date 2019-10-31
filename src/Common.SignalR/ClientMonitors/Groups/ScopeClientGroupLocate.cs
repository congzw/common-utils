namespace Common.SignalR.ClientMonitors.Groups
{
    public interface IGroupKey
    {
        string Group { get; set; }
    }

    public interface IScopeGroupLocate : IScopeKey, IGroupKey
    {
    }

    public interface IScopeClientGroupLocate : IScopeClientKey, IGroupKey
    {
    }

    public class ScopeGroupLocate : IScopeGroupLocate
    {
        public string ScopeId { get; set; }
        public string Group { get; set; }
    }

    public class ScopeClientGroupLocate : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
    }
}
