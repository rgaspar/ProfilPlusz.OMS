using Application.Common.CQS.Queries;
using Application.Common.Extensions;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ColorManager.Queries;

public record GetColorListDto
{
    public string? Id { get; init; }
    public string? Name { get; set; }
    public string? Number { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAtUtc { get; init; }
}

public class GetColorListProfile : Profile
{
    public GetColorListProfile()
    {
        CreateMap<Color, GetColorListDto>();
    }
}

public class GetColorListResult
{
    public List<GetColorListDto>? Data { get; init; }
}

public class GetColorListRequest : IRequest<GetColorListResult>
{
    public bool IsDeleted { get; init; } = false;
}

public class GetColorListHandler : IRequestHandler<GetColorListRequest, GetColorListResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetColorListHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetColorListResult> Handle(
        GetColorListRequest request,
        CancellationToken cancellationToken)
    {
        var query = _context
            .Color
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .AsQueryable();

        var entities = await query.ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<GetColorListDto>>(entities);

        return new GetColorListResult
        {
            Data = dtos
        };
    }
}