using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using UI.Models;
using Infrastructure.UnitOfWorks;
using Core.Interfaces.IUnitOfWorks;
using System.Threading.Tasks;
using Core.Interfaces.IServices.IEmailServices;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IEmailService emailService, ILogger<HomeController> logger)
    {
        _context = context;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<IActionResult> WelcometoJobros()
    {
        var model = new WelcomePageViewModel()
        {
            AvailableJobs = await _unitOfWork.Jobs.AsQueryable().CountAsync(j => j.IsActive),
            Employers = await _unitOfWork.Employers.AsQueryable().CountAsync(),
            ApplicationsSubmitted = await _unitOfWork.Applications.AsQueryable().CountAsync()
             
        };
       
        if (User?.Identity?.IsAuthenticated ?? false)
            return RedirectToAction("Index", "Home"); 

        return View(model);
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

    public async Task<IActionResult> SplashPage()
    {
        var categories = await _unitOfWork.Categories
            .AsQueryable()
            .OrderBy(c => c.Name)
            .Select(c => c.Name)
            .ToListAsync();

        var locations = await _unitOfWork.Jobs
            .AsQueryable()
            .Where(j => !string.IsNullOrWhiteSpace(j.Location))
            .Select(j => j.Location)
            .Distinct()
            .OrderBy(l => l)
            .ToListAsync();

        var model = new SplashPageViewModel
        {
            Categories = categories,
            Locations = locations
        };

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

        return View(model); 
    }

    public IActionResult About()
    {
        return View();
    }
    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SendEmailContact(SendEmailContactViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please fill in all required fields correctly.";
            return View("Contact", model);
        }
        try
        {
            var fromName = $"{model.FirstName} {model.LastName}";
            var subject = model.Subject switch
            {
                "account" => "Account Issue",
                "job-posting" => "Job Posting Inquiry",
                "application" => "Application Status",
                "technical" => "Technical Support",
                "partnership" => "Partnership Request",
                _ => "General Inquiry"
            };

            var htmlContent = $"<p><strong>From:</strong> {fromName} ({model.Email})</p>" +
                              $"<p><strong>Message:</strong></p><p>{model.Message}</p>" +
                              $"<p><em>Newsletter subscription: {(model.SubscribeToNewsletter ? "Yes" : "No")}</em></p>";

            await _emailService.SendEmailAsync(
                fromName: fromName,
                fromEmail: model.Email,
                subject: $"[Contact Us] {subject}",
                htmlContent: htmlContent
            );

            TempData["SuccessMessage"] = "Your message has been sent successfully. We will get back to you shortly.";

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send contact email.");
            TempData["ErrorMessage"] = "Sorry, we couldn't send your message. Please try again later.";
        }

        return RedirectToAction(nameof(Contact),"Home");
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

    //[HttpPost]
    //public async Task<IActionResult> Search()
    //{
    //    return RedirectToAction("Index", "Jobs");
    //}
}
