using Application.Common.Services.ExcelImport;
using MiniExcelLibs;

namespace Application.Tests.ExcelImport;

public class ExcelImportServiceTests
{
    private readonly ExcelImportService _service = new();

    private static MemoryStream CreateExcelStream(params Dictionary<string, object?>[] rows)
    {
        var stream = new MemoryStream();
        stream.SaveAs(rows.Cast<object>().ToList());
        stream.Position = 0;
        return stream;
    }

    [Fact]
    public async Task ImportAsync_AllRowsValid_ReturnsCorrectSuccessCount()
    {
        using var stream = CreateExcelStream(
            new Dictionary<string, object?> { ["Name"] = "Row1" },
            new Dictionary<string, object?> { ["Name"] = "Row2" });

        var result = await _service.ImportAsync(
            stream,
            new TestMapper(),
            (_, _, _, _) => Task.FromResult<string?>(null),
            default);

        Assert.Equal(2, result.SuccessCount);
        Assert.Equal(0, result.ErrorCount);
        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ImportAsync_ProcessRowReturnsError_CountedAsError()
    {
        using var stream = CreateExcelStream(new Dictionary<string, object?> { ["Name"] = "Row1" });

        var result = await _service.ImportAsync(
            stream,
            new TestMapper(),
            (_, _, _, _) => Task.FromResult<string?>("Érvénytelen adat"),
            default);

        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(1, result.ErrorCount);
        Assert.Equal("Érvénytelen adat", result.Errors[0].ErrorMessage);
        Assert.Equal(2, result.Errors[0].RowNumber);
    }

    [Fact]
    public async Task ImportAsync_ProcessRowThrows_CapturesExceptionMessage()
    {
        using var stream = CreateExcelStream(new Dictionary<string, object?> { ["Name"] = "Row1" });

        var result = await _service.ImportAsync(
            stream,
            new TestMapper(),
            (_, _, _, _) => throw new InvalidOperationException("Váratlan hiba"),
            default);

        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(1, result.ErrorCount);
        Assert.Equal("Váratlan hiba", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task ImportAsync_MixedRows_ReturnsCorrectCounts()
    {
        using var stream = CreateExcelStream(
            new Dictionary<string, object?> { ["Name"] = "Valid" },
            new Dictionary<string, object?> { ["Name"] = "Error" },
            new Dictionary<string, object?> { ["Name"] = "Valid2" });

        var result = await _service.ImportAsync(
            stream,
            new TestMapper(),
            (req, _, _, _) => Task.FromResult<string?>(req.Name == "Error" ? "Bad row" : null),
            default);

        Assert.Equal(2, result.SuccessCount);
        Assert.Equal(1, result.ErrorCount);
    }

    [Fact]
    public async Task ImportAsync_PassesOriginalRowToCallback()
    {
        IDictionary<string, object>? capturedRow = null;
        using var stream = CreateExcelStream(new Dictionary<string, object?> { ["Name"] = "TestRow" });

        await _service.ImportAsync(
            stream,
            new TestMapper(),
            (_, row, _, _) => { capturedRow = row; return Task.FromResult<string?>(null); },
            default);

        Assert.NotNull(capturedRow);
        Assert.Equal("TestRow", capturedRow["Name"]?.ToString());
    }

    [Fact]
    public async Task ImportAsync_ErrorRow_ContainsOriginalRowData()
    {
        using var stream = CreateExcelStream(new Dictionary<string, object?> { ["Name"] = "TestRow" });

        var result = await _service.ImportAsync(
            stream,
            new TestMapper(),
            (_, _, _, _) => Task.FromResult<string?>("hiba"),
            default);

        Assert.True(result.Errors[0].OriginalRow.ContainsKey("Name"));
        Assert.Equal("TestRow", result.Errors[0].OriginalRow["Name"]?.ToString());
    }

    [Fact]
    public async Task ImportAsync_RowNumber_CorrectlyPassedToCallback()
    {
        var capturedRowNumbers = new List<int>();
        using var stream = CreateExcelStream(
            new Dictionary<string, object?> { ["Name"] = "Row1" },
            new Dictionary<string, object?> { ["Name"] = "Row2" });

        await _service.ImportAsync(
            stream,
            new TestMapper(),
            (_, _, rowNumber, _) => { capturedRowNumbers.Add(rowNumber); return Task.FromResult<string?>(null); },
            default);

        Assert.Equal([2, 3], capturedRowNumbers);
    }

    [Fact]
    public async Task ImportAsync_CorruptStream_ReturnsReadableError()
    {
        using var badStream = new MemoryStream([0x00, 0x01, 0x02]);

        var result = await _service.ImportAsync(
            badStream,
            new TestMapper(),
            (_, _, _, _) => Task.FromResult<string?>(null),
            default);

        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(1, result.ErrorCount);
        Assert.Contains("fájl nem olvasható", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public void GenerateErrorReport_EmptyErrors_ReturnsEmptyArray()
    {
        var result = _service.GenerateErrorReport([], ["Name"]);

        Assert.Empty(result);
    }

    [Fact]
    public void GenerateErrorReport_WithErrors_ReturnsNonEmptyBytes()
    {
        var errors = new List<ImportError>
        {
            new()
            {
                RowNumber = 2,
                OriginalRow = new Dictionary<string, object> { ["Name"] = "Hibás sor" },
                ErrorMessage = "Teszt hiba"
            }
        };

        var result = _service.GenerateErrorReport(errors, ["Name"]);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateTemplate_ReturnsNonEmptyBytes()
    {
        var result = _service.GenerateTemplate(["Szám", "Név", "Egységár"]);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateCombinedReport_BothEmpty_ReturnsEmptyArray()
    {
        var result = _service.GenerateCombinedReport([], [], ["Szám"]);

        Assert.Empty(result);
    }

    [Fact]
    public void GenerateCombinedReport_OnlyWarnings_ReturnsNonEmptyBytes()
    {
        var warnings = new List<ImportWarning>
        {
            new()
            {
                RowNumber = 2,
                OriginalRow = new Dictionary<string, object> { ["Szám"] = "P001" },
                Details = "Név: 'Régi' → 'Új'"
            }
        };

        var result = _service.GenerateCombinedReport([], warnings, ["Szám"]);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateCombinedReport_OnlyErrors_ReturnsNonEmptyBytes()
    {
        var errors = new List<ImportError>
        {
            new()
            {
                RowNumber = 2,
                OriginalRow = new Dictionary<string, object> { ["Szám"] = "P001" },
                ErrorMessage = "Hiányzó kötelező mező"
            }
        };

        var result = _service.GenerateCombinedReport(errors, [], ["Szám"]);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateCombinedReport_MixedErrorsAndWarnings_ReturnsNonEmptyBytes()
    {
        var errors = new List<ImportError>
        {
            new() { RowNumber = 2, OriginalRow = new Dictionary<string, object> { ["Szám"] = "P001" }, ErrorMessage = "Hiba" }
        };
        var warnings = new List<ImportWarning>
        {
            new() { RowNumber = 3, OriginalRow = new Dictionary<string, object> { ["Szám"] = "P002" }, Details = "Felülírás részletei" }
        };

        var result = _service.GenerateCombinedReport(errors, warnings, ["Szám"]);

        Assert.NotEmpty(result);
    }

    // ImportWithValidationFirstAsync tests

    [Fact]
    public async Task ImportWithValidationFirstAsync_AllValid_RunsProcessAndReturnsSuccess()
    {
        using var stream = CreateExcelStream(
            new Dictionary<string, object?> { ["Name"] = "Row1" },
            new Dictionary<string, object?> { ["Name"] = "Row2" });

        var result = await _service.ImportWithValidationFirstAsync(
            stream,
            new TestMapper(),
            (_, _, _, _) => Task.FromResult<string?>(null),
            (_, _, _, _) => Task.FromResult<string?>(null),
            default);

        Assert.Equal(2, result.SuccessCount);
        Assert.Equal(0, result.ErrorCount);
    }

    [Fact]
    public async Task ImportWithValidationFirstAsync_ValidationFails_ProcessNotCalled()
    {
        using var stream = CreateExcelStream(
            new Dictionary<string, object?> { ["Name"] = "Valid" },
            new Dictionary<string, object?> { ["Name"] = "Invalid" });

        bool processCalled = false;

        var result = await _service.ImportWithValidationFirstAsync(
            stream,
            new TestMapper(),
            (req, _, _, _) => Task.FromResult<string?>(req.Name == "Invalid" ? "Validation failed" : null),
            (_, _, _, _) => { processCalled = true; return Task.FromResult<string?>(null); },
            default);

        Assert.False(processCalled);
        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(1, result.ErrorCount);
        Assert.Equal("Validation failed", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task ImportWithValidationFirstAsync_AllValidationPass_AllProcessed()
    {
        using var stream = CreateExcelStream(
            new Dictionary<string, object?> { ["Name"] = "A" },
            new Dictionary<string, object?> { ["Name"] = "B" },
            new Dictionary<string, object?> { ["Name"] = "C" });

        var processed = new List<string>();

        var result = await _service.ImportWithValidationFirstAsync(
            stream,
            new TestMapper(),
            (_, _, _, _) => Task.FromResult<string?>(null),
            (req, _, _, _) => { processed.Add(req.Name); return Task.FromResult<string?>(null); },
            default);

        Assert.Equal(3, result.SuccessCount);
        Assert.Equal(0, result.ErrorCount);
        Assert.Equal(["A", "B", "C"], processed);
    }

    [Fact]
    public async Task ImportWithValidationFirstAsync_ValidationError_ReturnsErrors_WithOriginalRow()
    {
        using var stream = CreateExcelStream(new Dictionary<string, object?> { ["Name"] = "Bad" });

        var result = await _service.ImportWithValidationFirstAsync(
            stream,
            new TestMapper(),
            (_, _, _, _) => Task.FromResult<string?>("bad data"),
            (_, _, _, _) => Task.FromResult<string?>(null),
            default);

        Assert.True(result.HasErrors);
        Assert.Equal("bad data", result.Errors[0].ErrorMessage);
        Assert.Equal("Bad", result.Errors[0].OriginalRow["Name"]?.ToString());
    }

    [Fact]
    public async Task ImportWithValidationFirstAsync_CorruptStream_ReturnsReadableError()
    {
        using var badStream = new MemoryStream([0x00, 0x01, 0x02]);

        var result = await _service.ImportWithValidationFirstAsync(
            badStream,
            new TestMapper(),
            (_, _, _, _) => Task.FromResult<string?>(null),
            (_, _, _, _) => Task.FromResult<string?>(null),
            default);

        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(1, result.ErrorCount);
        Assert.Contains("fájl nem olvasható", result.Errors[0].ErrorMessage);
    }

    private record TestRequest(string Name);

    private class TestMapper : IExcelRowMapper<TestRequest>
    {
        public string[] TemplateHeaders => ["Name"];

        public Task<TestRequest> MapAsync(IDictionary<string, object> row, CancellationToken ct) =>
            Task.FromResult(new TestRequest(
                row.TryGetValue("Name", out var val) ? val?.ToString() ?? "" : ""));
    }
}
