using Core.DTOs.Application;
using Core.DTOs.Job;
using Core.Interfaces.IServices.Commands;
using Core.Interfaces.IServices.IQueries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // 1. Job Browsing
        public async Task<IActionResult> Browse(string keyword = null, int? categoryId = null, string employer = null, string location = null, string jobType = null, int page = 1)
        {
            var jobs = await _queryService.GetActiveJobsAsync(keyword, categoryId, employer, location, jobType, page, 10);
            return View(jobs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var job = await _queryService.GetJobByIdAsync(id);
            if (job == null) return NotFound();
            return View(job);
        }

        // 2. Apply
        [HttpPost]
        public async Task<IActionResult> Apply(ApplicationCreateDto dto)
        {
            try
            {
                var result = await _commandService.ApplyAsync(CurrentUserId, dto);
                TempData["Success"] = "Application submitted successfully.";
                return RedirectToAction(nameof(Applications));
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = dto.JobId });
            }
        }

        // 3. Profile
        public async Task<IActionResult> Profile()
        {
            var profile = await _queryService.GetProfileAsync(CurrentUserId);
            if (profile == null) return NotFound();
            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(Core.DTOs.JobSeeker.JobSeekerUpdateDto dto)
        {
            try
            {
                await _commandService.UpdateProfileAsync(CurrentUserId, dto);
                TempData["Success"] = "Profile updated.";
                return RedirectToAction(nameof(Profile));
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Profile));
            }
        }

        // 4. Applications
        public async Task<IActionResult> Applications()
        {
            var apps = await _queryService.GetApplicationsByJobSeekerAsync(CurrentUserId);
            return View(apps);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            try
            {
                await _commandService.DeleteApplicationAsync(CurrentUserId, id);
                TempData["Success"] = "Application deleted.";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Applications));
        }
    }
}
