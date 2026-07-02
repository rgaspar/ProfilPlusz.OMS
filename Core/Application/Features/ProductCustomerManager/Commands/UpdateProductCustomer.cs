using Application.Common.Repositories;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.ProductCustomerManager.Commands;

public class UpdateProductCustomerResult
{
    public ProductCustomer? Data { get; set; }
}

public class UpdateProductCustomerRequest : IRequest<UpdateProductCustomerResult>
{
    public string? Id { get; init; }
    public string? ProductId { get; init; }
    public string? CustomerId { get; init; }
    public string? UpdatedById { get; init; }
}

public class UpdateProductCustomerValidator : AbstractValidator<UpdateProductCustomerRequest>
{
    public UpdateProductCustomerValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
    }
}

public class UpdateProductCustomerHandler(
    ICommandRepository<ProductCustomer> repository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateProductCustomerRequest, UpdateProductCustomerResult>
{
    public async Task<UpdateProductCustomerResult> Handle(
        UpdateProductCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(request.Id ?? string.Empty, cancellationToken)
            ?? throw new Exception("Product customer entry not found.");

        entity.UpdatedById = request.UpdatedById;
        entity.ProductId = request.ProductId;
        entity.CustomerId = request.CustomerId;

        repository.Update(entity);
        await unitOfWork.SaveAsync(cancellationToken);

        return new UpdateProductCustomerResult { Data = entity };
    }
}
