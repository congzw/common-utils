using System.Threading.Tasks;
using Common.Logs.Refs;

namespace Common.Logs.Remotes.Api
{
    public interface ISimpleLogApi
    {
        Task SendAsync(LogMessageArgs args);
    }

    public class LogApi
    {
        
    }
}
