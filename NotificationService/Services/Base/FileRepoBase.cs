using DAL_NS.Entity;
using DAL_NS.Entity.Base;
using NotificationService.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Threading.Tasks;
using NotificationService.Interfaces;
using NotificationService.Exceptions;
using System.Threading;

namespace NotificationService.Services.Base
{
    public class FileRepoBase<TModel> : IBaseService<TModel, string> 
        where TModel : BaseEntity<string>
    {
        //private List<TModel> notifications;
        private readonly IFileProviderService<TModel> _fileProviderService;
        private readonly AutoResetEvent _waitHandler = new AutoResetEvent(true);
        public FileRepoBase(IFileProviderService<TModel> fileProviderService)
        {
            _fileProviderService = fileProviderService;
        }
        public Task CommitTransactionAsync()
        {
            return Task.CompletedTask;
        }

        public TModel Create(TModel item)
        {
            List<TModel> notifications = _fileProviderService.ReadFromDisck();
            //TODO: check existing Id
            //we will consider that there will be no conflicts 
            item.Id = Guid.NewGuid().ToString();
            notifications.Add(item);
            _fileProviderService.WriteToDisck(notifications);
            return item;
        }

        public void CreateRange(IEnumerable<TModel> items)
        {
            List<TModel> notifications = _fileProviderService.ReadFromDisck();
            //TODO: check existing Id
            //we will consider that there will be no conflicts 
            foreach (var item in items)
            {
                item.Id = Guid.NewGuid().ToString();
            }
            notifications.AddRange(items);
            _fileProviderService.WriteToDisck(notifications);
        }

        public Task CreateRangeAsync(IEnumerable<TModel> items)
        {
            CreateRange(items);
            return Task.CompletedTask;
        }

        public void Delete(string id)
        {
            List<TModel> notifications = _fileProviderService.ReadFromDisck();
            var model = notifications.FirstOrDefault(i => i.Id == id);
            if (model != null)
            {
                bool res = notifications.Remove(model);
                if (!res)
                    throw new EntityNotFoundException($"Item with this Id = {model.Id} does not exist");
            }
            _fileProviderService.WriteToDisck(notifications);
        }

        public Task DeleteRangeAsync(IEnumerable<TModel> items)
        {
            List<TModel> notifications = _fileProviderService.ReadFromDisck();
            foreach (var item in items)
            {
                bool res = notifications.Remove(item);
                if (!res)
                    throw new EntityNotFoundException($"Item with this Id = {item.Id} does not exist");
            }
            _fileProviderService.WriteToDisck(notifications);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            
        }

        public TModel Get(Func<TModel, bool> func)
        {
            throw new NotImplementedException();
            //_notifications.Find(func);
        }

        public System.Linq.IQueryable<TModel> GetAll()
        {
            return GetAll_Queryable();
        }

        public IEnumerable<TModel> GetAll_Enumerable()
        {
            List<TModel> notifications = _fileProviderService.ReadFromDisck();
            return notifications.AsEnumerable();
        }

        public System.Linq.IQueryable<TModel> GetAll_Queryable()
        {
            List<TModel> notifications = _fileProviderService.ReadFromDisck();
            return notifications.AsQueryable();
        }

        public Task<TModel> GetAsync(System.Linq.Expressions.Expression<Func<TModel, bool>> func)
        {
            throw new NotImplementedException();
        }

        public bool IsExists(string id)
        {
            throw new NotImplementedException();
        }

        public Task RollbackTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task StartTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(TModel item)
        {
            var notifications = _fileProviderService.ReadFromDisck();
            bool res = notifications.Remove(item);
            if (!res)
                throw new EntityNotFoundException($"Item with this Id = {item.Id} does not exist");
            notifications.Add(item);
            _fileProviderService.WriteToDisck(notifications);
        }

        public Task UpdateRangeAsync(IEnumerable<TModel> items)
        {
            var notifications = _fileProviderService.ReadFromDisck();
            foreach (var item in items)
            {
                bool res = notifications.Remove(item);
                if (!res)
                    throw new EntityNotFoundException($"Item with this Id = {item.Id} does not exist");
                notifications.Add(item);
            }
            _fileProviderService.WriteToDisck(notifications);
            return Task.CompletedTask;
        }

        
    }
}
