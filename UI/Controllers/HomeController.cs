using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult WelcometoJobros()
    {
       
        if (User?.Identity?.IsAuthenticated ?? false)
            return RedirectToAction("Index", "Home"); 

        return View();
    }

    public async Task<IActionResult> Index()
    {
       
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("SplashPage", "Home");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.Contains("Admin") ? "Admin" :
                   roles.Contains("Employer") ? "Employer" :
                   "JobSeeker";

        ViewBag.Role = role;

        
        switch (role)
        {
            case "Admin":
                ViewBag.UsersCount = await _context.Users.CountAsync();
                ViewBag.ActiveJobs = await _context.Jobs.CountAsync(j => j.IsActive);
                break;

            case "Employer":
                ViewBag.MyJobsCount = await _context.Jobs.CountAsync(j => j.EmployerId == user.Id);
                break;

            case "JobSeeker":
                ViewBag.AppliedJobsCount = await _context.Applications.CountAsync(a => a.JobSeekerId == user.Id);
                break;
        }

        return View(); 
    }

    public IActionResult SplashPage()
    {
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            else if (User.IsInRole("Employer"))
            {
                return RedirectToAction("Dashboard", "Employer");
            }

            else if (User.IsInRole("JobSeeker"))
            {
                return RedirectToAction("Dashboard", "JobSeeker");
            }

            else
            {
                return RedirectToAction("SplashPage", "Home");
            }
        }

        return View(); 
    }

    public IActionResult About()
    {
        return View();
    }
    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult Services()
    {
        return View();
    }

    public IActionResult Privacy_Policy()
    {
        return View();
    }

    public IActionResult Terms_Condition()
    {
        return View();
    }

}
