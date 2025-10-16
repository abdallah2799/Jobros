using Core.Interfaces.IRepositories;
using Infrastructure.Data;
using Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    

    public class Repository<T> : IRepository<T> where T : class
    {
        readonly ApplicationDbContext db;
        readonly DbSet<T> _dbSet;
        public Repository(ApplicationDbContext db)
        {
            this.db= db;
            _dbSet = db.Set<T>();
        }
          async Task IRepository<T>.AddAsync(T entity)
        {
          await  db.Set<T>().AddAsync(entity);
        }

        IQueryable<T> IRepository<T>.AsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        void IRepository<T>.Delete(T entity)
        {
            db.Set<T>().Remove(entity);
        }

         async Task<IEnumerable<T>> IRepository<T>.FindAsync(Expression<Func<T, bool>> predicate)
        {
            
            return await  db.Set<T>().Where(predicate).ToListAsync(); ;
            
        }

        async Task<IEnumerable<T>> IRepository<T>.GetAllAsync()
        {
           return await db.Set<T>().ToListAsync();
        }

        async Task<T?> IRepository<T>.GetByIdAsync(int id)
        {
          return await  db.Set<T>().FindAsync(id);
        }

        void IRepository<T>.Update(T entity)
        {
            db.Entry(entity).State = EntityState.Modified;
        }
    }
}
