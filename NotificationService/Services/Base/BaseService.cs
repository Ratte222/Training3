﻿using AuxiliaryLib.Helpers;
using NotificationService.Interfaces.Base;
using DAL_NS.EF;
using DAL_NS.Entity.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Services.Base
{
    public class BaseService<TModel, TContext, T> : IBaseService<TModel, T> 
        where TModel : BaseEntity<T>//, IDisposable
        where TContext : DbContext
    {
        protected TContext _context;
        public BaseService(TContext appDBContext)
        {
            _context = appDBContext;
        }

        public virtual IQueryable<TModel> GetAll_Queryable()
        {
            return _context.Set<TModel>().AsNoTracking();
        }

        public virtual IQueryable<TModel> GetAll()
        {
            return _context.Set<TModel>();
        }

        public virtual IEnumerable<TModel> GetAll_Enumerable()
        {
            return _context.Set<TModel>().AsNoTracking().AsEnumerable();
        }

        public virtual TModel Get(Func<TModel, bool> func)
        {
            return _context.Set<TModel>().FirstOrDefault(func);
        }

        public virtual async Task<TModel> GetAsync(Expression<Func<TModel, bool>> func)
        {
            return await _context.Set<TModel>().FirstOrDefaultAsync(func);
        }

        public virtual TModel Create(TModel model)
        {
            var result = _context.Set<TModel>().Add(model);
            _context.SaveChanges();
            model.Id = result.Entity.Id;
            return model;
        }

        public virtual async Task CreateRangeAsync(IEnumerable<TModel> items)
        {
            await _context.Set<TModel>().AddRangeAsync(items);
            await _context.SaveChangesAsync();
        }

        public bool IsExists(T id)
        {
            return GetAll_Queryable().Count(x => x.Id.Equals(id)) > 0;
        }
        public virtual void CreateRange(IEnumerable<TModel> items)
        {
            _context.Set<TModel>().AddRange(items);
            _context.SaveChanges();
        }

        public virtual void Update(TModel item)
        {
            _context.Set<TModel>().Update(item);
            _context.SaveChanges();
        }

        public virtual void Delete(T id)
        {
            TModel model = _context.Set<TModel>().Find(id);
            _context.Entry(model).State = EntityState.Detached;

            if (model == null)
            {
                return;
            }
            
            _context.Set<TModel>().Remove(model);
            _context.SaveChanges();
        }

        protected virtual async Task Paginate(IQueryable<TModel> query, PageResponse<TModel> pageResponse)
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

        public virtual async Task UpdateRangeAsync(IEnumerable<TModel> items)
        {
            if (items is null)
                return;
            _context.Set<TModel>().UpdateRange(items);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<TModel> items)
        {
            if (items is null)
                return;
            _context.Set<TModel>().RemoveRange(items);
            await _context.SaveChangesAsync();
            
        }

        public virtual async Task StartTransactionAsync()
        {
           await _context.Database.BeginTransactionAsync();
        }

        public virtual async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public virtual async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
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
            disposed = disposed || Dispose(false);
        }

        #endregion IDisposable
    }
}