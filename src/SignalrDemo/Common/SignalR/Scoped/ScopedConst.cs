namespace Common.SignalR.Scoped
{
    public class ScopedConst
    {
        private readonly string _defaultScopeGroup = string.Empty;
        public string DefaultScopeGroup()
        {
            return _defaultScopeGroup;
        }

        public static ScopedConst ForOther = new ScopedConst();
        public static ScopedConstForServer ForServer = new ScopedConstForServer();
        public static ScopedConstForClient ForClient = new ScopedConstForClient();
    }

    public class ScopedConstForServer
    {

    }

    public class ScopedConstForClient
    {

    }

}