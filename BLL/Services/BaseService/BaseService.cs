using AuxiliaryLib.Helpers;
using BLL.Interfaces.Base;
using DAL.EF;
using DAL.Entity.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.BaseService
{
    public class BaseService<T> : IBaseService<T> where T : BaseEntity<int>//, IDisposable
    {
        protected AppDBContext _context;
        public BaseService(AppDBContext appDBContext)
        {
            _context = appDBContext;
        }

        public virtual IQueryable<T> GetAll_Queryable()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public virtual IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public virtual IEnumerable<T> GetAll_Enumerable()
        {
            return _context.Set<T>().AsNoTracking().AsEnumerable();
        }

        public virtual T Get(Func<T, bool> func)
        {
            return _context.Set<T>().FirstOrDefault(func);
        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> func)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(func);
        }

        public virtual T Create(T model)
        {
            var result = _context.Set<T>().Add(model);
            _context.SaveChanges();
            model.Id = result.Entity.Id;
            return model;
        }

        public virtual async Task CreateRangeAsync(IEnumerable<T> items)
        {
            await _context.Set<T>().AddRangeAsync(items);
            await _context.SaveChangesAsync();
        }

        public bool IsExists(int id)
        {
            return GetAll_Queryable().Count(x => x.Id == id) > 0;
        }
        public virtual void CreateRange(IEnumerable<T> items)
        {
            _context.Set<T>().AddRange(items);
            _context.SaveChanges();
        }

        public virtual void Update(T item)
        {
            _context.Set<T>().Update(item);
            _context.SaveChanges();
        }

        public virtual void Delete(int id)
        {
            T model = _context.Set<T>().Find(id);
            _context.Entry(model).State = EntityState.Detached;

            if (model == null)
            {
                return;
            }
            
            _context.Set<T>().Remove(model);
            _context.SaveChanges();
        }

        protected virtual async Task Paginate(IQueryable<T> query, PageResponse<T> pageResponse)
        {
            var totalCount = await query.CountAsync();

            pageResponse.TotalItems = totalCount;
            
            if (pageResponse.TotalItems != 0)
            {
                pageResponse.Items = await query
                .Skip(pageResponse.Skip)
                .Take(pageResponse.Take)
                .ToListAsync();
            }

            //return pageResponse;
        }

        #region IDisposable

        private bool disposed = false;

        public void Dispose()
        {
            if (!disposed && Dispose(true))
            {
                disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        protected virtual bool Dispose(bool manual)
        {
            if (manual)
            {
                _context?.Dispose();
                _context = null;
            }
            return true;
        }

        ~BaseService()
        {
            disposed = !disposed ? Dispose(false) : true;
        }

        #endregion IDisposable
    }
}
