using AutoMapper;
using Core.DTOs.Application;
using Core.DTOs.EmployerDTOs;
using Core.DTOs.Job;
using Core.Entities;
using Core.Interfaces.IServices.IEmailServices;
using Core.Interfaces.IServices.IEmployer;
using Core.Interfaces.IUnitOfWorks;
using Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmployerService : IJobService, IProfileService, IApplicationsServices
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Employer> _hasher;
        private readonly IEmailService _emailService;
        private readonly ReportExportService _reportExportService;
        private readonly ILogger<EmployerService> _logger;

        public EmployerService(
            IUnitOfWork uow,
            IMapper mapper,
            IPasswordHasher<Employer> hasher,
            IEmailService emailService,
            ReportExportService reportExportService,
            ILogger<EmployerService> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _hasher = hasher;
            _emailService = emailService;
            _reportExportService = reportExportService;
            _logger = logger;
        }

        async Task<bool> IJobService.ActivateJobAsync(int jobId, int employerId)
        {
            try
            {
                var job = (await _uow.Jobs.FindAsync(j => j.Id == jobId && j.EmployerId == employerId))
                          .FirstOrDefault();

                if (job == null)
                {
                    _logger.LogWarning("Job with Id {JobId} not found for Employer {EmployerId}.", jobId, employerId);
                    return false;
                }

                job.IsActive = true;
                var result = await _uow.CompleteAsync();

                if (result > 0)
                    _logger.LogInformation("Job {JobId} activated successfully.", jobId);
                else
                    _logger.LogWarning("Failed to activate job {JobId}.", jobId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating job {JobId} for employer {EmployerId}.", jobId, employerId);
                throw;
            }
        }

        async Task<bool> IJobService.DeactivateJobAsync(int jobId, int employerId)
        {
            try
            {
                var job = (await _uow.Jobs.FindAsync(j => j.Id == jobId && j.EmployerId == employerId))
                          .FirstOrDefault();

                if (job == null)
                {
                    _logger.LogWarning("Job with Id {JobId} not found for Employer {EmployerId}.", jobId, employerId);
                    return false;
                }

                job.IsActive = false;
                var result = await _uow.CompleteAsync();

                if (result > 0)
                    _logger.LogInformation("Job {JobId} deactivated successfully.", jobId);
                else
                    _logger.LogWarning("Failed to deactivate job {JobId}.", jobId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating job {JobId} for employer {EmployerId}.", jobId, employerId);
                throw;
            }
        }

        async Task<bool> IJobService.CreateJobAsync(int employerId, CreateJobDto model)
        {
            try
            {
                if (model is null) throw new ArgumentNullException(nameof(model));

                var job = _mapper.Map<Core.Entities.Job>(model);
                job.EmployerId = employerId;
                job.CreatedAt = DateTime.UtcNow;
                job.IsActive = true;

                await _uow.Jobs.AddAsync(job);
                var result = await _uow.CompleteAsync();

                if (result > 0)
                    _logger.LogInformation("Job '{Title}' created successfully with Id {JobId}.", job.Title, job.Id);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job for employer {EmployerId}.", employerId);
                throw;
            }
        }

        async Task<bool> IJobService.UpdateJobAsync(int jobId, int employerId, EditJobDto model)
        {
            try
            {
                var job = (await _uow.Jobs.FindAsync(j => j.Id == jobId && j.EmployerId == employerId))
                         .FirstOrDefault();

                if (job == null)
                {
                    _logger.LogWarning("Job with Id {JobId} not found for Employer {EmployerId}.", jobId, employerId);
                    return false;
                }

                _uow.Jobs.Update(_mapper.Map(model, job));
                var result = await _uow.CompleteAsync();

                if (result > 0)
                    _logger.LogInformation("Job {JobId} updated successfully.", jobId);
                else
                    _logger.LogWarning("Failed to update job {JobId}.", jobId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating job {JobId} for employer {EmployerId}.", jobId, employerId);
                throw;
            }
        }

        async Task<bool> IJobService.DeleteJobAsync(int jobId, int employerId)
        {
            try
            {
                var job = (await _uow.Jobs.FindAsync(j => j.Id == jobId && j.EmployerId == employerId))
                          .FirstOrDefault();

                if (job == null)
                {
                    _logger.LogWarning("Job with Id {JobId} not found for Employer {EmployerId}.", jobId, employerId);
                    return false;
                }

                _uow.Jobs.Delete(job);
                var result = await _uow.CompleteAsync();

                if (result > 0)
                    _logger.LogInformation("Job {JobId} deleted successfully.", jobId);
                else
                    _logger.LogWarning("Failed to delete job {JobId}.", jobId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job {JobId} for employer {EmployerId}.", jobId, employerId);
                throw;
            }
        }

        async Task<IEnumerable<JobDto>> IJobService.GetEmployerJobsAsync(int employerId)
        {
            try
            {
                var jobs = await _uow.Jobs.FindAsync(j => j.EmployerId == employerId);
                var jobDtos = jobs.Select(j => _mapper.Map<JobDto>(j));
                return jobDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching jobs for employer {EmployerId}.", employerId);
                throw;
            }
        }

        async Task<JobDto?> IJobService.GetJobByIdAsync(int jobId, int employerId)
        {
            try
            {
                var job = (await _uow.Jobs.FindAsync(j => j.Id == jobId && j.EmployerId == employerId))
                          .FirstOrDefault();

                if (job == null)
                {
                    _logger.LogWarning("Job with Id {JobId} not found for Employer {EmployerId}.", jobId, employerId);
                    return null;
                }

                return _mapper.Map<JobDto>(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job {JobId}.", jobId);
                throw;
            }
        }

        bool IJobService.IsJobOwnedByEmployer(int jobId, int employerId)
        {
            // Use IQueryable to avoid blocking on async methods
            try
            {
                return _uow.Jobs.AsQueryable().Any(j => j.Id == jobId && j.EmployerId == employerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking ownership for job {JobId}.", jobId);
                return false;
            }
        }

        async Task<Core.DTOs.EmployerDTOs.JobAnalyticsDto> IJobService.GetJobAnalyticsAsync(int jobId, int employerId)
        {
            // Verify job ownership
            var job = await _uow.Jobs.GetByIdAsync(jobId);
                        if (job == null || job.EmployerId != employerId)
            {
                _logger.LogWarning("Job {JobId} not found or not owned by employer {EmployerId}.", jobId, employerId);
                return new Core.DTOs.EmployerDTOs.JobAnalyticsDto();
            }

            // Query only the job's applications (efficient filter)
            var jobApplications = (await _uow.Applications.FindAsync(a => a.JobId == jobId)).ToList();

            return new Core.DTOs.EmployerDTOs.JobAnalyticsDto
            {
                TotalApplicants = jobApplications.Count,
                AcceptedCount = jobApplications.Count(a => string.Equals(a.Status, "Accepted", StringComparison.OrdinalIgnoreCase)),
                RejectedCount = jobApplications.Count(a => string.Equals(a.Status, "Rejected", StringComparison.OrdinalIgnoreCase)),
                PendingCount = jobApplications.Count(a => string.Equals(a.Status, "Pending", StringComparison.OrdinalIgnoreCase))
            };
        }

        async Task<EmployerProfileDto?> IProfileService.GetProfileAsync(int employerId)
        {
            var employer = await _uow.Employers.GetByIdAsync(employerId);
            if (employer == null)
            {
                _logger.LogWarning("Employer with Id {EmployerId} not found.", employerId);
                return null;
            }
            return _mapper.Map<EmployerProfileDto>(employer);
        }

        async Task<bool> IProfileService.UpdateProfileAsync(int employerId, EditEmployerProfileDto model)
        {
            try
            {
                if (model is null) throw new ArgumentNullException(nameof(model));

                var prof = await _uow.Employers.GetByIdAsync(employerId);
                if (prof == null)
                {
                    _logger.LogWarning("Employer {EmployerId} not found.", employerId);
                    return false;
                }

                _uow.Employers.Update(_mapper.Map(model, prof));
                var result = await _uow.CompleteAsync();

                if (result > 0)
                    _logger.LogInformation("Employer profile {EmployerId} updated successfully.", employerId);
                else
                    _logger.LogWarning("Failed to update employer profile {EmployerId}.", employerId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employer profile {EmployerId}.", employerId);
                throw;
            }
        }

        async Task<bool> IProfileService.ChangePasswordAsync(int employerId, ChangePasswordDto model)
        {
            try
            {
                var emp = await _uow.Employers.GetByIdAsync(employerId);
                if (emp == null)
                    return false;

                var verification = _hasher.VerifyHashedPassword(emp, emp.PasswordHash, model.CurrentPassword);
                if (verification == PasswordVerificationResult.Failed)
                    return false;

                emp.PasswordHash = _hasher.HashPassword(emp, model.NewPassword);
                _uow.Employers.Update(emp);
                var result = await _uow.CompleteAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while changing password for employer {EmployerId}.", employerId);
                return false;
            }
        }

        async Task<IEnumerable<ApplicationsDTo>> IApplicationsServices.GetApplicationsByEmployerAsync(int employerId, string? jobTitle = null, string? applicantName = null)
        {
            try
            {
                var applications = await _uow.Applications.FindAsync(a =>
                    a.Job.EmployerId == employerId &&
                    (string.IsNullOrEmpty(jobTitle) || a.Job.Title.Contains(jobTitle)) &&
                    (string.IsNullOrEmpty(applicantName) || a.JobSeeker.FullName.Contains(applicantName))
                );

                return applications.Select(a => _mapper.Map<ApplicationsDTo>(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching applications for employer {EmployerId}.", employerId);
                throw;
            }
        }

        async Task<ApplicationDetailsDto?> IApplicationsServices.GetApplicationDetailsAsync(int applicationId, int employerId)
        {
            var app = await _uow.Applications.GetByIdAsync(applicationId);
            if (app == null || app.Job.EmployerId != employerId)
            {
                _logger.LogWarning("Application {ApplicationId} not found or not owned by employer {EmployerId}.", applicationId, employerId);
                return null;
            }
            return _mapper.Map<ApplicationDetailsDto>(app);
        }

        async Task<bool> IApplicationsServices.AcceptApplicationAsync(int applicationId, int employerId)
        {
            return await UpdateApplicationStatusAndNotifyAsync(
                applicationId,
                employerId,
                "Accepted",
                (app) => $"Dear {app.JobSeeker.FullName},\n\nYou have been shortlisted for an interview for the position {app.Job.Title}."
            );
        }

        async Task<bool> IApplicationsServices.RejectApplicationAsync(int applicationId, int employerId)
        {
            return await UpdateApplicationStatusAndNotifyAsync(
                applicationId,
                employerId,
                "Rejected",
                (app) => $"Dear {app.JobSeeker.FullName},\n\nThank you for applying for {app.Job.Title}. We regret to inform you that your application was not selected."
            );
        }

        async Task<byte[]> IApplicationsServices.ExportApplicantsAsync(int jobId, string format)
        {
            format = format?.ToLower() switch
            {
                "excel" or "xlsx" => "excel",
                "pdf" => "pdf",
                _ => "excel"
            };

            var applications = (await _uow.Applications.GetAllAsync("Job,JobSeeker"))
                               .Where(a => a.JobId == jobId).ToList();

            if (!applications.Any())
                return Array.Empty<byte>();

            var dtos = applications.Select(a => _mapper.Map<ApplicationDto>(a)).ToList();
            var bundle = new ReportBundle();
            bundle.Add("Applicants", dtos);
            return _reportExportService.ExportBundle(bundle, format);
        }

        // Private helper method
        private async Task<bool> UpdateApplicationStatusAndNotifyAsync(
            int applicationId,
            int employerId,
            string status,
            Func<Core.Entities.Application, string> emailBodyFunc
        )
        {
            try
            {
                var app = await _uow.Applications.GetByIdAsync(applicationId);
                if (app == null || app.Job.EmployerId != employerId)
                {
                    _logger.LogWarning("Application {ApplicationId} not found for Employer {EmployerId}.", applicationId, employerId);
                    return false;
                }

                app.Status = status;
                _uow.Applications.Update(app);
                var result = await _uow.CompleteAsync();

                if (result > 0)
                {
                    try
                    {
                        var plainText = emailBodyFunc(app);
                        var html = $"<p>{plainText.Replace("\n", "<br/>")}</p>";

                        await _emailService.SendEmailAsync(
                            app.JobSeeker.Email,
                            status == "Accepted" ? "Interview Invitation" : "Application Status Update",
                            html,
                            plainText
                        );
                    }
                    catch (Exception emailEx)
                    {
                        // Log email failure but do not fail the method
                        _logger.LogWarning(emailEx, "Failed to send email to {Email}.", app.JobSeeker.Email);
                    }

                    return true;
                }

                _logger.LogWarning("Failed to update application {ApplicationId} status to {Status}.", applicationId, status);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating application {ApplicationId} for employer {EmployerId}.", applicationId, employerId);
                return false;
            }
        }
    }
}
