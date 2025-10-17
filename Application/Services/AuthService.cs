using AutoMapper;
using Core.DTOs.Auth;
using Core.Entities; // where ApplicationUser, Employer, JobSeeker, etc. live
using Core.Interfaces.IServices.IAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IMapper _mapper;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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

            // 6. Return safe data
            return _mapper.Map<UserResponseDTO>(user);
        }

        // -----------------------------
        // Login
        // -----------------------------
        public async Task<UserResponseDTO> LoginAsync(UserLoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("Invalid credentials");

            if (!user.IsActive)
                throw new Exception("Account is inactive");

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, isPersistent: false, lockoutOnFailure: false);
            Console.WriteLine(dto.Password+$"\n {result.IsNotAllowed}");
            if (!result.Succeeded)
                throw new Exception("Invalid login attempt");

            return _mapper.Map<UserResponseDTO>(user);
        }
        // -----------------------------
        // ForgetPassword
        // -----------------------------
        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("No account found with that email.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Normally, you’d send this token via email using a proper service
            // For now, just log or display it temporarily (for testing)
            Console.WriteLine($"Reset token for {email}: {token}");
            // Later, you'll email it via a real EmailService
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("Invalid request.");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

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
