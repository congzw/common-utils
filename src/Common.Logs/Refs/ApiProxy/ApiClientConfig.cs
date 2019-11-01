namespace Common.Logs.Refs.ApiProxy
{
    public class ApiClientConfig
    {
        public ApiClientConfig()
        {
            BaseUri = "http://localhost:16685/api/trace";
            FailTimeoutMilliseconds = 200;
        }

        public int FailTimeoutMilliseconds { get; set; }
        public string BaseUri { get; set; }


        private bool fixBaseUri = false;
        public string GetRequestUri(string method)
        {
            if (!fixBaseUri)
            {
                BaseUri = BaseUri.TrimEnd('/') + "/";
            }

            return BaseUri + method;
        }
    }
}
