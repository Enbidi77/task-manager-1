using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Database;

namespace TaskManager.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly FlowTaskDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(FlowTaskDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            var entry = await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entry.Entity;
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(
            int skip,
            int take,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeString = "",
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (!string.IsNullOrWhiteSpace(includeString))
            {
                foreach (var includeProperty in includeString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.Skip(skip).Take(take).ToListAsync(cancellationToken);
        }
    }
}
