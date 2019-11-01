using System;
using System.Threading.Tasks;

namespace Common.Logs.Refs.ApiProxy
{
    public class SimpleApiProxy : ISimpleApiProxy
    {
        private readonly IWebApiHelper _webApiHelper;
        private readonly ApiProxyConfig _config;

        public SimpleApiProxy(IWebApiHelper webApiHelper, ApiProxyConfig apiProxyConfig)
        {
            _webApiHelper = webApiHelper;
            _config = apiProxyConfig;
        }

        public Task<TResult> Get<T, TResult>(T args, TResult defaultResult)
        {
            var requestUri = _config.GetRequestUri("Foo");
            //todo append query string form args
            return _webApiHelper.GetAsJson(requestUri, defaultResult);
        }

        public Task Post<T>(T args)
        {
            var requestUri = _config.GetRequestUri("Foo");
            return _webApiHelper.PostAsJson(requestUri, args);
        }

        public Task<TResult> Post<T, TResult>(T args, TResult defaultResult)
        {
            var requestUri = _config.GetRequestUri("Foo");
            return _webApiHelper.PostAsJson(requestUri, args, defaultResult);
        }
        
        public Task<DateTime> GetDate()
        {
            var requestUri = _config.GetRequestUri(nameof(GetDate));
            return _webApiHelper.GetAsJson<DateTime>(requestUri, new DateTime(2000, 1, 1));
        }

        public Task<bool> TryTestApiConnection()
        {
            var requestUri = _config.GetRequestUri(nameof(GetDate));
            return _webApiHelper.CheckTargetStatus(requestUri, _config.FailTimeoutMilliseconds);
        }
    }
}