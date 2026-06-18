namespace Application.Common.Services.ExcelImport;

public interface IExcelRowMapper<TRequest>
{
    string[] TemplateHeaders { get; }
    Task<TRequest> MapAsync(IDictionary<string, object> row, CancellationToken cancellationToken);
}
