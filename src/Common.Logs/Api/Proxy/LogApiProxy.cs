using System;
using System.Globalization;
using System.Threading.Tasks;
using Common.Logs.Refs.ApiProxy;

namespace Common.Logs.Api.Proxy
{
    public interface ILogApiProxy : ILogApi
    {
        //for test connection state
        Task<string> GetDate();
    }

    public class LogApiProxy : ILogApiProxy
    {
        private ISimpleApiClient _proxy = null;

        public LogApiProxy(ISimpleApiClient apiProxy)
        {
            _proxy = apiProxy;
        }

        public Task Log(LogArgs args)
        {
            return _proxy.Post(nameof(Log), args);
        }

        public Task<string> GetDate()
        {
            return Task.FromResult(DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }
    }
}
