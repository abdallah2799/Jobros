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
        IRepository<Job> Jobs { get; }
        IRepository<Category> Categories { get; }
        IRepository<Application> Applications { get; }

        /// <summary>
        /// Commit all changes in a single transaction (or save changes).
        /// </summary>
        Task<int> CompleteAsync();
    }
}
