using System;
using System.Threading.Tasks;
using Common;
using Common.SignalR.ClientMonitors;
using Common.SignalR.ClientMonitors.ClientStubs;
using Common.SignalR.EventBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace SignalrDemo.Api
{
    [Route("api/hub")]
    [ApiController]
    public class HubApiController : ControllerBase
    {
        private readonly SignalREventBus _bus;
        private readonly IHubContext<_AnyHub> _hubContext;

        public HubApiController(SignalREventBus bus, IHubContext<_AnyHub> hubContext)
        {
            _bus = bus;
            _hubContext = hubContext;
        }

        [Route("getDate")]
        [HttpGet]
        public string GetDate()
        {
            return DateTime.Now.ToString("s");
        }
        
        [Route("ClientStub")]
        [HttpGet]
        public async Task<string> ClientStub([FromQuery]InvokeClientStub args)
        {
            if (string.IsNullOrWhiteSpace(args.ScopeId))
            {
                return "BAD SCOPE!";
            }

            //for demo!
            args.Method = "updateMessage";
            args.SetBagValue("foo", "From Server foo");
            args.MethodArgs = new { message = "From Server message" };

            await _bus.Raise(new InvokeClientStubEvent(_hubContext.AsHubContextWrapper(), args));
            return "OK";
        }
        
        [Route("raise1")]
        [HttpGet]
        public async Task<string> Raise1([FromServices] SignalREventBus bus, [FromServices] IHubContext<_AnyHub> hubContext, string scopeId)
        {
            var asHubContextWrapper = hubContext.AsHubContextWrapper();
            if (string.IsNullOrWhiteSpace(scopeId))
            {
                return "BAD SCOPE!";
            }

            var stub = new InvokeClientStub();
            stub.ScopeId = scopeId;
            stub.Method = "updateState";
            stub.SetBagValue("bar", "From Server bar");
            stub.MethodArgs = new { state = "From Server State" };
            //[1240] [_AnyHub] InvokeClientStub >>>>>>>> {"Method":"updateState","MethodArgs":{"state":"<637296097161340000>"},"Bags":{"bar":"bar"}} 
            await bus.Raise(new InvokeClientStubEvent(asHubContextWrapper, stub));
            return "updateState OK";
        }

        [Route("raise2")]
        [HttpGet]
        public async Task<string> Raise2([FromServices] SignalREventBus bus, [FromServices] IHubContext<_AnyHub> hubContext, string scopeId)
        {
            var asHubContextWrapper = hubContext.AsHubContextWrapper();
            if (string.IsNullOrWhiteSpace(scopeId))
            {
                return "BAD SCOPE!";
            }

            //[14040] [_AnyHub] InvokeClientStub >>>>>>>> {"ScopeId":"s1","Method":"updateMessage","MethodArgs":{"message":"[637296272749570000]"},"Bags":{"foo":"foo"}} 
            //[14040] [_AnyHub] InvokeClientStub >>>>>>>> {"ScopeId":"s1","Method":"updateMessage","MethodArgs":{"message":"[637296272764020000]"},"Bags":{"foo":"foo"}} 
            //[14040] [_AnyHub] InvokeClientStub >>>>>>>> {"ScopeId":"s1","Method":"updateMessage","MethodArgs":{"message":"From Server message"},"Bags":{"bar":"From Server bar"}} 

            var stub = new InvokeClientStub();
            stub.ScopeId = scopeId;
            stub.Method = "updateMessage";
            stub.SetBagValue("bar", "From Server bar");
            stub.MethodArgs = new { message = "From Server message" };
            await bus.Raise(new InvokeClientStubEvent(asHubContextWrapper, stub));
            return "updateMessage OK";
        }
    }
}
