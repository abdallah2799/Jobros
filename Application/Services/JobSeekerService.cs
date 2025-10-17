using AutoMapper;
using Core.DTOs.Application;
using Core.DTOs.Job;
using Core.DTOs.JobSeeker;
using Core.Entities;
using Core.Interfaces.IServices.Commands;
using Core.Interfaces.IUnitOfWorks;
using System.Linq;
using Core.Interfaces.IServices.IQueries;

namespace Application.Services
{
    public class JobSeekerService : IJobSeekerQueryService, IJobSeekerCommandService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public JobSeekerService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        // Queries
        public async Task<JobSeekerDto?> GetProfileAsync(int jobSeekerId)
        {
            var user = await _uow.Applications.GetByIdAsync(jobSeekerId);

            var app = (await _uow.Applications.FindAsync(a => a.JobSeekerId == jobSeekerId)).FirstOrDefault();
            if (app == null)
            {
                // No applications; return minimal from Applications table not possible. Return null.
                return null;
            }

            var js = app.JobSeeker;
            return new JobSeekerDto
            {
                Id = jobSeekerId,
                FullName = js.FullName,
                Email = js.Email,
                Bio = js.Bio,
                Skills = js.Skills,
                ExperienceYears = js.ExperienceYears,
                ResumeUrl = js.ResumeUrl
            };
        }

        public async Task<IEnumerable<JobDto>> BrowseJobsAsync()
        {
            var jobs = await _uow.Jobs.GetAllAsync();
            return jobs.Select(j => new JobDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                Requirements = j.Requirements,
                Location = j.Location,
                SalaryRange = j.SalaryRange,
                JobType = j.JobType,
                IsActive = j.IsActive,
                EmployerName = j.Employer?.CompanyName
            }).ToList();
        }

        public async Task<IEnumerable<ApplicationDto>> GetMyApplicationsAsync(int jobSeekerId)
        {
            var apps = await _uow.Applications.FindAsync(a => a.JobSeekerId == jobSeekerId);
            return apps.Select(a => new ApplicationDto
            {
                Id = a.Id,
                JobId = a.JobId,
                JobSeekerId = a.JobSeekerId,
                Status = a.Status,
                AppliedAt = a.AppliedAt,
                CoverLetter = a.CoverLetter,
                JobTitle = a.Job?.Title
            }).ToList();
        }

        public async Task<ApplicationDto?> GetApplicationByIdAsync(int applicationId, int jobSeekerId)
        {
            var app = await _uow.Applications.GetByIdAsync(applicationId);
            if (app == null || app.JobSeekerId != jobSeekerId) return null;
            return new ApplicationDto
            {
                Id = app.Id,
                JobId = app.JobId,
                JobSeekerId = app.JobSeekerId,
                Status = app.Status,
                AppliedAt = app.AppliedAt,
                CoverLetter = app.CoverLetter,
                JobTitle = app.Job?.Title
            };
        }

        // Commands
        public async Task<int> ApplyToJobAsync(ApplicationCreateDto dto, int jobSeekerId)
        {
            // Basic checks
            var job = await _uow.Jobs.GetByIdAsync(dto.JobId);
            if (job == null || !job.IsActive)
                throw new System.Exception("Job not found or not active");

            // Prevent duplicate application
            var existing = (await _uow.Applications.FindAsync(a => a.JobId == dto.JobId && a.JobSeekerId == jobSeekerId)).FirstOrDefault();
            if (existing != null)
                throw new System.Exception("You have already applied to this job");

            var application = new Core.Entities.Application
            {
                JobId = dto.JobId,
                JobSeekerId = jobSeekerId,
                CoverLetter = dto.CoverLetter,
                AppliedAt = System.DateTime.Now,
                Status = "Pending"
            };

            await _uow.Applications.AddAsync(application);
            var res = await _uow.CompleteAsync();
            return application.Id;
        }

        public async Task UpdateProfileAsync(int jobSeekerId, JobSeekerUpdateDto dto)
        {
            // There's no Users repo in unit of work. Use Applications repository to find a JobSeeker via an application.
            var app = (await _uow.Applications.FindAsync(a => a.JobSeekerId == jobSeekerId)).FirstOrDefault();
            if (app == null)
                throw new System.Exception("JobSeeker not found or no related applications to locate profile");

            var js = app.JobSeeker;
            if (dto.FullName != null) js.FullName = dto.FullName;
            if (dto.Bio != null) js.Bio = dto.Bio;
            if (dto.Skills != null) js.Skills = dto.Skills;
            if (dto.ExperienceYears.HasValue) js.ExperienceYears = dto.ExperienceYears;
            if (dto.ResumeUrl != null) js.ResumeUrl = dto.ResumeUrl;

            // Update in context
            _uow.Applications.Update(app); // indirectly marks jobseeker modified as tracked
            await _uow.CompleteAsync();
        }

        public async Task DeleteApplicationAsync(int applicationId, int jobSeekerId)
        {
            var app = await _uow.Applications.GetByIdAsync(applicationId);
            if (app == null || app.JobSeekerId != jobSeekerId)
                throw new System.Exception("Application not found");

            _uow.Applications.Delete(app);
            await _uow.CompleteAsync();
        }
    }
}
