using DAL.Entity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace BLL.Services
{
    public class NamedPipeClient
    {
        private readonly ILogger<NamedPipeClient> _logger;
        private string pipeName = "notificationServiceAddNotificationToQueue";

        public NamedPipeClient(ILogger<NamedPipeClient> logger)
        {
            (_logger) = (logger);
        }

        public bool SendNotification(Notification notification)
        {
            bool result = false;
            var pipeClient =
                         new NamedPipeClientStream(".", pipeName,
                             PipeDirection.InOut, PipeOptions.None,
                             TokenImpersonationLevel.Impersonation);

            _logger.LogDebug("Connecting to server...\n");
            pipeClient.Connect();

            var ss = new StreamString(pipeClient);
            // Validate the server's signature string.
            if (ss.ReadString() == "I am the one true server!")
            {
                string jsonString = JsonConvert.SerializeObject(notification);
                ss.WriteString(jsonString);
                try 
                {
                    var res = ss.ReadString();
                    if (res.Equals("Notification received successfully"))//all good
                    {
                        _logger.LogInformation(res);
                        result = true;
                    }
                    else//has problem
                    {
                        _logger.LogWarning("Notification not processed by notification server");
                    }
                }
                catch(OverflowException ex)//most likely problems with the notification server
                {
                    _logger.LogWarning("Notification not processed by notification server");
                }
                
            }
            else
            {
                _logger.LogInformation("Server could not be verified.");
            }
            pipeClient.Close();
            return result;
        }
    }

    // Defines the data protocol for reading and writing strings on our stream.
    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            int len;
            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            var inBuffer = new byte[len];
            ioStream.Read(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }

}


