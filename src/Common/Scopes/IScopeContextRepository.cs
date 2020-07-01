namespace Common.Scopes
{
    public interface IScopeContextRepository
    {
        ScopeContext GetScopeContext(string scopeId, bool createIfNotExist);
        void SetScopeContext(ScopeContext scopeContext);
        void RemoveScopeContext(string scopeId);
        void ClearAll();
    }
}
