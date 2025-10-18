using Core.Entities;
using Core.Interfaces.IRepositories;
using Core.Interfaces.IUnitOfWorks;
using Infrastructure.Repositories;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext db;

        private IRepository<Job>? _jobs;
        private IRepository<Category>? _categories;
        private IRepository<Application>? _applications;
        public UnitOfWork(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IRepository<Job> Jobs => _jobs ??= new Repository<Job>(db);

        public IRepository<Category> Categories => _categories ??= new Repository<Category>(db);

        public IRepository<Application> Applications => _applications ??= new Repository<Application>(db);

        public async Task<int> CompleteAsync()
        {
            return await db.SaveChangesAsync();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
