using MiniExcelLibs;

namespace Application.Common.Services.ExcelImport;

public class ExcelImportService
{
    public async Task<ImportResult> ImportAsync<TRequest>(
        Stream excelStream,
        IExcelRowMapper<TRequest> mapper,
        Func<TRequest, IDictionary<string, object>, CancellationToken, Task<string?>> processRow,
        CancellationToken cancellationToken)
    {
        var rows = await MiniExcelLibs.MiniExcel.QueryAsync(excelStream, useHeaderRow: true);

        var errors = new List<ImportError>();
        int successCount = 0;
        int rowNumber = 1;

        foreach (IDictionary<string, object> row in rows)
        {
            rowNumber++;

            try
            {
                var request = await mapper.MapAsync(row, cancellationToken);
                var errorMessage = await processRow(request, row, cancellationToken);

                if (errorMessage is null)
                    successCount++;
                else
                    errors.Add(new ImportError { RowNumber = rowNumber, OriginalRow = row, ErrorMessage = errorMessage });
            }
            catch (Exception ex)
            {
                errors.Add(new ImportError { RowNumber = rowNumber, OriginalRow = row, ErrorMessage = ex.Message });
            }
        }

        return new ImportResult
        {
            SuccessCount = successCount,
            ErrorCount = errors.Count,
            Errors = errors
        };
    }

    public byte[] GenerateCombinedReport(List<ImportError> errors, List<ImportWarning> warnings, string[] headers)
    {
        if (errors.Count == 0 && warnings.Count == 0)
            return [];

        var data = new List<object>();

        foreach (var error in errors)
        {
            var dict = BuildReportRow(error.OriginalRow, headers, "Hiba", error.ErrorMessage);
            data.Add(dict);
        }

        foreach (var warning in warnings)
        {
            var dict = BuildReportRow(warning.OriginalRow, headers, "Felülírás", warning.Details);
            data.Add(dict);
        }

        using var stream = new MemoryStream();
        stream.SaveAs(data);
        return stream.ToArray();
    }

    public byte[] GenerateErrorReport(List<ImportError> errors, string[] headers)
    {
        if (errors.Count == 0)
            return [];

        var data = errors.Select(e =>
        {
            var dict = new Dictionary<string, object?>();
            foreach (var header in headers)
                dict[header] = e.OriginalRow.TryGetValue(header, out var val) ? val : null;
            dict["Hiba"] = e.ErrorMessage;
            return (object)dict;
        }).ToList();

        using var stream = new MemoryStream();
        stream.SaveAs(data);
        return stream.ToArray();
    }

    public byte[] GenerateTemplate(string[] headers)
    {
        var data = new List<Dictionary<string, object?>>
        {
            headers.ToDictionary(h => h, _ => (object?)null)
        };

        using var stream = new MemoryStream();
        stream.SaveAs(data);
        return stream.ToArray();
    }

    private static Dictionary<string, object?> BuildReportRow(
        IDictionary<string, object> originalRow,
        string[] headers,
        string type,
        string details)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var header in headers)
            dict[header] = originalRow.TryGetValue(header, out var val) ? val : null;
        dict["Típus"] = type;
        dict["Részletek"] = details;
        return dict;
    }
}
