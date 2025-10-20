using AutoMapper;
using Core.DTOs.Admin;
using Core.DTOs.Auth;
using Core.DTOs.Job;
using Core.Entities;
using Core.Interfaces.IServices.IAdmin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        // You might need IUnitOfWork for jobs/applications later
        // private readonly IUnitOfWork _unitOfWork;

        public AdminService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        Task<IdentityResult> IAdminService.ActivateUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        async Task<IEnumerable<UserResponseDTO>> IAdminService.GetUnverifiedEmployersAsync()
        {
            var unverifiedEmployers = await _userManager.Users.OfType<Employer>().Where(e => !e.IsVerified).ToListAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(unverifiedEmployers);
        }

        public async Task<IdentityResult> ApproveEmployerAsync(string employerId)
        {
            var user = await _userManager.FindByIdAsync(employerId);

            // Safety check: ensure the user exists and is actually an Employer
            if (user is not Employer employer)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Employer not found." });
            }

            employer.IsVerified = true;
            var result = await _userManager.UpdateAsync(employer);

            // Optional: Send an email notification upon approval
            // if (result.Succeeded) { ... send email ... }

            return result;
        }

        public async Task<IdentityResult> RejectEmployerAsync(string employerId)
        {
            // Deleting
            var user = await _userManager.FindByIdAsync(employerId);
            if (user is not Employer employer || employer.IsVerified)
            {
                return await Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Employer not found or is already verified." }));
            }
            return await _userManager.DeleteAsync(employer);
        }

        Task<IdentityResult> IAdminService.CreateAdminAsync(UserRegisterDTO model)
        {
            throw new NotImplementedException();
        }

        Task<IdentityResult> IAdminService.DeactivateUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        Task IAdminService.DeleteJobAsync(int jobId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<JobDto>> IAdminService.GetAllJobsAsync()
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<UserResponseDTO>> IAdminService.GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        Task<DashboardStatsDTO> IAdminService.GetDashboardStatsAsync()
        {
            throw new NotImplementedException();
        }

        Task IAdminService.SendAnnouncementToAllUsersAsync(string subject, string message)
        {
            throw new NotImplementedException();
        }
    }
}
