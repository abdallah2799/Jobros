// Application/Services/Reporting/ExcelReportExporter.cs
using ClosedXML.Excel;
using System.Reflection;

public class ExcelReportExporter : IReportExporter
{
    public byte[] Export<T>(IEnumerable<T> data, string title = null)
        => ExportBundle(CreateBundle(title ?? "Sheet1", data?.Cast<object>()));

    public byte[] ExportBundle(ReportBundle bundle)
    {
        using var wb = new XLWorkbook();
        foreach (var kv in bundle.Datasets)
        {
            var sheetName = SafeSheetName(kv.Key);
            var ws = wb.Worksheets.Add(sheetName);

            var list = kv.Value?.ToList() ?? new List<object>();
            if (!list.Any()) continue;

            var first = list.First();
            var props = first.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Header
            for (int c = 0; c < props.Length; c++)
            {
                ws.Cell(1, c + 1).Value = props[c].Name;
                ws.Cell(1, c + 1).Style.Font.Bold = true;
            }

            // Rows
            int r = 2;
            foreach (var item in list)
            {
                for (int c = 0; c < props.Length; c++)
                {
                    var val = props[c].GetValue(item);

                    // Convert to something ClosedXML accepts (string, number, bool, DateTime)
                    if (val == null)
                        ws.Cell(r, c + 1).Value = string.Empty;
                    else if (val is DateTime dt)
                        ws.Cell(r, c + 1).Value = dt;
                    else if (val is bool b)
                        ws.Cell(r, c + 1).Value = b;
                    else if (val is IConvertible)
                        ws.Cell(r, c + 1).Value = Convert.ToString(val);
                    else
                        ws.Cell(r, c + 1).Value = val.ToString();
                }
                r++;
            }


            ws.Columns().AdjustToContents();
        }

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    private ReportBundle CreateBundle(string name, IEnumerable<object> data)
    {
        var bundle = new ReportBundle();
        bundle.Add(name, data);
        return bundle;
    }

    private string SafeSheetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Sheet1";
        var invalid = new[] { '/', '\\', '?', '*', '[', ']' };
        var safe = string.Concat(name.Select(ch => invalid.Contains(ch) ? '_' : ch));
        return safe.Length > 31 ? safe[..31] : safe;
    }
}
