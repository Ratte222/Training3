using BLL.Helpers;
using BLL.Interfaces;
using DAL_NS.Entity;
using MongoDB.Driver;
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class NotificationMongoRepository : INotificationMongoRepository
    {
        //https://dev.to/mpetrinidev/a-guide-to-bulk-write-operations-in-mongodb-with-c-51fk
        private readonly MongoDBSettings _mongoDBSettings;
        private readonly IMongoCollection<Notification> _notificationCollection;

        public NotificationMongoRepository(MongoDBSettings mongoDBSettings)
        {
            _mongoDBSettings = mongoDBSettings;
            var client = new MongoClient(mongoDBSettings.ConnectionString);
            var database = client.GetDatabase(mongoDBSettings.DatabaseName);
            _notificationCollection = database.GetCollection<Notification>(mongoDBSettings.NotificationDatabaseName);            
        }
        public async Task AddRangeAsync(IEnumerable<Notification> notifications)
        {
            //await _notificationCollection.InsertOneAsync(notifications.First());
            await _notificationCollection.InsertManyAsync(notifications);
            //var listWrites = new List<WriteModel<Notification>>();
            //foreach(var notification in notifications)
            //{
            //    listWrites.Add(new InsertOneModel<Notification>(notification));
            //}
            //await _notificationCollection.BulkWriteAsync(listWrites);
        }

        public IQueryable<Notification> GetQueryable()
        {
            return _notificationCollection.AsQueryable();
        }

        public IEnumerable<Notification> Find(FilterDefinition<Notification> filterDefinition)
        {
            return _notificationCollection.Find(filterDefinition).ToEnumerable();
        }

        public async Task ReplaceManyByIdAsync(IEnumerable<Notification> notifications)
        {
            var listWrites = new List<WriteModel<Notification>>();
            foreach (var notification in notifications)
            {
                var filter = Builders<Notification>.Filter.Eq(nameof(notification.Id), notification.Id);
                listWrites.Add(new ReplaceOneModel<Notification>(filter, notification));//x => x.Id == notification.Id
            };
            await _notificationCollection.BulkWriteAsync(listWrites);
        }

        public void ReplaceOneById(Notification notification)
        {
            var filter = Builders<Notification>.Filter.Eq(nameof(notification.Id), notification.Id);
            _notificationCollection.ReplaceOne(filter, notification);            
        }

        public async Task DeleteManyAsync(IEnumerable<Notification> notifications)
        {
            var listWrites = new List<WriteModel<Notification>>();
            foreach (var notification in notifications)
            {
                var filter = Builders<Notification>.Filter.Eq(nameof(notification.Id), notification.Id);
                listWrites.Add(new DeleteOneModel<Notification>(filter));
            };
            await _notificationCollection.BulkWriteAsync(listWrites);
        }

        public long Count(FilterDefinition<Notification> filterDefinition)
        {
            return _notificationCollection.CountDocuments(filterDefinition);
        }
    }
}
