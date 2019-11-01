using System;
using System.Threading.Tasks;
using Common.Logs.Refs;

namespace Common.Logs.Remotes
{
    public interface ISimpleLogProxy : ISimpleLog
    {
        bool IgnoreWrappedLog { get; set; }
    }

    public class SimpleLogProxy : ISimpleLogProxy
    {
        private readonly ISimpleLog _simpleLog;

        public SimpleLogProxy(ISimpleLog simpleLog, bool ignoreWrappedLog)
        {
            _simpleLog = simpleLog;
            IgnoreWrappedLog = IgnoreWrappedLog;
            this.Category = _simpleLog.Category;
            this.EnabledLevel = _simpleLog.EnabledLevel;
        }

        public bool IgnoreWrappedLog { get; set; }

        public string Category { get; set; }
        public SimpleLogLevel EnabledLevel { get; set; }
        public async Task Log(object message, SimpleLogLevel level)
        {
            if (!IgnoreWrappedLog)
            {
                await _simpleLog.Log(message, level).ConfigureAwait(false);
            }
            var logSender = LogSender.Resolve();
            var args = LogMessageArgs.Create(Category, message, level);
            await logSender.SendAsync(args).ConfigureAwait(false);
        }
    }

    public static class SimpleLogProxySetup
    {
        private static Func<CreateSimpleLogArgs, ISimpleLog> oldCreateSimpleLogFunc = null;

        public static void SetupSimpleLogProxy(ISimpleLogFactory simpleLogFactory, bool ignoreWrappedLog)
        {
            if (simpleLogFactory == null)
            {
                throw new ArgumentNullException(nameof(simpleLogFactory));
            }

            if (oldCreateSimpleLogFunc == null)
            {
                oldCreateSimpleLogFunc = simpleLogFactory.CreateSimpleLogFunc;
            }

            simpleLogFactory.CreateSimpleLogFunc = args =>
            {
                var oldOne = oldCreateSimpleLogFunc(args);
                return new SimpleLogProxy(oldOne, ignoreWrappedLog);
            };
        }

    }
}