namespace Common.SignalR
{
    public class HubConst
    {
        public static string OnClientMethodInvoke = "OnClientMethodInvoke";
        public static string OnInvokeClientStub = "OnInvokeClientStub";
        public static string UpdateScopedConnections = "UpdateScopedConnections";
        public static string KeyArgs = "args";

        public static string ClientStub_JoinGroupStubInvoke = "JoinGroupStubInvoke";
        public static string ClientStub_RemoveFromGroupStubInvoke = "RemoveFromGroupStubInvoke";

        #region for manage monitor

        public static string ManageMonitorClientId = "Monitor";
        public static string ManageMonitorScopeId = "Manage";
        public static string UpdateMonitorInfo = "UpdateMonitorInfo";

        #endregion

        #region for scope locate

        public static string Args_ScopeId = "scopeId";
        public static string Args_ClientId = "clientId";
        public static string Args_Id = "id";

        public static string DefaultScopeId = string.Empty;
        public static string AllGroupId = string.Empty;

        #endregion

        public static HubConst Ext = new HubConst();
    }
}
