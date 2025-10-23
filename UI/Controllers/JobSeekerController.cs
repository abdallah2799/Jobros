using System;
using Core.DTOs.Application;
using Core.Interfaces.IServices.Commands;
using Core.Interfaces.IServices.IQueries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Claims;
using UI.Models.JobSeeker;
using Core.DTOs.JobSeeker;

namespace UI.Controllers
{
    [Authorize(Roles = "JobSeeker")]
    public class JobSeekerController : Controller
    {
        private readonly IJobSeekerQueryService _queryService;
        private readonly IJobSeekerCommandService _commandService;

        public JobSeekerController(IJobSeekerQueryService queryService, IJobSeekerCommandService commandService)
        {
            _queryService = queryService;
            _commandService = commandService;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        private async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/uploads/{folder}/{fileName}";
        }

        // 1. Job Browsing
        public async Task<IActionResult> Browse(string keyword = null, int? categoryId = null, string employer = null, string location = null, string jobType = null, int page = 1)
        {
            var jobs = await _queryService.GetActiveJobsAsync(keyword, categoryId, employer, location, jobType, page, 10);
            var totalCount = await _queryService.GetActiveJobsTotalCountAsync(keyword, categoryId, employer, location, jobType);
            var totalPages = (int)Math.Ceiling(totalCount / 10.0);

            var model = new BrowseJobsViewModel
            {
                Jobs = jobs,
                Keyword = keyword ?? "",
                CategoryId = categoryId,
                Employer = employer ?? "",
                Location = location ?? "",
                JobType = jobType ?? "",
                Page = page,
                TotalPages = totalPages
            };

            return View(model);
        }

        // 2. Job Details
        public async Task<IActionResult> Details(int id)
        {
            var job = await _queryService.GetJobByIdAsync(id);
            if (job == null) return NotFound();

            var alreadyApplied = await _queryService.HasAppliedAsync(CurrentUserId, id);

            var model = new JobDetailsViewModel
            {
                Job = job,
                AlreadyApplied = alreadyApplied
            };

            return View(model);
        }

        // 3. Apply (no file upload — uses profile CV)
        [HttpPost]
        public async Task<IActionResult> Apply(ApplicationCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Details), new { id = dto.JobId });
            }

            try
            {
                await _commandService.ApplyAsync(CurrentUserId, dto);
                TempData["SuccessMessage"] = "Application submitted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Applications));
        }

        // 4. Profile
        public async Task<IActionResult> Profile()
        {
            var profile = await _queryService.GetProfileAsync(CurrentUserId);
            if (profile == null) return NotFound();

            return View(new ProfileViewModel { Profile = profile });
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            string? resumeUrl = null;
            if (model.ResumeFile != null)
            {
                // Optional: validate file type/size here
                resumeUrl = await SaveFileAsync(model.ResumeFile, "resumes");
            }

            var dto = new JobSeekerUpdateDto
            {
                FullName = model.Profile.FullName,
                Bio = model.Profile.Bio,
                Skills = model.Profile.Skills,
                ExperienceYears = model.Profile.ExperienceYears
            };

            try
            {
                await _commandService.UpdateProfileAsync(CurrentUserId, dto, resumeUrl);
                TempData["SuccessMessage"] = "Profile updated.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Profile));
        }

        // 5. Applications
        public async Task<IActionResult> Applications()
        {
            var apps = await _queryService.GetApplicationsByJobSeekerAsync(CurrentUserId);
            return View(new ApplicationsViewModel { Applications = apps });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            try
            {
                await _commandService.DeleteApplicationAsync(CurrentUserId, id);
                TempData["SuccessMessage"] = "Application deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Applications));
        }
    }
}