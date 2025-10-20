using AutoMapper;
using Core.DTOs.Application;
using Core.DTOs.Job;
using Core.Entities;
using Core.Interfaces.IServices.Commands;
using Core.Interfaces.IUnitOfWorks;
using System.Linq;
using Core.Interfaces.IServices.IQueries;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.DTOs.JobSeeker;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class JobSeekerService : IJobSeekerQueryService, IJobSeekerCommandService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobSeekerService(IUnitOfWork uow, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _mapper = mapper;
            _userManager = userManager;
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

        public async Task<JobDto?> GetJobByIdAsync(int jobId)
        {
            var job = await _uow.Jobs.AsQueryable().Include(j => j.Employer).FirstOrDefaultAsync(j => j.Id == jobId && j.IsActive);
            if (job == null) return null;
            return _mapper.Map<JobDto>(job);
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
            if (js == null) return null;
            return _mapper.Map<JobSeekerDto>(js);
        }

        public async Task<ApplicationDto> ApplyAsync(int jobSeekerId, ApplicationCreateDto dto)
        {
            var job = await _uow.Jobs.GetByIdAsync(dto.JobId);
            if (job == null || !job.IsActive)
                throw new System.Exception("Job not found or not active");

            var existing = await _uow.Applications.FindAsync(a => a.JobId == dto.JobId && a.JobSeekerId == jobSeekerId);
            if (existing.Any())
                throw new System.Exception("Already applied to this job");

            var app = new Core.Entities.Application
            {
                JobId = dto.JobId,
                JobSeekerId = jobSeekerId,
                CoverLetter = dto.CoverLetter,
            };

            if (!string.IsNullOrWhiteSpace(dto.ResumeUrl))
                app.JobSeeker = await _uow.JobSeekers.GetByIdAsync(jobSeekerId); 

            await _uow.Applications.AddAsync(app);
            await _uow.CompleteAsync();

            var result = _mapper.Map<ApplicationDto>(app);
            result.JobTitle = job.Title;

            // TODO: send confirmation email using existing email service (not injected here)

            return result;
        }

        public async Task<bool> DeleteApplicationAsync(int jobSeekerId, int applicationId)
        {
            var app = await _uow.Applications.GetByIdAsync(applicationId);
            if (app == null || app.JobSeekerId != jobSeekerId)
                throw new System.Exception("Application not found");

            if (app.Status != "Pending")
                throw new System.Exception("Only pending applications can be deleted");

            _uow.Applications.Delete(app);
            await _uow.CompleteAsync();
            return true;
        }

        public async Task<bool> UpdateProfileAsync(int jobSeekerId, JobSeekerUpdateDto dto)
        {
            var js = await _uow.JobSeekers.GetByIdAsync(jobSeekerId);
            if (js == null)
                throw new System.Exception("JobSeeker not found");

            js.FullName = dto.FullName ?? js.FullName;
            js.Bio = dto.Bio;
            js.Skills = dto.Skills;
            js.ExperienceYears = dto.ExperienceYears;
            js.ResumeUrl = dto.ResumeUrl;

            _uow.JobSeekers.Update(js);
            await _uow.CompleteAsync();
            return true;
        }

        public Task<bool> ChangePasswordAsync(int jobSeekerId, string currentPassword, string newPassword)
        {
            
            throw new System.NotImplementedException();
        }
    }
}
