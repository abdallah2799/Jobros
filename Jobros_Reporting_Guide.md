# 📊 Jobros Reporting Guide

This document explains **how to create and export reports** (Excel or PDF) in the Jobros project using our centralized reporting module.

---

## 🧱 Reporting Module Overview

All report exports use the shared **ReportExportService**, located at:

`Application/Services/Reporting/ReportExportService.cs`

This service wraps two specialized exporters:
- **ExcelReportExporter** → generates `.xlsx` files using *ClosedXML*.
- **PdfReportExporter** → generates `.pdf` files using *iTextSharp*.

Developers can use the same unified call for both:
```csharp
var bytes = _reportExportService.ExportBundle(bundle, format);
```
Where:

`bundle` → contains one or more datasets (your report data).

`format` → `"excel"` or `"pdf"`.

---

## 🧩 Typical Usage Flow
Every report follows four steps:

1. 🧠 **Create a stored procedure** that returns the required data.
2. 🧾 **Create a DTO** to receive the stored procedure output.
3. 🧰 **Write a service method** (in EmployerService, AdminService, etc.) that executes the stored procedure, fills the DTO(s), builds a ReportBundle, and calls ReportExportService.
4. 📤 **Return or download** the generated file from your controller (optional).

---

## ⚙️ Step 1 — Create a Stored Procedure
Example (SQL Server):
```sql
CREATE PROCEDURE usp_GetEmployerJobReport
    @EmployerId INT
AS
BEGIN
    SELECT 
        j.Id AS JobId,
        j.Title AS JobTitle,
        COUNT(a.Id) AS ApplicantsCount,
        SUM(CASE WHEN a.Status = 'Accepted' THEN 1 ELSE 0 END) AS AcceptedCount
    FROM Job j
    LEFT JOIN Application a ON a.JobId = j.Id
    WHERE j.EmployerId = @EmployerId
    GROUP BY j.Id, j.Title;
END
```

---

## 🧾 Step 2 — Create a DTO
Example DTO for the above stored procedure:

File: `Core/DTOs/EmployerJobReportDto.cs`

```csharp
namespace Core.DTOs
{
    public class EmployerJobReportDto
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; }
        public int ApplicantsCount { get; set; }
        public int AcceptedCount { get; set; }
    }
}
```
> The column names in the stored procedure must match the DTO property names.

---

## 🧰 Step 3 — Implement the Service Logic
You’ll typically do this in your own service class:
(e.g., EmployerService or AdminService inside `Application/Services/`)

Here’s how a report method should look:
```csharp
using Core.DTOs;
using System.Data;
using Microsoft.Data.SqlClient;
using Application.Services.Reporting;

public class EmployerService
{
    private readonly IConfiguration _config;
    private readonly ReportExportService _reportService;

    public EmployerService(IConfiguration config, ReportExportService reportService)
    {
        _config = config;
        _reportService = reportService;
    }

    public byte[] ExportEmployerJobReport(int employerId, string format = "excel")
    {
        var connectionString = _config.GetConnectionString("DefaultConnection");
        var bundle = new ReportBundle();

        using var conn = new SqlConnection(connectionString);
        using var cmd = new SqlCommand("usp_GetEmployerJobReport", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@EmployerId", employerId);

        conn.Open();
        using var reader = cmd.ExecuteReader();

        var jobs = DataReaderMapper.MapReaderToList<EmployerJobReportDto>(reader);
        bundle.Add("Employer Jobs", jobs);

        // Add optional summary sheet
        var summary = new[]
        {
            new { TotalJobs = jobs.Count, TotalApplicants = jobs.Sum(x => x.ApplicantsCount) }
        };
        bundle.Add("Summary", summary);

        return _reportService.ExportBundle(bundle, format);
    }
}
```

---

## 🖥️ Step 4 — Use It in a Controller (optional)
If you want the admin or employer to download the report, just call your service method from a controller.

```csharp
[HttpGet("export-employer-report/{employerId}")]
public IActionResult ExportEmployerReport(int employerId, string format = "excel")
{
    var bytes = _employerService.ExportEmployerJobReport(employerId, format);

    var contentType = format.ToLower() == "pdf"
        ? "application/pdf"
        : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    var ext = format == "pdf" ? "pdf" : "xlsx";
    var fileName = $"EmployerReport_{DateTime.UtcNow:yyyyMMddHHmmss}.{ext}";

    return File(bytes, contentType, fileName);
}
```

---

## 📦 Step 5 — Understanding ReportBundle
The ReportBundle is how you pass one or more datasets to the exporter.

Example:
```csharp
var bundle = new ReportBundle();
bundle.Add("Applicants", applicantsList);
bundle.Add("Summary", summaryStats);
```
When exporting:

- **Excel** → each dataset becomes a separate sheet.
- **PDF** → each dataset becomes a separate section/table.

---

## 🧠 Notes and Best Practices
- Always use DTOs, not entity models.
- Always map stored procedure columns → DTO properties exactly.
- You can add multiple result sets by calling `reader.NextResult()` and mapping each one to a different DTO/sheet.
- Use `"pdf"` or `"excel"` (case-insensitive) as export formats.
- Reports should be built inside the Application layer; controllers only call them.
- Don’t access DbContext or repositories directly from the UI.

---

## 🧪 Example of a Multi-Result Report
If your stored procedure returns multiple result sets:

```csharp
using var reader = cmd.ExecuteReader();

var list1 = DataReaderMapper.MapReaderToList<JobReportDto>(reader);
bundle.Add("Job Details", list1);

if (reader.NextResult())
{
    var list2 = DataReaderMapper.MapReaderToList<ApplicantStatDto>(reader);
    bundle.Add("Applicant Stats", list2);
}
```

---

## 🧩 Quick Reference

| Step | Developer Action | Layer |
|------|-------------------|--------|
| 1 | Create stored procedure | Database |
| 2 | Create matching DTO | Core |
| 3 | Implement method that calls SP + builds ReportBundle | Application |
| 4 | (Optional) Add controller endpoint for download | UI |

---

## 🧰 Troubleshooting

| Issue | Likely Cause | Fix |
|--------|---------------|------|
| Empty sheet/table | DTO property names don’t match SP columns | Rename columns or properties |
| “Invalid format” exception | Wrong format argument | Use "pdf" or "excel" |
| Broken foreign keys | Missing reader.NextResult() | Ensure each result set is mapped correctly |
| NullReferenceException in Job/JobSeeker | SP missing join or alias | Review stored procedure logic |

---

### Team Reminder
All reporting logic should go through **ReportExportService**.  
Never use DbContext or EF directly in controllers — keep the onion clean 🍃.

---

**Author:** Abdullah Ragab  
**Updated:** October 2025  
**Purpose:** Jobros Reporting Module Developer Guide
