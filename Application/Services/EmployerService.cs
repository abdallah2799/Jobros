using AutoMapper;
using Core.DTOs.EmployerDTOs;
using Core.DTOs.Job;
using Core.Entities;
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
    public class EmployerService : IJobService,IProfileService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Employer> _hasher;
        public EmployerService(IUnitOfWork uow, IMapper mapper, IMapper _mapper,IPasswordHasher<Employer> hasher)
        {
            _uow = uow;
            _mapper = mapper;
            _hasher = hasher;
        }
      async  Task<bool> IJobService.ActivateJobAsync(int jobId, int employerId)
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

        async Task<bool> IProfileService.ChangePasswordAsync(int employerId, string currentPassword, string newPassword)
        {
            try
            {
                var emp = await _uow.Employers.GetByIdAsync(employerId);
                if (emp == null)
                    return (false);

                var verification = _hasher.VerifyHashedPassword(emp, emp.PasswordHash, currentPassword);
                if (verification == PasswordVerificationResult.Failed)
                    return false;

                emp.PasswordHash = _hasher.HashPassword(emp, newPassword);
                _uow.Employers.Update(emp);

                var result = await _uow.CompleteAsync();

                if (result > 0)
               
                    return true;
                
                else
                
                    return false;
                
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error while changing password: {ex.Message}");

                return false;
            }
        }

       
    }
}
