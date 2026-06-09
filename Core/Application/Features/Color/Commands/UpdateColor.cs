using Application.Common.Repositories;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.ColorManager.Commands;

public class UpdateColorResult
{
    public Color? Data { get; set; }
}

public class UpdateColorRequest : IRequest<UpdateColorResult>
{
    public string? Id { get; init; }

    public string? Name { get; set; }

    public string? UpdatedById { get; init; }
}

public class UpdateColorValidator : AbstractValidator<UpdateColorRequest>
{
    public UpdateColorValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}

public class UpdateColorHandler : IRequestHandler<UpdateColorRequest, UpdateColorResult>
{
    private readonly ICommandRepository<Color> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateColorHandler(
        ICommandRepository<Color> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateColorResult> Handle(
        UpdateColorRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(
            request.Id ?? string.Empty,
            cancellationToken);

        if (entity == null)
        {
            throw new Exception($"Color not found: {request.Id}");
        }

        entity.UpdatedById = request.UpdatedById;

        entity.Name = request.Name;

        _repository.Update(entity);

        await _unitOfWork.SaveAsync(cancellationToken);

        return new UpdateColorResult
        {
            Data = entity
        };
    }
}