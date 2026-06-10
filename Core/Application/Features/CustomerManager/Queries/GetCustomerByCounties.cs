using Application.Common.CQS.Queries;
using Application.Common.Extensions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CustomerManager.Queries;

public class GetCustomersByCountiesResult
{
    public List<Customer>? Data { get; init; }
}

public class GetCustomersByCountiesRequest : IRequest<GetCustomersByCountiesResult>
{
    public IEnumerable<string> States { get; init; } = [];
    public bool IsDeleted { get; init; } = false;
}

public class GetCustomersByCountiesHandler
    : IRequestHandler<GetCustomersByCountiesRequest, GetCustomersByCountiesResult>
{
    private readonly IQueryContext _context;

    public GetCustomersByCountiesHandler(IQueryContext context)
    {
        _context = context;
    }

    public async Task<GetCustomersByCountiesResult> Handle(
        GetCustomersByCountiesRequest request,
        CancellationToken cancellationToken)
    {
        if (request.States == null || !request.States.Any())
        {
            return new GetCustomersByCountiesResult
            {
                Data = []
            };
        }

        var normalizedCounties = request.States.Select(NormalizeCounty).ToList();

        var customers = await _context
            .Customer
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .Include(x => x.AddressList)
            .Where(customer =>
                customer.AddressList.Any(address =>
                    address.Type == AddressType.Headquarters &&
                    !string.IsNullOrWhiteSpace(address.State) &&
                    normalizedCounties.Contains(address.State.Trim().ToLower())
                ))
            .ToListAsync(cancellationToken);


        return new GetCustomersByCountiesResult
        {
            Data = customers
        };
    }

    private static string NormalizeCounty(string county)
    {
        return county?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}