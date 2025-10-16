using System;
using System.Linq.Expressions;

namespace Core.Interfaces.IRepositories
{
    /// <summary>
    /// A small Specification contract for reusable filter expressions.
    /// Repositories may offer FindAsync(ISpecification{T}) overloads that use this.
    /// </summary>
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
    }
}
