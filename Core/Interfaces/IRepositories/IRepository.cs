using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Interfaces.IRepositories
{
    /// <summary>
    /// Generic repository contract with expression-based filtering and queryable access.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(string? includeProperties);
        /// <summary>
        /// Find by predicate (translated to SQL by EF when used).
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Expose IQueryable for composing complex queries (filtering, sorting, paging) in services.
        /// </summary>
        IQueryable<T> AsQueryable();

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
