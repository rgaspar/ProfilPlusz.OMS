using Application.Common.CQS.Queries;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BrandManager.Queries;

public record GetBrandSingleDto
{
    public string? Id { get; init; }
    public string? Name { get; set; }
    public string? Number { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAtUtc { get; init; }
}

public class GetBrandSingleProfile : Profile
{
    public GetBrandSingleProfile()
    {
        CreateMap<Brand, GetBrandSingleDto>();
    }
}

public class GetBrandSingleResult
{
    public GetBrandSingleDto? Data { get; init; }
}

public class GetBrandSingleRequest : IRequest<GetBrandSingleResult>
{
    public string? Id { get; init; }
}

public class GetBrandSingleHandler : IRequestHandler<GetBrandSingleRequest, GetBrandSingleResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetBrandSingleHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetBrandSingleResult> Handle(
        GetBrandSingleRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Id))
        {
            throw new Exception("Id is required");
        }

        var entity = await _context
            .Brand
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new Exception($"Brand not found: {request.Id}");
        }

        var dto = _mapper.Map<GetBrandSingleDto>(entity);

        return new GetBrandSingleResult
        {
            Data = dto
        };
    }
}