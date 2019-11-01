using System.Threading.Tasks;

namespace Common.Logs.Refs.ApiProxy
{
    public class SimpleApiClient : ISimpleApiClient
    {
        private readonly IWebApiHelper _webApiHelper;
        private readonly ApiClientConfig _config;

        public SimpleApiClient(IWebApiHelper webApiHelper, ApiClientConfig config)
        {
            _webApiHelper = webApiHelper;
            _config = config;
        }

        public Task<TResult> Get<T, TResult>(string method, T args, TResult defaultResult)
        {
            var requestUri = _config.GetRequestUri(method);
            //todo append query string form args
            return _webApiHelper.GetAsJson(requestUri, defaultResult);
        }

        public Task Post<T>(string method, T args)
        {
            var requestUri = _config.GetRequestUri(method);
            return _webApiHelper.PostAsJson(requestUri, args);
        }

        public Task<TResult> Post<T, TResult>(string method, T args, TResult defaultResult)
        {
            var requestUri = _config.GetRequestUri(method);
            return _webApiHelper.PostAsJson(requestUri, args, defaultResult);
        }
    }
}