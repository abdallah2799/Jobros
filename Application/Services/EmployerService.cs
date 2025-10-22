using AutoMapper;
using Core.DTOs.Job;
using Core.Interfaces.IServices.IEmployer;
using Core.Interfaces.IUnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmployerService : IJobService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public EmployerService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
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
                _uow.Jobs.Update(job);

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

    }
}
