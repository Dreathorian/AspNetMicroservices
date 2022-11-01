using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Common;
using Ordering.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Repositories
{
    public abstract class RepositoryBase<TEntity> : IAsyncRepository<TEntity>
        where TEntity : EntityBase
    {
        private readonly DbContext _dbContext;

        public RepositoryBase(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync() => await _dbContext.Set<TEntity>().ToListAsync();

        public async Task<IReadOnlyList<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string? includeString = null,
            bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            if (disableTracking) query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeString))
            {
                query = query.Include(includeString);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return orderBy != null
                ? await orderBy(query).ToListAsync()
                : await query.ToListAsync();
        }

        public async Task<IReadOnlyList<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            List<Expression<Func<TEntity, object>>>? includes = null,
            bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            if (disableTracking) query = query.AsNoTracking();

            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (predicate != null) query = query.Where(predicate);

            return orderBy != null
                ? await orderBy(query).ToListAsync()
                : await query.ToListAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id) => await _dbContext.Set<TEntity>().FindAsync(id);

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
