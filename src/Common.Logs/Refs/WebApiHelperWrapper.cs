using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Common.Logs.Refs
{
    public class WebApiHelperWrapper : IWebApiHelper
    {
        public WebApiHelperWrapper(IWebApiHelper webApiHelper)
        {
            Proxy = webApiHelper;
            GetDateNow = () => DateTime.Now;
            TestConnectionGetApiUri = string.Empty;
            CheckSmart = CheckIfNotOkAndExpired.Create(TimeSpan.FromSeconds(3));
        }

        public Task<bool> CheckTargetStatus(string uri, int defaultTimeoutMilliseconds)
        {
            return Proxy.CheckTargetStatus(uri, defaultTimeoutMilliseconds);
        }

        public Task<TResult> GetAsJson<TResult>(string uri, TResult defaultResult)
        {
            var isOk = CheckApiStatusOkSmart();
            if (!isOk)
            {
                return Task.FromResult(defaultResult);
            }
            return SafeInvokeTask(Proxy.GetAsJson(uri, defaultResult), defaultResult);
        }

        public Task<TResult> PostAsJson<TInput, TResult>(string uri, TInput input, TResult defaultResult)
        {
            var isOk = CheckApiStatusOkSmart();
            if (!isOk)
            {
                return Task.FromResult(defaultResult);
            }
            return SafeInvokeTask(Proxy.PostAsJson(uri, input, defaultResult), defaultResult);
        }

        public Task PostAsJson<TInput>(string uri, TInput input)
        {
            var isOk = CheckApiStatusOkSmart();
            if (!isOk)
            {
                return Task.CompletedTask;
            }
            return SafeInvokeTask(Proxy.PostAsJson(uri, input));
        }

        public bool LogMessage { get; set; }
        
        public void Reset(TimeSpan? checkApiStatusInterval = null, Func<DateTime> getDateNow = null)
        {
            if (checkApiStatusInterval != null)
            {
                CheckSmart = CheckIfNotOkAndExpired.Create(checkApiStatusInterval);
            }
            if (getDateNow != null)
            {
                GetDateNow = getDateNow;
            }
        }

        public IWebApiHelper Proxy { get; set; }

        public CheckIfNotOkAndExpired CheckSmart { get; set; }

        public Func<DateTime> GetDateNow { get; set; }
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
        
        #region for di extensions and simple use

        private static readonly WebApiHelperWrapper Instance = new WebApiHelperWrapper(WebApiHelper.Resolve());

        public static Func<WebApiHelperWrapper> Resolve { get; set; } = () => Instance;

        #endregion
    }
}
