using AutoMapper;
using Core.DTOs.EmployerDTOs;
using Core.DTOs.Job;
using Core.Entities;
using Core.Interfaces.IServices.IEmailServices;
using Core.Interfaces.IServices.IEmployer;
using Core.Interfaces.IUnitOfWorks;
using Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmployerService : IJobService,IProfileService , IApplicationsServices
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Employer> _hasher;
        private readonly IEmailService _emailService;
        public EmployerService(IUnitOfWork uow, IMapper mapper, IMapper _mapper,IPasswordHasher<Employer> hasher, IEmailService emailService)
        {
            _uow = uow;
            _mapper = mapper;
            _hasher = hasher;
            _emailService = emailService;
        }
        // implement IJobService methods
        async Task<bool> IJobService.ActivateJobAsync(int jobId, int employerId)
        {
            try
            {

                var job = (await _uow.Jobs.FindAsync(j => j.Id == jobId && j.EmployerId == employerId))
                          .FirstOrDefault();

                if (job == null)
                {
                    Console.WriteLine($"Job with Id {jobId} not found for Employer {employerId}.");
                    return false;
                }


                job.IsActive = true;

                var result = await _uow.CompleteAsync();

                Console.WriteLine(result > 0
                    ? $"Job {jobId} deactivated successfully."
                    : $"Failed to deactivate job {jobId}.");

                return result > 0;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error deactivating job {jobId}: {ex.Message}");
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
                    Console.WriteLine($"Job with Id {jobId} not found for Employer {employerId}.");
                    return false;
                }


                job.IsActive = false;

                var result = await _uow.CompleteAsync();

                Console.WriteLine(result > 0
                    ? $"Job {jobId} deactivated successfully."
                    : $"Failed to deactivate job {jobId}.");

                return result > 0;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error deactivating job {jobId}: {ex.Message}");
                throw;
            }

        }

        async Task<bool> IJobService.CreateJobAsync(int employerId, CreateJobDto model)
        {
            try
            {
                var job = _mapper.Map<Core.Entities.Job>(model);
                job.EmployerId = employerId;
                job.CreatedAt = DateTime.UtcNow;
                job.IsActive = true;
                await _uow.Jobs.AddAsync(job);
                var result = await _uow.CompleteAsync();


                if (result > 0)
                    Console.WriteLine($"Job '{job.Title}' created successfully with Id {job.Id}.");

                return result > 0;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error creating job: {ex.Message}");
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
                    Console.WriteLine($"Job with Id {jobId} not found for Employer {employerId}.");
                    return false;
                }
               ;
                _uow.Jobs.Update(_mapper.Map(model, job));

                var result = await _uow.CompleteAsync();

                Console.WriteLine(result > 0
                    ? $"Job {jobId} deactivated successfully."
                    : $"Failed to deactivate job {jobId}.");

                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deactivating job {jobId}: {ex.Message}");
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
                    Console.WriteLine($"Job with Id {jobId} not found for Employer {employerId}.");
                    return false;
                }


                _uow.Jobs.Delete(job);

                var result = await _uow.CompleteAsync();

                Console.WriteLine(result > 0
                    ? $"Job {jobId} deactivated successfully."
                    : $"Failed to deactivate job {jobId}.");

                return result > 0;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error deactivating job {jobId}: {ex.Message}");
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
             
                Console.WriteLine($"Error fetching jobs for employer {employerId}: {ex.Message}");
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
                    Console.WriteLine($"Job with Id {jobId} not found for Employer {employerId}.");
                    return null; 
                }

                var jobDto = _mapper.Map<JobDto>(job);

                return jobDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching job {jobId}: {ex.Message}");
                throw; 
            }
        }

      bool IJobService.IsJobOwnedByEmployer(int jobId, int employerId)
        {
            var job = _uow.Jobs.FindAsync(j => j.Id == jobId && j.EmployerId == employerId)
                .GetAwaiter().GetResult()
                .FirstOrDefault();

            return job != null;
        }
        // implement IProfileService methods

        async Task<EmployerProfileDto?> IProfileService.GetProfileAsync(int employerId)
        {     var employer = await _uow.Employers.GetByIdAsync(employerId);
            if (employer == null)
            {
                Console.WriteLine($"Employer with Id {employerId} not found.");
                return null;
            }
            var profileDto = _mapper.Map<EmployerProfileDto>(employer);
            return profileDto;
        }

        async Task<bool> IProfileService.UpdateProfileAsync(int employerId, EditEmployerProfileDto model)
        {

            try
            {
                var prof =  (await _uow.Employers.FindAsync(employerId => employerId == employerId)).FirstOrDefault();
             if(prof == null)
            {

                Console.WriteLine($"Employer {employerId} is not found");
                return false;
            }
             _uow.Employers.Update( _mapper.Map(model,prof));
            var result = await _uow.CompleteAsync();
            Console.WriteLine(result > 0
                  ? $"Job {employerId} deactivated successfully."
                  : $"Failed to deactivate job {employerId}.");

            return result > 0;
        }
            catch (Exception ex)
            {

                Console.WriteLine($"Error deactivating job {employerId}: {ex.Message}");
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

                // Verify current password
                var verification = _hasher.VerifyHashedPassword(emp, emp.PasswordHash, model.CurrentPassword);
                if (verification == PasswordVerificationResult.Failed)
                    return false;

                // Set new password
                emp.PasswordHash = _hasher.HashPassword(emp, model.NewPassword);
                _uow.Employers.Update(emp);

                // Save changes
                var result = await _uow.CompleteAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while changing password: {ex.Message}");
                return false;
            }
        }

        // implement IApplicationsServices methods
        async Task<IEnumerable<ApplicationsDTo>> IApplicationsServices.GetApplicationsByEmployerAsync(int employerId, string? jobTitle = null, string? applicantName= null)
        {
            try
            {
                var applications = await _uow.Applications.FindAsync(a =>
                    a.Job.EmployerId == employerId &&
                    (string.IsNullOrEmpty(jobTitle) || a.Job.Title.Contains(jobTitle)) &&
                    (string.IsNullOrEmpty(applicantName) || a.JobSeeker.FullName.Contains(applicantName))
                );

                var applicationDtos = applications.Select(a => _mapper.Map<ApplicationsDTo>(a));
                return applicationDtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching applications for employer {employerId}: {ex.Message}");
                throw;
            }
        }

        async Task<ApplicationDetailsDto?> IApplicationsServices.GetApplicationDetailsAsync(int applicationId, int employerId)
        {
          var app =  await _uow.Applications.GetByIdAsync(applicationId);
            if(app == null || app.Job.EmployerId != employerId)
            {
                Console.WriteLine($"Application {applicationId} not found for Employer {employerId}");
                return null;
            }
            var appDetails = _mapper.Map<ApplicationDetailsDto>(app);
            return appDetails;

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
                    Console.WriteLine($"Application {applicationId} not found for Employer {employerId}");
                    return false;
                }

                app.Status = status;
                _uow.Applications.Update(app);

                var result = await _uow.CompleteAsync();

                if (result > 0)
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            app.JobSeeker.Email,
                            status == "Accepted" ? "Interview Invitation" : "Application Status Update",
                            emailBodyFunc(app) // <-- This now matches the type
                        );
                    }
                    catch (Exception emailEx)
                    {
                        // Log email failure but do not fail the method
                        Console.WriteLine($"Failed to send email to {app.JobSeeker.Email}: {emailEx.Message}");
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Log any other exception
                Console.WriteLine($"Error updating application {applicationId} for Employer {employerId}: {ex.Message}");
                return false;
            }
        }

    }
}
