using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

[Route("reports")]
public class ReportsController : Controller
{
    private readonly ITestReportingService _reportingService;
    private readonly ReportExportService _reportService;

    public ReportsController(ITestReportingService reportingService, ReportExportService reportService)
    {
        _reportingService = reportingService;
        _reportService = reportService;
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
}
