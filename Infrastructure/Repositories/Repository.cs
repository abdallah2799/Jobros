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
        UnitOfWork unit;
        public Repository(UnitOfWork unit)
        {
            this.unit= unit;
        }
          async Task IRepository<T>.AddAsync(T entity)
        {
          await  unit.db.Set<T>().AddAsync(entity);
        }

        IQueryable<T> IRepository<T>.AsQueryable()
        {
            throw new NotImplementedException();
        }

        void IRepository<T>.Delete(T entity)
        {
            unit.db.Set<T>().Remove(entity);
        }

         async Task<IEnumerable<T>> IRepository<T>.FindAsync(Expression<Func<T, bool>> predicate)
        {
            
            return await  unit.db.Set<T>().Where(predicate).ToListAsync(); ;
            
        }

        async Task<IEnumerable<T>> IRepository<T>.GetAllAsync()
        {
           return await unit.db.Set<T>().ToListAsync();
        }

        async Task<T?> IRepository<T>.GetByIdAsync(int id)
        {
          return await  unit.db.Set<T>().FindAsync(id);
        }

        void IRepository<T>.Update(T entity)
        {
            unit.db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
