using AutoMapper;
using Core.DTOs.Admin;
using Core.DTOs.Auth;
using Core.Entities;
using Core.Interfaces.IServices.IAdmin;
using Core.Interfaces.IServices.IEmailServices;
using Core.Interfaces.IUnitOfWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public AdminService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<DashboardStatsDTO> GetDashboardStatsAsync()
        {
            return new DashboardStatsDTO
            {
                TotalUsers = await _userManager.Users.CountAsync(),
                PendingApprovals = await _userManager.Users.OfType<Employer>().CountAsync(e => !e.IsVerified),
                ActiveJobs = await _unitOfWork.Jobs.AsQueryable().CountAsync(j => j.IsActive),
                TotalApplications = await _unitOfWork.Applications.AsQueryable().CountAsync(),
                TotalEmployers = await _userManager.Users.OfType<Employer>().CountAsync(),
                TotalJobSeekers = await _userManager.Users.OfType<JobSeeker>().CountAsync()
            };
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
        }

        public async Task<IdentityResult> CreateAdminAsync(UserRegisterDTO model)
        {
            var user = new Admin { UserName = model.Email, Email = model.Email, FullName = model.FullName, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            return result;
        }

        public async Task<IdentityResult> DeactivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            user.IsActive = false;
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ActivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            user.IsActive = true;
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetUnverifiedEmployersAsync()
        {
            var employers = await _userManager.Users.OfType<Employer>().Where(e => !e.IsVerified).ToListAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(employers);
        }

        public async Task<IdentityResult> ApproveEmployerAsync(string employerId)
        {
            var user = await _userManager.FindByIdAsync(employerId);
            if (user is not Employer employer) return IdentityResult.Failed(new IdentityError { Description = "Employer not found." });
            employer.IsVerified = true;
            return await _userManager.UpdateAsync(employer);
        }

        public async Task<IdentityResult> RejectEmployerAsync(string employerId)
        {
            var user = await _userManager.FindByIdAsync(employerId);
            if (user is not Employer employer || employer.IsVerified) return IdentityResult.Failed(new IdentityError { Description = "Employer not found or is already verified." });
            return await _userManager.DeleteAsync(employer);
        }

        public async Task<IEnumerable<JobAdminViewDTO>> GetAllJobsAsync(bool? isActive, int? employerId)
        {
            // Start with the base query
            IQueryable<Job> query = _unitOfWork.Jobs.AsQueryable()
                .Include(j => j.Employer); // Always include employer for the company name

            // Conditionally add a WHERE clause for the IsActive filter
            if (isActive.HasValue)
            {
                query = query.Where(j => j.IsActive == isActive.Value);
            }

            // Conditionally add a WHERE clause for the EmployerId filter
            if (employerId.HasValue)
            {
                query = query.Where(j => j.EmployerId == employerId.Value);
            }

            // Execute the final, composed query
            var jobs = await query.OrderByDescending(j => j.CreatedAt).ToListAsync();

            return _mapper.Map<IEnumerable<JobAdminViewDTO>>(jobs);
        }

        public async Task<IEnumerable<EmployerDropdownDTO>> GetAllEmployersAsync()
        {
            var employers = await _userManager.Users.OfType<Employer>().OrderBy(e => e.CompanyName).ToListAsync();
            return _mapper.Map<IEnumerable<EmployerDropdownDTO>>(employers);
        }

        public async Task<IdentityResult> DeleteJobAsync(int jobId)
        {
            var job = await _unitOfWork.Jobs.GetByIdAsync(jobId);
            if (job == null) return IdentityResult.Failed(new IdentityError { Description = "Job not found." });

            _unitOfWork.Jobs.Delete(job);
            await _unitOfWork.CompleteAsync();
            return IdentityResult.Success;
        }

        public async Task<IEnumerable<ApplicationAdminViewDTO>> GetAllApplicationsAsync()
        {
            var applications = await _unitOfWork.Applications.AsQueryable()
                .Include(a => a.Job).ThenInclude(j => j.Employer)
                .Include(a => a.JobSeeker)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ApplicationAdminViewDTO>>(applications);
        }

        public async Task SendAnnouncementToAllUsersAsync(AnnouncementDTO announcement)
        {
            var users = await _userManager.Users.Where(u => u.EmailConfirmed).ToListAsync();
            foreach (var user in users)
            {
                await _emailService.SendEmailAsync(user.Email, announcement.Subject, announcement.Body);
            }
        }

        public async Task<IEnumerable<OverdueApplicationDTO>> GetOverdueApplicationsAsync(int daysThreshold)
        {
            // Calculate the cutoff date. Any application submitted before this date is considered overdue.
            var cutoffDate = DateTime.UtcNow.AddDays(-daysThreshold);

            // This query finds all applications that are still 'Pending' and were submitted before the cutoff date.
            var overdueApplications = await _unitOfWork.Applications.AsQueryable()
                .Where(app => app.Status == "Pending" && app.AppliedAt < cutoffDate)
                .Include(app => app.Job).ThenInclude(job => job.Employer) // Need Employer for CompanyName and Email
                .Include(app => app.JobSeeker) // Need JobSeeker for ApplicantName
                .OrderBy(app => app.AppliedAt) // Show the oldest ones first
                .Select(app => new OverdueApplicationDTO
                {
                    ApplicationId = app.Id,
                    JobTitle = app.Job.Title,
                    ApplicantName = app.JobSeeker.FullName,
                    CompanyName = app.Job.Employer.CompanyName,
                    EmployerEmail = app.Job.Employer.Email,
                    AppliedAt = app.AppliedAt,
                    // Calculate the number of days it has been pending
                    DaysPending = (int)(DateTime.UtcNow - app.AppliedAt).TotalDays
                })
                .ToListAsync();

            return overdueApplications;
        }

        public async Task<IEnumerable<DailyRegistrationDTO>> GetDailyRegistrationsAsync(int daysToGoBack)
        {
            // Calculate the start date for our query window.
            var startDate = DateTime.UtcNow.Date.AddDays(-daysToGoBack);

            var registrationData = await _userManager.Users
                // 1. Filter to only include users created within our time window.
                .Where(u => u.CreatedAt >= startDate)

                // 2. Group all users by the DATE part of their CreatedAt timestamp. This puts all users from the same day into one group.
                .GroupBy(u => u.CreatedAt.Date)

                // 3. Project the results into our DTO.
                .Select(group => new DailyRegistrationDTO
                {
                    Date = group.Key, // The Key of the group is the Date we grouped by.
                    RegistrationCount = group.Count() // The Count is the number of users in that group.
                })

                // 4. Order the results by date chronologically.
                .OrderBy(dto => dto.Date)

                .ToListAsync();

            return registrationData;
        }
    }
}