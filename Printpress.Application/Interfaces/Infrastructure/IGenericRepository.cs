using System.Linq.Expressions;

namespace Printpress.Application
{
    public interface IGenericRepository<T> where T : class
    {
        Task<PagedList<T>> AllAsync(Paging paging, Sorting sorting = null, params string[] includes);
        Task<List<T>> AllAsync(params string[] includes);
        bool Any();
        bool Any(Expression<Func<T, bool>> query);
        int Count();
        int Count(Expression<Func<T, bool>> query);
        IEnumerable<T> Filter(Expression<Func<T, bool>> query, bool track = true, params string[] includes);
        IEnumerable<T> Filter(Expression<Func<T, bool>> query, params string[] includes);
        PagedList<T> Filter(Paging paging, Expression<Func<T, bool>> query, Sorting sorting = null);

        Task<PagedList<T>> FilterAsync(Paging paging, Expression<Func<T, bool>> query, Sorting sorting = null);
        PagedList<T> Filter(Paging paging, Expression<Func<T, bool>> query, Sorting sorting = null, params string[] includes);
        T Find(params object[] id);
        Task<T> FindAsync(params object[] id);
        T FirstOrDefault(bool track = true);
        T FirstOrDefault(Expression<Func<T, bool>> query, bool track = true);
        T FirstOrDefault(Expression<Func<T, bool>> query, bool track = true, params string[] includes);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> query, bool track = true, params string[] includes);
        T FirstOrDefault(Expression<Func<T, bool>> query, params string[] includes);
        void RemoveAll(Expression<Func<T, bool>> query);
        void Remove(T entity);
        Task<T> AddAsync(T entity, string userId);
        T Update(T entity, string userId);
        void AddOrUpdate(T entity, string userId);
    }
}