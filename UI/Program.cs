using Application.DTO_Mappers;
using Application.Services;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.IServices.Commands;
using Core.Interfaces.IServices.IAuth;
using Core.Interfaces.IServices.IEmailServices;
using Core.Interfaces.IServices.IQueries;
using Core.Interfaces.IUnitOfWorks;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Auth Service
builder.Services.AddScoped<IAuthService, AuthService>();

// Email Service
builder.Services.AddSendGrid(options =>
{
    options.ApiKey = builder.Configuration["SendGrid:ApiKey"]?? Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
});

builder.Services.AddScoped<IEmailService, SendGridEmailService>();


// JobSeeker services
builder.Services.AddScoped<IJobSeekerQueryService, JobSeekerService>();
builder.Services.AddScoped<IJobSeekerCommandService, JobSeekerService>();

// AutoMapper
builder.Services.AddAutoMapper(op => op.AddProfile(typeof(MappingProfile)));
builder.Services.AddTransient<RoleResolver>();

// MVC
builder.Services.AddControllersWithViews();



var app = builder.Build();


// Automatic Seeding on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var context = services.GetRequiredService<ApplicationDbContext>();

    await DbInitializer.SeedAsync(userManager, roleManager, context);
}

// Configure Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Serve static files (wwwroot)
app.UseStaticFiles();

app.UseRouting();

// Identity Middleware
app.UseAuthentication();
app.UseAuthorization();

// Map Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// If your template uses it (optional, for static assets mapping)
app.MapStaticAssets();

app.Run();
