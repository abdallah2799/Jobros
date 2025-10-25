using Core.DTOs.Auth;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.IServices.IAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _userManager = userManager;
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


        //----------------------------
        // Confirm Email 
        //---------------------------
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(int userId, string token)
        {
            if (userId == 0 || string.IsNullOrEmpty(token))
                return BadRequest("Invalid email confirmation request.");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // Mark success message
                TempData["SuccessMessage"] = "🎉 Email confirmed! You can now log in to your Jobros account.";
                return RedirectToAction("Login", "Auth");
            }

            TempData["ErrorMessage"] = "❌ Email confirmation failed. The link may have expired or already been used.";
            return RedirectToAction("Login", "Auth");
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

                // Log successful login info
                Console.WriteLine($"✅ Login successful for {user.FullName} ({user.Email}) — Role: {user.Role}");

                // Display success toast
                TempData["SuccessMessage"] = $"Welcome back, {user.FullName} ({user.Role}).";

                // Redirect based on role (you can update these when dashboards are ready)
                return user.Role switch
                {
                    "Admin" => RedirectToAction("Dashboard", "Admin"),
                    "Employer" => RedirectToAction("Dashboard", "Employer", new { employerId = user.Id }),
                    "JobSeeker" => RedirectToAction("Dashboard", "JobSeeker"),
                    _ => RedirectToAction("SplashPage", "Home")
                };

            }

            catch (Exception ex)
            {
                // Log and show error toast
                Console.WriteLine($"❌ Login error: {ex.Message}");
                TempData["ErrorMessage"] = ex.Message;

                // Return to same page with model state intact
                return View(dto);
            }
        }

        #endregion

        #region ForgetPassword
        #region Forgot Password

        // Step 1: Ask user for email
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["ErrorMessage"] = "Please enter your email address.";
                return View();
            }

            try
            {
                await _authService.ForgotPasswordAsync(email);
                TempData["SuccessMessage"] = "Password reset link has been sent to your email.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
        }

        // Step 2: Opened from email link (GET)
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                TempData["ErrorMessage"] = "Invalid password reset link.";
                return RedirectToAction("Login");
            }

            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }

        // Step 3: Submit new password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string email, string token, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                TempData["ErrorMessage"] = "Please enter a new password.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Passwords do not match.";
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
                TempData["ErrorMessage"] = ex.Message;
            }

            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }

        #endregion

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
            return RedirectToAction("WelcometoJobros", "Home");
        }
        #endregion
    }
}
