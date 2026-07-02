using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;

namespace Application.Features.ProductManager.Commands;

public class UpdateProductResult
{
    public Product? Data { get; set; }
}

public class UpdateProductRequest : IRequest<UpdateProductResult>
{
    public string? Id { get; init; }

    public string? Number { get; init; }
    public string? Name { get; init; }
    public string? FactoryName { get; init; }
    public string? Description { get; init; }

    public decimal? UnitPrice { get; init; }
    public bool? Physical { get; init; }

    public string? UnitMeasureId { get; init; }
    public string? ProductGroupId { get; init; }

    public string? Manufacturer { get; init; }
    public string? ManufacturerNumber { get; init; }
    public string? Ean { get; init; }

    public string? BrandId { get; init; }
    public string? ColorId { get; init; }

    public decimal? SalesUnitQuantity { get; init; }
    public decimal? MinimumSalesQuantity { get; init; }
    public decimal? OrderQuantityStep { get; init; }
    public int? PackageQuantity { get; init; }

    public bool IsStockProduct { get; init; }
    public int? WarningStock { get; init; }
    public int? MinimumStock { get; init; }

    public bool HasSerialNumber { get; init; }

    public decimal? Weight { get; init; }
    public decimal? Length { get; init; }

    public string? Image1Url { get; init; }
    public string? Image2Url { get; init; }
    public string? Image3Url { get; init; }

    public string? VideoUrl { get; init; }
    public string? PdfUrl { get; init; }

    public Currency PurchaseCurrency { get; init; }
    public Currency SalesCurrency { get; init; }

    public ProductStatus Status { get; init; }

    public string? UpdatedById { get; init; }
}

public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Number)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.UnitMeasureId)
            .NotEmpty();

        RuleFor(x => x.ProductGroupId)
            .NotEmpty();

        RuleFor(x => x.PurchaseCurrency)
            .IsInEnum();

        RuleFor(x => x.SalesCurrency)
            .IsInEnum();

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Az egységárnak nullánál nagyobbnak kell lennie.");

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Ean)
            .MaximumLength(50);
    }
}

public class UpdateProductHandler : IRequestHandler<UpdateProductRequest, UpdateProductResult>
{
    private readonly ICommandRepository<Product> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductHandler(
        ICommandRepository<Product> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateProductResult> Handle(
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(
            request.Id ?? string.Empty,
            cancellationToken);

        if (entity == null)
        {
            throw new KeyNotFoundException(
                $"Product not found: {request.Id}");
        }

        entity.UpdatedById = request.UpdatedById;

        entity.Number = request.Number;
        entity.Name = request.Name;        

        entity.FactoryName = request.FactoryName;
        entity.Description = request.Description;

        entity.UnitPrice = request.UnitPrice;
        entity.Physical = request.Physical ?? true;

        entity.UnitMeasureId = request.UnitMeasureId;
        entity.ProductGroupId = request.ProductGroupId;

        entity.Manufacturer = request.Manufacturer;
        entity.ManufacturerNumber = request.ManufacturerNumber;
        entity.Ean = request.Ean;

        entity.BrandId = request.BrandId;
        entity.ColorId = request.ColorId;

        entity.SalesUnitQuantity = request.SalesUnitQuantity;
        entity.MinimumSalesQuantity = request.MinimumSalesQuantity;
        entity.OrderQuantityStep = request.OrderQuantityStep;
        entity.PackageQuantity = request.PackageQuantity;

        entity.IsStockProduct = request.IsStockProduct;
        entity.WarningStock = request.WarningStock;
        entity.MinimumStock = request.MinimumStock;

        entity.HasSerialNumber = request.HasSerialNumber;

        entity.Weight = request.Weight;
        entity.Length = request.Length;

        entity.Image1Url = request.Image1Url;
        entity.Image2Url = request.Image2Url;
        entity.Image3Url = request.Image3Url;

        entity.VideoUrl = request.VideoUrl;
        entity.PdfUrl = request.PdfUrl;

        entity.PurchaseCurrency = request.PurchaseCurrency;
        entity.SalesCurrency = request.SalesCurrency;

        entity.Status = request.Status;

        _repository.Update(entity);

        await _unitOfWork.SaveAsync(cancellationToken);

        return new UpdateProductResult
        {
            Data = entity
        };
    }
}

