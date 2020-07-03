using System;

namespace Common.SignalR.ClientMonitors.Groups
{
    public class ScopeGroupLocate : IScopeGroupLocate
    {
        public string ScopeId { get; set; }
        public string Group { get; set; }

        public static ScopeGroupLocate CreateLocateArgs(string scopeId, string @group)
        {
            var theScope = string.IsNullOrWhiteSpace(scopeId) ? HubConst.DefaultScopeId : scopeId;
            var theGroup = string.IsNullOrWhiteSpace(group) ? HubConst.AllGroupId : group;
            return new ScopeGroupLocate {ScopeId = theScope, Group = theGroup };
        }
    }

    public static class ScopeGroupLocateExtensions
    {
        public static string GetLocateGroupKey(this IScopeGroupLocate locate)
        {
            if (locate == null)
            {
                throw new ArgumentNullException(nameof(locate));
            }
            return string.Format("{0}.{1}", locate.ScopeId, locate.Group);
        }

        //T All { get; }
        //T AllExcept(IReadOnlyList<string> excludedConnectionIds);
        //T Group(string groupName);
        //T GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds);
        //T Groups(IReadOnlyList<string> groupNames);
        //T Client(string connectionId);
        //T Clients(IReadOnlyList<string> connectionIds);
        //T User(string userId);
        //T Users(IReadOnlyList<string> userIds);
    }
}