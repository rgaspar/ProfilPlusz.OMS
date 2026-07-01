using Application.Common.CQS.Queries;
using Application.Common.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CustomerManager.Queries;

public class CustomerExistsByEmailResult
{
    public bool Exists { get; init; }
}

public class CustomerExistsByEmailRequest : IRequest<CustomerExistsByEmailResult>
{
    public string Email { get; init; } = default!;
    public bool IsDeleted { get; init; } = false;
}

public class CustomerExistsByEmailHandler
    : IRequestHandler<CustomerExistsByEmailRequest, CustomerExistsByEmailResult>
{
    private readonly IQueryContext _context;

    public CustomerExistsByEmailHandler(IQueryContext context)
    {
        _context = context;
    }

    public async Task<CustomerExistsByEmailResult> Handle(
    CustomerExistsByEmailRequest request,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return new CustomerExistsByEmailResult
            {
                Exists = false
            };
        }

        var normalizedEmail = NormalizeEmail(request.Email);

        var exists = await _context
        .Customer
        .AsNoTracking()
        .ApplyIsDeletedFilter(request.IsDeleted)
        .AnyAsync(customer =>
            customer.EmailAddress != null && customer.EmailAddress.Trim().ToLower() == normalizedEmail,
            cancellationToken);

        return new CustomerExistsByEmailResult
        {
            Exists = exists
        };
    }
    private static string NormalizeEmail(string email)
    {
        return email?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}