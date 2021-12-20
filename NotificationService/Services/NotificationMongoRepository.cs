using NotificationService.Helpers;
using NotificationService.Interfaces;
using DAL_NS.Entity;
using MongoDB.Driver;
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace NotificationService.Services
{
    public class NotificationMongoRepository : INotificationService, IProblemNotificationService//INotificationMongoRepository
    {
        //https://dev.to/mpetrinidev/a-guide-to-bulk-write-operations-in-mongodb-with-c-51fk
        private readonly MongoDBSettings _mongoDBSettings;
        private readonly IMongoCollection<Notification> _notificationCollection;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly MongoClient _mongoClient;

        public NotificationMongoRepository(MongoDBSettings mongoDBSettings)
        {
            _mongoDBSettings = mongoDBSettings;
            _mongoClient = new MongoClient(mongoDBSettings.ConnectionString);
            _mongoDatabase = _mongoClient.GetDatabase(mongoDBSettings.DatabaseName);
            _notificationCollection = 
                _mongoDatabase.GetCollection<Notification>(mongoDBSettings.NotificationDatabaseName);            
        }

        public Task CommitTransactionAsync()
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }

        public Notification Create(Notification item)
        {
            _notificationCollection.InsertOne(item);
            return item;
        }

        public void CreateRange(IEnumerable<Notification> items)
        {
            _notificationCollection.InsertMany(items);
        }

        public async Task CreateRangeAsync(IEnumerable<Notification> items)
        {
            await _notificationCollection.InsertManyAsync(items);
        }

        public void Delete(string id)
        {
            Notification notification = new Notification();
            var filter = Builders<Notification>.Filter.Eq(nameof(notification.Id), notification.Id);
            _notificationCollection.DeleteOne(filter);
        }

        public async Task DeleteRangeAsync(IEnumerable<Notification> items)
        {
            var listWrites = new List<WriteModel<Notification>>();
            foreach (var notification in items)
            {
                var filter = Builders<Notification>.Filter.Eq(nameof(notification.Id), notification.Id);
                listWrites.Add(new DeleteOneModel<Notification>(filter));
            };
            await _notificationCollection.BulkWriteAsync(listWrites);
        }

        public void Dispose()
        {
            
        }

        public Notification Get(Func<Notification, bool> func)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Notification> GetAll()
        {
            return _notificationCollection.AsQueryable();
        }

        public IEnumerable<Notification> GetAll_Enumerable()
        {
            return _notificationCollection.AsQueryable().AsEnumerable();
        }

        public IQueryable<Notification> GetAll_Queryable()
        {
            return _notificationCollection.AsQueryable();
        }

        public Task<Notification> GetAsync(Expression<Func<Notification, bool>> func)
        {
            throw new NotImplementedException();
        }

        public Notification[] GetForNotificationServiceSender()
        {
            return GetAll_Queryable().ToArray();
        }

        public bool IsExists(string id)
        {
            throw new NotImplementedException();
        }

        //in theory, this is not correct
        public Task RollbackTransactionAsync()
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }

        public Task StartTransactionAsync()
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }

        public void Update(Notification item)
        {
            var filter = Builders<Notification>.Filter.Eq(nameof(item.Id), item.Id);
            _notificationCollection.ReplaceOne(filter, item);
        }

        public async Task UpdateRangeAsync(IEnumerable<Notification> items)
        {
            var listWrites = new List<WriteModel<Notification>>();
            foreach (var notification in items)
            {
                var filter = Builders<Notification>.Filter.Eq(nameof(notification.Id), notification.Id);
                listWrites.Add(new ReplaceOneModel<Notification>(filter, notification));//x => x.Id == notification.Id
            };
            await _notificationCollection.BulkWriteAsync(listWrites);
        }


        //public async Task AddRangeAsync(IEnumerable<Notification> notifications)
        //{
        //    await _notificationCollection.InsertManyAsync(notifications);            
        //}

        //public IQueryable<Notification> GetQueryable()
        //{
        //    return _notificationCollection.AsQueryable();
        //}

        //public IEnumerable<Notification> Find(FilterDefinition<Notification> filterDefinition)
        //{
        //    return _notificationCollection.Find(filterDefinition).ToEnumerable();
        //}

        //public async Task ReplaceManyByIdAsync(IEnumerable<Notification> notifications)
        //{
        //    var listWrites = new List<WriteModel<Notification>>();
        //    foreach (var notification in notifications)
        //    {
        //        var filter = Builders<Notification>.Filter.Eq(nameof(notification.Id), notification.Id);
        //        listWrites.Add(new ReplaceOneModel<Notification>(filter, notification));//x => x.Id == notification.Id
        //    };
        //    await _notificationCollection.BulkWriteAsync(listWrites);
        //}

        //public void ReplaceOneById(Notification notification)
        //{
        //    var filter = Builders<Notification>.Filter.Eq(nameof(notification.Id), notification.Id);
        //    _notificationCollection.ReplaceOne(filter, notification);            
        //}

        //public async Task DeleteManyAsync(IEnumerable<Notification> notifications)
        //{
        //    var listWrites = new List<WriteModel<Notification>>();
        //    foreach (var notification in notifications)
        //    {
        //        var filter = Builders<Notification>.Filter.Eq(nameof(notification.Id), notification.Id);
        //        listWrites.Add(new DeleteOneModel<Notification>(filter));
        //    };
        //    await _notificationCollection.BulkWriteAsync(listWrites);
        //}

        //public long Count(FilterDefinition<Notification> filterDefinition)
        //{
        //    return _notificationCollection.CountDocuments(filterDefinition);
        //}

        //public async Task<IClientSessionHandle> StartSessionAsync()
        //{
        //    return await _mongoClient.StartSessionAsync();
        //}
    }
}
