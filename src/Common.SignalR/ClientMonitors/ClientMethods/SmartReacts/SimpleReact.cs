using System.Threading.Tasks;
using Common.SignalR.Refs;

namespace Common.SignalR.ClientMonitors.ClientMethods.SmartReacts
{
    public class SimpleReact : IClientMethodInvokeProcess
    {
        public static string SimpleReactActionId = "SimpleReact";

        public float ProcessOrder { get; set; }
        public bool ShouldProcess(IClientMethodInvoke invoke)
        {
            var smartActionInfo = invoke.TryGetSmartActionInfo<SmartActionInfo>();
            return smartActionInfo != null && SimpleReactActionId.MyEquals(smartActionInfo.SmartActionId);
        }

        public Task ProcessAsync(IClientMethodInvoke invoke)
        {
            var smartActionInfo = invoke.TryGetSmartActionInfo();
            var reactorClientId = smartActionInfo.ReactorClientId;
            var reactorMethod = smartActionInfo.ReactorMethod;
            var smartId = smartActionInfo.SmartActionId;
            var smartResult = string.Format("{0}, {1}, {2} Processed At Server!", smartId, reactorClientId, reactorMethod);
            invoke.TrySaveSmartActionResult(smartResult);
            return Task.CompletedTask;
        }
    }
}
