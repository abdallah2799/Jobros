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
    public class AdminController: Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public IActionResult Dashboard()
        {
            // logic for the dashboard
            return View();
        }

        public async Task<IActionResult> UserManagement()
        {
            var allUsers = await _adminService.GetAllUsersAsync();
            var unverifiedEmployers = await _adminService.GetUnverifiedEmployersAsync();

            
            var viewModel = new UserManagementViewModel
            {
                AllUsers = allUsers,
                UnverifiedEmployers = unverifiedEmployers
            };

            return View(viewModel);
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
    }
}
