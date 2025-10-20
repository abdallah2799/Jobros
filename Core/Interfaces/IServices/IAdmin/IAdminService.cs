using Core.DTOs.Admin;
using Core.DTOs.Auth;
using Core.DTOs.Job;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices.IAdmin
{
    public interface IAdminService
    {
        // User Management
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<IdentityResult> CreateAdminAsync(UserRegisterDTO model);
        Task<IdentityResult> DeactivateUserAsync(string userId);
        Task<IdentityResult> ActivateUserAsync(string userId);

        // Employer Verification
        Task<IEnumerable<UserResponseDTO>> GetUnverifiedEmployersAsync();
        Task<IdentityResult> ApproveEmployerAsync(string employerId);
        Task<IdentityResult> RejectEmployerAsync(string employerId);

        // Job & Application Oversight
        Task<IEnumerable<JobDto>> GetAllJobsAsync(); // Assuming JobDto exists
        Task DeleteJobAsync(int jobId);

        // Analytics
        Task<DashboardStatsDTO> GetDashboardStatsAsync();

        // System Config
        Task SendAnnouncementToAllUsersAsync(string subject, string message);
    }
}
