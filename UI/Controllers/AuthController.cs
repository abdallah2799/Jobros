using Core.DTOs.Auth;
using Core.Interfaces;
using Core.Interfaces.IServices.IAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        #region Register
        // ----------------------------
        // Register (GET)
        // ----------------------------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ----------------------------
        // Register (POST)
        // ----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _authService.RegisterAsync(dto);
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Json(new { exists = false });

            var exists = await _authService.EmailExistsAsync(email);
            return Json(new { exists });
        }

        #endregion

        #region Login
        // ----------------------------
        // Login (GET)
        // ----------------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ----------------------------
        // Login (POST)
        // ----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var user = await _authService.LoginAsync(dto);

                // Redirect based on role
                //return user.Role switch
                //{
                //    "Admin" => RedirectToAction("Dashboard", "Admin"),
                //    "Employer" => RedirectToAction("Dashboard", "Employer"),
                //    "JobSeeker" => RedirectToAction("Dashboard", "JobSeeker"),
                //    _ => RedirectToAction("Index", "Home")
                //};

                // Temporary: log successful login info to console for testing
                Console.WriteLine($"✅ Login successful for {user.FullName} ({user.Email}) — Role: {user.Role}");

                // Option 1: Show message in UI temporarily
                TempData["SuccessMessage"] = $"Login successful! Welcome, {user.FullName} ({user.Role}).";

                // Redirect to Home for now
                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }
        #endregion

        #region ForgetPassword
        // Step 1: Request reset
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Please enter your email address.");
                return View();
            }

            try
            {
                await _authService.ForgotPasswordAsync(email);
                TempData["SuccessMessage"] = "Password reset link has been sent (check console for token during testing).";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // Step 2: Reset password form
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string email, string token, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError("", "Password is required.");
                ViewBag.Email = email;
                ViewBag.Token = token;
                return View();
            }

            try
            {
                var success = await _authService.ResetPasswordAsync(email, token, newPassword);
                if (success)
                {
                    TempData["SuccessMessage"] = "Password reset successful! You can now log in.";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }
        #endregion

        #region Logout
        // ----------------------------
        // Logout
        // ----------------------------
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login");
        }
        #endregion
    }
}
