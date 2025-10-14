using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UI.Models;


namespace UI.Controllers
{
    public class ProfileController : Controller
    {

        public IActionResult Profile()
        {
            var role = User.IsInRole("Employer") ? "Employer" : "JobSeeker";
            ViewBag.Role = role;
            return View();
        }

    }
}
