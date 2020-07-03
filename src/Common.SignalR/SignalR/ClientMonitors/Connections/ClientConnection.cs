using System;
using System.Collections.Generic;

namespace Common.SignalR.ClientMonitors.Connections
{
    public interface IClientConnection : IClientConnectionLocate
    {
        string ClientType { get; set; }
        //add more abstract prop if need
    }

    public class ClientConnection : IClientConnection, IHaveBags
    {
        public ClientConnection()
        {
            //Groups = new List<string>();
            var now = GetDateNow();
            CreateAt = now;
            LastUpdateAt = now;
            ClientType = string.Empty;
            Bags = BagsHelper.Create();
        }

        public Guid Id { get; set; }
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string ConnectionId { get; set; }
        public string ClientType { get; set; }



        public DateTime CreateAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public IDictionary<string, object> Bags { get; set; }
        
        #region for extensions

        public static Func<DateTime> GetDateNow = () => DateHelper.Instance.GetDateNow();

        #endregion
    }
}
