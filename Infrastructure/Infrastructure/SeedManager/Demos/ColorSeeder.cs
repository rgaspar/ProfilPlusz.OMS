using Application.Common.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SeedManager.Demos;

public class ColorSeeder
{
    private readonly ICommandRepository<Color> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ColorSeeder(
        ICommandRepository<Color> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task GenerateDataAsync()
    {
        var colors = new[]
        {
            "Cream",
            "Fekete",
            "Kék",
            "Átlátszó",
            "Rozsdamentes"
        };

        var existing = await _repository
            .GetQuery()
            .Select(x => x.Name)
            .ToListAsync();

        foreach (var color in colors)
        {
            if (!existing.Contains(color))
            {
                await _repository.CreateAsync(
                    new Color
                    {
                        Name = color
                    });
            }
        }

        await _unitOfWork.SaveAsync();
    }
}