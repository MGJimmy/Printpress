using Printpress.Application;
using System.Linq.Expressions;

namespace Printpress.Infrastructure
{
    public static class IQueryableExtensions
    {
        private const int _maxPageSize = 200;
        public static IQueryable<TEntity> SelectPage<TEntity>(this IQueryable<TEntity> query, Paging paging)
        {
            if (paging.PageNumber <= 0) throw new ArgumentException("page number must be grater than zero ");

            if (paging.PageSize <= 0) throw new ArgumentException("page size must be grater than zero ");

            if (paging.PageSize >= _maxPageSize) throw new ArgumentException("page size grater than max page size");

            query = query.Skip((paging.PageNumber - 1) * paging.PageSize).Take(paging.PageSize);

            return query;
        }
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, Sorting sorting)
        {
            if (string.IsNullOrWhiteSpace(sorting?.Field))
            {
                return query;
            }

            var propertyNames = sorting.Field.Split(".");

            var pe = Expression.Parameter(typeof(TEntity), string.Empty);
            var property = propertyNames.Aggregate<string, Expression>(pe, Expression.PropertyOrField);
            var lambda = Expression.Lambda(property, pe);
            var orderByDir = sorting.Dir == SortingDirection.ASC ? "OrderBy" : "OrderByDescending";

            var call = Expression.Call(typeof(Queryable), orderByDir, new[] { typeof(TEntity), property.Type }, query.Expression, Expression.Quote(lambda));

            return (IOrderedQueryable<TEntity>)query.Provider.CreateQuery<TEntity>(call);
        }
    }
}
