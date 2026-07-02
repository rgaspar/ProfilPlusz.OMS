using Application.Common.CQS.Queries;
using Application.Common.Extensions;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProductCustomerManager.Queries;

public record GetProductCustomerListDto
{
    public string? Id { get; init; }
    public string? ProductId { get; init; }
    public string? ProductNumber { get; init; }
    public string? ProductName { get; init; }
    public string? CustomerId { get; init; }
    public string? CustomerName { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
}

public class GetProductCustomerListProfile : Profile
{
    public GetProductCustomerListProfile()
    {
        CreateMap<ProductCustomer, GetProductCustomerListDto>()
            .ForMember(dest => dest.ProductNumber, opt => opt.MapFrom(src => src.Product != null ? src.Product.Number : null))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : null));
    }
}

public class GetProductCustomerListResult
{
    public List<GetProductCustomerListDto>? Data { get; init; }
}

public class GetProductCustomerListRequest : IRequest<GetProductCustomerListResult>
{
    public bool IsDeleted { get; init; } = false;
}

public class GetProductCustomerListHandler(IMapper mapper, IQueryContext context)
    : IRequestHandler<GetProductCustomerListRequest, GetProductCustomerListResult>
{
    public async Task<GetProductCustomerListResult> Handle(
        GetProductCustomerListRequest request,
        CancellationToken cancellationToken)
    {
        var entities = await context.ProductCustomer
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .Include(pc => pc.Product)
            .Include(pc => pc.Customer)
            .ToListAsync(cancellationToken);

        return new GetProductCustomerListResult
        {
            Data = mapper.Map<List<GetProductCustomerListDto>>(entities)
        };
    }
}
