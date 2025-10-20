// Application/Services/Reporting/ReportExportService.cs
public class ReportExportService
{
    private readonly ExcelReportExporter _excel;
    private readonly PdfReportExporter _pdf;

    public ReportExportService(ExcelReportExporter excel, PdfReportExporter pdf)
    {
        _excel = excel;
        _pdf = pdf;
    }

    public byte[] ExportBundle(ReportBundle bundle, string format)
    {
        if (string.IsNullOrWhiteSpace(format)) format = "excel";
        return format.ToLower() switch
        {
            "pdf" => _pdf.ExportBundle(bundle),
            "excel" or "xlsx" => _excel.ExportBundle(bundle),
            _ => throw new ArgumentException("Unsupported export format")
        };
    }
}
