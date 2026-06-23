using Application.Common.CQS.Queries;
using Application.Common.Extensions;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.PriceListManager.Queries;

public record GetPriceListDto
{
    public string? Id { get; init; }

    public string? ProductId { get; init; }
    public string? ProductNumber { get; init; }
    public string? ProductName { get; init; }

    public string? CustomerId { get; init; }
    public string? CustomerName { get; init; }

    public string? TaxId { get; init; }
    public string? TaxName { get; init; }
    public double? TaxPercentage { get; init; }

    public double NetPrice { get; init; }
    public double? GrossPrice { get; init; }

    public decimal? QuantityDiscount { get; init; }
    public DateTime? DiscountFrom { get; init; }
    public DateTime? DiscountTo { get; init; }

    public DateTime? CreatedAtUtc { get; init; }
}

public class GetPriceListProfile : Profile
{
    public GetPriceListProfile()
    {
        CreateMap<PriceList, GetPriceListDto>()

            .ForMember(
                dest => dest.ProductNumber,
                opt => opt.MapFrom(src => src.Product != null ? src.Product.Number : null))

            .ForMember(
                dest => dest.ProductName,
                opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))

            .ForMember(
                dest => dest.CustomerName,
                opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : null))

            .ForMember(
                dest => dest.TaxName,
                opt => opt.MapFrom(src => src.Tax != null ? src.Tax.Name : null))

            .ForMember(
                dest => dest.TaxPercentage,
                opt => opt.MapFrom(src => src.Tax != null ? src.Tax.Percentage : null));
    }
}

public class GetPriceListResult
{
    public List<GetPriceListDto>? Data { get; init; }
}

public class GetPriceListRequest : IRequest<GetPriceListResult>
{
    public bool IsDeleted { get; init; } = false;
    public string? ProductId { get; init; }
}

public class GetPriceListHandler(IMapper mapper, IQueryContext context)
    : IRequestHandler<GetPriceListRequest, GetPriceListResult>
{
    public async Task<GetPriceListResult> Handle(
        GetPriceListRequest request,
        CancellationToken cancellationToken)
    {
        var query = context
            .PriceList
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .Include(pl => pl.Product)
            .Include(pl => pl.Customer)
            .Include(pl => pl.Tax);

        var filtered = string.IsNullOrEmpty(request.ProductId)
            ? query
            : query.Where(pl => pl.ProductId == request.ProductId);

        var entities = await filtered.ToListAsync(cancellationToken);

        return new GetPriceListResult
        {
            Data = mapper.Map<List<GetPriceListDto>>(entities)
        };
    }
}
