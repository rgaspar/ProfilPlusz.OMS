using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.PriceListManager.Commands;

public class DeletePriceListResult
{
    public PriceList? Data { get; set; }
}

public class DeletePriceListRequest : IRequest<DeletePriceListResult>
{
    public string? Id { get; init; }
    public string? DeletedById { get; init; }
}

public class DeletePriceListHandler(
    ICommandRepository<PriceList> repository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeletePriceListRequest, DeletePriceListResult>
{
    public async Task<DeletePriceListResult> Handle(
        DeletePriceListRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(request.Id ?? string.Empty, cancellationToken)
            ?? throw new Exception("Price list entry not found.");

        entity.IsDeleted = true;
        entity.UpdatedById = request.DeletedById;

        repository.Update(entity);
        await unitOfWork.SaveAsync(cancellationToken);

        return new DeletePriceListResult { Data = entity };
    }
}
