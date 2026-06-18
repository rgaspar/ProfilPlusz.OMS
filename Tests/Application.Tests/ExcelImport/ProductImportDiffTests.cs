using Application.Features.ProductManager.Commands;
using Domain.Entities;
using Domain.Enums;

namespace Application.Tests.ExcelImport;

public class ProductImportDiffTests
{
    private static Dictionary<string, object> Row(params (string key, string val)[] pairs) =>
        pairs.ToDictionary(p => p.key, p => (object)p.val);

    private static Dictionary<string, string> EmptyNames() => [];

    [Fact]
    public void BuildDiff_NoChanges_ReturnsNincsValtozas()
    {
        var existing = new Product { Name = "Termék", UnitPrice = 100, Status = ProductStatus.Active };
        var req = new CreateProductRequest { Name = "Termék", UnitPrice = 100, Status = ProductStatus.Active };
        var row = Row(("Mértékegység", ""), ("Termékcsoport", ""));

        var diff = ImportProductsFromExcelHandler.BuildDiff(existing, req, row, EmptyNames(), EmptyNames());

        Assert.Equal("Nincs változás", diff);
    }

    [Fact]
    public void BuildDiff_NameChanged_IncludesNameChange()
    {
        var existing = new Product { Name = "Régi név" };
        var req = new CreateProductRequest { Name = "Új név" };
        var row = Row(("Mértékegység", ""), ("Termékcsoport", ""));

        var diff = ImportProductsFromExcelHandler.BuildDiff(existing, req, row, EmptyNames(), EmptyNames());

        Assert.Contains("Név", diff);
        Assert.Contains("Régi név", diff);
        Assert.Contains("Új név", diff);
    }

    [Fact]
    public void BuildDiff_UnitPriceChanged_IncludesPriceChange()
    {
        var existing = new Product { UnitPrice = 100.0 };
        var req = new CreateProductRequest { UnitPrice = 200.0 };
        var row = Row(("Mértékegység", ""), ("Termékcsoport", ""));

        var diff = ImportProductsFromExcelHandler.BuildDiff(existing, req, row, EmptyNames(), EmptyNames());

        Assert.Contains("Egységár", diff);
        Assert.Contains("100", diff);
        Assert.Contains("200", diff);
    }

    [Fact]
    public void BuildDiff_SamePriceFloatingPoint_NotIncluded()
    {
        var existing = new Product { UnitPrice = 100.0 };
        var req = new CreateProductRequest { UnitPrice = 100.0 };
        var row = Row(("Mértékegység", ""), ("Termékcsoport", ""));

        var diff = ImportProductsFromExcelHandler.BuildDiff(existing, req, row, EmptyNames(), EmptyNames());

        Assert.DoesNotContain("Egységár", diff);
    }

    [Fact]
    public void BuildDiff_StatusChanged_IncludesStatusChange()
    {
        var existing = new Product { Status = ProductStatus.Active };
        var req = new CreateProductRequest { Status = ProductStatus.Blocked };
        var row = Row(("Mértékegység", ""), ("Termékcsoport", ""));

        var diff = ImportProductsFromExcelHandler.BuildDiff(existing, req, row, EmptyNames(), EmptyNames());

        Assert.Contains("Státusz", diff);
        Assert.Contains("Active", diff);
        Assert.Contains("Blocked", diff);
    }

    [Fact]
    public void BuildDiff_UnitMeasureChanged_ShowsNameNotId()
    {
        var existing = new Product { UnitMeasureId = "um-guid-1" };
        var req = new CreateProductRequest { UnitMeasureId = "um-guid-2" };
        var unitMeasureNamesById = new Dictionary<string, string> { ["um-guid-1"] = "Darab" };
        var row = Row(("Mértékegység", "Kilogramm"), ("Termékcsoport", ""));

        var diff = ImportProductsFromExcelHandler.BuildDiff(existing, req, row, unitMeasureNamesById, EmptyNames());

        Assert.Contains("Mértékegység", diff);
        Assert.Contains("Darab", diff);
        Assert.Contains("Kilogramm", diff);
    }

    [Fact]
    public void BuildDiff_ProductGroupChanged_ShowsNameNotId()
    {
        var existing = new Product { ProductGroupId = "pg-guid-1" };
        var req = new CreateProductRequest { ProductGroupId = "pg-guid-2" };
        var productGroupNamesById = new Dictionary<string, string> { ["pg-guid-1"] = "Elektronika" };
        var row = Row(("Mértékegység", ""), ("Termékcsoport", "Bútor"));

        var diff = ImportProductsFromExcelHandler.BuildDiff(existing, req, row, EmptyNames(), productGroupNamesById);

        Assert.Contains("Termékcsoport", diff);
        Assert.Contains("Elektronika", diff);
        Assert.Contains("Bútor", diff);
    }

    [Fact]
    public void BuildDiff_MultipleChanges_AllIncludedSeparatedBySemicolon()
    {
        var existing = new Product { Name = "Old", UnitPrice = 100, Status = ProductStatus.Active };
        var req = new CreateProductRequest { Name = "New", UnitPrice = 200, Status = ProductStatus.Blocked };
        var row = Row(("Mértékegység", ""), ("Termékcsoport", ""));

        var diff = ImportProductsFromExcelHandler.BuildDiff(existing, req, row, EmptyNames(), EmptyNames());

        Assert.Contains("Név", diff);
        Assert.Contains("Egységár", diff);
        Assert.Contains("Státusz", diff);
        Assert.Contains("; ", diff);
    }

    [Fact]
    public void BuildDiff_PhysicalChangedFromTrueToFalse_Included()
    {
        var existing = new Product { Physical = true };
        var req = new CreateProductRequest { Physical = false };
        var row = Row(("Mértékegység", ""), ("Termékcsoport", ""));

        var diff = ImportProductsFromExcelHandler.BuildDiff(existing, req, row, EmptyNames(), EmptyNames());

        Assert.Contains("Fizikai", diff);
    }

    [Fact]
    public void BuildDiff_CurrencyChanged_Included()
    {
        var existing = new Product { PurchaseCurrency = Currency.HUF };
        var req = new CreateProductRequest { PurchaseCurrency = Currency.EUR };
        var row = Row(("Mértékegység", ""), ("Termékcsoport", ""));

        var diff = ImportProductsFromExcelHandler.BuildDiff(existing, req, row, EmptyNames(), EmptyNames());

        Assert.Contains("BeszerzésiPénznem", diff);
        Assert.Contains("HUF", diff);
        Assert.Contains("EUR", diff);
    }

    [Fact]
    public void BuildDiff_UnknownUnitMeasureId_FallsBackToId()
    {
        var existing = new Product { UnitMeasureId = "unknown-guid" };
        var req = new CreateProductRequest { UnitMeasureId = "other-guid" };
        var row = Row(("Mértékegység", "Valami"), ("Termékcsoport", ""));

        var diff = ImportProductsFromExcelHandler.BuildDiff(existing, req, row, EmptyNames(), EmptyNames());

        Assert.Contains("Mértékegység", diff);
        Assert.Contains("unknown-guid", diff);
    }
}
