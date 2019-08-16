// ReSharper disable once CheckNamespace
namespace Common
{
    #region demo for LogWithSimpleEventBus
    
    public static class SimpleEventBusExtensions
    {
        public static ISimpleLogFactory LogWithSimpleEventBus(this ISimpleLogFactory simpleLogFactory, ISimpleEventBus<AsyncMessageEvent> simpleEventBus = null)
        {
            simpleEventBus = simpleEventBus ?? SimpleEventBus<AsyncMessageEvent>.Resolve();

            var logActions = simpleLogFactory.LogActions;
            
            logActions.SetActions("LogWithSimpleEventBus", true, args =>
            {
                if (args?.Message != null)
                {
                    simpleEventBus.Raise(new AsyncMessageEvent(args.Message.ToString()));
                }
            });

            var initLog = simpleLogFactory.CreateLogFor("SimpleEventBusExtensions");
            initLog.LogInfo(">>>> LogWithSimpleEventBus");
            return simpleLogFactory;
        }
    }

    #endregion
}