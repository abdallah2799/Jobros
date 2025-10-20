// Application/Services/Reporting/IReportExporter.cs
public interface IReportExporter
{
    // Single dataset convenience
    byte[] Export<T>(IEnumerable<T> data, string title = null);

    // Multi-dataset container
    byte[] ExportBundle(ReportBundle bundle);
}

// Application/Services/Reporting/ReportBundle.cs
public class ReportBundle
{
    // Key = sheet/section name, Value = boxed IEnumerable
    public Dictionary<string, IEnumerable<object>> Datasets { get; } = new();

    public void Add<T>(string name, IEnumerable<T> items)
        => Datasets[name ?? Guid.NewGuid().ToString()] = items?.Cast<object>() ?? Enumerable.Empty<object>();
}
