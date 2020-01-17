using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Common
{
    public interface INetCheckHelper
    {
        Task<bool> CheckHttpGetAsync(string uri, TimeSpan? timeout = null);
        Task<bool> PingAsync(string ipAddress, TimeSpan? timeout = null);
    }

    public class NetCheckHelper : INetCheckHelper, IDisposable
    {
        private HttpClient _httpClient;

        public HttpClient HttpClient
        {
            get => _httpClient ?? (_httpClient = new HttpClient() { Timeout = TimeSpan.FromMilliseconds(500) });
            set => _httpClient = value;
        }

        public async Task<bool> CheckHttpGetAsync(string uri, TimeSpan? timeout = null)
        {
            if (timeout != null)
            {
                var httpClient = new HttpClient() { Timeout = timeout.Value };
                return await RunCheckHttpGetAsync(uri, httpClient, true);
            }
            return await RunCheckHttpGetAsync(uri, HttpClient, false);
        }

        public async Task<bool> PingAsync(string ipAddress, TimeSpan? timeout = null)
        {
            var ping = new Ping();
            if (timeout == null)
            {
                timeout = TimeSpan.FromMilliseconds(200);
            }

            try
            {
                var pingReply = await ping.SendPingAsync(ipAddress, (int)timeout.Value.TotalMilliseconds);

                return pingReply.Status == IPStatus.Success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        private async Task<bool> RunCheckHttpGetAsync(string uri, HttpClient httpClient, bool dispose)
        {
            try
            {
                var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));
                //return response.StatusCode == HttpStatusCode.OK;
                var responseStatusCode = (int)response.StatusCode;
                return responseStatusCode >= 200 && responseStatusCode < 300;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
            finally
            {
                if (dispose)
                {
                    httpClient.Dispose();
                }
            }
        }
        
        public void Dispose()
        {
            HttpClient?.Dispose();
        }

        #region for extensions and simple use

        public static INetCheckHelper Instance => Resolve == null ? _default : Resolve();
        private static readonly INetCheckHelper _default = new NetCheckHelper();
        public static Func<INetCheckHelper> Resolve = null;

        #endregion
    }
}
