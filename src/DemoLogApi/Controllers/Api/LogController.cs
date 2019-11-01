using System;
using System.Threading.Tasks;
using Common.Logs.Api;
using Common.Logs.Api.Proxy;
using Microsoft.AspNetCore.Mvc;

namespace DemoLogApi.Controllers.Api
{
    [Route("api/log")]
    [ApiController]
    public class LogController : ControllerBase
    {
        [Route("getDate")]
        [HttpGet]
        public string GetDate()
        {
            return DateTime.Now.ToString("s");
        }

        private readonly ILogApi _logApi;

        public LogController(ILogApi logApi)
        {
            _logApi = logApi;
        }

        [Route("Log")]
        [HttpPost]
        public Task Log(LogArgs args)
        {
            return _logApi.Log(args);
        }

        //for test only
        [Route("LogTest")]
        [HttpGet]
        public Task LogTest([FromQuery]LogArgs args)
        {
            return _logApi.Log(args);
        }

        //for test only: a c# client call
        [Route("LogCallTest")]
        [HttpGet]
        public Task LogCallTest()
        {
            var logApiProxy = LogApiProxy.Resolve();
            return logApiProxy.Log(new LogArgs() {Category = "Foo", Level = 2, Message = "ABC"});
        }
    }
}
