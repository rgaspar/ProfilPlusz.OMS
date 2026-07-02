using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.ProductCustomerManager.Commands;

public class DeleteProductCustomerResult
{
    public ProductCustomer? Data { get; set; }
}

public class DeleteProductCustomerRequest : IRequest<DeleteProductCustomerResult>
{
    public string? Id { get; init; }
    public string? DeletedById { get; init; }
}

public class DeleteProductCustomerHandler(
    ICommandRepository<ProductCustomer> repository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteProductCustomerRequest, DeleteProductCustomerResult>
{
    public async Task<DeleteProductCustomerResult> Handle(
        DeleteProductCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(request.Id ?? string.Empty, cancellationToken)
            ?? throw new Exception("Product customer entry not found.");

        entity.IsDeleted = true;
        entity.UpdatedById = request.DeletedById;

        repository.Update(entity);
        await unitOfWork.SaveAsync(cancellationToken);

        return new DeleteProductCustomerResult { Data = entity };
    }
}
