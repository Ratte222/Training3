using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Interfaces.Base
{
    public interface IBaseService<TModel, T> : IDisposable where TModel : class
    {
        IEnumerable<TModel> GetAll_Enumerable();
        IQueryable<TModel> GetAll_Queryable();
        IQueryable<TModel> GetAll();
        TModel Get(Func<TModel, bool> func);
        Task<TModel> GetAsync(Expression<Func<TModel, bool>> func);
        TModel Create(TModel item);
        Task CreateRangeAsync(IEnumerable<TModel> items);
        void CreateRange(IEnumerable<TModel> items);
        void Update(TModel item);
        Task UpdateRangeAsync(IEnumerable<TModel> items);
        void Delete(T id);
        Task DeleteRangeAsync(IEnumerable<TModel> items);
        bool IsExists(T id);
        Task StartTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
