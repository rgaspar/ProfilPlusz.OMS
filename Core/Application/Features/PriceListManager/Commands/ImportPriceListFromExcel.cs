using Application.Common.Repositories;
using Application.Common.Services.ExcelImport;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.PriceListManager.Commands;

public class ImportPriceListFromExcelErrorDto
{
    public int RowNumber { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
}

public class ImportPriceListFromExcelResult
{
    public int SuccessCount { get; init; }
    public int ErrorCount { get; init; }
    public int OverwriteCount { get; init; }
    public List<ImportPriceListFromExcelErrorDto> Errors { get; init; } = [];
}

public class ImportPriceListFromExcelRequest : IRequest<ImportPriceListFromExcelResult>
{
    public required Stream ExcelStream { get; init; }
    public string? CreatedById { get; init; }
}

public class ImportPriceListFromExcelHandler(
    ICommandRepository<PriceList> repository,
    IEntityDbSet db,
    IUnitOfWork unitOfWork,
    ExcelImportService excelImportService,
    IValidator<CreatePriceListRequest> validator
) : IRequestHandler<ImportPriceListFromExcelRequest, ImportPriceListFromExcelResult>
{
    public async Task<ImportPriceListFromExcelResult> Handle(
        ImportPriceListFromExcelRequest request,
        CancellationToken cancellationToken)
    {
        var products = await db.Product
            .AsNoTracking()
            .Where(p => p.Number != null)
            .ToDictionaryAsync(p => p.Number!, p => p.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var customers = await db.Customer
            .AsNoTracking()
            .ToDictionaryAsync(c => c.Name ?? string.Empty, c => c.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var taxes = await db.Tax
            .AsNoTracking()
            .ToDictionaryAsync(t => t.Name ?? string.Empty, t => t.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var existingEntries = await db.PriceList
            .AsNoTracking()
            .Where(pl => !pl.IsDeleted)
            .ToListAsync(cancellationToken);

        var mapper = new PriceListExcelRowMapper(products, customers, taxes);

        var result = await excelImportService.ImportAsync<CreatePriceListRequest>(
            request.ExcelStream,
            mapper,
            async (createRequest, _, _, ct) =>
            {
                var validation = await validator.ValidateAsync(createRequest, ct);
                if (!validation.IsValid)
                    return string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                var existing = existingEntries.FirstOrDefault(pl =>
                    pl.ProductId == createRequest.ProductId
                    && pl.CustomerId == createRequest.CustomerId
                    && pl.DiscountFrom == createRequest.DiscountFrom
                    && pl.DiscountTo == createRequest.DiscountTo
                    && pl.QuantityDiscount == createRequest.QuantityDiscount);

                if (existing is not null)
                {
                    var tracked = await repository.GetAsync(existing.Id, ct);
                    if (tracked is null)
                        return "Nem található a rekord frissítéshez.";

                    tracked.UpdatedById = request.CreatedById;
                    tracked.TaxId = createRequest.TaxId;
                    tracked.NetPrice = createRequest.NetPrice;
                    tracked.GrossPrice = createRequest.GrossPrice;

                    repository.Update(tracked);
                    await unitOfWork.SaveAsync(ct);
                    return null;
                }

                var entity = new PriceList
                {
                    CreatedById = request.CreatedById,
                    ProductId = createRequest.ProductId,
                    CustomerId = createRequest.CustomerId,
                    TaxId = createRequest.TaxId,
                    NetPrice = createRequest.NetPrice,
                    GrossPrice = createRequest.GrossPrice,
                    QuantityDiscount = createRequest.QuantityDiscount,
                    DiscountFrom = createRequest.DiscountFrom,
                    DiscountTo = createRequest.DiscountTo
                };

                await repository.CreateAsync(entity, ct);
                await unitOfWork.SaveAsync(ct);
                return null;
            },
            cancellationToken);

        return new ImportPriceListFromExcelResult
        {
            SuccessCount = result.SuccessCount,
            ErrorCount = result.ErrorCount,
            OverwriteCount = 0,
            Errors = result.Errors
                .Select(e => new ImportPriceListFromExcelErrorDto { RowNumber = e.RowNumber, ErrorMessage = e.ErrorMessage })
                .ToList()
        };
    }
}

internal sealed class PriceListExcelRowMapper : IExcelRowMapper<CreatePriceListRequest>
{
    private readonly Dictionary<string, string> _products;
    private readonly Dictionary<string, string> _customers;
    private readonly Dictionary<string, string> _taxes;

    public string[] TemplateHeaders =>
    [
        "Árucikk szám", "Vevőkód", "ÁFA", "Nettó ár", "Bruttó ár",
        "Mennyiségi kedvezmény", "Kedvezménytől", "Kedvezményig"
    ];

    public PriceListExcelRowMapper(
        Dictionary<string, string> products,
        Dictionary<string, string> customers,
        Dictionary<string, string> taxes)
    {
        _products = products;
        _customers = customers;
        _taxes = taxes;
    }

    public Task<CreatePriceListRequest> MapAsync(IDictionary<string, object> row, CancellationToken cancellationToken)
    {
        var productNumber = Get(row, "Árucikk szám");
        var customerCode = GetOrNull(row, "Vevőkód");
        var taxName = GetOrNull(row, "ÁFA");

        _products.TryGetValue(productNumber, out var productId);
        var customerId = customerCode is not null && _customers.TryGetValue(customerCode, out var cid) ? cid : null;
        var taxId = taxName is not null && _taxes.TryGetValue(taxName, out var tid) ? tid : null;

        var request = new CreatePriceListRequest
        {
            ProductId = productId,
            CustomerId = customerId,
            TaxId = taxId,
            NetPrice = GetDouble(row, "Nettó ár") ?? 0,
            GrossPrice = GetDouble(row, "Bruttó ár"),
            QuantityDiscount = GetDecimal(row, "Mennyiségi kedvezmény"),
            DiscountFrom = GetDate(row, "Kedvezménytől"),
            DiscountTo = GetDate(row, "Kedvezményig")
        };

        return Task.FromResult(request);
    }

    private static string Get(IDictionary<string, object> row, string key) =>
        row.TryGetValue(key, out var val) ? val?.ToString() ?? string.Empty : string.Empty;

    private static string? GetOrNull(IDictionary<string, object> row, string key) =>
        row.TryGetValue(key, out var val) ? val?.ToString() : null;

    private static double? GetDouble(IDictionary<string, object> row, string key) =>
        row.TryGetValue(key, out var val) && double.TryParse(val?.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : null;

    private static decimal? GetDecimal(IDictionary<string, object> row, string key) =>
        row.TryGetValue(key, out var val) && decimal.TryParse(val?.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : null;

    private static DateTime? GetDate(IDictionary<string, object> row, string key)
    {
        if (!row.TryGetValue(key, out var val)) return null;
        if (val is DateTime dt) return dt;
        return DateTime.TryParse(val?.ToString(), out var parsed) ? parsed : null;
    }
}
