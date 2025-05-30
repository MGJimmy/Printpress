using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Printpress.Application;
using Printpress.Domain;
using Printpress.Domain.Entities;
using System.Linq.Expressions;

namespace Printpress.Infrastructure.Repository
{
    internal class GenericRepository<T> : IGenericRepository<T> where T : Entity
    {
        public GenericRepository(ApplicationDbContext dbContext)
        {
            Context = dbContext;
        }
        protected ApplicationDbContext Context { get; set; }
        public async Task<List<T>> AllAsync(params string[] includes)
        {
            var Items = Context.Set<T>().AsNoTracking().AsQueryable();

            if (includes?.Any() == true)
            {
                foreach (var include in includes)
                {
                    Items = Items.Include(include);
                }
            }

            return await Items.ToListAsync();
        }
        public async Task<PagedList<T>> AllAsync(Paging paging, Sorting sorting = null, params string[] includes)
        {
            var Items = Context.Set<T>()
                .OrderBy(sorting)
                .SelectPage(paging);

            if (includes?.Any() == true)
            {
                foreach (var include in includes)
                {
                    Items = Items.Include(include);
                }
            }

            return new PagedList<T>
            {
                Items = await Items.ToListAsync(),
                TotalCount = Count(),
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize
            };
        }
        public IEnumerable<T> Filter(Expression<Func<T, bool>> query, params string[] includes)
        {
            var Items = Context.Set<T>().Where(query);

            if (includes?.Any() == true)
            {
                foreach (var include in includes)
                {
                    Items = Items.Include(include);
                }
            }

            return Items.AsEnumerable();
        }
        public IEnumerable<T> Filter(Expression<Func<T, bool>> query, bool track = true, params string[] includes)
        {
            var Items = track ? Context.Set<T>().Where(query) : Context.Set<T>().AsNoTracking().Where(query);

            if (includes?.Any() == true)
            {
                foreach (var include in includes)
                {
                    Items = Items.Include(include);
                }
            }

            return Items.AsEnumerable();
        }
        public PagedList<T> Filter(Paging paging, Expression<Func<T, bool>> query, Sorting sorting = null)
        {
            var Items = Context.Set<T>()
                .Where(query)
                .OrderBy(sorting)
                .SelectPage(paging);

            return new PagedList<T>
            {
                Items = Items.AsEnumerable(),
                TotalCount = Count(query),
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize
            };
        }
        public async Task<PagedList<T>> FilterAsync(Paging paging, Expression<Func<T, bool>> query, Sorting sorting = null)
        {
            var Items = Context.Set<T>()
                .Where(query)
                .OrderBy(sorting)
                .SelectPage(paging);

            return new PagedList<T>
            {
                Items = await Items.ToListAsync(),
                TotalCount = Count(query),
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize
            };
        }
        public PagedList<T> Filter(Paging paging, Expression<Func<T, bool>> query, Sorting sorting = null, params string[] includes)
        {
            var Items = Context.Set<T>()
                .Where(query)
                .OrderBy(sorting)
                .SelectPage(paging);

            if (includes?.Any() == true)
            {
                includes.ToList().ForEach(x => Items = Items.Include(x));
            }

            return new PagedList<T>
            {
                Items = Items.AsEnumerable(),
                TotalCount = Count(query),
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize
            };
        }
        public T FirstOrDefault(bool track = true)
        {
            return track ? Context.Set<T>().FirstOrDefault() : Context.Set<T>().AsNoTracking().FirstOrDefault();
        }
        public T FirstOrDefault(Expression<Func<T, bool>> query, bool track = true)
        {
            return track ? Context.Set<T>().FirstOrDefault(query) : Context.Set<T>().AsNoTracking().FirstOrDefault(query);
        }
        public T FirstOrDefault(Expression<Func<T, bool>> query, params string[] includes)
        {
            var Items = Context.Set<T>().Where(query);

            if (includes?.Any() == true)
            {
                foreach (var include in includes)
                {
                    Items = Items.Include(include);
                }
            }

            return Items.FirstOrDefault();
        }
        public T FirstOrDefault(Expression<Func<T, bool>> query, bool track = true, params string[] includes)
        {
            var Items = Context.Set<T>().Where(query);

            if (includes?.Any() == true)
            {
                foreach (var include in includes)
                {
                    Items = Items.Include(include);
                }
            }

            return track ? Items.FirstOrDefault() : Items.AsNoTracking().FirstOrDefault();
        }
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> query, bool track = true, params string[] includes)
        {
            var Items = Context.Set<T>().Where(query);

            if (includes?.Any() == true)
            {
                foreach (var include in includes)
                {
                    Items = Items.Include(include);
                }
            }

            return await (track ? Items.FirstOrDefaultAsync() : Items.AsNoTracking().FirstOrDefaultAsync());
        }
        public T Find(params object[] id)
        {
            return Context.Set<T>().Find(id);
        }
        public async Task<T> FindAsync(params object[] id)
        {
            return await Context.Set<T>().FindAsync(id);
        }
        public int Count()
        {
            return Context.Set<T>().Count();
        }
        public int Count(Expression<Func<T, bool>> query)
        {
            return Context.Set<T>().Count(query);
        }
        public bool Any()
        {
            return Context.Set<T>().Any();
        }
        public bool Any(Expression<Func<T, bool>> query)
        {
            return Context.Set<T>().Any(query);
        }
        public void Remove(T entity)
        {
            if (entity != null)
            {
                Context.Set<T>().Remove(entity);
            }
        }
        public void RemoveAll(Expression<Func<T, bool>> query)
        {
            Context.Set<T>().Where(query).ExecuteDelete();
        }
        public void AddOrUpdate(T entity, string userId)
        {
            Context.CurrentUserId = userId;

            Context.ChangeTrackedEntityStates(entity);
        }
        public async Task<T> AddAsync(T entity, string userId)
        {
            Context.CurrentUserId = userId;

            await Context.AddAsync(entity);
            return entity;
        }
        public T Update(T entity, string userId)
        {
            Context.CurrentUserId = userId;

            Context.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;

            return entity;
        }

    }
}
