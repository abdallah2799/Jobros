using Core.DTOs.Application;
using Core.DTOs.JobSeeker;
using Core.DTOs.Job;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices.IQueries
{
    public interface IJobSeekerQueryService
    {
        Task<JobSeekerDto> GetProfileAsync(int jobSeekerId);
        Task<IEnumerable<JobDto>> BrowseJobsAsync();
        Task<IEnumerable<ApplicationDto>> GetMyApplicationsAsync(int jobSeekerId);
        Task<ApplicationDto> GetApplicationByIdAsync(int applicationId, int jobSeekerId);
    }
}
