using System;
using System.Threading.Tasks;
using Common.Logs.Refs;

namespace Common.Logs.Api.Proxy
{
    public interface ILogApiProxy : ILogApi
    {
    }
    
    public class ApiClientConfig
    {
        public ApiClientConfig()
        {
            BaseUri = "http://localhost:16685/api/trace";
            FailTimeoutMilliseconds = 200;
        }

        public int FailTimeoutMilliseconds { get; set; }
        public string BaseUri { get; set; }


        private bool fixBaseUri = false;
        public string GetRequestUri(string method)
        {
            if (!fixBaseUri)
            {
                BaseUri = BaseUri.TrimEnd('/') + "/";
            }

            return BaseUri + method;
        }
    }

    public class LogApiProxy : ILogApiProxy
    {
        private readonly IWebApiHelper _apiHelper = null;
        private readonly ApiClientConfig _config;

        public LogApiProxy(IWebApiHelper webApiHelper, ApiClientConfig config)
        {
            _apiHelper = webApiHelper;
            _config = config;
        }

        public Task Log(LogArgs args)
        {
            var requestUri = _config.GetRequestUri(nameof(Log));
            return _apiHelper.PostAsJson(requestUri, args);
        }

        #region for di extensions and simple use
        
        public static Func<ILogApiProxy> Resolve { get; set; } = () => null;

        #endregion
    }
}
