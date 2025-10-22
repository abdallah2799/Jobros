using Application.DTO_Mappers;
using Application.HealthChecks;
using Application.Services;
using Application.Services.Reporting;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.IServices.Commands;
using Core.Interfaces.IServices.IAdmin;
using Core.Interfaces.IServices.IAuth;
using Core.Interfaces.IServices.IEmailServices;
using Core.Interfaces.IServices.IQueries;
using Core.Interfaces.IUnitOfWorks;
using Core.Interfaces.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SendGrid.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using System.Text.Json;

// ===============================================
//            Configure Builder
// ===============================================

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------
// Connection String
// -----------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// -----------------------------------------------
// Configure Serilog (File + Database)
// -----------------------------------------------

var columnOptions = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
    {
        new SqlColumn("UserName", SqlDbType.NVarChar, dataLength: 100),
        new SqlColumn("RequestPath", SqlDbType.NVarChar, dataLength: 255),
        new SqlColumn("SourceContext", SqlDbType.NVarChar, dataLength: 255),
        new SqlColumn("MachineName", SqlDbType.NVarChar, dataLength: 100)
    }
};


try
{
    Log.Logger = new LoggerConfiguration()
     .MinimumLevel.Debug()
     .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
     .Enrich.FromLogContext()
     .Enrich.WithMachineName()
     .Enrich.WithEnvironmentUserName()
     .WriteTo.Console()
     .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
     .WriteTo.MSSqlServer(
         connectionString: connectionString,
         sinkOptions: new MSSqlServerSinkOptions
         {
             TableName = "Logs",
             AutoCreateSqlTable = true
         },
         columnOptions: columnOptions,
         restrictedToMinimumLevel: LogEventLevel.Information
     )
     .CreateLogger();

    builder.Host.UseSerilog();
}
catch (Exception ex)
{
    Console.WriteLine($"[Serilog] Initialization failed: {ex.Message}");
    throw;
}

// ===============================================
//            Service Registrations
// ===============================================

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity Configuration
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

// Cookie Paths
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});

// HttpContext Accessor
builder.Services.AddHttpContextAccessor();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Auth Service
builder.Services.AddScoped<IAuthService, AuthService>();

// Admin Service
builder.Services.AddScoped<IAdminService, AdminService>();

// Email (SendGrid)
builder.Services.AddSendGrid(options =>
{
    options.ApiKey = builder.Configuration["SendGrid:ApiKey"]
                     ?? Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
});
builder.Services.AddScoped<IEmailService, SendGridEmailService>();

// JobSeeker Services
builder.Services.AddScoped<IJobSeekerQueryService, JobSeekerService>();
builder.Services.AddScoped<IJobSeekerCommandService, JobSeekerService>();

// Admin Services
builder.Services.AddScoped<IAdminService, AdminService>();

// AutoMapper Configuration
builder.Services.AddAutoMapper(op => op.AddProfile(typeof(MappingProfile)));
builder.Services.AddTransient<RoleResolver>();

// MVC Controllers + Views
builder.Services.AddControllersWithViews();

// Memory cache needed for Session
builder.Services.AddDistributedMemoryCache();

// Configure Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;                   
    options.Cookie.IsEssential = true;               
});

// Reporting Services

// Test
//----------------------------------
builder.Services.AddScoped<ITestReportingService, TestReportingService>();
//----------------------------------

builder.Services.AddTransient<ExcelReportExporter>();
builder.Services.AddTransient<PdfReportExporter>();
builder.Services.AddTransient<ReportExportService>();

// -----------------------------------------------
// Health Checks (Database + Email)
// -----------------------------------------------
builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: connectionString,
        healthQuery: "SELECT 1;",
        name: "Database",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
        timeout: TimeSpan.FromSeconds(3)
    )
    .AddCheck<EmailHealthCheck>("EmailService");


// ===============================================
//                Build App
// ===============================================

var app = builder.Build();

// ===============================================
//          Global Logging Middleware
// ===============================================

app.UseHttpsRedirection();

// Logging every request/response and catching exceptions
app.Use(async (context, next) =>
{
    var sw = System.Diagnostics.Stopwatch.StartNew();

    try
    {
        await next.Invoke();
        sw.Stop();

        Log.Information("HTTP {Method} {Path} responded {StatusCode} in {Elapsed:0.0000} ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            sw.Elapsed.TotalMilliseconds);
    }
    catch (Exception ex)
    {
        sw.Stop();
        Log.Error(ex, "Unhandled exception for {Method} {Path}",
            context.Request.Method, context.Request.Path);
        throw;
    }
});

// ===============================================
//           Database Seeding on Startup
// ===============================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var context = services.GetRequiredService<ApplicationDbContext>();

    await DbInitializer.SeedAsync(userManager, roleManager, context);
}

// ===============================================
//          Middleware Configuration
// ===============================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// ===============================================
//              Endpoint Mapping
// ===============================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=SplashPage}/{id?}");

// Health Check Endpoint (visible in /health)
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var response = new
        {
            status = report.Status.ToString(),
            results = report.Entries.Select(e => new
            {
                key = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds.ToString("0.00") + " ms"
            })
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
});


// ===============================================
//            Run Application
// ===============================================

try
{
    Log.Information("Starting Jobros application...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed.");
}
finally
{
    Log.CloseAndFlush();
}
