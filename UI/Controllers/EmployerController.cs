using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    public class EmployerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
