namespace Common.Scopes
{
    /// <summary>
    /// 逻辑划分范围唯一标识
    /// </summary>
    public interface IScopeKey
    {
        /// <summary>
        /// 逻辑划分范围唯一标识，例如：按教室划分等场景
        /// </summary>
        string ScopeId { get; set; }
    }
}