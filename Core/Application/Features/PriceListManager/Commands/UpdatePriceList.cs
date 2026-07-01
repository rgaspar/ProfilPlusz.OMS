using Application.Common.Repositories;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.PriceListManager.Commands;

public class UpdatePriceListResult
{
    public PriceList? Data { get; set; }
}

public class UpdatePriceListRequest : IRequest<UpdatePriceListResult>
{
    public string? Id { get; init; }
    public string? ProductId { get; init; }
    public string? CustomerId { get; init; }
    public string? TaxId { get; init; }

    public double NetPrice { get; init; }
    public double? GrossPrice { get; init; }

    public decimal? QuantityDiscount { get; init; }
    public DateTime? DiscountFrom { get; init; }
    public DateTime? DiscountTo { get; init; }

    public string? UpdatedById { get; init; }
}

public class UpdatePriceListValidator : AbstractValidator<UpdatePriceListRequest>
{
    public UpdatePriceListValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.NetPrice)
            .GreaterThanOrEqualTo(0);
    }
}

public class UpdatePriceListHandler(
    ICommandRepository<PriceList> repository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdatePriceListRequest, UpdatePriceListResult>
{
    public async Task<UpdatePriceListResult> Handle(
        UpdatePriceListRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(request.Id ?? string.Empty, cancellationToken)
            ?? throw new Exception("Price list entry not found.");

        entity.UpdatedById = request.UpdatedById;
        entity.ProductId = request.ProductId;
        entity.CustomerId = request.CustomerId;
        entity.TaxId = request.TaxId;
        entity.NetPrice = request.NetPrice;
        entity.GrossPrice = request.GrossPrice;
        entity.QuantityDiscount = request.QuantityDiscount;
        entity.DiscountFrom = request.DiscountFrom;
        entity.DiscountTo = request.DiscountTo;

        repository.Update(entity);
        await unitOfWork.SaveAsync(cancellationToken);

        return new UpdatePriceListResult { Data = entity };
    }
}
