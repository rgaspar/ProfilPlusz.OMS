using Application.Common.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class BrandSeeder
{
    private readonly ICommandRepository<Brand> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public BrandSeeder(
        ICommandRepository<Brand> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task GenerateDataAsync()
    {
        var brands = new[]
        {
            "Progress",
            "Global",
            "Faber"
        };

        var existing = await _repository
            .GetQuery()
            .Select(x => x.Name)
            .ToListAsync();

        foreach (var brand in brands)
        {
            if (!existing.Contains(brand))
            {
                await _repository.CreateAsync(
                    new Brand
                    {
                        Name = brand
                    });
            }
        }

        await _unitOfWork.SaveAsync();
    }
}