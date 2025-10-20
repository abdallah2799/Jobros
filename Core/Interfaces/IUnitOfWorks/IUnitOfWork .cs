using Core.Interfaces.IRepositories;
using Core.Entities;
using System;
using System.Threading.Tasks;

namespace Core.Interfaces.IUnitOfWorks
{
    /// <summary>
    /// Unit of Work aggregates repositories and controls saving (atomic commit).
    /// Add repositories here as your domain grows.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        public IRepository<Job> Jobs { get; }
        public IRepository<Category> Categories { get; }
        public IRepository<Application> Applications { get; }
        public IRepository<JobSeeker> JobSeekers { get; }

        /// <summary>
        /// Commit all changes in a single transaction (or save changes).
        /// </summary>
        public Task<int> CompleteAsync();
    }
}
