# 📧 Jobros Email Service Guide

This document explains **how to use the centralized SendGrid-based email service** across the Jobros project for sending any transactional or notification emails.

---

## 🧱 Overview

All outgoing emails in **Jobros** are sent via a single shared service:

**File:** `Infrastructure/Services/SendGridEmailService.cs`  
**Implements:** `Core.Interfaces.IServices.IEmailServices.IEmailService`

This design ensures:
- ✅ Consistent email formatting and delivery handling  
- ✅ Centralized configuration of API keys and sender identity  
- ✅ Easy integration with any application layer (Auth, Employer, Admin, etc.)

---

## ⚙️ Service Implementation

### Location
```csharp
Infrastructure/Services/SendGridEmailService.cs
```

### Dependencies
- `ISendGridClient` → Injected SendGrid client  
- `IConfiguration` → Reads `SendGrid` settings from `appsettings.json`

### Main Methods

```csharp
public class SendGridEmailService : IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlContent, string plainTextContent = null);
    Task<bool> PingAsync();
}
```

---

## 🧩 Configuration

Before using the service, ensure your **SendGrid API key** and sender information are stored in configuration:

### `appsettings.json`
```json
"SendGrid": {
  "ApiKey": "YOUR_SENDGRID_API_KEY",
  "FromEmail": "noreply@jobros.com",
  "FromName": "Jobros Support"
}
```

### Dependency Injection (in `Program.cs` or `Startup.cs`)
```csharp
builder.Services.AddSingleton<ISendGridClient>(sp =>
    new SendGridClient(builder.Configuration["SendGrid:ApiKey"]));

builder.Services.AddScoped<IEmailService, SendGridEmailService>();
```

---

## 📨 Sending an Email

### Basic Example
```csharp
await _emailService.SendEmailAsync(
    toEmail: "user@example.com",
    subject: "Welcome to Jobros!",
    htmlContent: "<h3>Hello!</h3><p>Your account was created successfully.</p>"
);
```

### Optional Plain Text Content
```csharp
await _emailService.SendEmailAsync(
    "user@example.com",
    "Test Email",
    "<p>This is <b>HTML</b> content</p>",
    "This is plain text fallback"
);
```

If the email fails, the service throws an exception with SendGrid’s response details for debugging:
```
SendGrid failed: BadRequest - {"errors":[...]}
```

---

## 🧠 Ping the Service (Health Check)

You can verify that the SendGrid API key is valid and responsive:
```csharp
bool isHealthy = await _emailService.PingAsync();
if (!isHealthy)
    Console.WriteLine("⚠️ SendGrid not reachable or API key invalid.");
```

---

## 🧩 Typical Use Cases

Below are examples from real application layers using the email service.

---

### 📝 **1. Registration Confirmation**

**Location:** `Application/Services/AuthService.cs`

After a new user registers, send a confirmation email:
```csharp
var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

var confirmUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://" +
                 $"{_httpContextAccessor.HttpContext.Request.Host}/Auth/ConfirmEmail" +
                 $"?userId={user.Id}&token={Uri.EscapeDataString(token)}";

await _emailService.SendEmailAsync(
    user.Email,
    "Confirm your Jobros account",
    $"<h3>Welcome to Jobros, {user.FullName}!</h3>" +
    $"<p>Please confirm your email by clicking below:</p>" +
    $"<a href='{confirmUrl}'>Confirm Email</a>"
);
```

---

### 🔐 **2. Forgot Password**

**Location:** `Application/Services/AuthService.cs`

When the user requests a password reset:
```csharp
var token = await _userManager.GeneratePasswordResetTokenAsync(user);

var resetUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://" +
               $"{_httpContextAccessor.HttpContext.Request.Host}/Auth/ResetPassword" +
               $"?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

var body = $@"
<h3>Hello {user.FullName},</h3>
<p>We received a request to reset your Jobros password.</p>
<p><a href='{resetUrl}'>Reset Password</a></p>
<p>If this wasn’t you, ignore this email.</p>
";

await _emailService.SendEmailAsync(
    user.Email,
    "Reset your Jobros password",
    body
);
```

---

### ⚙️ **3. Admin or Employer Notifications (Custom Use)**

Developers can use the same service for internal alerts or employer communication:
```csharp
await _emailService.SendEmailAsync(
    employer.Email,
    "New Job Application Received",
    $"<p>Dear {employer.FullName},</p><p>A new applicant has applied to your job posting.</p>"
);
```

---

## 🧾 Best Practices

✅ **Always use HTML templates or consistent styling** for better readability.  
✅ **Avoid hardcoded sender info** — always read from configuration.  
✅ **Catch and log exceptions** when sending emails in background jobs.  
✅ **Do not block user flows** — prefer background queueing (future improvement).  
✅ **Use PingAsync** during system health checks or diagnostics.  

---

## 🧩 Quick Reference

| Task | Method | Layer | Example |
|------|---------|--------|----------|
| Send registration confirmation | `SendEmailAsync()` | Application | AuthService.RegisterAsync |
| Send password reset link | `SendEmailAsync()` | Application | AuthService.ForgotPasswordAsync |
| Check SendGrid connection | `PingAsync()` | Infrastructure | Health monitoring |
| Notify employer/admin | `SendEmailAsync()` | Application | EmployerService or AdminService |

---

## 🧰 Troubleshooting

| Issue | Likely Cause | Fix |
|--------|---------------|------|
| `SendGrid failed: Unauthorized` | Invalid API key | Update `appsettings.json` |
| `NullReferenceException in FromEmail` | Missing config section | Check `SendGrid:FromEmail` |
| Email not received | Sent to spam or invalid recipient | Check email content and address |
| `PingAsync` returns false | Network or API key issue | Verify SendGrid credentials |

---

### Team Reminder
> All email-sending logic should go through **`IEmailService`**,  
> never use `SendGridClient` directly in controllers or services.

---

**Author:** Abdullah Ragab  
**Updated:** October 2025  
**Purpose:** Jobros Centralized Email Service Developer Guide
