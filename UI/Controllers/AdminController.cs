using Core.DTOs.Admin;
using Core.DTOs.Application;
using Core.DTOs.Auth;
using Core.DTOs.Job;
using Core.Interfaces.IServices.IAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.Admin;

namespace UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }


        // ===================== DASHBOARD ===================== 
        public IActionResult Dashboard()
        {

            var stats = new DashboardStatsDTO
            {
                TotalUsers = 10,
                TotalEmployers = 3,
                TotalJobSeekers = 7,
                ActiveJobs = 5,
                JobsPerCategory = new Dictionary<string, int>
        {
            { "IT", 3 },
            { "Marketing", 2 }
        },
                UsersPerRole = new Dictionary<string, int>
        {
            { "Admin", 1 },
            { "Employer", 3 },
            { "Job Seeker", 6 }
        }
            };

            return View(stats);
        }


        // ===================== USER MANAGEMENT =====================
        public IActionResult UserManagement()
        {
            ViewBag.Users = new List<UserResponseDTO>
    {
        new UserResponseDTO
        {
            Id = 1,
            FullName = "Ali",
            Email = "ali@example.com",
            Role = "Job Seeker",
            IsActive = true,
            CreatedAt = DateTime.Now.AddDays(-10)
        },
        new UserResponseDTO
        {
            Id = 2,
            FullName = "Sara",
            Email = "sara@example.com",
            Role = "Employer",
            IsActive = true,
            CreatedAt = DateTime.Now.AddDays(-5)
        },
        new UserResponseDTO
        {
            Id = 3,
            FullName = "Mohamed",
            Email = "mohamed@example.com",
            Role = "Employer",
            IsActive = false,
            CreatedAt = DateTime.Now.AddDays(-2)
        }
    };

            return View();
        }

        //public async Task<IActionResult> UserManagement()
        //{
        //    var allUsers = await _adminService.GetAllUsersAsync();
        //    var unverifiedEmployers = await _adminService.GetUnverifiedEmployersAsync();


        //    var viewModel = new UserManagementViewModel
        //    {
        //        AllUsers = allUsers,
        //        UnverifiedEmployers = unverifiedEmployers
        //    };

        //    return View(viewModel);
        //}


        // ADD USER PAGE
        public IActionResult AddUser()
        {
            var model = new UserResponseDTO();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(UserResponseDTO model)
        {
            return View();

        }

        // USER DETAILS PAGE
        public IActionResult UserDetails(int id)
        {

            var user = new UserResponseDTO
            {
                Id = id,
                FullName = "Ali",
                Email = "ali@example.com",
                Role = "Job Seeker",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            return View(user); // مهم جداً
        }

        // EDIT USER PAGE
        public IActionResult EditUser(int id)
        {
            var user = new UserResponseDTO
            {
                Id = id,
                FullName = "Ali",
                Email = "ali@example.com",
                Role = "Job Seeker",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserResponseDTO model)
        {

            return RedirectToAction(nameof(UserManagement));
        }


        // DELETE USER
        public async Task<IActionResult> DeleteUser(string id)
        {
            return RedirectToAction(nameof(UserManagement));
        }

        

        // ===================== EMPLOYER APPROVAL =====================
        // GET: Admin/ApproveEmployer
        public IActionResult ApproveEmployer()
        {
            
            var newEmployers = new List<Core.DTOs.Employer.EmployerDto>
        {
        new Core.DTOs.Employer.EmployerDto { Id = 1, CompanyName = "ABC Corp", Email = "abc@example.com", RegistrationDate = DateTime.Now.AddDays(-5) },
        new Core.DTOs.Employer.EmployerDto { Id = 2, CompanyName = "XYZ Ltd", Email = "xyz@example.com", RegistrationDate = DateTime.Now.AddDays(-2) },
        new Core.DTOs.Employer.EmployerDto { Id = 3, CompanyName = "Creative Studio", Email = "studio@example.com", RegistrationDate = DateTime.Now.AddDays(-1) },
        };

            return View(newEmployers); 
        }


        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against CSRF attacks
        public async Task<IActionResult> ApproveEmployer(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var result = await _adminService.ApproveEmployerAsync(id);

            if (result.Succeeded)
            {
                // Add a success message to show the user on the next page
                TempData["SuccessMessage"] = "Employer has been successfully approved.";
            }
            else
            {
                // Add an error message
                TempData["ErrorMessage"] = "Failed to approve employer. " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            // Redirect back to the user management page to see the updated list.
            return RedirectToAction(nameof(UserManagement));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectEmployer(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var result = await _adminService.RejectEmployerAsync(id);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Employer has been successfully rejected and removed.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reject employer. " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(UserManagement));
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