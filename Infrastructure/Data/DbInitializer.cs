using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            ApplicationDbContext context)
        {
            // 1️⃣ Ensure database exists and migrations are applied
            await context.Database.MigrateAsync();

            // 2️⃣ Create Roles
            string[] roles = { "Admin", "Employer", "JobSeeker" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
            }

            // 3️⃣ Create Admin
            if (await userManager.FindByEmailAsync("admin@jobros.com") == null)
            {
                var admin = new Admin
                {
                    UserName = "admin@jobros.com",
                    Email = "admin@jobros.com",
                    FullName = "Super Admin",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(admin, "Admin@123"); // Automatically hashed
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // 4️⃣ Create Employer
            if (await userManager.FindByEmailAsync("employer@jobros.com") == null)
            {
                var employer = new Employer
                {
                    UserName = "employer@jobros.com",
                    Email = "employer@jobros.com",
                    FullName = "Tech Corp",
                    CompanyName = "Tech Corp",
                    Industry = "Software",
                    Website = "www.techcorp.com",
                    Location = "Cairo, Egypt",
                    Description = "Leading software development company",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(employer, "Employer@123");
                await userManager.AddToRoleAsync(employer, "Employer");
            }

            // 5️⃣ Create Job Seeker
            if (await userManager.FindByEmailAsync("seeker@jobros.com") == null)
            {
                var seeker = new JobSeeker
                {
                    UserName = "seeker@jobros.com",
                    Email = "seeker@jobros.com",
                    FullName = "Mohamed Seeker",
                    Bio = "Passionate .NET developer looking for backend opportunities",
                    Skills = "C#, ASP.NET, SQL Server, Entity Framework",
                    ExperienceYears = 2,
                    ResumeUrl = "/uploads/cv/mohamed.pdf",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(seeker, "Seeker@123");
                await userManager.AddToRoleAsync(seeker, "JobSeeker");
            }

            // 6️⃣ Seed Categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Software Development", Description = "Programming and IT jobs" },
                    new Category { Name = "Marketing", Description = "Digital marketing and sales" },
                    new Category { Name = "Design", Description = "UI/UX, Graphic Design" },
                    new Category { Name = "Business", Description = "Operations, HR, and Management" }
                );

                await context.SaveChangesAsync();
            }

            // 7️⃣ Optional: Add a sample job for Employer
            if (!context.Jobs.Any())
            {
                var employer = await userManager.FindByEmailAsync("employer@jobros.com") as Employer;
                var category = context.Categories.FirstOrDefault();

                if (employer != null && category != null)
                {
                    context.Jobs.Add(new Job
                    {
                        EmployerId = employer.Id,
                        CategoryId = category.Id,
                        Title = "Junior Backend Developer",
                        Description = "Build REST APIs using ASP.NET Core.",
                        Requirements = "Strong C#, EF Core, SQL knowledge.",
                        SalaryRange = "8k - 12k EGP",
                        JobType = "FullTime",
                        Location = "Cairo, Egypt",
                        ExpirationDate = System.DateTime.Now.AddMonths(1)
                    });

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
