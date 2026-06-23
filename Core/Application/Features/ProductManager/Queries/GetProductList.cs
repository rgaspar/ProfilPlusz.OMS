using Application.Common.CQS.Queries;
using Application.Common.Extensions;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProductManager.Queries;

public record GetProductListDto
{
    public string? Id { get; init; }

    public string? Number { get; init; }                 // Cikkszám
    public string? Name { get; init; }                   // Megnevezés
    public string? FactoryName { get; init; }            // Gyári megnevezés

    public string? Description { get; init; }

    public double? UnitPrice { get; init; }
    public bool? Physical { get; init; }

    // kategorizálás
    public string? ProductGroupId { get; init; }
    public string? ProductGroupName { get; init; }

    public string? BrandId { get; init; }
    public string? BrandName { get; init; }

    public string? ColorId { get; init; }
    public string? ColorName { get; init; }

    public string? Manufacturer { get; init; }
    public string? ManufacturerNumber { get; init; }

    public string? Ean { get; init; }

    // mennyiségek
    public decimal? SalesUnitQuantity { get; init; }
    public decimal? MinimumSalesQuantity { get; init; }
    public decimal? OrderQuantityStep { get; init; }
    public int? PackageQuantity { get; init; }

    // készlet
    public bool IsStockProduct { get; init; }
    public int? WarningStock { get; init; }
    public int? MinimumStock { get; init; }
    public bool HasSerialNumber { get; init; }

    // fizikai adatok
    public decimal? Weight { get; init; }

    // média
    public string? Image1Url { get; init; }
    public string? Image2Url { get; init; }
    public string? Image3Url { get; init; }
    public string? VideoUrl { get; init; }
    public string? PdfUrl { get; init; }

    // státusz
    public ProductStatus Status { get; init; }

    // pénznem
    public Currency PurchaseCurrency { get; init; }
    public Currency SalesCurrency { get; init; }

    // mértékegység
    public string? UnitMeasureId { get; init; }
    public string? UnitMeasureName { get; init; }

    // dátum
    public DateTime? CreatedAtUtc { get; init; }

    public bool HasPriceList { get; init; }
}

public class GetProductListProfile : Profile
{
    public GetProductListProfile()
    {
        CreateMap<Product, GetProductListDto>()

            .ForMember(
                dest => dest.UnitMeasureName,
                opt => opt.MapFrom(src =>
                    src.UnitMeasure != null
                        ? src.UnitMeasure.Name
                        : null))

            .ForMember(
                dest => dest.ProductGroupName,
                opt => opt.MapFrom(src =>
                    src.ProductGroup != null
                        ? src.ProductGroup.Name
                        : null))

            .ForMember(
                dest => dest.BrandName,
                opt => opt.MapFrom(src =>
                    src.Brand != null
                        ? src.Brand.Name
                        : null))

            .ForMember(
                dest => dest.ColorName,
                opt => opt.MapFrom(src =>
                    src.Color != null
                        ? src.Color.Name
                        : null))

            .ForMember(dest => dest.HasPriceList, opt => opt.Ignore());
    }
}

public class GetProductListResult
{
    public List<GetProductListDto>? Data { get; init; }
}

public class GetProductListRequest : IRequest<GetProductListResult>
{
    public bool IsDeleted { get; init; } = false;
}

public class GetProductListHandler : IRequestHandler<GetProductListRequest, GetProductListResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetProductListHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetProductListResult> Handle(
    GetProductListRequest request,
    CancellationToken cancellationToken)
    {
        var query = _context
            .Product
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)

            .Include(x => x.UnitMeasure)
            .Include(x => x.ProductGroup)
            .Include(x => x.Brand)
            .Include(x => x.Color)

            .AsQueryable();

        var entities = await query.ToListAsync(cancellationToken);

        var priceListProductIds = (await _context.PriceList
            .AsNoTracking()
            .Where(pl => !pl.IsDeleted && pl.ProductId != null)
            .Select(pl => pl.ProductId!)
            .ToListAsync(cancellationToken))
            .ToHashSet();

        var dtos = _mapper.Map<List<GetProductListDto>>(entities)
            .Select(dto => dto with { HasPriceList = priceListProductIds.Contains(dto.Id ?? string.Empty) })
            .ToList();

        return new GetProductListResult
        {
            Data = dtos
        };
    }
}