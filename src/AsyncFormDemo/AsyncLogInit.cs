using Common;

namespace AsyncFormDemo
{
    public class AsyncLogInit
    {
        public static ISimpleLogFactory Setup()
        {
            var simpleLogFactory = SimpleLogFactory.Resolve();
            simpleLogFactory.LogWithSimpleEventBus();
            return simpleLogFactory;
        }
    }
}
