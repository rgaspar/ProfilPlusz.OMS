using Application.Common.Repositories;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.PriceListManager.Commands;

public class CreatePriceListResult
{
    public PriceList? Data { get; set; }
}

public class CreatePriceListRequest : IRequest<CreatePriceListResult>
{
    public string? ProductId { get; init; }
    public string? CustomerId { get; init; }
    public string? TaxId { get; init; }

    public double NetPrice { get; init; }
    public double? GrossPrice { get; init; }

    public decimal? QuantityDiscount { get; init; }
    public DateTime? DiscountFrom { get; init; }
    public DateTime? DiscountTo { get; init; }

    public string? CreatedById { get; init; }
}

public class CreatePriceListValidator : AbstractValidator<CreatePriceListRequest>
{
    public CreatePriceListValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.NetPrice)
            .GreaterThanOrEqualTo(0);
    }
}

public class CreatePriceListHandler(
    ICommandRepository<PriceList> repository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreatePriceListRequest, CreatePriceListResult>
{
    public async Task<CreatePriceListResult> Handle(
        CreatePriceListRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new PriceList
        {
            CreatedById = request.CreatedById,
            ProductId = request.ProductId,
            CustomerId = request.CustomerId,
            TaxId = request.TaxId,
            NetPrice = request.NetPrice,
            GrossPrice = request.GrossPrice,
            QuantityDiscount = request.QuantityDiscount,
            DiscountFrom = request.DiscountFrom,
            DiscountTo = request.DiscountTo
        };

        await repository.CreateAsync(entity, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        return new CreatePriceListResult { Data = entity };
    }
}
