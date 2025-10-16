using Core.Entities;
using Core.Interfaces.IRepositories;
using Core.Interfaces.IUnitOfWorks;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.UnitOfWorks
{
    internal class UnitOfWork : IUnitOfWork
    {
        ApplicationDbContext db;
         public UnitOfWork(ApplicationDbContext db)
          {
                this.db = db;
        }
        private IRepository<Job>? _jobs;
        private IRepository<Category>? _categories;
        private IRepository<Application>? _applications;
       IRepository<Job> IUnitOfWork.Jobs{get{return _jobs ??= new Repository<Job>(db);}}

        IRepository<Category> IUnitOfWork.Categories { get { return _categories ??= new Repository<Category>(db); } }

        IRepository<Application> IUnitOfWork.Applications { get { return _applications ??= new Repository<Application>(db); } }

        async Task<int> IUnitOfWork.CompleteAsync()
        {
          return await db.SaveChangesAsync();
        }

        void IDisposable.Dispose()
        {
            db.Dispose();
        }
    }
}
