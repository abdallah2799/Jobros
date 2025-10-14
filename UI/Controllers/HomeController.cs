using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UI.Models;

namespace UI.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
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
