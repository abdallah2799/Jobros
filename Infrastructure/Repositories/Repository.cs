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

namespace Core.Interfaces.Repositories
{
    

    internal class Repository<T> : IRepository<T> where T : class
    {
        ApplicationDbContext db;
        public Repository(ApplicationDbContext db)
        {
            this.db= db;
        }
          async Task IRepository<T>.AddAsync(T entity)
        {
          await  db.Set<T>().AddAsync(entity);
        }

        IQueryable<T> IRepository<T>.AsQueryable()
        {
            throw new NotImplementedException();
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
            db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
