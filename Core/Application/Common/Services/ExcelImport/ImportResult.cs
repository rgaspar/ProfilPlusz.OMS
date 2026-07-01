namespace Application.Common.Services.ExcelImport;

public class ImportError
{
    public int RowNumber { get; init; }
    public IDictionary<string, object> OriginalRow { get; init; } = new Dictionary<string, object>();
    public string ErrorMessage { get; init; } = string.Empty;
}

public class ImportWarning
{
    public int RowNumber { get; init; }
    public IDictionary<string, object> OriginalRow { get; init; } = new Dictionary<string, object>();
    public string Details { get; init; } = string.Empty;
}

public class ImportResult
{
    public int SuccessCount { get; init; }
    public int ErrorCount { get; init; }
    public List<ImportError> Errors { get; init; } = [];
    public bool HasErrors => ErrorCount > 0;
}
