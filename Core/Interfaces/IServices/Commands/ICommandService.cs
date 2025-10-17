using System.Threading.Tasks;

namespace Core.Interfaces.IServices.Commands
{
    /// <summary>
    /// Generic command (write) service - CQRS style.
    /// Contains operations that change state.
    /// </summary>
    public interface ICommandService<TDto> where TDto : class
    {
        Task<int> CreateAsync(TDto dto);        // returns created id or status code
        Task UpdateAsync(int id, TDto dto);
        Task DeleteAsync(int id);


    }
}
