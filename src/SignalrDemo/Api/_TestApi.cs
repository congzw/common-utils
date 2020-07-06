using System;
using System.Threading.Tasks;
using Common;
using Common.SignalR;
using Common.SignalR.ClientMonitors;
using Common.SignalR.ClientMonitors.ClientStubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace SignalrDemo.Api
{
    [Route("api/test")]
    [ApiController]
    public class TestApiController : ControllerBase
    {
        [Route("getDate")]
        [HttpGet]
        public string GetDate()
        {
            return DateTime.Now.ToString("s");
        }

        [Route("raise")]
        [HttpGet]
        public async Task<string> Raise([FromServices] SignalREventBus bus, [FromServices] IHubContext<_AnyHub> hubContext)
        {
            var asHubContextWrapper = hubContext.AsHubContextWrapper();

            var stub = new InvokeClientStub();
            stub.Method = "updateState";
            stub.SetBagValue("bar", "From Server bar");
            stub.MethodArgs = new { state = "From Server State"};
            //[1240] [_AnyHub] InvokeClientStub >>>>>>>> {"Method":"updateState","MethodArgs":{"state":"<637296097161340000>"},"Bags":{"bar":"bar"}} 
            await bus.Raise(new InvokeClientStubEvent(asHubContextWrapper, stub));
            return "updateState OK";
        }
    }
}
