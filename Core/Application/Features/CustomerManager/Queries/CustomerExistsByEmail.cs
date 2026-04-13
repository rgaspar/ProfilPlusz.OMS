using Application.Common.CQS.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CustomerManager.Queries;

public class CustomerExistsByEmailRequest : IRequest<bool>
{
    public string EmailAddress { get; init; } = default!;
}

public class CustomerExistsByEmailHandler
    : IRequestHandler<CustomerExistsByEmailRequest, bool>
{
    private readonly IQueryContext _context;

    public CustomerExistsByEmailHandler(IQueryContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        CustomerExistsByEmailRequest request,
        CancellationToken cancellationToken)
    {
        return await _context.Customer
            .AsNoTracking()
            .AnyAsync(
                x => x.EmailAddress == request.EmailAddress,
                cancellationToken);
    }
}