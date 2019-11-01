using System;
using System.Threading.Tasks;
using Common.Logs.Refs;

namespace Common.Logs.Remotes
{
    public class ApiLogSender : ILogMessageApi
    {
        public LogSenderConfig Config { get; set; }

        public Task LogMessage(LogMessageArgs args)
        {
            //todo
            return Task.CompletedTask;
        }
    }
    
    public class LogSenderConfig
    {
        public string ApiUri { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
    }

    public static class LogSenderSetup
    {
        private static ApiLogSender apiLogSender = null;

        public static void SetupRemoteApi(LogSenderConfig config)
        {
            if (apiLogSender == null)
            {
                apiLogSender = new ApiLogSender();
            }
            Init(config);
            LogMessageApi.Resolve = () => apiLogSender;
        }

        private static void Init(LogSenderConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            apiLogSender.Config = config;
            //todo api init
        }
    }
}
