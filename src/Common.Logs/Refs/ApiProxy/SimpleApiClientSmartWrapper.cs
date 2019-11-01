using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Common.Logs.Refs.ApiProxy
{
    public class SimpleApiClientSmartWrapper : ISimpleApiClient
    {
        private readonly ISimpleApiClient _nullApiProxy = NullSimpleApiClient.Instance;

        public SimpleApiClientSmartWrapper(ISimpleApiClient apiProxy)
        {
            Proxy = apiProxy;
            GetDateNow = () => DateTime.Now;
            TestConnectionGetApiUri = string.Empty;
            CheckSmart = CheckIfNotOkAndExpired.Create(TimeSpan.FromSeconds(3));
        }

        public void Reset(ISimpleApiClient apiProxy, TimeSpan? checkApiStatusInterval = null, Func<DateTime> getDateNow = null)
        {
            Proxy = apiProxy ?? throw new ArgumentNullException(nameof(apiProxy));

            if (checkApiStatusInterval != null)
            {
                CheckSmart = CheckIfNotOkAndExpired.Create(checkApiStatusInterval);
            }
            if (getDateNow != null)
            {
                GetDateNow = getDateNow;
            }
        }
        
        public ISimpleApiClient Proxy { get; set; }

        public CheckIfNotOkAndExpired CheckSmart { get; set; }
        
        public Func<DateTime> GetDateNow { get; set; }


        public Task<TResult> Get<T, TResult>(string method, T args, TResult defaultResult)
        {
            var isOk = CheckApiStatusOkSmart();
            if (!isOk)
            {
                return _nullApiProxy.Get(method, args, defaultResult);
            }
            return SafeInvokeTask(Proxy.Get(method, args, defaultResult), defaultResult);
        }

        public Task Post<T>(string method, T args)
        {
            var isOk = CheckApiStatusOkSmart();
            if (!isOk)
            {
                return _nullApiProxy.Post(method, args);
            }
            return SafeInvokeTask(Proxy.Post(method, args));
        }

        public Task<TResult> Post<T, TResult>(string method, T args, TResult defaultResult)
        {
            var isOk = CheckApiStatusOkSmart();
            if (!isOk)
            {
                return _nullApiProxy.Post(method, args, defaultResult);
            }
            return SafeInvokeTask(Proxy.Post(method, args, defaultResult), defaultResult);
        }
        
        private bool CheckApiStatusOkSmart()
        {
            return CheckSmart.CheckIfNecessary(GetDateNow(), () => AsyncHelper.RunSync(TryTestApiConnection));
        }

        private Task SafeInvokeTask(Task task)
        {
            var failTask = task.ContinueWith(HandleApiTaskEx, TaskContinuationOptions.OnlyOnFaulted);
            return Task.WhenAny(task, failTask);
        }

        private Task<TResult> SafeInvokeTask<TResult>(Task<TResult> task, TResult defaultResult)
        {
            var failTask = task.ContinueWith(HandleApiTaskEx, TaskContinuationOptions.OnlyOnFaulted);
            var theTask = Task.WhenAny(task, failTask);
            if (theTask == failTask)
            {
                return Task.FromResult(defaultResult);
            }
            return theTask as Task<TResult>;
        }

        private void HandleApiTaskEx(Task source)
        {
            CheckSmart.StatusOk = false;
            source.Exception?.Handle(ex =>
            {
                //todo log ex
                Trace.WriteLine("ApiTaskEx: " + ex.Message);
                return true;
            });
        }

        public string TestConnectionGetApiUri { get; set; }
        public int TestTimeoutMilliseconds { get; set; }

        public Task<bool> TryTestApiConnection()
        {
            if (string.IsNullOrWhiteSpace(TestConnectionGetApiUri))
            {
                return Task.FromResult(false);
            }

            var webApiHelper = WebApiHelper.Resolve();
            return webApiHelper.CheckTargetStatus(TestConnectionGetApiUri, TestTimeoutMilliseconds);
        }

        #region for di extensions and simple use

        private static readonly SimpleApiClientSmartWrapper Instance = new SimpleApiClientSmartWrapper(NullSimpleApiClient.Instance);

        public static Func<SimpleApiClientSmartWrapper> Resolve { get; set; } = () => Instance;

        #endregion
    }
}