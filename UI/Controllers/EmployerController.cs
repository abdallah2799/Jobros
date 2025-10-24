using Core.DTOs.EmployerDTOs;
using Core.DTOs.Job;
using Core.Interfaces.IServices.IEmployer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UI.Models.Employer;
using System.Security.Claims;
using Core.Interfaces.IUnitOfWorks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;

namespace UI.Controllers
{
    [Authorize(Roles = "Employer")]
    public class EmployerController : Controller
    {
        private readonly IJobService _jobService;
        private readonly IProfileService _profileService;
        private readonly IApplicationsServices _applicationsServices;
        private readonly IUnitOfWork _uow;
        private readonly IWebHostEnvironment _env;

        public EmployerController(IJobService jobService, IProfileService profileService, IApplicationsServices applicationsServices, IUnitOfWork uow, IWebHostEnvironment env
            )
        {
            _jobService = jobService;
            _profileService = profileService;
            _applicationsServices = applicationsServices;
            _uow = uow;
            _env = env;
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idClaim, out var id))
                return id;
            throw new InvalidOperationException("Unable to determine current user id.");
        }

        // Employer profile management

        public async Task<IActionResult> Profile(int? employerId)
        {
            var id = employerId ?? GetCurrentUserId();
            var profile = await _profileService.GetProfileAsync(id);
            if (profile == null)
                return NotFound();

            var vm = new EmployerProfileViewModel
            {
                EmployerId = id,
                Profile = profile
            };

            return View(vm);
        }
        public async Task<IActionResult> EditProfile(int? employerId)
        {
            var id = employerId ?? GetCurrentUserId();

            var profile = await _profileService.GetProfileAsync(id);
            if (profile == null)
                return NotFound();

            var vm = new EditEmployerProfileViewModel
            {
                EmployerId = id,
                Profile = new EditEmployerProfileDto
                {
                    FullName = profile.FullName,
                    CompanyName = profile.CompanyName,
                    Industry = profile.Industry,
                    Website = profile.Website,
                    Location = profile.Location,
                    Description = profile.Description
                }
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditEmployerProfileViewModel vm)
        {
            var id = vm.EmployerId != 0 ? vm.EmployerId : GetCurrentUserId();

            if (!ModelState.IsValid)
                return View(vm);
            var isUpdated = await _profileService.UpdateProfileAsync(id, vm.Profile);
            if (!isUpdated)
            {
                ModelState.AddModelError("", "Failed to update profile.");
                TempData["ErrorMessage"] = "Failed to update profile.";
                return View(vm);
            }

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Profile), new { employerId = id });
        }

        public IActionResult ChangePassword(int? employerId)
        {
            var id = employerId ?? GetCurrentUserId();
            ViewBag.EmployerId = id;
            return View(new ChangePasswordDto());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model, int? employerId)
        {
            var id = employerId ?? GetCurrentUserId();

            if (!ModelState.IsValid)
                return View(model);
            var result = await _profileService.ChangePasswordAsync(id, model);
            if (!result)
            {
                ModelState.AddModelError("", "Failed to change password.");
                TempData["ErrorMessage"] = "Failed to change password. Check your current password.";
                return View(model);
            }

            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction(nameof(Profile), new { employerId = id });
        }

        // Employer manages their job 
        public async Task<IActionResult> Jobs(int? employerId, string? status = null)
        {
            var id = employerId ?? GetCurrentUserId();
            var jobs = (await _jobService.GetEmployerJobsAsync(id)).ToList();

            if (!string.IsNullOrEmpty(status))
            {
                if (status.Equals("active", StringComparison.OrdinalIgnoreCase))
                    jobs = jobs.Where(j => j.IsActive).ToList();
                else if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase))
                    jobs = jobs.Where(j => !j.IsActive).ToList();
            }

            ViewBag.EmployerId = id;
            ViewBag.FilterStatus = status;
            return View(jobs);
        }

        public async Task<IActionResult> JobDetails(int jobId, int? employerId)
        {
            var id = employerId ?? GetCurrentUserId();
            var job = await _jobService.GetJobByIdAsync(jobId, id);
            if (job == null)
                return NotFound();

            ViewBag.EmployerId = id;
            return View(job);
        }
        public IActionResult CreateJob(int? employerId)
        {
            var id = employerId ?? GetCurrentUserId();
            var vm = new CreateJobViewModel { EmployerId = id };

            // populate categories
            var cats = _uow.Categories.AsQueryable().OrderBy(c => c.Name).Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToList();
            vm.Categories = cats;

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJob(CreateJobViewModel vm)
        {
            var id = vm.EmployerId != 0 ? vm.EmployerId : GetCurrentUserId();

            // re-populate categories when returning view
            var cats = _uow.Categories.AsQueryable().OrderBy(c => c.Name).Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToList();
            vm.Categories = cats;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fix validation errors.";
                return View(vm);
            }

            // validate selected category exists using async repository method
            if (vm.Job.CategoryId != 0)
            {
                var cat = await _uow.Categories.GetByIdAsync(vm.Job.CategoryId);
                if (cat == null)
                {
                    ModelState.AddModelError("Job.CategoryId", "Selected category does not exist.");
                    TempData["ErrorMessage"] = "Selected category does not exist.";
                    return View(vm);
                }
            }
            else
            {
                ModelState.AddModelError("Job.CategoryId", "Please select a category.");
                TempData["ErrorMessage"] = "Please select a category.";
                return View(vm);
            }

            vm.Job.EmployerId = id; // ensure model has correct owner

            try
            {
                var result = await _jobService.CreateJobAsync(id, vm.Job);
                if (!result)
                {
                    ModelState.AddModelError("", "Failed to create job.");
                    TempData["ErrorMessage"] = "Failed to create job.";
                    return View(vm);
                }
            }
            catch (Exception ex)
            {
                // surface useful error to developer/user
                TempData["ErrorMessage"] = "An error occurred while creating job: " + ex.GetBaseException().Message;
                return View(vm);
            }

            TempData["SuccessMessage"] = "Job created successfully.";
            return RedirectToAction(nameof(Jobs), new { employerId = id });
        }
        public async Task<IActionResult> EditJob(int jobId, int? employerId)
        {
            var id = employerId ?? GetCurrentUserId();
            var job = await _jobService.GetJobByIdAsync(jobId, id);
            if (job == null)
                return NotFound();

            var model = new EditJobDto
            {
                Title = job.Title,
                Description = job.Description,
                Requirements = job.Requirements,
                SalaryRange = job.SalaryRange,
                JobType = job.JobType,
                Location = job.Location,
                Id = job.Id
            };

            ViewBag.JobId = jobId;
            ViewBag.EmployerId = id;

            var vm = new EditJobViewModel { EmployerId = id, JobId = jobId, Job = model };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJob(EditJobViewModel vm)
        {
            var id = vm.EmployerId != 0 ? vm.EmployerId : GetCurrentUserId();
            var jobId = vm.JobId != 0 ? vm.JobId : vm.Job.Id;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fix validation errors.";
                ViewBag.JobId = jobId;
                ViewBag.EmployerId = id;
                return View(vm);
            }
            var isUpdated = await _jobService.UpdateJobAsync(jobId, id, vm.Job);

            if (!isUpdated)
            {
                ModelState.AddModelError("", "Failed to update job.");
                TempData["ErrorMessage"] = "Failed to update job.";
                return View(vm);
            }

            TempData["SuccessMessage"] = "Job updated successfully.";
            return RedirectToAction("JobDetails", new { jobId = jobId, employerId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateJob(int jobId, int? employerId)
        {
            var id = employerId ?? GetCurrentUserId();
            await _jobService.DeactivateJobAsync(jobId, id);
            return RedirectToAction(nameof(Jobs), new { employerId = id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateJob(int jobId, int? employerId)
        {
            var id = employerId ?? GetCurrentUserId();
            await _jobService.ActivateJobAsync(jobId, id);
            return RedirectToAction(nameof(Jobs), new { employerId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJob(int jobId, int? employerId)
        {
            var id = employerId ?? GetCurrentUserId();
            await _jobService.DeleteJobAsync(jobId, id);
            return RedirectToAction(nameof(Jobs), new { employerId = id });
        }


        // View applicants for a specific job
        public async Task<IActionResult> Applications(string? jobTitle, string? applicantName, int? employerId)
        {
            var id = employerId ?? GetCurrentUserId();
            var applications = await _applicationsServices.GetApplicationsByEmployerAsync(id, jobTitle, applicantName);
            ViewBag.EmployerId = id;
            ViewBag.FilterJobTitle = jobTitle;
            ViewBag.FilterApplicantName = applicantName;
            return View(applications);
        }
        public async Task<IActionResult> ApplicationDetails(int applicationId, int? employerId)
        {
            var id = employerId ?? GetCurrentUserId();
            var application = await _applicationsServices.GetApplicationDetailsAsync(applicationId, id);
            if (application == null)
                return NotFound();

            ViewBag.EmployerId = id;
            return View(application);
        }

        // New: download CV for an application (employer access)
        public async Task<IActionResult> DownloadCv(int applicationId, int? employerId)
        {
            var id = employerId ?? GetCurrentUserId();

            var application = await _applicationsServices.GetApplicationDetailsAsync(applicationId, id);
            if (application == null)
            {
                TempData["ErrorMessage"] = "Application not found or you don't have permission.";
                return RedirectToAction(nameof(Applications), new { employerId = id });
            }

            var cvPath = application.CvFilePath;
            if (string.IsNullOrEmpty(cvPath))
            {
                TempData["ErrorMessage"] = "No CV uploaded for this applicant.";
                return RedirectToAction(nameof(ApplicationDetails), new { applicationId = applicationId, employerId = id });
            }

            // If it's an absolute URL, redirect to it
            if (cvPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || cvPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return Redirect(cvPath);
            }

            var physicalPath = Path.Combine(_env.WebRootPath ?? string.Empty, cvPath.TrimStart('/', '\\'));
            if (!System.IO.File.Exists(physicalPath))
            {
                TempData["ErrorMessage"] = "CV file not found on server.";
                return RedirectToAction(nameof(ApplicationDetails), new { applicationId = applicationId, employerId = id });
            }

            var fileName = Path.GetFileName(physicalPath);
            var contentType = "application/octet-stream";
            return PhysicalFile(physicalPath, contentType, fileName);
        }

        // Job analytics / stats
        public async Task<IActionResult> JobStats(int jobId, int? employerId)
        {
            var id = employerId ?? GetCurrentUserId();
            var stats = await _jobService.GetJobAnalyticsAsync(jobId, id);
            return View(stats);
        }


    }
}
