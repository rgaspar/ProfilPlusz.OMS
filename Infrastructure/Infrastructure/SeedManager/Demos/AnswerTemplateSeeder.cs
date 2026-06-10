using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Repositories;

namespace Infrastructure.SeedManager.Demos;

public class AnswerTemplateSeeder
{
    private readonly ICommandRepository<AnswerTemplate> _answerTemplateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AnswerTemplateSeeder(
        ICommandRepository<AnswerTemplate> answerTemplateRepository,
        IUnitOfWork unitOfWork
    )
    {
        _answerTemplateRepository = answerTemplateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task GenerateDataAsync()
    {
        var answerTemplates = new List<AnswerTemplate>
        {
            new AnswerTemplate
            {
                Key = "EMAIL_CUSTOMER_RECOMMENDATION",
                Path = "Email/EmailCustomerRecommendation.html",
                Subject = "ProfilPlusz - Partner ajánlás a rendeléséhez",
                DefaultRecipients = "joni9103@outlook.com"
            }
        };

        foreach (var answerTemplate in answerTemplates)
        {
            await _answerTemplateRepository.CreateAsync(answerTemplate);
        }

        await _unitOfWork.SaveAsync();
    }
}


