using System;
using System.Threading.Tasks;
using Common.Logs.Refs;

namespace Common.Logs.Remotes
{
    public interface ILogMessageApi
    {
        Task LogMessage(LogMessageArgs args);
    }

    public class LogMessageApi : ILogMessageApi
    {
        public Task LogMessage(LogMessageArgs args)
        {
            return Task.CompletedTask;
        }

        #region for di extensions

        private static readonly Lazy<ILogMessageApi> LazyInstance = new Lazy<ILogMessageApi>(() => new LogMessageApi());
        public static Func<ILogMessageApi> Resolve { get; set; } = () => LazyInstance.Value;

        #endregion
    }

    public static class LogSenderInit
    {
        public static string SendLog = "SendLog";

        public static ISimpleLogFactory Init(ISimpleLogFactory simpleLogFactory)
        {
            var logActions = simpleLogFactory.LogActions;
            logActions[SendLog] = new LogMessageAction(SendLog, true, args =>
            {
                var logSender = LogMessageApi.Resolve();
                logSender.LogMessage(args);
            });
            return simpleLogFactory;
        }
    }
}
