using Application.Common.Repositories;
using Application.Features.NumberSequenceManager;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.ColorManager.Commands;

public class CreateColorResult
{
    public Color? Data { get; set; }
}

public class CreateColorRequest : IRequest<CreateColorResult>
{
    public string? Name { get; set; }

    public string? CreatedById { get; init; }
}

public class CreateColorValidator : AbstractValidator<CreateColorRequest>
{
    public CreateColorValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}

public class CreateColorHandler : IRequestHandler<CreateColorRequest, CreateColorResult>
{
    private readonly ICommandRepository<Color> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly NumberSequenceService _numberSequenceService;

    public CreateColorHandler(
        ICommandRepository<Color> repository,
        IUnitOfWork unitOfWork,
        NumberSequenceService numberSequenceService
    )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _numberSequenceService = numberSequenceService;
    }

    public async Task<CreateColorResult> Handle(
        CreateColorRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = new Color
        {
            CreatedById = request.CreatedById,

            Name = request.Name
        };

        await _repository.CreateAsync(entity, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CreateColorResult
        {
            Data = entity
        };
    }
}