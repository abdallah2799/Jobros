using Core.DTOs.Application;
using Core.DTOs.Job;
using Core.DTOs.JobSeeker;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices.Commands
{
    public interface IJobSeekerCommandService
    {
        Task<ApplicationDto> ApplyAsync(int jobSeekerId, ApplicationCreateDto dto);
        Task<bool> DeleteApplicationAsync(int jobSeekerId, int applicationId);
        Task<bool> UpdateProfileAsync(int jobSeekerId, JobSeekerUpdateDto dto, string? resumeUrl = null);
        Task<bool> ChangePasswordAsync(int jobSeekerId, string currentPassword, string newPassword);
    }
}
