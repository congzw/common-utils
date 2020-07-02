using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Common.SignalR
{
    public class SignalREventBus
    {
        public IEnumerable<ISignalREventHandler> Handlers { get; }

        public SignalREventBus(IEnumerable<ISignalREventHandler> signalREventHandlers)
        {
            Handlers = signalREventHandlers ?? Enumerable.Empty<ISignalREventHandler>();
        }

        public async Task Raise(ISignalREvent hubEvent)
        {
            var sortedHandlers = Handlers
                .Where(x => x.ShouldHandle(hubEvent))
                .OrderBy(x => x.HandleOrder)
                .ToList();

            foreach (var handler in sortedHandlers)
            {
                try
                {
                    //todo trace log
                    await handler.HandleAsync(hubEvent).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    //todo log
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
