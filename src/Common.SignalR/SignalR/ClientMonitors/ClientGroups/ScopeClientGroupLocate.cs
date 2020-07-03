using System;

namespace Common.SignalR.ClientMonitors.ClientGroups
{
    public class ScopeClientGroupLocate : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
        
        public static ScopeClientGroupLocate CreateLocateArgs(string scopeId, string @group, string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            var theScope = string.IsNullOrWhiteSpace(scopeId) ? HubConst.DefaultScopeId : scopeId;
            var theGroup = string.IsNullOrWhiteSpace(group) ? HubConst.AllGroupId : group;
            return new ScopeClientGroupLocate { ScopeId = theScope, Group = theGroup, ClientId = clientId };
        }
    }

    public static class ScopeClientGroupLocateExtensions
    {
        //T All { get; }
        //T AllExcept(IReadOnlyList<string> excludedConnectionIds);
        //T Group(string groupName);
        //T GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds);
        //T Groups(IReadOnlyList<string> groupNames);

        //OK methods
        //T Client(string connectionId);
        //T Clients(IReadOnlyList<string> connectionIds);
        //T User(string userId);
        //T Users(IReadOnlyList<string> userIds);


        //T Client(string connectionId);
        //T Clients(IReadOnlyList<string> connectionIds);

        public static string GetLocateClientKey(this IScopeClientGroupLocate locate)
        {
            if (locate == null)
            {
                throw new ArgumentNullException(nameof(locate));
            }
            return string.Format("{0}.{1}.{2}", locate.ScopeId, locate.Group, locate.ClientId);
        }

        //public static IEnumerable<ScopeClientGroupLocate> GetLocatesExcept(IReadOnlyList<ScopeClientGroupLocate> excludedLocates)
        //{
        //    var query = QueryLocates();
        //    return query.Where(x => x.GetLocateClientKey())
        //    //return repository.Query()
        //    //    .Where(x => x.ClientType == ClientType.MainScreen
        //    //                && (string.IsNullOrEmpty(ScopeContext.Current.ScopeId) ? 
        //    //                    x.ScopeId == null : x.ScopeId == ScopeContext.Current.ScopeId))
        //    //    .Select(x => x.ClientId).ToList();
        //}


        public static bool SameLocateKey(this IScopeClientGroupLocate locate, IScopeClientGroupLocate locate2)
        {
            if (locate == null || locate2 == null)
            {
                return false;
            }
            return locate.GetLocateClientKey().Equals(locate2.GetLocateClientKey(), StringComparison.OrdinalIgnoreCase);
        }

    }
}
