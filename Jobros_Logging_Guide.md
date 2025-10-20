# 🪵 Jobros Logging Guide

### **Overview**
Jobros uses **Serilog** as its centralized structured logging engine.  
It automatically records all HTTP requests, unhandled exceptions, and any manual logs developers add using the standard ASP.NET Core `ILogger<T>` interface.

All logs are written to:
- 🖥️ **Console** – for real-time feedback during development  
- 📄 **Files** – daily rotating text files (`/Logs/log-YYYY-MM-DD.txt`)  
- 🗄️ **SQL Server** – automatically created **`Logs`** table

---

## 🧱 Logs Table Structure

The log table was **auto-generated** by the Serilog MSSQL sink.  
Its schema is as follows:

| Column | Type | Description |
|---------|------|-------------|
| **Id** | int | Primary key (auto-increment) |
| **Message** | nvarchar(max) | Final rendered log message |
| **MessageTemplate** | nvarchar(max) | Original message format (with placeholders) |
| **Level** | nvarchar(128) | Log severity (Information, Warning, Error, etc.) |
| **TimeStamp** | datetime | When the event occurred |
| **Exception** | nvarchar(max) | Stack trace or exception details (if any) |
| **Properties** | nvarchar(max) | Structured JSON data (context placeholders) |
| **UserName** | nvarchar(100) | Logged user (if enriched or manually added) |
| **RequestPath** | nvarchar(255) | The HTTP route being handled |
| **SourceContext** | nvarchar(255) | The class name or logger source |
| **MachineName** | nvarchar(100) | Host machine that logged the event |

Sample query:
```sql
SELECT TOP (100)
      [Id],
      [Message],
      [Level],
      [TimeStamp],
      [Exception],
      [UserName],
      [RequestPath],
      [MachineName]
FROM [JobrosDB_Team].[dbo].[Logs]
ORDER BY [TimeStamp] DESC;
```

---

## ⚙️ Using the Logger in Code

You don’t need to import Serilog directly.  
ASP.NET Core injects an `ILogger<T>` instance automatically anywhere you need it.

### Example in a Service
```csharp
using Microsoft.Extensions.Logging;

public class EmployerService : IEmployerService
{
    private readonly ILogger<EmployerService> _logger;

    public EmployerService(ILogger<EmployerService> logger)
    {
        _logger = logger;
    }

    public async Task PostJobAsync(JobDto job)
    {
        _logger.LogInformation("Attempting to post new job: {Title}", job.Title);

        try
        {
            // Simulate database logic
            await Task.Delay(100);

            _logger.LogInformation("Job '{Title}' posted successfully by {Employer}", job.Title, "TechCorp");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error posting job '{Title}'", job.Title);
            throw;
        }
    }
}
```

---

### Example in a Controller
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public class JobsController : Controller
{
    private readonly IJobService _jobService;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IJobService jobService, ILogger<JobsController> logger)
    {
        _jobService = jobService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(JobDto job)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid job submission by {User}", User.Identity?.Name);
            return BadRequest("Invalid data.");
        }

        try
        {
            await _jobService.CreateJobAsync(job);
            _logger.LogInformation("User {User} created a new job '{Title}'", User.Identity?.Name, job.Title);
            return RedirectToAction("MyJobs");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job '{Title}' by {User}", job.Title, User.Identity?.Name);
            return StatusCode(500, "An error occurred.");
        }
    }
}
```

---

## 🧭 Log Levels Explained

| Level | Meaning | Example |
|--------|----------|---------|
| **Trace** | Extremely detailed internal diagnostics (rarely used) | Function entry/exit |
| **Debug** | Developer-only diagnostic info | Variable values, flow tracing |
| **Information** | Expected behavior and milestones | “Job created successfully” |
| **Warning** | Unexpected but recoverable conditions | “Job expired before any applicants” |
| **Error** | Failures that should be investigated | Exceptions, database failures |
| **Critical** | System-wide breakdowns | “Database connection lost” |

Example usage:
```csharp
_logger.LogDebug("Fetched {Count} jobs for category {Category}", jobCount, categoryName);
_logger.LogInformation("Job {Id} updated by {User}", jobId, User.Identity?.Name);
_logger.LogWarning("Slow response time detected at {Endpoint}", endpoint);
_logger.LogError(ex, "Unhandled exception during job posting");
_logger.LogCritical("Database unreachable on {Server}", Environment.MachineName);
```

---

## 🗃️ Log Destinations and Behavior

| Destination | Purpose | File/Table |
|--------------|----------|------------|
| 🖥️ Console | Real-time development output | Appears in Visual Studio terminal |
| 📄 File | Daily rotating file log | `/Logs/log-YYYY-MM-DD.txt` |
| 🗄️ SQL Server | Persistent structured storage | `[dbo].[Logs]` |

Every entry is stored automatically — including middleware-level logs for all HTTP requests, status codes, and exceptions.

---

## 🧩 Automatic Logging in Jobros

The **global middleware** you configured handles these automatically:

- Every HTTP request →  
  `GET /Jobs/Details responded 200 in 153 ms`
- Every unhandled exception →  
  `Unhandled exception for POST /Auth/Login`
- Each request includes `RequestPath`, `UserName` (if available), and `MachineName`.

You can add contextual logs inside your services and controllers for:
- Successful or failed user actions (posting a job, updating profile)
- Security or admin events
- Business-critical operations (payments, hiring decisions)

---

## 🧱 Best Practices

✅ **Always use placeholders** for variables:  
```csharp
_logger.LogInformation("Employer {EmployerId} posted job '{JobTitle}'", employerId, jobTitle);
```
→ These placeholders become structured fields in the `Properties` JSON column.

✅ **Never log sensitive data** (passwords, tokens, personal info).

✅ **Use proper log levels**:
- `Information` for normal actions
- `Warning` for unusual events
- `Error` for exceptions

✅ **Wrap risky operations** in try/catch with `_logger.LogError(ex, ...)`.

✅ **Avoid repetitive logs** inside loops or frequent background jobs.

---

## 🧩 Optional Helper — Unified Action Logger

To make audit logs consistent across modules, create a helper like:

```csharp
public static class AppLogger
{
    public static void LogAction(ILogger logger, string user, string action, string entity)
    {
        logger.LogInformation("User {User} performed {Action} on {Entity} at {Time}",
            user, action, entity, DateTime.Now);
    }
}
```

Usage:
```csharp
AppLogger.LogAction(_logger, User.Identity?.Name, "Deleted", "Job #45");
```

---

## 🔍 How to Check Logs in SQL Server

Run:
```sql
SELECT TOP (100)
    [Id],
    [Level],
    [Message],
    [TimeStamp],
    [UserName],
    [RequestPath],
    [MachineName]
FROM [JobrosDB_Team].[dbo].[Logs]
ORDER BY [TimeStamp] DESC;
```

Example result:

| Id | Level | Message | UserName | RequestPath | TimeStamp |
|----|--------|----------|-----------|--------------|------------|
| 102 | Information | User “techcorp@jobros.com” created job “Backend Developer” | techcorp@jobros.com | /Jobs/Create | 2025-10-19 10:31 |
| 103 | Error | Error posting job “UI Designer” | designhub@jobros.com | /Jobs/Create | 2025-10-19 10:32 |

---

## 📘 Summary

| Feature | Description |
|----------|-------------|
| **Logger Engine** | Serilog |
| **Injection** | `ILogger<T>` (automatic) |
| **Middleware** | Logs all HTTP requests and exceptions |
| **Destinations** | Console, File, SQL Server |
| **Table** | `Logs` (auto-created) |
| **Schema** | Matches Serilog standard columns |
| **File Rotation** | Daily (`/Logs/log-YYYY-MM-DD.txt`) |
| **Properties Column** | Contains structured JSON for placeholders |
| **Safe to Use In** | Controllers, Services, Background Jobs |

---

### ✅ TL;DR
- Use `_logger.LogInformation()`, `_logger.LogWarning()`, `_logger.LogError()` normally.  
- Everything gets written to file + SQL automatically.  
- The `Logs` table is structured and queryable — perfect for admin dashboards, audits, and monitoring.
