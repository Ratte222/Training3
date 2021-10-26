using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Training3.Controllers
{
    //https://stackoverflow.com/questions/44851970/implement-sending-server-sent-events-in-c-sharp-no-asp-net-mvc
    //https://dev.to/praneetnadkar/understanding-server-sent-events-17n8
    //https://developer.mozilla.org/ru/docs/Web/API/Server-sent_events/Using_server-sent_events
    [Route("api/[controller]")]
    [ApiController]
    public class SSEController : ControllerBase
    {
        [HttpGet]
        public async Task Get()
        {
            var response = Response;
            response.Headers.Add("Content-Type", "text/event-stream");
            response.Headers.Add("Access-Control-Allow-Origin", "*");

            for (var i = 0; true; ++i)
            {
                await response.WriteAsync($"data: Controller {i} at {DateTime.Now}\r\r");

                //response.Body.Flush();
                await Task.Delay(5 * 1000);
            }
        }
    }
}
