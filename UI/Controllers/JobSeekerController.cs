using Core.DTOs.Application;
using Core.DTOs.Job;
using Core.DTOs.JobSeeker;
using Core.Interfaces.IServices.Commands;
using Core.Interfaces.IServices.IQueries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    [Authorize(Roles = "JobSeeker")]
    public class JobSeekerController : Controller
    {
        private readonly IJobSeekerQueryService _query;
        private readonly IJobSeekerCommandService _command;

        public JobSeekerController(IJobSeekerQueryService query, IJobSeekerCommandService command)
        {
            _query = query;
            _command = command;
        }

        // Browse available jobs
        public async Task<IActionResult> Browse()
        {
            var jobs = await _query.BrowseJobsAsync();
            return View(jobs);
        }

        // Show job details and apply form
        public async Task<IActionResult> Details(int id)
        {
            // fetch job
            var jobs = await _query.BrowseJobsAsync();
            var job = jobs.FirstOrDefault(j => j.Id == id);
            if (job == null) return NotFound();
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(ApplicationCreateDto dto)
        {
            if (!ModelState.IsValid) return RedirectToAction("Details", new { id = dto.JobId });

            // Get current user id
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);

            try
            {
                await _command.ApplyToJobAsync(dto, userId);
                TempData["SuccessMessage"] = "Application submitted successfully.";
                return RedirectToAction("MyApplications");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Details", new { id = dto.JobId });
            }
        }

        public async Task<IActionResult> MyApplications()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            var apps = await _query.GetMyApplicationsAsync(userId);
            return View(apps);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            try
            {
                await _command.DeleteApplicationAsync(id, userId);
                TempData["SuccessMessage"] = "Application deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("MyApplications");
        }

        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            var profile = await _query.GetProfileAsync(userId);
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(JobSeekerUpdateDto dto)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            try
            {
                await _command.UpdateProfileAsync(userId, dto);
                TempData["SuccessMessage"] = "Profile updated.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Profile");
        }
    }
}
