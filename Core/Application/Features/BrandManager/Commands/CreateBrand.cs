using Application.Common.Repositories;
using Application.Features.NumberSequenceManager;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.BrandManager.Commands;

public class CreateBrandResult
{
    public Brand? Data { get; set; }
}

public class CreateBrandRequest : IRequest<CreateBrandResult>
{
    public string? Name { get; set; }

    public string? CreatedById { get; init; }
}

public class CreateBrandValidator : AbstractValidator<CreateBrandRequest>
{
    public CreateBrandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}

public class CreateBrandHandler : IRequestHandler<CreateBrandRequest, CreateBrandResult>
{
    private readonly ICommandRepository<Brand> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly NumberSequenceService _numberSequenceService;

    public CreateBrandHandler(
        ICommandRepository<Brand> repository,
        IUnitOfWork unitOfWork,
        NumberSequenceService numberSequenceService
    )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _numberSequenceService = numberSequenceService;
    }

    public async Task<CreateBrandResult> Handle(
        CreateBrandRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var entity = new Brand
        {
            CreatedById = request.CreatedById,

            Name = request.Name
        };

        await _repository.CreateAsync(entity, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CreateBrandResult
        {
            Data = entity
        };
    }
}