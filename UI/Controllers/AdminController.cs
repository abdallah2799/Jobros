using Core.DTOs.Admin;
using Core.DTOs.Auth;
using Core.Interfaces.IServices.IAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using UI.Models.Admin; // We need this for the ViewModel

namespace UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller // Use Controller, NOT ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // GET: /Admin/Dashboard or /Admin
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var stats = await _adminService.GetDashboardStatsAsync();
            return View(stats); // Returns the Dashboard.cshtml view with data
        }

        // GET: /Admin/UserManagement
        [HttpGet]
        public async Task<IActionResult> UserManagement()
        {
            var allUsers = await _adminService.GetAllUsersAsync();
            var unverifiedEmployers = await _adminService.GetUnverifiedEmployersAsync();

            var viewModel = new UserManagementViewModel
            {
                AllUsers = allUsers,
                UnverifiedEmployers = unverifiedEmployers
            };

            return View(viewModel); // Returns UserManagement.cshtml with data
        }

        // POST: /Admin/ApproveEmployer/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveEmployer(string id)
        {
            var result = await _adminService.ApproveEmployerAsync(id);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Employer approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve employer: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }
            return RedirectToAction(nameof(UserManagement));
        }

        // POST: /Admin/RejectEmployer/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectEmployer(string id)
        {
            var result = await _adminService.RejectEmployerAsync(id);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Employer rejected successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reject employer: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }
            return RedirectToAction(nameof(UserManagement));
        }

        // GET: /Admin/JobOversight
        [HttpGet]
        public async Task<IActionResult> JobOversight(int? selectedEmployerId, bool? isActiveFilter)
        {
            // 1. Get the filtered list of jobs by passing the parameters to the service
            var filteredJobs = await _adminService.GetAllJobsAsync(isActiveFilter, selectedEmployerId);

            // 2. Get the full list of employers for the dropdown
            var allEmployers = await _adminService.GetAllEmployersAsync();

            // 3. Create the ViewModel
            var viewModel = new JobOversightViewModel
            {
                Jobs = filteredJobs,
                EmployerList = allEmployers.Select(e => new SelectListItem
                {
                    Text = e.CompanyName, // This will now work correctly
                    Value = e.Id.ToString()
                }),
                SelectedEmployerId = selectedEmployerId,
                IsActiveFilter = isActiveFilter
            };

            return View(viewModel);
        }

        // POST: /Admin/DeleteJob/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var result = await _adminService.DeleteJobAsync(id);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Job deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete job.";
            }
            return RedirectToAction(nameof(JobOversight));
        }

        // GET: /Admin/ApplicationOversight
        [HttpGet]
        public async Task<IActionResult> ApplicationOversight()
        {
            var applications = await _adminService.GetAllApplicationsAsync();
            return View(applications); // Returns ApplicationOversight.cshtml
        }

        // GET: /Admin/Announcements
        [HttpGet]
        public IActionResult Announcements()
        {
            return View(); // Returns a view with a form for announcements
        }

        // POST: /Admin/Announcements
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Announcements(AnnouncementDTO announcement)
        {
            if (ModelState.IsValid)
            {
                await _adminService.SendAnnouncementToAllUsersAsync(announcement);
                TempData["SuccessMessage"] = "Announcement sent to all users.";
                return RedirectToAction(nameof(Dashboard));
            }
            return View(announcement); // Return view with validation errors
        }

        // A default threshold of 7 days will be used if none is provided.
        private const int DefaultOverdueDays = 7;

        // GET: /Admin/OverdueApplications?days=14
        [HttpGet]
        public async Task<IActionResult> OverdueApplications(int? days)
        {
            // Use the provided 'days' value, or fall back to the default.
            int daysThreshold = days ?? DefaultOverdueDays;

            var overdueApps = await _adminService.GetOverdueApplicationsAsync(daysThreshold);

            // Pass the threshold to the view so it can display "Showing applications pending for more than X days"
            ViewData["DaysThreshold"] = daysThreshold;

            return View(overdueApps);
        }

        // A default of 30 days will be used for the report.
        private const int DefaultRegistrationReportDays = 30;

        // GET: /Admin/RegistrationAnalytics
        [HttpGet]
        public async Task<IActionResult> RegistrationAnalytics()
        {
            var registrationData = await _adminService.GetDailyRegistrationsAsync(DefaultRegistrationReportDays);

            ViewData["ReportDays"] = DefaultRegistrationReportDays;

            return View(registrationData);
        }
    }
}