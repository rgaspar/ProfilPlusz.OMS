using Application.Common.Repositories;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.ProductCustomerManager.Commands;

public class CreateProductCustomerResult
{
    public ProductCustomer? Data { get; set; }
}

public class CreateProductCustomerRequest : IRequest<CreateProductCustomerResult>
{
    public string? ProductId { get; init; }
    public string? CustomerId { get; init; }
    public string? CreatedById { get; init; }
}

public class CreateProductCustomerValidator : AbstractValidator<CreateProductCustomerRequest>
{
    public CreateProductCustomerValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}

public class CreateProductCustomerHandler(
    ICommandRepository<ProductCustomer> repository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateProductCustomerRequest, CreateProductCustomerResult>
{
    public async Task<CreateProductCustomerResult> Handle(
        CreateProductCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new ProductCustomer
        {
            CreatedById = request.CreatedById,
            ProductId = request.ProductId,
            CustomerId = request.CustomerId
        };

        await repository.CreateAsync(entity, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        return new CreateProductCustomerResult { Data = entity };
    }
}
