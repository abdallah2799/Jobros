using Core.DTOs.Admin;
using Core.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices.IAdmin
{
    public interface IAdminService
    {
        // === Dashboard ===
        Task<DashboardStatsDTO> GetDashboardStatsAsync();

        // === User Management ===
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<IdentityResult> CreateAdminAsync(UserRegisterDTO model);
        Task<IdentityResult> DeactivateUserAsync(string userId);
        Task<IdentityResult> ActivateUserAsync(string userId);
        Task<IdentityResult> DeleteUserAsync(string userId);

        // === Employer Verification ===
        Task<IEnumerable<UserResponseDTO>> GetUnverifiedEmployersAsync();
        Task<IdentityResult> ApproveEmployerAsync(string employerId);
        Task<IdentityResult> RejectEmployerAsync(string employerId);

        // === Job Oversight ===
        Task<IEnumerable<JobAdminViewDTO>> GetAllJobsAsync(bool? isActive, int? employerId);
        Task<IEnumerable<EmployerDropdownDTO>> GetAllEmployersAsync();
        Task<IdentityResult> DeleteJobAsync(int jobId);

        // === Application Oversight ===
        Task<IEnumerable<ApplicationAdminViewDTO>> GetAllApplicationsAsync();
        Task<IEnumerable<OverdueApplicationDTO>> GetOverdueApplicationsAsync(int daysThreshold);
        Task<IEnumerable<DailyRegistrationDTO>> GetDailyRegistrationsAsync(int daysToGoBack);

        // === System Config ===
        Task SendAnnouncementToAllUsersAsync(AnnouncementDTO announcement);
    }
}