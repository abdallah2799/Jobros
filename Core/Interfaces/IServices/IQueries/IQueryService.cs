using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices.Queries
{
    /// <summary>
    /// Generic query (read-only) service - CQRS style.
    /// Query parameter types / DTOs are intentionally left flexible per entity.
    /// </summary>
    public interface IQueryService<TDto, TFilter>
        where TDto : class
        where TFilter : class
    {
        Task<TDto?> GetByIdAsync(int id);
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<IEnumerable<TDto>> QueryAsync(TFilter filter); // filter DTO drives the composed query
    }
}
