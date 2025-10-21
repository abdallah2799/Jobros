using Core.DTOs.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
    
        public IActionResult Index()
        {
           
            if (User.IsInRole("Admin"))
            {
                ViewBag.UserRole = "Admin";
                ViewBag.ControllerName = "Admin";
            }
            else if (User.IsInRole("Employer"))
            {
                ViewBag.UserRole = "Employer";
                ViewBag.ControllerName = "Employer";
            }
            else if (User.IsInRole("JobSeeker"))
            {
                ViewBag.UserRole = "JobSeeker";
                ViewBag.ControllerName = "JobSeeker";
            }
            else
            {
                return RedirectToAction("SplashPage", "Home");
            }

            
            ViewBag.UserName = User.Identity.Name ?? "User";

            return View();
        }

      
    }
}