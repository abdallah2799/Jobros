using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UI.Models;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult SplashPage()
        {
            return View();
        }

        [Authorize]
        public IActionResult Index()
        {
            ViewBag.JobsCount = _context.Jobs.Count();

            
            ViewBag.NewEmployersCount = 5; 

            ViewBag.NewApplicationsCount = _context.Applications
                .Count(a => a.AppliedAt >= DateTime.Now.AddDays(-7));

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
}
