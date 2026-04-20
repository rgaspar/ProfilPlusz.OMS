using Application.Common.Repositories;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.ColorManager.Commands;

public class DeleteColorResult
{
    public Color? Data { get; set; }
}

public class DeleteColorRequest : IRequest<DeleteColorResult>
{
    public string? Id { get; init; }
    public string? DeletedById { get; init; }
}

public class DeleteColorValidator : AbstractValidator<DeleteColorRequest>
{
    public DeleteColorValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class DeleteColorHandler : IRequestHandler<DeleteColorRequest, DeleteColorResult>
{
    private readonly ICommandRepository<Color> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteColorHandler(
        ICommandRepository<Color> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteColorResult> Handle(
        DeleteColorRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(
            request.Id ?? string.Empty,
            cancellationToken);

        if (entity == null)
        {
            throw new Exception($"Color not found: {request.Id}");
        }

        entity.UpdatedById = request.DeletedById;

        _repository.Delete(entity);

        await _unitOfWork.SaveAsync(cancellationToken);

        return new DeleteColorResult
        {
            Data = entity
        };
    }
}