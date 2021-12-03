using System.Threading;
using NotificationService.Interfaces;
using System;
using Newtonsoft.Json;
using DAL_NS.Entity;
using Microsoft.Extensions.Logging;
using System.Linq;
using Newtonsoft.Json.Linq;
using AuxiliaryLib.Helpers;
using MongoDB.Driver;
using AuxiliaryLib.NamedPipe;
using System.Threading.Tasks;

namespace NotificationService.Services.NamedPipe
{
    public static class ServerFunctions
    {
        public static event Func<int, Task> Re_sendProblemNotificationsEvent;
        public static bool AddNotification(AutoResetEvent waitHandler, INotificationService notificationService, StreamString ss,
            ILogger logger)
        {
            bool result = false;
            ss.WriteString("I am the one true server!");
            string JsonNotification = ss.ReadString();
            logger.LogDebug($"Recive message \"{JsonNotification}\"");
            //Console.WriteLine("Reading file: {0} on thread[{1}] as user: {2}.",
            //    filename, threadId, pipeServer.GetImpersonationUserName());
            Notification notification = JsonConvert.DeserializeObject<Notification>(JsonNotification);
            //notification.Id = Guid.NewGuid().ToString();
            //notification.DateTimeCreate = DateTime.UtcNow;
            waitHandler.WaitOne();
            try
            {
                notificationService.Create(notification);
                result = true;
            }
            finally
            {
                waitHandler.Set();
            }
            ss.WriteString("Notification received successfully");
            return result;
        }

        public static bool UpdateProblemNotification(INotificationMongoRepository notificationMongoRepository,
            StreamString ss, ILogger logger)
        {
            bool result = false;
            ss.WriteString("I am the one true server!");
            string JsonNotification = ss.ReadString();
            logger.LogDebug($"Recive message \"{JsonNotification}\"");
            //Console.WriteLine("Reading file: {0} on thread[{1}] as user: {2}.",
            //    filename, threadId, pipeServer.GetImpersonationUserName());
            Notification notification = JsonConvert.DeserializeObject<Notification>(JsonNotification);
            //notification.Id = Guid.NewGuid().ToString();
            notification.DateTimeCreate = DateTime.UtcNow;
            notificationMongoRepository.ReplaceOneById(notification);
            ss.WriteString("Notification updated successfully");
            return result;
        }

        public static bool CheckProblemNotification(INotificationMongoRepository notificationMongoRepository, 
            StreamString ss, ILogger logger)
        {
            bool result = false;
            ss.WriteString("I am the one true server!");
            string paginateDataJson = ss.ReadString();
            var j = JObject.Parse(paginateDataJson);//i taining work with json key
            PageResponse<Notification> pageResponse = new 
                PageResponse<Notification>(Convert.ToInt32(j.SelectToken("PageLength")),
                Convert.ToInt32(j.SelectToken("PageNumber")));
            //int pageLength = Convert.ToInt32(j.SelectToken("PageLength"));
            //int pageNumber = Convert.ToInt32(j.SelectToken("PageNumber"));
            var query = notificationMongoRepository.GetQueryable()
                .Skip(pageResponse.Skip).Take(pageResponse.Take);
            pageResponse.Items = query.ToList();
            if (pageResponse.Items.Count > 0)
            {
                pageResponse.TotalItems = notificationMongoRepository.Count(Builders<Notification>.Filter.Empty);
                string problemNotificationsJson = JsonConvert.SerializeObject(pageResponse);
                ss.WriteString(problemNotificationsJson);
            }
            else
            {
                ss.WriteString(ServiceAnswers.AnswerNotFound);
            }
            return result;
        }

        public static void Re_sendProblemNotifications(AutoResetEvent wainHandler, INotificationService notificationService,
            INotificationMongoRepository notificationMongoRepository, StreamString ss, ILogger logger)
        {
            ss.WriteString("I am the one true server!");
            if(ss.ReadString().Equals("Re_send"))
            {
                int take = 0;

                if (int.TryParse(ss.ReadString(), out take))

                    Re_sendProblemNotificationsEvent?.Invoke(take).GetAwaiter();

                //wainHandler.WaitOne();
                //try
                //{                    
                //notificationMongoRepository.DeleteManyAsync(problemNotifications);
                //problemNotifications.ForEach(n =>
                //{
                //    n.NumberOfAttemptToSent = 0;
                //    n.DateTimeOfTheLastAttemptToSend = null;
                //});
                //notificationService.CreateRangeAsync(problemNotifications).GetAwaiter();
                //}
                //finally
                //{
                //    wainHandler.Set();
                //}                
                ss.WriteString(ServiceAnswers.AnswerOk);
            }

        }
    }
}