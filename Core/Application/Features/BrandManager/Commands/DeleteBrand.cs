using Application.Common.Repositories;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.BrandManager.Commands;

public class DeleteBrandResult
{
    public Brand? Data { get; set; }
}

public class DeleteBrandRequest : IRequest<DeleteBrandResult>
{
    public string? Id { get; init; }
    public string? DeletedById { get; init; }
}

public class DeleteBrandValidator : AbstractValidator<DeleteBrandRequest>
{
    public DeleteBrandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class DeleteBrandHandler : IRequestHandler<DeleteBrandRequest, DeleteBrandResult>
{
    private readonly ICommandRepository<Brand> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBrandHandler(
        ICommandRepository<Brand> repository,
        IUnitOfWork unitOfWork
    )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteBrandResult> Handle(
        DeleteBrandRequest request,
        CancellationToken cancellationToken
    )
    {
        var entity = await _repository.GetAsync(
            request.Id ?? string.Empty,
            cancellationToken
        );

        if (entity == null)
        {
            throw new Exception($"Entity not found: {request.Id}");
        }

        entity.UpdatedById = request.DeletedById;

        _repository.Delete(entity);

        await _unitOfWork.SaveAsync(cancellationToken);

        return new DeleteBrandResult
        {
            Data = entity
        };
    }
}