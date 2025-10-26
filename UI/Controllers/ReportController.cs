using Core.Interfaces.IUnitOfWorks;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("reports")]
public class ReportsController : Controller
{
    private readonly ITestReportingService _reportingService;
    private readonly ReportExportService _reportService;
    private readonly IUnitOfWork _unitOfWork;

    public ReportsController(ITestReportingService reportingService, ReportExportService reportService, IUnitOfWork unitOfWork)
    {
        _reportingService = reportingService;
        _reportService = reportService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("applicants")]
    public async Task<IActionResult> ExportApplicants(string format = "excel")
    {
        // Step 1: get the data from your Application layer (through UnitOfWork)
        var applicants = await _reportingService.GetApplicantsReportAsync();

        // Step 2: Build the report bundle
        var bundle = new ReportBundle();
        bundle.Add("Applicants", applicants);

        // Step 3: Add a summary dataset
        var stats = new[]
        {
            new
            {
                TotalApplicants = applicants.Count(),
                Accepted = applicants.Count(a =>
                    (string)a.GetType().GetProperty("Status")?.GetValue(a) == "Accepted")
            }
        };
        bundle.Add("Summary", stats);

        // Step 4: Export report
        var bytes = _reportService.ExportBundle(bundle, format);

        // Step 5: Return file to browser
        var contentType = format.ToLower() == "pdf"
            ? "application/pdf"
            : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        var ext = format.ToLower() == "pdf" ? "pdf" : "xlsx";
        var fileName = $"Applicants_{DateTime.UtcNow:yyyyMMddHHmmss}.{ext}";

        return File(bytes, contentType, fileName);
    }

    [HttpGet("applicants/{jobid:int}")]
    public async Task<IActionResult> ExportApplicants(int jobid, string format = "excel")
    {
        // Step 1: Get job with employer info
        var job = await _unitOfWork.Jobs
            .AsQueryable()
            .Include(j => j.Employer)
            .FirstOrDefaultAsync(j => j.Id == jobid);

        if (job == null)
            return NotFound("Job not found.");

        // Step 2: Get applicants
        var applicants = await _reportingService.GetApplicantsReportAsync(jobid);

        // Step 3: Build report bundle
        var bundle = new ReportBundle();
        bundle.Add("Applicants", applicants);

        var stats = new[]
        {
        new
        {
            TotalApplicants = applicants.Count(),
            Accepted = applicants.Count(a =>
                (string)a.GetType().GetProperty("Status")?.GetValue(a) == "Accepted")
        }
        };
        bundle.Add("Summary", stats);

        // Step 4: Export
        var bytes = _reportService.ExportBundle(bundle, format);

        // Step 5: Generate safe filename
        var employerName = job.Employer?.CompanyName ?? "UnknownEmployer";
        var jobTitle = job.Title ?? "UnknownJob";

        var safeEmployer = SanitizeFileName(employerName);
        var safeJobTitle = SanitizeFileName(jobTitle);

        var ext = format.ToLower() == "pdf" ? "pdf" : "xlsx";
        var fileName = $"{safeEmployer}-{safeJobTitle}-Applicants.{ext}";

        var contentType = format.ToLower() == "pdf"
            ? "application/pdf"
            : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        return File(bytes, contentType, fileName);
    }

    private string SanitizeFileName(string input)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", input.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))
                             .Trim('_')
                             .Replace(" ", "_");
    }
}
