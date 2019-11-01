using System;
using System.Threading.Tasks;
using Common.Logs.Refs;

namespace Common.Logs.Remotes
{
    public interface ILogSender
    {
        Task SendAsync(LogMessageArgs args);
    }

    public class LogSender : ILogSender
    {
        public Task SendAsync(LogMessageArgs args)
        {
            return Task.CompletedTask;
        }

        #region for di extensions

        private static readonly Lazy<ILogSender> LazyInstance = new Lazy<ILogSender>(() => new LogSender());
        public static Func<ILogSender> Resolve { get; set; } = () => LazyInstance.Value;

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
                var logSender = LogSender.Resolve();
                logSender.SendAsync(args);
            });
            return simpleLogFactory;
        }
    }
}
