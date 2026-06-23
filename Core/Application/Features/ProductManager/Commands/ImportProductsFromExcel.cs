using Application.Common.Repositories;
using Application.Common.Services.ExcelImport;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProductManager.Commands;

public class ImportProductsFromExcelErrorDto
{
    public int RowNumber { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
}

public class ImportProductsFromExcelResult
{
    public int SuccessCount { get; init; }
    public int ErrorCount { get; init; }
    public int OverwriteCount { get; init; }
    public List<ImportProductsFromExcelErrorDto> Errors { get; init; } = [];
}

public class ImportProductsFromExcelRequest : IRequest<ImportProductsFromExcelResult>
{
    public required Stream ExcelStream { get; init; }
    public string? CreatedById { get; init; }
}

public class ImportProductsFromExcelHandler : IRequestHandler<ImportProductsFromExcelRequest, ImportProductsFromExcelResult>
{
    private readonly ICommandRepository<Product> _productRepository;
    private readonly IEntityDbSet _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ExcelImportService _excelImportService;
    private readonly IValidator<CreateProductRequest> _validator;

    public ImportProductsFromExcelHandler(
        ICommandRepository<Product> productRepository,
        IEntityDbSet db,
        IUnitOfWork unitOfWork,
        ExcelImportService excelImportService,
        IValidator<CreateProductRequest> validator)
    {
        _productRepository = productRepository;
        _db = db;
        _unitOfWork = unitOfWork;
        _excelImportService = excelImportService;
        _validator = validator;
    }

    public async Task<ImportProductsFromExcelResult> Handle(
        ImportProductsFromExcelRequest request,
        CancellationToken cancellationToken)
    {
        var unitMeasures = await _db.UnitMeasure
            .AsNoTracking()
            .ToDictionaryAsync(u => u.Name ?? string.Empty, u => u.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var productGroups = await _db.ProductGroup
            .AsNoTracking()
            .ToDictionaryAsync(g => g.Name ?? string.Empty, g => g.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var existingProductsByNumber = await _db.Product
            .AsNoTracking()
            .Where(p => p.Number != null)
            .ToDictionaryAsync(p => p.Number!, p => p.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var unitMeasureNamesById = unitMeasures.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        var productGroupNamesById = productGroups.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        var mapper = new ProductExcelRowMapper(unitMeasures, productGroups);
        var overwriteWarnings = new List<ImportWarning>();

        var result = await _excelImportService.ImportAsync<CreateProductRequest>(
            request.ExcelStream,
            mapper,
            async (createRequest, originalRow, rowNumber, ct) =>
            {
                var validation = await _validator.ValidateAsync(createRequest, ct);
                if (!validation.IsValid)
                    return string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));

                if (createRequest.Number is not null
                    && existingProductsByNumber.TryGetValue(createRequest.Number, out var existingId))
                {
                    var existing = await _db.Product
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == existingId, ct);

                    var tracked = await _productRepository.GetAsync(existingId, ct);
                    if (tracked is null)
                        return "Nem található a termék frissítéshez.";

                    var diff = BuildDiff(existing!, createRequest, originalRow, unitMeasureNamesById, productGroupNamesById);
                    UpdateEntity(tracked, createRequest, request.CreatedById);
                    _productRepository.Update(tracked);
                    await _unitOfWork.SaveAsync(ct);

                    overwriteWarnings.Add(new ImportWarning
                    {
                        RowNumber = rowNumber,
                        OriginalRow = originalRow,
                        Details = diff
                    });

                    return null;
                }

                var entity = MapToEntity(createRequest, request.CreatedById);
                await _productRepository.CreateAsync(entity, ct);
                await _unitOfWork.SaveAsync(ct);
                return null;
            },
            cancellationToken);

        return new ImportProductsFromExcelResult
        {
            SuccessCount = result.SuccessCount,
            ErrorCount = result.ErrorCount,
            OverwriteCount = overwriteWarnings.Count,
            Errors = result.Errors
                .Select(e => new ImportProductsFromExcelErrorDto { RowNumber = e.RowNumber, ErrorMessage = e.ErrorMessage })
                .ToList()
        };
    }

    internal static string BuildDiff(
        Product existing,
        CreateProductRequest req,
        IDictionary<string, object> originalRow,
        Dictionary<string, string> unitMeasureNamesById,
        Dictionary<string, string> productGroupNamesById)
    {
        var changes = new List<string>();

        void Compare(string label, object? oldVal, object? newVal)
        {
            if (oldVal is double oldD && newVal is double newD)
            {
                if (Math.Abs(oldD - newD) > 1e-10)
                    changes.Add($"{label}: '{oldD}' → '{newD}'");
                return;
            }
            var oldStr = oldVal?.ToString() ?? "";
            var newStr = newVal?.ToString() ?? "";
            if (!string.Equals(oldStr, newStr, StringComparison.OrdinalIgnoreCase))
                changes.Add($"{label}: '{oldStr}' → '{newStr}'");
        }

        Compare("Név", existing.Name, req.Name);
        Compare("GyárNeve", existing.FactoryName, req.FactoryName);
        Compare("Leírás", existing.Description, req.Description);
        Compare("Egységár", existing.UnitPrice, req.UnitPrice);
        Compare("Fizikai", existing.Physical, req.Physical ?? true);

        var oldUnitMeasureName = existing.UnitMeasureId is not null
            && unitMeasureNamesById.TryGetValue(existing.UnitMeasureId, out var uName) ? uName : existing.UnitMeasureId ?? "";
        var newUnitMeasureName = originalRow.TryGetValue("Mértékegység", out var umVal) ? umVal?.ToString() ?? "" : "";
        Compare("Mértékegység", oldUnitMeasureName, newUnitMeasureName);

        var oldProductGroupName = existing.ProductGroupId is not null
            && productGroupNamesById.TryGetValue(existing.ProductGroupId, out var pgName) ? pgName : existing.ProductGroupId ?? "";
        var newProductGroupName = originalRow.TryGetValue("Termékcsoport", out var pgVal) ? pgVal?.ToString() ?? "" : "";
        Compare("Termékcsoport", oldProductGroupName, newProductGroupName);

        Compare("Gyártó", existing.Manufacturer, req.Manufacturer);
        Compare("GyártóiSzám", existing.ManufacturerNumber, req.ManufacturerNumber);
        Compare("EAN", existing.Ean, req.Ean);
        Compare("BeszerzésiPénznem", existing.PurchaseCurrency, req.PurchaseCurrency);
        Compare("ÉrtékesítésiPénznem", existing.SalesCurrency, req.SalesCurrency);
        Compare("Státusz", existing.Status, req.Status);

        return changes.Count > 0 ? string.Join("; ", changes) : "Nincs változás";
    }

    private static void UpdateEntity(Product entity, CreateProductRequest req, string? updatedById)
    {
        entity.UpdatedById = updatedById;
        entity.Name = req.Name;
        entity.FactoryName = req.FactoryName;
        entity.Description = req.Description;
        entity.UnitPrice = req.UnitPrice;
        entity.Physical = req.Physical ?? true;
        entity.UnitMeasureId = req.UnitMeasureId;
        entity.ProductGroupId = req.ProductGroupId;
        entity.Manufacturer = req.Manufacturer;
        entity.ManufacturerNumber = req.ManufacturerNumber;
        entity.Ean = req.Ean;
        entity.PurchaseCurrency = req.PurchaseCurrency;
        entity.SalesCurrency = req.SalesCurrency;
        entity.Status = req.Status;
    }

    private static Product MapToEntity(CreateProductRequest req, string? createdById) => new()
    {
        CreatedById = createdById,
        Number = req.Number,
        Name = req.Name,
        FactoryName = req.FactoryName,
        Description = req.Description,
        UnitPrice = req.UnitPrice,
        Physical = req.Physical ?? true,
        UnitMeasureId = req.UnitMeasureId,
        ProductGroupId = req.ProductGroupId,
        Manufacturer = req.Manufacturer,
        ManufacturerNumber = req.ManufacturerNumber,
        Ean = req.Ean,
        BrandId = req.BrandId,
        ColorId = req.ColorId,
        SalesUnitQuantity = req.SalesUnitQuantity,
        MinimumSalesQuantity = req.MinimumSalesQuantity,
        OrderQuantityStep = req.OrderQuantityStep,
        PackageQuantity = req.PackageQuantity,
        IsStockProduct = req.IsStockProduct,
        WarningStock = req.WarningStock,
        MinimumStock = req.MinimumStock,
        HasSerialNumber = req.HasSerialNumber,
        Weight = req.Weight,
        Image1Url = req.Image1Url,
        Image2Url = req.Image2Url,
        Image3Url = req.Image3Url,
        VideoUrl = req.VideoUrl,
        PdfUrl = req.PdfUrl,
        PurchaseCurrency = req.PurchaseCurrency,
        SalesCurrency = req.SalesCurrency,
        Status = req.Status
    };
}

internal sealed class ProductExcelRowMapper : IExcelRowMapper<CreateProductRequest>
{
    private readonly Dictionary<string, string> _unitMeasures;
    private readonly Dictionary<string, string> _productGroups;

    public string[] TemplateHeaders =>
    [
        "Szám", "Név", "GyárNeve", "Leírás",
        "Egységár", "Mértékegység", "Termékcsoport", "Fizikai",
        "Gyártó", "GyártóiSzám", "EAN",
        "BeszerzésiPénznem", "ÉrtékesítésiPénznem", "Státusz"
    ];

    public ProductExcelRowMapper(
        Dictionary<string, string> unitMeasures,
        Dictionary<string, string> productGroups)
    {
        _unitMeasures = unitMeasures;
        _productGroups = productGroups;
    }

    public Task<CreateProductRequest> MapAsync(IDictionary<string, object> row, CancellationToken cancellationToken)
    {
        var unitMeasureName = Get(row, "Mértékegység");
        var productGroupName = Get(row, "Termékcsoport");

        _unitMeasures.TryGetValue(unitMeasureName, out var unitMeasureId);
        _productGroups.TryGetValue(productGroupName, out var productGroupId);

        var request = new CreateProductRequest
        {
            Number = Get(row, "Szám"),
            Name = Get(row, "Név"),
            FactoryName = GetOrNull(row, "GyárNeve"),
            Description = GetOrNull(row, "Leírás"),
            UnitPrice = GetDouble(row, "Egységár"),
            UnitMeasureId = unitMeasureId,
            ProductGroupId = productGroupId,
            Physical = GetBool(row, "Fizikai") ?? true,
            Manufacturer = GetOrNull(row, "Gyártó"),
            ManufacturerNumber = GetOrNull(row, "GyártóiSzám"),
            Ean = GetOrNull(row, "EAN"),
            PurchaseCurrency = ParseEnum<Currency>(Get(row, "BeszerzésiPénznem"), Currency.HUF),
            SalesCurrency = ParseEnum<Currency>(Get(row, "ÉrtékesítésiPénznem"), Currency.HUF),
            Status = ParseEnum<ProductStatus>(Get(row, "Státusz"), ProductStatus.Active)
        };

        return Task.FromResult(request);
    }

    private static string Get(IDictionary<string, object> row, string key) =>
        row.TryGetValue(key, out var val) ? val?.ToString() ?? string.Empty : string.Empty;

    private static string? GetOrNull(IDictionary<string, object> row, string key) =>
        row.TryGetValue(key, out var val) ? val?.ToString() : null;

    private static double? GetDouble(IDictionary<string, object> row, string key) =>
        row.TryGetValue(key, out var val) && double.TryParse(val?.ToString(), out var d) ? d : null;

    private static bool? GetBool(IDictionary<string, object> row, string key)
    {
        if (!row.TryGetValue(key, out var val)) return null;
        var s = val?.ToString()?.ToLowerInvariant();
        return s is "igen" or "true" or "1" or "yes";
    }

    private static T ParseEnum<T>(string value, T defaultValue) where T : struct, Enum =>
        Enum.TryParse<T>(value, ignoreCase: true, out var result) ? result : defaultValue;
}
