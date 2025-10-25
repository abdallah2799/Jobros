using Core.DTOs.Application;
using Core.DTOs.Job;
using Core.DTOs.JobSeeker;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices.IQueries
{
    public interface IJobSeekerQueryService
    {
        Task<JobSeekerDashboardStatsDto> GetDashboardsatsAsync(int jobSeekerId);

        Task<IEnumerable<JobDto>> GetActiveJobsAsync(
            string? keyword = null,
            int? categoryId = null,
            string? employerName = null,
            string? location = null,
            string? jobType = null,
            int page = 1,
            int pageSize = 10);

        Task<int> GetActiveJobsTotalCountAsync(
            string? keyword = null,
            int? categoryId = null,
            string? employerName = null,
            string? location = null,
            string? jobType = null);

        Task<JobDto?> GetJobByIdAsync(int jobId);

        Task<bool> HasAppliedAsync(int jobSeekerId, int jobId);

        Task<IEnumerable<ApplicationDto>> GetApplicationsByJobSeekerAsync(int jobSeekerId);

        Task<JobSeekerDto?> GetProfileAsync(int jobSeekerId);
    }
}

