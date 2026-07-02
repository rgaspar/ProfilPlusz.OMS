using MiniExcelLibs;

namespace Application.Common.Services.ExcelImport;

public class ExcelImportService
{
    public async Task<ImportResult> ImportAsync<TRequest>(
        Stream excelStream,
        IExcelRowMapper<TRequest> mapper,
        Func<TRequest, IDictionary<string, object>, int, CancellationToken, Task<string?>> processRow,
        CancellationToken cancellationToken)
    {
        IEnumerable<dynamic> rows;
        try
        {
            rows = await MiniExcelLibs.MiniExcel.QueryAsync(excelStream, useHeaderRow: true);
        }
        catch (Exception ex)
        {
            return new ImportResult
            {
                SuccessCount = 0,
                ErrorCount = 1,
                Errors = [new ImportError { RowNumber = 0, OriginalRow = new Dictionary<string, object>(), ErrorMessage = $"A fájl nem olvasható: {ex.Message}" }]
            };
        }

        var errors = new List<ImportError>();
        int successCount = 0;
        int rowNumber = 1;

        try
        {
            foreach (IDictionary<string, object> row in rows)
            {
                rowNumber++;

                try
                {
                    var request = await mapper.MapAsync(row, cancellationToken);
                    var errorMessage = await processRow(request, row, rowNumber, cancellationToken);

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
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            errors.Add(new ImportError
            {
                RowNumber = rowNumber,
                OriginalRow = new Dictionary<string, object>(),
                ErrorMessage = $"A fájl nem olvasható: {ex.Message}"
            });
        }

        return new ImportResult
        {
            SuccessCount = successCount,
            ErrorCount = errors.Count,
            Errors = errors
        };
    }

    public async Task<ImportResult> ImportWithValidationFirstAsync<TRequest>(
        Stream excelStream,
        IExcelRowMapper<TRequest> mapper,
        Func<TRequest, IDictionary<string, object>, int, CancellationToken, Task<string?>> validateRow,
        Func<TRequest, IDictionary<string, object>, int, CancellationToken, Task<string?>> processRow,
        CancellationToken cancellationToken)
    {
        IEnumerable<dynamic> rows;
        try
        {
            rows = await MiniExcelLibs.MiniExcel.QueryAsync(excelStream, useHeaderRow: true);
        }
        catch (Exception ex)
        {
            return new ImportResult
            {
                SuccessCount = 0,
                ErrorCount = 1,
                Errors = [new ImportError { RowNumber = 0, OriginalRow = new Dictionary<string, object>(), ErrorMessage = $"A fájl nem olvasható: {ex.Message}" }]
            };
        }

        var allRows = new List<(TRequest Request, IDictionary<string, object> Row, int RowNumber)>();
        var errors = new List<ImportError>();
        int rowNumber = 1;

        try
        {
            foreach (IDictionary<string, object> row in rows)
            {
                rowNumber++;

                try
                {
                    var request = await mapper.MapAsync(row, cancellationToken);
                    var errorMessage = await validateRow(request, row, rowNumber, cancellationToken);

                    if (errorMessage is null)
                        allRows.Add((request, row, rowNumber));
                    else
                        errors.Add(new ImportError { RowNumber = rowNumber, OriginalRow = row, ErrorMessage = errorMessage });
                }
                catch (Exception ex)
                {
                    errors.Add(new ImportError { RowNumber = rowNumber, OriginalRow = row, ErrorMessage = ex.Message });
                }
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            errors.Add(new ImportError
            {
                RowNumber = rowNumber,
                OriginalRow = new Dictionary<string, object>(),
                ErrorMessage = $"A fájl nem olvasható: {ex.Message}"
            });
        }

        if (errors.Count > 0)
            return new ImportResult { SuccessCount = 0, ErrorCount = errors.Count, Errors = errors };

        int successCount = 0;
        foreach (var (request, row, rn) in allRows)
        {
            try
            {
                var errorMessage = await processRow(request, row, rn, cancellationToken);
                if (errorMessage is null)
                    successCount++;
                else
                    errors.Add(new ImportError { RowNumber = rn, OriginalRow = row, ErrorMessage = errorMessage });
            }
            catch (Exception ex)
            {
                errors.Add(new ImportError { RowNumber = rn, OriginalRow = row, ErrorMessage = ex.Message });
            }
        }

        return new ImportResult { SuccessCount = successCount, ErrorCount = errors.Count, Errors = errors };
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

    public byte[] GenerateTemplateWithReferenceSheet(
        string[] headers,
        IDictionary<string, object?> sampleRow,
        List<Dictionary<string, object?>> referenceRows)
    {
        var mainData = new List<Dictionary<string, object?>>
        {
            headers.ToDictionary(h => h, h => sampleRow.TryGetValue(h, out var v) ? v : null)
        };

        var sheets = new Dictionary<string, object>
        {
            ["Sablon"] = mainData,
            ["Referencia"] = referenceRows
        };

        using var stream = new MemoryStream();
        stream.SaveAs(sheets);
        return stream.ToArray();
    }

    public byte[] GenerateTemplateWithMultipleSheets(
        string[] headers,
        IDictionary<string, object?> sampleRow,
        Dictionary<string, IEnumerable<Dictionary<string, object?>>> extraSheets)
    {
        var mainData = new List<Dictionary<string, object?>>
        {
            headers.ToDictionary(h => h, h => sampleRow.TryGetValue(h, out var v) ? v : null)
        };

        var sheets = new Dictionary<string, object> { ["Sablon"] = mainData };

        foreach (var (sheetName, sheetData) in extraSheets)
            sheets[sheetName] = sheetData;

        using var stream = new MemoryStream();
        stream.SaveAs(sheets);
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
