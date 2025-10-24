using Core.DTOs.EmployerDTOs;
using Core.DTOs.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices.IEmployer
{
    public interface IJobService
    {
        Task<bool> CreateJobAsync(int employerId, CreateJobDto model);
        Task<IEnumerable<JobDto>> GetEmployerJobsAsync(int employerId);
        Task<JobDto?> GetJobByIdAsync(int jobId, int employerId);
        Task<bool> UpdateJobAsync(int jobId, int employerId, EditJobDto model);
        Task<bool> DeleteJobAsync(int jobId, int employerId);
        Task<bool> ActivateJobAsync(int jobId, int employerId);
        Task<bool> DeactivateJobAsync(int jobId, int employerId);
        bool IsJobOwnedByEmployer(int jobId, int employerId);

        // Use fully qualified type to avoid ambiguity between JobAnalyticsDto classes
        Task<Core.DTOs.EmployerDTOs.JobAnalyticsDto> GetJobAnalyticsAsync(int jobId, int employerId);

    }
}
