using Application.Features.ProductManager.Commands;
using Domain.Enums;

namespace Application.Tests.ExcelImport;

public class ProductExcelRowMapperTests
{
    private static Dictionary<string, object> Row(params (string key, string val)[] pairs) =>
        pairs.ToDictionary(p => p.key, p => (object)p.val);

    private static ProductExcelRowMapper CreateMapper(
        Dictionary<string, string>? unitMeasures = null,
        Dictionary<string, string>? productGroups = null) =>
        new(
            unitMeasures ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
            productGroups ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

    [Fact]
    public void TemplateHeaders_ContainsRequiredColumns()
    {
        var mapper = CreateMapper();

        Assert.Contains("Szám", mapper.TemplateHeaders);
        Assert.Contains("Név", mapper.TemplateHeaders);
        Assert.Contains("Mértékegység", mapper.TemplateHeaders);
        Assert.Contains("Termékcsoport", mapper.TemplateHeaders);
        Assert.Contains("BeszerzésiPénznem", mapper.TemplateHeaders);
        Assert.Contains("ÉrtékesítésiPénznem", mapper.TemplateHeaders);
        Assert.Contains("Státusz", mapper.TemplateHeaders);
    }

    [Fact]
    public async Task MapAsync_BasicScalarFields_MappedCorrectly()
    {
        var mapper = CreateMapper();
        var row = Row(
            ("Szám", "P001"),
            ("Név", "Teszt termék"),
            ("GyárNeve", "Gyár neve"),
            ("Leírás", "Leírás szöveg"),
            ("Egységár", "1500"),
            ("Mértékegység", ""),
            ("Termékcsoport", ""),
            ("Fizikai", "Igen"),
            ("Gyártó", "Gyártó Kft"),
            ("GyártóiSzám", "GM-001"),
            ("EAN", "1234567890123"),
            ("BeszerzésiPénznem", "EUR"),
            ("ÉrtékesítésiPénznem", "HUF"),
            ("Státusz", "Active"));

        var result = await mapper.MapAsync(row, default);

        Assert.Equal("P001", result.Number);
        Assert.Equal("Teszt termék", result.Name);
        Assert.Equal("Gyár neve", result.FactoryName);
        Assert.Equal("Leírás szöveg", result.Description);
        Assert.Equal(1500d, result.UnitPrice);
        Assert.Equal("Gyártó Kft", result.Manufacturer);
        Assert.Equal("GM-001", result.ManufacturerNumber);
        Assert.Equal("1234567890123", result.Ean);
    }

    [Fact]
    public async Task MapAsync_KnownUnitMeasureName_ResolvesToId()
    {
        var unitMeasures = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["Darab"] = "um-guid-1" };
        var mapper = CreateMapper(unitMeasures: unitMeasures);
        var row = Row(("Mértékegység", "Darab"), ("Szám", ""), ("Név", ""), ("Fizikai", ""), ("Egységár", ""), ("Termékcsoport", ""), ("BeszerzésiPénznem", ""), ("ÉrtékesítésiPénznem", ""), ("Státusz", ""));

        var result = await mapper.MapAsync(row, default);

        Assert.Equal("um-guid-1", result.UnitMeasureId);
    }

    [Fact]
    public async Task MapAsync_UnknownUnitMeasureName_ReturnsNullId()
    {
        var mapper = CreateMapper();
        var row = Row(("Mértékegység", "Ismeretlen"), ("Szám", ""), ("Név", ""), ("Fizikai", ""), ("Egységár", ""), ("Termékcsoport", ""), ("BeszerzésiPénznem", ""), ("ÉrtékesítésiPénznem", ""), ("Státusz", ""));

        var result = await mapper.MapAsync(row, default);

        Assert.Null(result.UnitMeasureId);
    }

    [Fact]
    public async Task MapAsync_KnownProductGroupName_ResolvesToId()
    {
        var productGroups = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["Elektronika"] = "pg-guid-1" };
        var mapper = CreateMapper(productGroups: productGroups);
        var row = Row(("Termékcsoport", "Elektronika"), ("Szám", ""), ("Név", ""), ("Fizikai", ""), ("Egységár", ""), ("Mértékegység", ""), ("BeszerzésiPénznem", ""), ("ÉrtékesítésiPénznem", ""), ("Státusz", ""));

        var result = await mapper.MapAsync(row, default);

        Assert.Equal("pg-guid-1", result.ProductGroupId);
    }

    [Fact]
    public async Task MapAsync_UnitMeasureNameCaseInsensitive_Resolves()
    {
        var unitMeasures = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["Darab"] = "um-guid-1" };
        var mapper = CreateMapper(unitMeasures: unitMeasures);
        var row = Row(("Mértékegység", "DARAB"), ("Szám", ""), ("Név", ""), ("Fizikai", ""), ("Egységár", ""), ("Termékcsoport", ""), ("BeszerzésiPénznem", ""), ("ÉrtékesítésiPénznem", ""), ("Státusz", ""));

        var result = await mapper.MapAsync(row, default);

        Assert.Equal("um-guid-1", result.UnitMeasureId);
    }

    [Fact]
    public async Task MapAsync_FizikaiIgen_ReturnsPhysicalTrue()
    {
        var mapper = CreateMapper();
        var row = Row(("Fizikai", "Igen"), ("Szám", ""), ("Név", ""), ("Mértékegység", ""), ("Termékcsoport", ""), ("Egységár", ""), ("BeszerzésiPénznem", ""), ("ÉrtékesítésiPénznem", ""), ("Státusz", ""));

        var result = await mapper.MapAsync(row, default);

        Assert.Equal(true, result.Physical);
    }

    [Fact]
    public async Task MapAsync_FizikaiNem_ReturnsPhysicalFalse()
    {
        var mapper = CreateMapper();
        var row = Row(("Fizikai", "nem"), ("Szám", ""), ("Név", ""), ("Mértékegység", ""), ("Termékcsoport", ""), ("Egységár", ""), ("BeszerzésiPénznem", ""), ("ÉrtékesítésiPénznem", ""), ("Státusz", ""));

        var result = await mapper.MapAsync(row, default);

        Assert.Equal(false, result.Physical);
    }

    [Fact]
    public async Task MapAsync_FizikaiMissing_DefaultsToTrue()
    {
        var mapper = CreateMapper();
        var row = Row(("Szám", ""), ("Név", ""), ("Mértékegység", ""), ("Termékcsoport", ""), ("Egységár", ""), ("BeszerzésiPénznem", ""), ("ÉrtékesítésiPénznem", ""), ("Státusz", ""));

        var result = await mapper.MapAsync(row, default);

        Assert.Equal(true, result.Physical);
    }

    [Fact]
    public async Task MapAsync_ValidCurrency_ParsedCorrectly()
    {
        var mapper = CreateMapper();
        var row = Row(("BeszerzésiPénznem", "EUR"), ("ÉrtékesítésiPénznem", "USD"), ("Szám", ""), ("Név", ""), ("Mértékegység", ""), ("Termékcsoport", ""), ("Fizikai", ""), ("Egységár", ""), ("Státusz", ""));

        var result = await mapper.MapAsync(row, default);

        Assert.Equal(Currency.EUR, result.PurchaseCurrency);
        Assert.Equal(Currency.USD, result.SalesCurrency);
    }

    [Fact]
    public async Task MapAsync_InvalidCurrency_UsesDefaultHuf()
    {
        var mapper = CreateMapper();
        var row = Row(("BeszerzésiPénznem", "XYZ"), ("ÉrtékesítésiPénznem", ""), ("Szám", ""), ("Név", ""), ("Mértékegység", ""), ("Termékcsoport", ""), ("Fizikai", ""), ("Egységár", ""), ("Státusz", ""));

        var result = await mapper.MapAsync(row, default);

        Assert.Equal(Currency.HUF, result.PurchaseCurrency);
    }

    [Fact]
    public async Task MapAsync_ValidStatus_ParsedCorrectly()
    {
        var mapper = CreateMapper();
        var row = Row(("Státusz", "Blocked"), ("Szám", ""), ("Név", ""), ("Mértékegység", ""), ("Termékcsoport", ""), ("Fizikai", ""), ("Egységár", ""), ("BeszerzésiPénznem", ""), ("ÉrtékesítésiPénznem", ""));

        var result = await mapper.MapAsync(row, default);

        Assert.Equal(ProductStatus.Blocked, result.Status);
    }

    [Fact]
    public async Task MapAsync_InvalidStatus_UsesDefaultActive()
    {
        var mapper = CreateMapper();
        var row = Row(("Státusz", "ErvenytelenStatusz"), ("Szám", ""), ("Név", ""), ("Mértékegység", ""), ("Termékcsoport", ""), ("Fizikai", ""), ("Egységár", ""), ("BeszerzésiPénznem", ""), ("ÉrtékesítésiPénznem", ""));

        var result = await mapper.MapAsync(row, default);

        Assert.Equal(ProductStatus.Active, result.Status);
    }

    [Fact]
    public async Task MapAsync_MissingOptionalFields_ReturnsNullForThem()
    {
        var mapper = CreateMapper();
        var row = Row(("Szám", ""), ("Név", ""), ("Mértékegység", ""), ("Termékcsoport", ""), ("Fizikai", ""), ("Egységár", ""), ("BeszerzésiPénznem", ""), ("ÉrtékesítésiPénznem", ""), ("Státusz", ""));

        var result = await mapper.MapAsync(row, default);

        Assert.Null(result.FactoryName);
        Assert.Null(result.Manufacturer);
        Assert.Null(result.ManufacturerNumber);
        Assert.Null(result.Ean);
        Assert.Null(result.Description);
    }
}
