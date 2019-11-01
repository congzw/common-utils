using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Common.Logs.Refs.ApiProxy
{
    public class WebApiProxySmartWrapper : ISimpleApiProxy
    {
        private readonly ISimpleApiProxy _nullApiProxy = NullSimpleApiProxy.Instance;

        public WebApiProxySmartWrapper(ISimpleApiProxy apiProxy)
        {
            Proxy = apiProxy;
            GetDateNow = () => DateTime.Now;
            CheckSmart = CheckIfNotOkAndExpired.Create(TimeSpan.FromSeconds(3));
        }

        public void Reset(ISimpleApiProxy apiProxy, TimeSpan? checkApiStatusInterval = null, Func<DateTime> getDateNow = null)
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
        
        public ISimpleApiProxy Proxy { get; set; }

        public CheckIfNotOkAndExpired CheckSmart { get; set; }
        
        public Func<DateTime> GetDateNow { get; set; }

        public Task<TResult> Get<T, TResult>(T args, TResult defaultResult)
        {
            var isOk = CheckApiStatusOkSmart();
            if (!isOk)
            {
                return _nullApiProxy.Get(args, defaultResult);
            }
            return SafeInvokeTask(Proxy.Get(args, defaultResult), defaultResult);
        }

        public Task Post<T>(T args)
        {
            var isOk = CheckApiStatusOkSmart();
            if (!isOk)
            {
                return _nullApiProxy.Post(args);
            }
            return SafeInvokeTask(Proxy.Post(args));
        }

        public Task<TResult> Post<T, TResult>(T args, TResult defaultResult)
        {
            var isOk = CheckApiStatusOkSmart();
            if (!isOk)
            {
                return _nullApiProxy.Post(args, defaultResult);
            }
            return SafeInvokeTask(Proxy.Post(args, defaultResult), defaultResult);
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

        public Task<bool> TryTestApiConnection()
        {
            return Proxy.TryTestApiConnection();
        }

        #region for di extensions and simple use

        private static readonly WebApiProxySmartWrapper Instance = new WebApiProxySmartWrapper(NullSimpleApiProxy.Instance);

        public static Func<WebApiProxySmartWrapper> Resolve { get; set; } = () => Instance;

        #endregion
    }
}