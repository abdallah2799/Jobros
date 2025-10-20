using Core.DTOs.Application;
using Core.DTOs.Job;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices.IQueries
{
    public interface IJobSeekerQueryService
    {
        Task<IEnumerable<JobDto>> GetActiveJobsAsync(string? keyword = null, int? categoryId = null, string? employerName = null, string? location = null, string? jobType = null, int page = 1, int pageSize = 10);
        Task<JobDto?> GetJobByIdAsync(int jobId);
        Task<IEnumerable<ApplicationDto>> GetApplicationsByJobSeekerAsync(int jobSeekerId);
        Task<Core.DTOs.JobSeeker.JobSeekerDto?> GetProfileAsync(int jobSeekerId);
    }
}
