using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    public class AccountController : Controller
    {
        
        public IActionResult Login()
        {
            return View();
        }

     
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Login(string email, string password)
        {
           
            return View();
        }

        
        [HttpPost]
        public IActionResult Register(string name, string email, string password)
        {
           
            return View();
        }
    }
}
