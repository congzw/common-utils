using System.Threading.Tasks;

namespace Common.Logs.Refs.ApiProxy
{
    public interface ISimpleApiClient
    {
        Task<TResult> Get<T, TResult>(string method, T args, TResult defaultResult);
        Task Post<T>(string method, T args);
        Task<TResult> Post<T, TResult>(string method, T args, TResult defaultResult);
    }
}
