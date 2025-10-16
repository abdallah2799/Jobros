using Core.Entities;
using Core.Interfaces.IRepositories;
using Core.Interfaces.IUnitOfWorks;
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
       public ApplicationDbContext db;
         public UnitOfWork(ApplicationDbContext db)
          {
                this.db = db;
        }

        IRepository<Job> IUnitOfWork.Jobs => throw new NotImplementedException();

        IRepository<Category> IUnitOfWork.Categories => throw new NotImplementedException();

        IRepository<Application> IUnitOfWork.Applications => throw new NotImplementedException();

        Task<int> IUnitOfWork.CompleteAsync()
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
