using Core.DTOs.Job;
using Core.Interfaces.IServices.IEmployer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    [Authorize(Roles = "Employer")]
    public class EmployerController : Controller
    {
        private readonly IJobService _jobService;
        public EmployerController(IJobService jobService)
        {
            _jobService = jobService;
        }
        // Employer manages their job 
        public async Task<IActionResult> Jobs(int employerId)
        {
            var jobs = await _jobService.GetEmployerJobsAsync(employerId);
            return View(jobs);
        }
        public async Task<IActionResult> JobDetails(int jobId, int employerId)
        {
            var job = await _jobService.GetJobByIdAsync(jobId, employerId);
            if (job == null)
                return NotFound();
            return View(job); 
        }
        public IActionResult CreateJob(int employerId)
        {
            ViewBag.EmployerId = employerId;
            return View(); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJob(int employerId, CreateJobDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _jobService.CreateJobAsync(employerId, model);
            if (!result)
                ModelState.AddModelError("", "Failed to create job.");

            return RedirectToAction(nameof(Jobs), new { employerId });
        }
        public async Task<IActionResult> EditJob(int jobId, int employerId)
        {
            var job = await _jobService.GetJobByIdAsync(jobId, employerId);
            if (job == null)
                return NotFound();

            var model = new EditJobDto
            {
                Title = job.Title,
                Description = job.Description,
                Requirements = job.Requirements,
                SalaryRange = job.SalaryRange,
                JobType = job.JobType,
                Location = job.Location
            };

            ViewBag.JobId = jobId;
            ViewBag.EmployerId = employerId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJob(int jobId, int employerId, EditJobDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.JobId = jobId;
                ViewBag.EmployerId = employerId;
                return View(model);
            }
            var isUpdated = await _jobService.UpdateJobAsync(jobId, employerId, model);

            if (!isUpdated)
            {
                return NotFound(); 
            }

            return RedirectToAction("JobDetails", new { jobId = jobId, employerId = employerId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateJob(int jobId, int employerId)
        {
            await _jobService.DeactivateJobAsync(jobId, employerId);
            return RedirectToAction(nameof(Jobs), new { employerId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateJob(int jobId, int employerId)
        {
            await _jobService.ActivateJobAsync(jobId, employerId);
            return RedirectToAction(nameof(Jobs), new { employerId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJob(int jobId, int employerId)
        {
            await _jobService.DeleteJobAsync(jobId, employerId);
            return RedirectToAction(nameof(Jobs), new { employerId });
        }
    }
}
