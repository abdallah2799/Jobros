using Core.DTOs.EmployerDTOs;
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
        private readonly IProfileService _profileService;
        private readonly IApplicationsServices _applicationsServices;
        public EmployerController(IJobService jobService, IProfileService profileService, IApplicationsServices applicationsServices
            )
        {
            _jobService = jobService;
            _profileService = profileService;
            _applicationsServices = applicationsServices;
        }

        // Employer profile management

        public async Task<IActionResult> Profile(int employerId)
        {
            var profile = await _profileService.GetProfileAsync(employerId);
            if (profile == null)
                return NotFound();
            return View(profile);
        }
        public async Task<IActionResult> EditProfile(int employerId)
        {

            var profile = await _profileService.GetProfileAsync(employerId);
            if (profile == null)
                return NotFound();

            return View(profile);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(int employerId, EditEmployerProfileDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var isUpdated = await _profileService.UpdateProfileAsync(employerId, model);
            if (!isUpdated)
            {
                ModelState.AddModelError("", "Failed to update profile.");
                return View(model);
            }
            return RedirectToAction(nameof(Profile), new { employerId });
        }

        public IActionResult ChangePassword(int employerId)
        {
            ViewBag.EmployerId = employerId;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(int employerId, ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await _profileService.ChangePasswordAsync(employerId, model);
            if (!result)
            {
                ModelState.AddModelError("", "Failed to change password.");
                return View(model);
            }
            return RedirectToAction(nameof(Profile), new { employerId });
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


        // View applicants for a specific job
        public async Task<IActionResult> Applications(int employerId, string? jobTitle, string? applicantName)
        {
            var applications = await _applicationsServices.GetApplicationsByEmployerAsync(employerId, jobTitle, applicantName);
            ViewBag.EmployerId = employerId;
            return View(applications);
        }
        public async Task<IActionResult> ApplicationDetails(int applicationId, int employerId)
        {
            var application = await _applicationsServices.GetApplicationDetailsAsync(applicationId, employerId);
            if (application == null)
                return NotFound();
            return View(application);
        }
        public async Task<IActionResult> AcceptApplication(int applicationId, int employerId)
        {
            var result = await _applicationsServices.AcceptApplicationAsync(applicationId, employerId);
            if (!result)
                return NotFound();
            return RedirectToAction(nameof(Applications), new { employerId });
        }

        public async Task<IActionResult> RejectApplication(int applicationId, int employerId)
        {
            var result = await _applicationsServices.RejectApplicationAsync(applicationId, employerId);
            if (!result)
                return NotFound();
            return RedirectToAction(nameof(Applications), new { employerId });
        }



    }
}
