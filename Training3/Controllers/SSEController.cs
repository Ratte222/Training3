using BLL.Events;
using BLL.Interfaces;
using DAL.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
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
        private readonly ExpenseEvents _expenseEvents;
        public SSEController(ExpenseEvents expenseEvents)
        {
            (_expenseEvents) = (expenseEvents);
        }

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

        [HttpGet("AddExpense")]
        public IActionResult Message()
        {
            return new PushStreamResult((Action<Stream, CancellationToken>)OnStreamAvailabe, HttpContext.RequestAborted, "text/event-stream");
        }

        void OnStreamAvailabe(Stream stream, CancellationToken requestAborted)
        {
            var wait = requestAborted.WaitHandle;
            void handler(Expense expense)
            {
                StreamWriter writer = null;
                try
                {
                    writer = new StreamWriter(stream);
                    WriteEvent(writer, "AddExpense", JsonConvert.SerializeObject(expense)/*$"I am work! {DateTime.Now}"*/);
                    //writer.FlushAsync();
                }
                finally
                {
                    writer.DisposeAsync().GetAwaiter();
                }
            }
            _expenseEvents.AddExpense += handler;

            wait.WaitOne();
            _expenseEvents.AddExpense -= handler;
        }
        
        //[HttpGet]
        //public HttpResponseMessage Message()
        //{
        //    // TODO: authorize user (out of the post scope)
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK); 
        //    response.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)OnStreamAvailabe,
        // "text/event-stream");
        //    return response;
        //}
        
        private static void WriteEvent(TextWriter writer, string eventType, string data)
        {
            if (!string.IsNullOrEmpty(eventType))
            {
                writer.WriteLine("event:" + eventType);
            }
            writer.WriteLine("data:" + data ?? "");
            writer.WriteLine();
            writer.WriteLine();
            writer.FlushAsync().GetAwaiter(); // StreamWriter.Flush calls Flush on underlying Stream
        }
    }
    public class PushStreamResult : IActionResult
    {
        private readonly Action<Stream, CancellationToken> _onStreamAvailabe;
        private readonly string _contentType;
        private readonly CancellationToken _requestAborted;

        public PushStreamResult(Action<Stream, CancellationToken> onStreamAvailabe, CancellationToken requestAborted, string contentType)
        {
            (_onStreamAvailabe, _requestAborted, _contentType) = (onStreamAvailabe, requestAborted, contentType);
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            var stream = context.HttpContext.Response.Body;
            context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.HttpContext.Response.GetTypedHeaders().ContentType = new MediaTypeHeaderValue(_contentType);
            _onStreamAvailabe(stream, _requestAborted);
            return Task.CompletedTask;
        }
    }
}
