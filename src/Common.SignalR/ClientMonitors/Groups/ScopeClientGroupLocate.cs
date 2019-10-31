namespace Common.SignalR.ClientMonitors.Groups
{
    public interface IScopeClientGroupLocate : IScopeClientKey
    {
        string Group { get; set; }
    }

    public class ScopeClientGroupLocate : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
    }
}
