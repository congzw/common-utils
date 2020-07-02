namespace Common.SignalR.ClientMonitors.ClientMethods
{
    public interface ISmartActionInfo
    {
        string SmartActionId { get; set; }
        string ReactorClientId { get; set; }
        string ReactorMethod { get; set; }
    }

    public class SmartActionInfo : ISmartActionInfo
    {
        public string SmartActionId { get; set; }
        public string ReactorMethod { get; set; }
        public string ReactorClientId { get; set; }
    }

    public static class SmartActionExtensions
    {
        public static string SmartActionInfoKey = "smartActionInfo";
        public static string SmartActionResultKey = "smartActionResult";

        public static T TryGetSmartActionInfo<T>(this IClientMethodInvoke clientMethodInvoke) where T : ISmartActionInfo
        {
            if (clientMethodInvoke == null)
            {
                return default(T);
            }

            clientMethodInvoke.Bags.TryGetValue(SmartActionInfoKey, out var smartActionInfo);
            if (smartActionInfo == null)
            {
                return default(T);
            }
            return (T)smartActionInfo;
        }

        public static SmartActionInfo TryGetSmartActionInfo(this IClientMethodInvoke clientMethodInvoke)
        {
            return TryGetSmartActionInfo<SmartActionInfo>(clientMethodInvoke);
        }

        public static void TrySaveSmartActionResult(this IClientMethodInvoke clientMethodInvoke, object result)
        {
            if (clientMethodInvoke == null)
            {
                return;
            }

            clientMethodInvoke.Bags[SmartActionResultKey] = result;
        }
    }
}
