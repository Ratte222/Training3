using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Training3.Controllers
{
    //https://stackoverflow.com/questions/44851970/implement-sending-server-sent-events-in-c-sharp-no-asp-net-mvc
    //https://dev.to/praneetnadkar/understanding-server-sent-events-17n8
    //https://developer.mozilla.org/ru/docs/Web/API/Server-sent_events/Using_server-sent_events
    //https://techblog.dorogin.com/server-sent-event-aspnet-core-a42dc9b9ffa9
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

        public IActionResult Message()
        {
            return new PushStreamResult(OnStreamAvailabe, HttpContext.RequestAborted);
        }

        void OnStreamAvailabe(Stream stream, CancellationToken requestAborted)
        {
            var wait = requestAborted.WaitHandle;
            var handler = (XLoggerEvent ent) => { ... }
            m_logStore.OnEvent += handler;

            wait.WaitOne(); m_logStore.OnEvent -= handler;
        }
        [HttpGet]
        public HttpResponseMessage Message()
        {
            // TODO: authorize user (out of the post scope)
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK); response.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)OnStreamAvailabe,
         "text/event-stream");
            return response;
        }
        
        private static void WriteEvent(TextWriter writer, string eventType, string data)
        {
            if (!string.IsNullOrEmpty(eventType))
            {
                writer.WriteLine("event:" + eventType);
            }
            writer.WriteLine("data:" + data ?? "");
            writer.WriteLine();
            writer.Flush(); // StreamWriter.Flush calls Flush on underlying Stream
        }
    }
    public class PushStreamResult : IActionResult
    {
        private readonly Action<Stream> _onStreamAvailabe;
        private readonly string _contentType;

        public PushStreamResult(Action<Stream> onStreamAvailabe, string contentType)
        {
            _onStreamAvailabe = onStreamAvailabe;
            _contentType = contentType;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            var stream = context.HttpContext.Response.Body;
            context.HttpContext.Response.GetTypedHeaders().ContentType = new MediaTypeHeaderValue(_contentType);
            _onStreamAvailabe(stream);
            return Task.CompletedTask;
        }
    }
}
