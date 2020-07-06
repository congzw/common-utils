namespace Common.SignalR
{
    public interface IScopeKey
    {
        /// <summary>
        /// 连接端业务范围的唯一标识，例如某堂课的Id等场景。逻辑上划分的Hub范围
        /// </summary>
        string ScopeId { get; set; }
    }

    public interface IClientKey
    {
        /// <summary>
        /// 连接端Client的唯一标识，例如某设备的Id
        /// </summary>
        string ClientId { get; set; }
    }

    public interface IGroupKey
    {
        /// <summary>
        /// 组名
        /// </summary>
        string Group { get; set; }
    }

    public interface IConnectionKey
    {
        string ConnectionId { get; set; }
    }

    #region locates

    //scope client
    public interface IScopeClientLocate : IScopeKey, IClientKey
    {
    }
    
    //scope group
    public interface IScopeGroupLocate : IScopeKey, IGroupKey
    {
    }

    //scope client group
    public interface IScopeClientGroupLocate : IScopeKey, IClientKey, IGroupKey, IScopeClientLocate, IScopeGroupLocate
    {
    }

    //scope client connection
    public interface IClientConnectionLocate :  IScopeClientLocate, IConnectionKey
    {
    }

    #endregion
}
