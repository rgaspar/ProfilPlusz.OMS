using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SeedManager.Demos;

public class ProductSeeder
{
    private readonly ICommandRepository<Product> _productRepository;
    private readonly ICommandRepository<ProductGroup> _productGroupRepository;
    private readonly ICommandRepository<UnitMeasure> _unitMeasureRepository;
    private readonly ICommandRepository<Brand> _brandRepository;
    private readonly ICommandRepository<Color> _colorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductSeeder(
        ICommandRepository<Product> productRepository,
        ICommandRepository<ProductGroup> productGroupRepository,
        ICommandRepository<UnitMeasure> unitMeasureRepository,
        ICommandRepository<Brand> brandRepository,
        ICommandRepository<Color> colorRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _productGroupRepository = productGroupRepository;
        _unitMeasureRepository = unitMeasureRepository;
        _brandRepository = brandRepository;
        _colorRepository = colorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task GenerateDataAsync()
    {
        var unitMeasureId = await _unitMeasureRepository
            .GetQuery()
            .Where(x => x.Name == "unit")
            .Select(x => x.Id)
            .FirstAsync();


        var groups = await _productGroupRepository
            .GetQuery()
            .ToDictionaryAsync(x => x.Name!, x => x.Id!);


        var brands = await EnsureBrandsAsync(new[]
        {
            "Progress",
            "Global",
            "Faber"
        });

        var colors = await EnsureColorsAsync(new[]
        {
            "Cream",
            "Fekete",
            "Kék",
            "Átlátszó",
            "Rozsdamentes"
        });


        var products = new List<Product>
        {
            new Product
            {
                Number = "CPTA 10-SL02",
                Name = "Progress Proterminal Stone Line Cream Alumínium L Profil Sarokelem 10 mm",
                FactoryName = "Stone Line Cream Corner",

                SalesUnitQuantity = 1,
                MinimumSalesQuantity = 1,
                OrderQuantityStep = 10,
                PackageQuantity = 10,

                IsStockProduct = true,
                WarningStock = 20,
                MinimumStock = 15,

                HasSerialNumber = false,

                ProductGroupId = groups["Sarokelem"],
                Manufacturer = "Progress",
                BrandId = brands["Progress"],
                ManufacturerNumber = "CPTA 10-SL02",

                ColorId = colors["Cream"],

                Weight = 0.001m,

                PurchaseCurrency = Currency.EUR,
                SalesCurrency = Currency.HUF,

                Status = ProductStatus.Active,

                UnitMeasureId = unitMeasureId
            },


            new Product
            {
                Number = "GBFLABL 10",
                Name = "L profil Matt Fekete Alumínium 10 mm (2,7m/szál)",
                FactoryName = "L black 10 mm",

                SalesUnitQuantity = 1,
                MinimumSalesQuantity = 1,
                OrderQuantityStep = 1,
                PackageQuantity = 100,

                Ean = "8031893354019",

                IsStockProduct = true,
                WarningStock = 60,
                MinimumStock = 50,

                ProductGroupId = groups["L profil"],
                Manufacturer = "Progress",
                BrandId = brands["Global"],
                ManufacturerNumber = "GBFLABL 10/",

                ColorId = colors["Fekete"],

                Weight = 0.03m,

                PurchaseCurrency = Currency.EUR,
                SalesCurrency = Currency.HUF,

                Status = ProductStatus.Active,

                UnitMeasureId = unitMeasureId
            },


            new Product
            {
                Number = "GBFLBA 10",
                Name = "L profil Matt Fekete Alumínium 10 mm (2,7m/szál)",
                FactoryName = "Glob. L matt 10 black",

                SalesUnitQuantity = 1,
                MinimumSalesQuantity = 1,
                OrderQuantityStep = 1,
                PackageQuantity = 100,

                Ean = "8031893236988",

                IsStockProduct = true,
                WarningStock = 60,
                MinimumStock = 50,

                ProductGroupId = groups["L profil"],
                Manufacturer = "Progress",
                BrandId = brands["Global"],
                ManufacturerNumber = "GBFLBA 10/",

                ColorId = colors["Fekete"],

                Weight = 0.03m,

                PurchaseCurrency = Currency.EUR,
                SalesCurrency = Currency.HUF,

                Status = ProductStatus.Blocked,

                UnitMeasureId = unitMeasureId
            },


            new Product
            {
                Number = "PDES 3530/EN",
                Name = "Progress Prodeso 3 rétegű vízszigetelő membrán 30 fm",
                FactoryName = "Prodeso",

                SalesUnitQuantity = 1,
                MinimumSalesQuantity = 1,
                OrderQuantityStep = 8,
                PackageQuantity = 8,

                Ean = "8031893203904",

                IsStockProduct = true,
                WarningStock = 10,
                MinimumStock = 8,

                ProductGroupId = groups["Membrán"],
                Manufacturer = "Progress",
                BrandId = brands["Progress"],
                ManufacturerNumber = "PDES 3530/EN",

                ColorId = colors["Kék"],

                Weight = 19.7m,

                PurchaseCurrency = Currency.EUR,
                SalesCurrency = Currency.HUF,

                Status = ProductStatus.Active,

                UnitMeasureId = unitMeasureId
            },


            new Product
            {
                Number = "SR0100003",
                Name = "Faber Cement Remover 1L",
                FactoryName = "Faber Cement Remover",

                SalesUnitQuantity = 1,
                MinimumSalesQuantity = 1,
                OrderQuantityStep = 12,
                PackageQuantity = 12,

                Ean = "8027365010323",

                IsStockProduct = true,
                WarningStock = 53,
                MinimumStock = 42,

                HasSerialNumber = true,

                ProductGroupId = groups["Tisztítószerek"],
                Manufacturer = "Faber",
                BrandId = brands["Faber"],
                ManufacturerNumber = "SR0100003",

                ColorId = colors["Átlátszó"],

                Weight = 1.002m,

                PurchaseCurrency = Currency.EUR,
                SalesCurrency = Currency.HUF,

                Status = ProductStatus.Discontinued,

                UnitMeasureId = unitMeasureId
            }
        };


        foreach (var product in products)
        {
            product.Physical = true;

            await _productRepository.CreateAsync(product);
        }

        await _unitOfWork.SaveAsync();
    }



    private async Task<Dictionary<string, string>> EnsureBrandsAsync(IEnumerable<string> names)
    {
        var existing = await _brandRepository
            .GetQuery()
            .ToDictionaryAsync(x => x.Name!, x => x.Id!);

        foreach (var name in names)
        {
            if (!existing.ContainsKey(name))
            {
                var entity = new Brand { Name = name };
                await _brandRepository.CreateAsync(entity);
                existing[name] = entity.Id!;
            }
        }

        return existing;
    }


    private async Task<Dictionary<string, string>> EnsureColorsAsync(IEnumerable<string> names)
    {
        var existing = await _colorRepository
            .GetQuery()
            .ToDictionaryAsync(x => x.Name!, x => x.Id!);

        foreach (var name in names)
        {
            if (!existing.ContainsKey(name))
            {
                var entity = new Color { Name = name };
                await _colorRepository.CreateAsync(entity);
                existing[name] = entity.Id!;
            }
        }

        return existing;
    }
}