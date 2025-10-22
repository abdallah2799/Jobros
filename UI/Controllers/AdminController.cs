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

            var viewModel = new UserManagementViewModel
            {
                Id = id,
                FullName = "Ali",
                Email = "ali@example.com",
                Role = "Job Seeker",
                IsActive = true,
                CreatedAt = DateTime.Now
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


        
        // TOGGLE USER STATUS (POST)
        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {

            return Ok();
        }

        // ===================== PROFILE =====================
       public IActionResult Profile()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(AdminProfileDTO model)
        {

            return RedirectToAction(nameof(Profile));
        }

        // ===================== JOB MANAGEMENT =====================
        public IActionResult AddJob()
        {
            return View();
        }

        public IActionResult Jobs()
        {

            var jobs = new List<JobDto>
        {
            new JobDto { Id = 1, Title = "Software Engineer", EmployerName = "TechCorp", IsActive = true },
            new JobDto { Id = 2, Title = "Frontend Developer", EmployerName = "WebWorks", IsActive = true },
            new JobDto { Id = 3, Title = "Backend Developer", EmployerName = "DevSolutions", IsActive = false },
            new JobDto { Id = 4, Title = "UI/UX Designer", EmployerName = "DesignHub", IsActive = true },
            new JobDto { Id = 5, Title = "QA Tester", EmployerName = "TestPro", IsActive = true },
            new JobDto { Id = 6, Title = "Project Manager", EmployerName = "ManageIt", IsActive = false },
        };

            return View(jobs);
        }

        // POST: Admin/AddJob
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddJob(JobDto model)
        {
            return View();
        }

        // GET: Admin/EditJob
        public async Task<IActionResult> EditJob(int id)
        {

            return View();
        }

        // POST: Admin/EditJob
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJob(JobDto model)
        {

            return View();
        }

        // POST: Admin/DeleteJob
        [HttpPost]
        [ValidateAntiForgeryToken]


        [HttpPost]
        public IActionResult DeleteJob(int id)
        {
            return RedirectToAction("Jobs");
        }

        public IActionResult ViewJob(int id)
        {

            return View();
        }

        // ===================== APPLICATION MANAGEMENT =====================
        public IActionResult Applications()
        {
           
            var applications = new List<ApplicationDto>
            {
                new ApplicationDto
                {
                    JobTitle = "Frontend Developer",
                    EmployerName = "ABC Corp",
                    JobSeekerName = "Ahmed",
                    Status = "Pending",
                    AppliedAt = DateTime.Now
                },
                new ApplicationDto
                {
                    JobTitle = "Backend Developer",
                    EmployerName = "XYZ Ltd",
                    JobSeekerName = "Mona",
                    Status = "Accepted",
                    AppliedAt = DateTime.Now.AddDays(-2)
                },
                new ApplicationDto
                {
                    JobTitle = "Designer",
                    EmployerName = "Creative Studio",
                    JobSeekerName = "Omar",
                    Status = "Rejected",
                    AppliedAt = DateTime.Now.AddDays(-5)
                }
            };

            return View(applications); 
        }
    

       public IActionResult ViewApplication(int id)
        {

            return View();
        }

        [HttpPost]
        public IActionResult AcceptApplication(int id)
        {

            return RedirectToAction("Applications");
        }

        [HttpPost]
        public IActionResult RejectApplication(int id)
        {

            return RedirectToAction("Applications");
        }



        // ===================== SETTINGS =====================
        public IActionResult Settings()
        {

            return View();
        }




        // POST: Admin/ToggleJobStatus
        [HttpPost]
        public async Task<IActionResult> ToggleJobStatus(int id)
        {
            return View();

        }
    }
}