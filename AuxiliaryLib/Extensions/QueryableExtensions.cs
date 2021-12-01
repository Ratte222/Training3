using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace AuxiliaryLib.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query,
            string orderProperty, bool isDescending)
        {
            var type = typeof(TEntity);
            var property = type.GetProperty(orderProperty);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(

                typeof(Queryable),
                isDescending ? "OrderByDescending" : "OrderBy",
                new Type[] { type, property.PropertyType },
                query.Expression,
                Expression.Quote(orderByExp)
                );
            return query.Provider.CreateQuery<TEntity>(resultExp);
        }

        public static void ParametricUpdateAndSave(this DbContext dbContext, object item, string[] parameterNames, string[] idsName = null)
        {
            _ = item ?? throw new NullReferenceException($"{nameof(item)} is null");
            _ = parameterNames ?? throw new NullReferenceException($"{nameof(parameterNames)} is null");
            idsName ??= new string[] { "Id" };
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                var type = item.GetType();
                var tCast = typeof(IEnumerable);
                if (type.IsGenericType && tCast.IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                    type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == tCast))
                {
                    IEnumerable listObject = (IEnumerable)item;
                    if (listObject != null)
                    {
                        foreach (object o in listObject)
                        {
                            ParametricUpdate(dbContext, o, parameterNames, idsName);
                        }
                    }
                }
                else
                {
                    ParametricUpdate(dbContext, item, parameterNames, idsName);
                }
                dbContext.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                //_logger.LogWarning(ex, "ParametricUpdate");
                throw ex;
            }
        }
        public static void ParametricUpdate(this DbContext dbContext, object item, string[] parameterNames, string[] idsName)
        {
            _ = item ?? throw new NullReferenceException($"{nameof(item)} is null");
            _ = parameterNames ?? throw new NullReferenceException($"{nameof(parameterNames)} is null");
            idsName ??= new string[] { "Id" };
            dbContext.Attach(item);
            var type = item.GetType();
            var instanceName = type.GetAllPublicProperty().Except(
                type.GetAllPublicCollection().Select(i => i.Name).ToArray())
                .Except(idsName);
            foreach (var parameterName in parameterNames)
            {
                if (instanceName.Any(i => i.Equals(parameterName)))
                {
                    var itemParam = type.GetProperty(parameterName).GetValue(item, null);
                    if (itemParam is not null)
                    {
                        if (!itemParam.GetType().IsClass)
                            dbContext.Entry(item).Property(parameterName).IsModified = true;
                    }
                }
            }
            var propertyInfos = type.GetAllPublicCollection();
            foreach (var propertyInfo in propertyInfos)
            {
                var pT = propertyInfo.PropertyType;
                var tCast = typeof(ICollection<>);
                if (pT.IsGenericType && tCast.IsAssignableFrom(pT.GetGenericTypeDefinition()) ||
                    pT.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == tCast))
                {
                    IEnumerable listObject = (IEnumerable)propertyInfo.GetValue(item, null);
                    if (listObject != null)
                    {
                        foreach (object o in listObject)
                        {
                            ParametricUpdate(dbContext, o, o.GetType().GetAllPublicProperty().ToArray(), idsName);
                        }
                    }
                }
            }
            //_appDBContext.SaveChanges();
        }
    }
}
