using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logs.Refs
{
    public interface IWebApiHelper
    {
        /// <summary>
        /// 检测连接是否正常
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="defaultTimeoutMilliseconds"></param>
        /// <returns></returns>
        Task<bool> CheckTargetStatus(string uri, int defaultTimeoutMilliseconds);

        /// <summary>
        /// 查询用
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="uri">目标地址</param>
        /// <param name="defaultResult">默认值</param>
        /// <returns>返回值</returns>
        Task<TResult> GetAsJson<TResult>(string uri, TResult defaultResult);

        /// <summary>
        /// 创建用
        /// </summary>
        /// <typeparam name="TInput">传入参数类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="uri">目标地址</param>
        /// <param name="input">参数值</param>
        /// <param name="defaultResult">默认值</param>
        /// <returns>返回值</returns>
        Task<TResult> PostAsJson<TInput, TResult>(string uri, TInput input, TResult defaultResult);

        /// <summary>
        ///  创建用
        /// </summary>
        /// <typeparam name="TInput">传入参数类型</typeparam>
        /// <param name="uri">目标地址</param>
        /// <param name="input">参数值</param>
        /// <returns></returns>
        Task PostAsJson<TInput>(string uri, TInput input);

        /// <summary>
        /// 是否记录异常
        /// </summary>
        bool LogMessage { get; set; }
    }

    public class WebApiHelper : IWebApiHelper
    {
        private readonly ISimpleLog _simpleLog = null;
        private readonly HttpClient _httpClient;
        private HttpClient _testHttpClient = null;

        public WebApiHelper(ISimpleLogFactory simpleLogFactory)
        {
            //todo: refactor code
            //var response = await httpClient.GetAsync(uri).ConfigureAwait(false);
            //var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip };
            //_httpClient = new HttpClient(handler);
            _httpClient = new HttpClient();
            _simpleLog = simpleLogFactory.CreateLogFor(this);
        }


        public async Task<bool> CheckTargetStatus(string uri, int defaultTimeoutMilliseconds)
        {
            var timeout = new TimeSpan(0, 0, 0, 0, defaultTimeoutMilliseconds);
            if (_testHttpClient == null)
            {
                _testHttpClient = new HttpClient { Timeout = timeout };
            }
            else
            {
                if (_testHttpClient.Timeout != timeout)
                {
                    _testHttpClient.Dispose();
                    _testHttpClient = new HttpClient { Timeout = timeout };
                }
            }

            try
            {
                ////test result: use ResponseHeadersRead is slower than normal, why?
                //var response = await _testHttpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                var response = await _testHttpClient.GetAsync(uri);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _simpleLog.LogInfo(string.Format("{0} 通讯异常:{1}", uri, ex.Message));
                return false;
            }
        }

        public async Task<TResult> GetAsJson<TResult>(string uri, TResult defaultResult)
        {
            var result = defaultResult;

            var response = await _httpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsJsonAsync<TResult>();
            }

            //HTTP StatusCode ,eg 200, 404...
            LogResult(uri, response.StatusCode, result);
            return result;
        }

        public async Task<TResult> PostAsJson<TInput, TResult>(string uri, TInput input, TResult defaultResult)
        {
            var result = defaultResult;
            var response = await _httpClient.PostAsJsonAsync(uri, input);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsJsonAsync<TResult>();
            }
            LogResult(uri, response.StatusCode, result);
            return result;
        }

        public async Task PostAsJson<TInput>(string uri, TInput input)
        {
            string result = null;
            var response = await _httpClient.PostAsJsonAsync(uri, input);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }
            LogResult(uri, response.StatusCode, result);
        }

        public bool LogMessage { get; set; }

        private void LogResult<T>(string uri, HttpStatusCode statusCode, T result)
        {
            if (!LogMessage)
            {
                return;
            }
            //todo record result 
            _simpleLog.LogInfo(string.Format("{0} -> {1}", uri, statusCode));
        }

        #region for di extensions

        private static readonly Lazy<IWebApiHelper> LazyInstance = new Lazy<IWebApiHelper>(() => new WebApiHelper(SimpleLogFactory.Resolve()));
        public static Func<IWebApiHelper> Resolve { get; set; } = () => LazyInstance.Value;

        #endregion


    }

    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var simpleJson = SimpleJson.Resolve();
            var json = simpleJson.SerializeObject(data, false);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return httpClient.PostAsync(url, content);
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            var simpleJson = SimpleJson.Resolve();
            return simpleJson.DeserializeObject<T>(json);
        }
    }
}
