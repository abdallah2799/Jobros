using AutoMapper;
using Core.DTOs.Auth;
using Core.Entities; // where ApplicationUser, Employer, JobSeeker, etc. live
using Core.Interfaces.IServices.IAuth;
using Core.Interfaces.IServices.IEmailServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            IHttpContextAccessor httpContextAccessor,
            IEmailService emailService,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _mapper = mapper;
        }

        // -----------------------------
        // Register
        // -----------------------------
        public async Task<UserResponseDTO> RegisterAsync(UserRegisterDTO dto)
        {
            // 1. Check if email exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("Email already registered");

            // 2. Create user based on role
            ApplicationUser user;

            switch (dto.Role)
            {
                case "Employer":
                    user = new Employer
                    {
                        UserName = dto.Email,
                        Email = dto.Email,
                        FullName = dto.FullName,
                        CompanyName = dto.CompanyName,
                        Industry = dto.Industry,
                        Website = dto.Website,
                        Location = dto.Location,
                        Description = dto.Description
                    };
                    break;

                case "JobSeeker":
                    user = new JobSeeker
                    {
                        UserName = dto.Email,
                        Email = dto.Email,
                        FullName = dto.FullName,
                        Bio = dto.Bio,
                        Skills = dto.Skills,
                        ExperienceYears = dto.ExperienceYears
                    };
                    break;

                case "Admin":
                    user = new Admin
                    {
                        UserName = dto.Email,
                        Email = dto.Email,
                        FullName = dto.FullName
                    };
                    break;

                default:
                    throw new Exception("Invalid role type");
            }

            // 3. Ensure role exists
            if (!await _roleManager.RoleExistsAsync(dto.Role))
                await _roleManager.CreateAsync(new IdentityRole<int>(dto.Role));

            // 4. Create the user
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            // 5. Assign role
            await _userManager.AddToRoleAsync(user, dto.Role);

            // after _userManager.CreateAsync(user, dto.Password)
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // build URL with token and userId
            var confirmUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/Auth/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            // send confirmation email
            await _emailService.SendEmailAsync(
                user.Email,
                "Confirm your Jobros account",
                $"<h3>Welcome to Jobros, {user.FullName}!</h3><p>Please confirm your email by clicking the link below:</p><p><a href='{confirmUrl}'>Confirm Email</a></p>"
            );


            // 6. Return safe data
            return _mapper.Map<UserResponseDTO>(user);
        }

        // -----------------------------
        // Login
        // -----------------------------
        public async Task<UserResponseDTO> LoginAsync(UserLoginDTO dto)
        {
            // 1️⃣ Find the user by email
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("Invalid credentials.");

            // 2️⃣ Check if account is active
            if (!user.IsActive)
                throw new Exception("Your account is inactive. Please contact support.");

            // 3️⃣ Check if email is confirmed
            if (!user.EmailConfirmed)
                throw new Exception("Please confirm your email before logging in. Check your inbox or spam folder.");

            // 4️⃣ Validate password
            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
                throw new Exception("Invalid email or password.");

            // 5️⃣ Return mapped DTO
            return _mapper.Map<UserResponseDTO>(user);
        }

        // -----------------------------
        // ForgetPassword
        // -----------------------------
        public async Task ForgotPasswordAsync(string email)
        {
            // 1️⃣ Check if user exists
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("No account found with that email.");

            // 2️⃣ Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // 3️⃣ Build secure reset URL (same domain)
            var resetUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/Auth/ResetPassword?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

            // 4️⃣ Compose email content
            var subject = "Reset your Jobros account password";
            var body = $@"
        <h3>Hello {user.FullName},</h3>
        <p>We received a request to reset your Jobros account password.</p>
        <p>To proceed, click the link below:</p>
        <p><a href='{resetUrl}' style='color:#00B074;font-weight:bold;'>Reset Password</a></p>
        <p>If you didn’t request this, you can safely ignore this email.</p>
        <br>
        <p>– The Jobros Team</p>
    ";

            // 5️⃣ Send email using existing service
            await _emailService.SendEmailAsync(user.Email, subject, body);

            Console.WriteLine($"✅ Password reset email sent to {email}");
        }


        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            // 1️⃣ Validate user
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("Invalid reset link or email.");

            // 2️⃣ Reset password with token
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            // 3️⃣ Success
            return true;
        }


        //-------------------------
        // Check If Email Already Registered
        //-------------------------
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userManager.Users.AnyAsync(u => u.Email == email);
        }


        // -----------------------------
        // Logout
        // -----------------------------
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
