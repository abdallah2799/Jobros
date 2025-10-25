using AutoMapper;
using Core.DTOs.Application;
using Core.DTOs.Job;
using Core.DTOs.JobSeeker;
using Core.Entities;
using Core.Interfaces.IServices.Commands;
using Core.Interfaces.IServices.IEmailServices;
using Core.Interfaces.IServices.IQueries;
using Core.Interfaces.IUnitOfWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Services
{
    public class JobSeekerService : IJobSeekerQueryService, IJobSeekerCommandService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public JobSeekerService(IUnitOfWork uow, IMapper mapper, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _uow = uow;
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<JobSeekerDashboardStatsDto> GetDashboardsatsAsync(int jobSeekerId)
        {
            //var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if( string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var jobSeekerId))
            //{
            //    throw new UnauthorizedAccessException("User is not authenticated or invalid JobSeeker ID.");
            //}
            var applications = await _uow.Applications.AsQueryable().Where(a => a.JobSeekerId == jobSeekerId)
                .ToListAsync();

            var total = applications.Count;
            var accepted = applications.Count(a => a.Status.Equals("Accepted", StringComparison.OrdinalIgnoreCase));
            var rejected = applications.Count(a => a.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase));
            var pending = applications.Count(a => a.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase));
            var topJobInfo = await _uow.Applications
                .AsQueryable()
                .Include(a => a.Job)
                .Where(a => a.Status == "Pending")
                .GroupBy(a => a.Job.Title)
                .Select(g => new { Title = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            return new JobSeekerDashboardStatsDto
            {
                JobSeekerName = (await _uow.JobSeekers.AsQueryable().FirstOrDefaultAsync(j => j.Id == jobSeekerId))?.FullName ?? "Unknown",
                TotalJobsAppliedFor = total,
                TopAppliedJobTitle = topJobInfo?.Title ?? "No pending applications in the system",
                TopAppliedJobApplicationsCount = topJobInfo?.Count ?? 0,
                AcceptedApplications = accepted,
                RejectedApplications = rejected,
                PendingApplications = pending,
            };

        }

        public async Task<IEnumerable<JobDto>> GetActiveJobsAsync(string? keyword = null, int? categoryId = null, string? employerName = null, string? location = null, string? jobType = null, int page = 1, int pageSize = 10)
        {
            var query = _uow.Jobs.AsQueryable().Where(j => j.IsActive);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim().ToLower();
                query = query.Where(j => j.Title.ToLower().Contains(k) || j.Description.ToLower().Contains(k) || j.Requirements.ToLower().Contains(k));
            }

            if (categoryId.HasValue)
                query = query.Where(j => j.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(employerName))
            {
                var en = employerName.Trim().ToLower();
                query = query.Where(j => j.Employer.FullName.ToLower().Contains(en) || j.Employer.CompanyName.ToLower().Contains(en));
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                var l = location.Trim().ToLower();
                query = query.Where(j => j.Location.ToLower().Contains(l));
            }

            if (!string.IsNullOrWhiteSpace(jobType))
                query = query.Where(j => j.JobType == jobType);


            var skip = (page - 1) * pageSize;
            var list = await query
                .Include(j => j.Employer)
                .OrderByDescending(j => j.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return list.Select(j => _mapper.Map<JobDto>(j));
        }

        public async Task<int> GetActiveJobsTotalCountAsync(string? keyword = null, int? categoryId = null, string? employerName = null, string? location = null, string? jobType = null)
        {
            var query = _uow.Jobs.AsQueryable().Where(j => j.IsActive);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim().ToLower();
                query = query.Where(j => j.Title.ToLower().Contains(k) || j.Description.ToLower().Contains(k) || j.Requirements.ToLower().Contains(k));
            }

            if (categoryId.HasValue)
                query = query.Where(j => j.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(employerName))
            {
                var en = employerName.Trim().ToLower();
                query = query.Where(j => j.Employer.FullName.ToLower().Contains(en) || j.Employer.CompanyName.ToLower().Contains(en));
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                var l = location.Trim().ToLower();
                query = query.Where(j => j.Location.ToLower().Contains(l));
            }

            if (!string.IsNullOrWhiteSpace(jobType))
                query = query.Where(j => j.JobType == jobType);

            return await query.CountAsync();
        }

        public async Task<JobDto?> GetJobByIdAsync(int jobId)
        {
            var job = await _uow.Jobs.AsQueryable().Include(j => j.Employer).FirstOrDefaultAsync(j => j.Id == jobId && j.IsActive);
            return job == null ? null : _mapper.Map<JobDto>(job);
        }

        public async Task<bool> HasAppliedAsync(int jobSeekerId, int jobId)
        {
            return await _uow.Applications.AsQueryable()
                .AnyAsync(a => a.JobId == jobId && a.JobSeekerId == jobSeekerId);
        }

        public async Task<IEnumerable<ApplicationDto>> GetApplicationsByJobSeekerAsync(int jobSeekerId)
        {
            var apps = await _uow.Applications.AsQueryable()
                .Where(a => a.JobSeekerId == jobSeekerId)
                .Include(a => a.Job)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

            return apps.Select(a => _mapper.Map<ApplicationDto>(a));
        }

        public async Task<JobSeekerDto?> GetProfileAsync(int jobSeekerId)
        {
            var js = await _uow.JobSeekers.AsQueryable().FirstOrDefaultAsync(j => j.Id == jobSeekerId);
            return js == null ? null : _mapper.Map<JobSeekerDto>(js);
        }

        public async Task<ApplicationDto> ApplyAsync(int jobSeekerId, ApplicationCreateDto dto)
        {
            var job = await _uow.Jobs.GetByIdAsync(dto.JobId);
            if (job == null || !job.IsActive)
                throw new Exception("Job not found or not active");

            var existing = await _uow.Applications.FindAsync(a => a.JobId == dto.JobId && a.JobSeekerId == jobSeekerId);
            if (existing.Any())
                throw new Exception("Already applied to this job");

            var app = new Core.Entities.Application
            {
                JobId = dto.JobId,
                JobSeekerId = jobSeekerId,
                CoverLetter = dto.CoverLetter,
            };

            await _uow.Applications.AddAsync(app);
            await _uow.CompleteAsync();

            // Send confirmation email
            var jobSeeker = await _userManager.FindByIdAsync(jobSeekerId.ToString());
            if (jobSeeker != null)
            {
                await _emailService.SendEmailAsync(jobSeeker.Email, "Application Submitted", $"Your application for '{job.Title}' has been received.");
            }

            var result = _mapper.Map<ApplicationDto>(app);
            result.JobTitle = job.Title;
            return result;
        }

        public async Task<bool> DeleteApplicationAsync(int jobSeekerId, int applicationId)
        {
            var app = await _uow.Applications.GetByIdAsync(applicationId);
            if (app == null || app.JobSeekerId != jobSeekerId)
                throw new Exception("Application not found");

            if (app.Status != "Pending")
                throw new Exception("Only pending applications can be deleted");

            _uow.Applications.Delete(app);
            await _uow.CompleteAsync();
            return true;
        }

        public async Task<bool> UpdateProfileAsync(int jobSeekerId, JobSeekerUpdateDto dto, string? resumeUrl = null)
        {
            var js = await _uow.JobSeekers.GetByIdAsync(jobSeekerId);
            if (js == null)
                throw new Exception("JobSeeker not found");

            js.FullName = dto.FullName ?? js.FullName;
            js.Bio = dto.Bio;
            js.Skills = dto.Skills;
            js.ExperienceYears = dto.ExperienceYears;
            if (!string.IsNullOrEmpty(resumeUrl))
                js.ResumeUrl = resumeUrl;

            _uow.JobSeekers.Update(js);
            await _uow.CompleteAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int jobSeekerId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(jobSeekerId.ToString());
            if (user == null) throw new Exception("User not found");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return true;
        }
    }
}
