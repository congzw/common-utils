// ReSharper disable once CheckNamespace
namespace Common.SignalR
{
    public static class HubCrossEventExtensions
    {
        public static bool IsOutsideHub(this IHubCrossEvent theEvent)
        {
            return theEvent.Context != null;
        }
    }
}