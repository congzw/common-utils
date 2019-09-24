using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
// ReSharper disable CheckNamespace

namespace Common.SignalR.Scoped
{
    #region demos

    //how to let FooHub scoped and support event handle:
    //1.  FooHub(HubEventBus bus), add methods and raise events with need. 
    //2.  services.AddScopedHub()

    #endregion

    public class AnyHub : Hub
    {
        private readonly HubEventBus _hubEventBus;

        public AnyHub(HubEventBus hubEventBus)
        {
            _hubEventBus = hubEventBus;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                await _hubEventBus.Raise(new OnConnectedEvent(this)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            await base.OnConnectedAsync().ConfigureAwait(false);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                await _hubEventBus.Raise(new OnDisconnectedEvent(this, exception)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }

        public Task UpdateScopedConnectionBags(IDictionary<string, object> bags)
        {
            return _hubEventBus.Raise(new OnUpdateBagsEvent(this, bags));
        }
    }
}