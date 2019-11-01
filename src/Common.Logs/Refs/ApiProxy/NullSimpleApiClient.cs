using System.Threading.Tasks;

namespace Common.Logs.Refs.ApiProxy
{
    public class NullSimpleApiClient : ISimpleApiClient
    {
        public Task<TResult> Get<T, TResult>(string method, T args, TResult defaultResult)
        {
            return Task.FromResult(defaultResult);
        }

        public Task Post<T>(string method, T args)
        {
            return Task.CompletedTask;
        }

        public Task<TResult> Post<T, TResult>(string method, T args, TResult defaultResult)
        {
            return Task.FromResult(defaultResult);
        }

        public static NullSimpleApiClient Instance = new NullSimpleApiClient();
    }
}