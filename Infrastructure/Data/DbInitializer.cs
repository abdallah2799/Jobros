using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            // 1️⃣ Ensure DB is up to date
            await context.Database.MigrateAsync();

            // 2️⃣ Roles
            string[] roles = { "Admin", "Employer", "JobSeeker" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
            }

            // 3️⃣ Admins
            var admins = new List<Admin>
            {
                new Admin { UserName = "admin@jobros.com", Email = "admin@jobros.com", FullName = "Super Admin", EmailConfirmed = true },
                new Admin { UserName = "support@jobros.com", Email = "support@jobros.com", FullName = "Support Admin", EmailConfirmed = true }
            };

            foreach (var admin in admins)
            {
                if (await userManager.FindByEmailAsync(admin.Email) == null)
                {
                    await userManager.CreateAsync(admin, "Admin@123");
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // 4️⃣ Employers
            var employers = new List<Employer>
            {
                new Employer
                {
                    UserName = "techcorp@jobros.com",
                    Email = "techcorp@jobros.com",
                    FullName = "Tech Corp",
                    CompanyName = "Tech Corp",
                    Industry = "Software",
                    Website = "https://techcorp.com",
                    Location = "Cairo, Egypt",
                    Description = "Leading software development company",
                    EmailConfirmed = true
                },
                new Employer
                {
                    UserName = "designhub@jobros.com",
                    Email = "designhub@jobros.com",
                    FullName = "Design Hub",
                    CompanyName = "Design Hub",
                    Industry = "Design",
                    Website = "https://designhub.com",
                    Location = "Alexandria, Egypt",
                    Description = "Creative digital agency specializing in UI/UX",
                    EmailConfirmed = true
                }
            };

            foreach (var emp in employers)
            {
                if (await userManager.FindByEmailAsync(emp.Email) == null)
                {
                    await userManager.CreateAsync(emp, "Employer@123");
                    await userManager.AddToRoleAsync(emp, "Employer");
                }
            }

            // 5️⃣ Job Seekers
            var seekers = new List<JobSeeker>
            {
                new JobSeeker
                {
                    UserName = "mohamed@jobros.com",
                    Email = "mohamed@jobros.com",
                    FullName = "Mohamed Ragab",
                    Bio = "Passionate .NET developer looking for backend opportunities",
                    Skills = "C#, ASP.NET, SQL Server, EF Core",
                    ExperienceYears = 2,
                    ResumeUrl = "/uploads/cv/mohamed.pdf",
                    EmailConfirmed = true
                },
                new JobSeeker
                {
                    UserName = "sara@jobros.com",
                    Email = "sara@jobros.com",
                    FullName = "Sara Ali",
                    Bio = "Creative designer seeking new challenges in UI/UX",
                    Skills = "Figma, Adobe XD, HTML, CSS",
                    ExperienceYears = 3,
                    ResumeUrl = "/uploads/cv/sara.pdf",
                    EmailConfirmed = true
                }
            };

            foreach (var seeker in seekers)
            {
                if (await userManager.FindByEmailAsync(seeker.Email) == null)
                {
                    await userManager.CreateAsync(seeker, "Seeker@123");
                    await userManager.AddToRoleAsync(seeker, "JobSeeker");
                }
            }

            // 6️⃣ Categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Software Development", Description = "Programming and IT jobs" },
                    new Category { Name = "Design", Description = "UI/UX, Graphic Design" },
                    new Category { Name = "Marketing", Description = "Digital marketing and sales" },
                    new Category { Name = "Business", Description = "Operations, HR, and Management" },
                    new Category { Name = "Engineering", Description = "Mechanical, Electrical, Civil engineering" }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // 7️⃣ Jobs
            if (!context.Jobs.Any())
            {
                var techCorp = await userManager.FindByEmailAsync("techcorp@jobros.com") as Employer;
                var designHub = await userManager.FindByEmailAsync("designhub@jobros.com") as Employer;
                var softwareCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Software Development");
                var designCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Design");

                if (techCorp != null && designHub != null)
                {
                    var jobs = new List<Job>
                    {
                        new Job
                        {
                            EmployerId = techCorp.Id,
                            CategoryId = softwareCategory.Id,
                            Title = "Junior Backend Developer",
                            Description = "Build and maintain REST APIs using ASP.NET Core.",
                            Requirements = "C#, SQL, Entity Framework",
                            SalaryRange = "8k - 12k EGP",
                            JobType = "FullTime",
                            Location = "Cairo, Egypt",
                            ExpirationDate = DateTime.Now.AddMonths(1)
                        },
                        new Job
                        {
                            EmployerId = techCorp.Id,
                            CategoryId = softwareCategory.Id,
                            Title = "Database Administrator",
                            Description = "Manage SQL Server databases and ensure performance.",
                            Requirements = "T-SQL, performance tuning, backup/restore",
                            SalaryRange = "10k - 14k EGP",
                            JobType = "FullTime",
                            Location = "Cairo, Egypt",
                            ExpirationDate = DateTime.Now.AddMonths(1)
                        },
                        new Job
                        {
                            EmployerId = designHub.Id,
                            CategoryId = designCategory.Id,
                            Title = "UI/UX Designer",
                            Description = "Design user-friendly interfaces for mobile apps.",
                            Requirements = "Figma, Adobe XD, prototyping",
                            SalaryRange = "7k - 10k EGP",
                            JobType = "Remote",
                            Location = "Alexandria, Egypt",
                            ExpirationDate = DateTime.Now.AddMonths(1)
                        }
                    };

                    context.Jobs.AddRange(jobs);
                    await context.SaveChangesAsync();
                }
            }

            // 8️⃣ Applications
            if (!context.Applications.Any())
            {
                var seeker1 = await userManager.FindByEmailAsync("mohamed@jobros.com") as JobSeeker;
                var seeker2 = await userManager.FindByEmailAsync("sara@jobros.com") as JobSeeker;
                var job1 = await context.Jobs.FirstOrDefaultAsync(j => j.Title.Contains("Backend"));
                var job2 = await context.Jobs.FirstOrDefaultAsync(j => j.Title.Contains("Designer"));

                if (seeker1 != null && job1 != null)
                {
                    context.Applications.Add(new Application
                    {
                        JobId = job1.Id,
                        JobSeekerId = seeker1.Id,
                        CoverLetter = "I’m passionate about backend development and have hands-on experience with ASP.NET Core.",
                        Status = "Pending"
                    });
                }

                if (seeker2 != null && job2 != null)
                {
                    context.Applications.Add(new Application
                    {
                        JobId = job2.Id,
                        JobSeekerId = seeker2.Id,
                        CoverLetter = "I love designing user interfaces that make users happy.",
                        Status = "Pending"
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
