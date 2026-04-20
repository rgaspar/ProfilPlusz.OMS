using Application.Common.CQS.Queries;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ColorManager.Queries;

public record GetColorSingleDto
{
    public string? Id { get; init; }
    public string? Name { get; set; }
    public string? Number { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAtUtc { get; init; }
}

public class GetColorSingleProfile : Profile
{
    public GetColorSingleProfile()
    {
        CreateMap<Color, GetColorSingleDto>();
    }
}

public class GetColorSingleResult
{
    public GetColorSingleDto? Data { get; init; }
}

public class GetColorSingleRequest : IRequest<GetColorSingleResult>
{
    public string? Id { get; init; }
}

public class GetColorSingleHandler : IRequestHandler<GetColorSingleRequest, GetColorSingleResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetColorSingleHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetColorSingleResult> Handle(
        GetColorSingleRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Id))
        {
            throw new Exception("Id is required");
        }

        var entity = await _context
            .Color
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new Exception($"Color not found: {request.Id}");
        }

        var dto = _mapper.Map<GetColorSingleDto>(entity);

        return new GetColorSingleResult
        {
            Data = dto
        };
    }
}