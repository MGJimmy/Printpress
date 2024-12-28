using Printpress.Application;
using System.Linq.Expressions;

namespace Printpress.Application
{
    public interface IGenericRepository<T> where T : class
    {
        PagedList<T> All(Paging paging, Sorting sorting = null);
        IEnumerable<T> All(params string[] includes);
        bool Any();
        bool Any(Expression<Func<T, bool>> query);
        int Count();
        int Count(Expression<Func<T, bool>> query);
        IEnumerable<T> Filter(Expression<Func<T, bool>> query, bool track = true, params string[] includes);
        IEnumerable<T> Filter(Expression<Func<T, bool>> query, params string[] includes);
        PagedList<T> Filter(Paging paging, Expression<Func<T, bool>> query, Sorting sorting = null);
        PagedList<T> Filter(Paging paging, Expression<Func<T, bool>> query, Sorting sorting = null, params string[] includes);
        T Find(params object[] id);
        T FirstOrDefault(bool track = true);
        T FirstOrDefault(Expression<Func<T, bool>> query, bool track = true);
        T FirstOrDefault(Expression<Func<T, bool>> query, bool track = true, params string[] includes);
        T FirstOrDefault(Expression<Func<T, bool>> query, params string[] includes);
        void Remove(T entity);
        void RemoveAll(Expression<Func<T, bool>> query);
    }
}