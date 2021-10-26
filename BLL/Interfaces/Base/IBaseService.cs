using DAL.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces.Base
{
    public interface IBaseService<T> /*: IDisposable */where T : BaseEntity<int> 
    {
        IEnumerable<T> GetAll_Enumerable();
        IQueryable<T> GetAll_Queryable();
        IQueryable<T> GetAll();
        T Get(Func<T, bool> func);
        Task<T> GetAsync(Expression<Func<T, bool>> func);
        T Create(T item);
        Task CreateRangeAsync(IEnumerable<T> items);
        void CreateRange(IEnumerable<T> items);
        void Update(T item);
        void Delete(int id);
        bool IsExists(int id);
    }
}
