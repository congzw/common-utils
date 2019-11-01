using System.Threading.Tasks;
using Common.Logs.Refs;

namespace Common.Logs.Api
{
    public interface ILogApi
    {
        Task Log(LogArgs args);
    }

    public class LogArgs
    {
        public string Category { get; set; }
        public object Message { get; set; }
        public int Level { get; set; }

    }

    public class LogApi : ILogApi
    {
        public LogApi()
        {
            Factory = SimpleLogFactory.Resolve();
        }

        public ISimpleLogFactory Factory { get; set; }

        public Task Log(LogArgs args)
        {
            //todo send to somewhere...
            var simpleLog = Factory.GetOrCreate(args.Category);
            return simpleLog.Log(args.Message, (SimpleLogLevel)args.Level);
        }
    }
}
