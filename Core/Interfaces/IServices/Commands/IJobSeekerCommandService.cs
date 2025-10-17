using Core.DTOs.Application;
using Core.DTOs.JobSeeker;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices.Commands
{
    public interface IJobSeekerCommandService
    {
        Task<int> ApplyToJobAsync(ApplicationCreateDto dto, int jobSeekerId);
        Task UpdateProfileAsync(int jobSeekerId, JobSeekerUpdateDto dto);
        Task DeleteApplicationAsync(int applicationId, int jobSeekerId);
    }
}
