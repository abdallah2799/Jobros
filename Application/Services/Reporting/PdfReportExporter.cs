using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Reflection;

public class PdfReportExporter : IReportExporter
{
    public byte[] Export<T>(IEnumerable<T> data, string title = null)
        => ExportBundle(CreateBundle(title ?? "Report", data?.Cast<object>()));

    public byte[] ExportBundle(ReportBundle bundle)
    {
        using var ms = new MemoryStream();
        var doc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
        PdfWriter.GetInstance(doc, ms);
        doc.Open();

        foreach (var kv in bundle.Datasets)
        {
            var sectionTitle = new Paragraph(kv.Key)
            {
                SpacingAfter = 8f,
                Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14)
            };
            doc.Add(sectionTitle);

            var list = kv.Value?.ToList() ?? new List<object>();
            if (!list.Any())
            {
                doc.Add(new Paragraph("(No data)"));
                doc.NewPage();
                continue;
            }

            var props = list.First().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var table = new PdfPTable(props.Length) { WidthPercentage = 100 };

            // Header row
            foreach (var p in props)
            {
                var headerCell = new PdfPCell(new Phrase(p.Name))
                {
                    BackgroundColor = BaseColor.LightGray,
                    Padding = 5
                };
                table.AddCell(headerCell);
            }

            // Data rows
            foreach (var item in list)
            {
                foreach (var p in props)
                {
                    var val = p.GetValue(item);
                    var cellValue = NormalizeValue(val);
                    var cell = new PdfPCell(new Phrase(cellValue))
                    {
                        Padding = 4
                    };
                    table.AddCell(cell);
                }
            }

            doc.Add(table);
            doc.NewPage();
        }

        doc.Close();
        return ms.ToArray();
    }

    private static string NormalizeValue(object val)
    {
        if (val == null) return string.Empty;
        return val switch
        {
            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm"),
            bool b => b ? "Yes" : "No",
            IConvertible c => Convert.ToString(c),
            _ => val.ToString()
        };
    }

    private ReportBundle CreateBundle(string name, IEnumerable<object> data)
    {
        var bundle = new ReportBundle();
        bundle.Add(name, data);
        return bundle;
    }
}
