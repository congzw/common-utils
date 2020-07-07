using System;
using Microsoft.AspNetCore.Mvc;

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
    }
}
