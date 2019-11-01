using System.Threading.Tasks;

namespace Common.Logs.Refs.ApiProxy
{
    public interface ISimpleApi
    {
        Task<TResult> Get<T, TResult>(T args, TResult defaultResult);
        Task Post<T>(T args);
        Task<TResult> Post<T, TResult>(T args, TResult defaultResult);
    }

    public interface ISimpleApiProxy : ISimpleApi
    {
        Task<bool> TryTestApiConnection();
    }
    
    public class NullSimpleApiProxy : ISimpleApiProxy
    {
        public Task<TResult> Get<T, TResult>(T args, TResult defaultResult)
        {
            return Task.FromResult(defaultResult);
        }

        public Task Post<T>(T args)
        {
            return Task.CompletedTask;
        }

        public Task<TResult> Post<T, TResult>(T args, TResult defaultResult)
        {
            return Task.FromResult(defaultResult);
        }

        public Task<bool> TryTestApiConnection()
        {
            return Task.FromResult(false);
        }

        public static NullSimpleApiProxy Instance = new NullSimpleApiProxy();
    }
}
