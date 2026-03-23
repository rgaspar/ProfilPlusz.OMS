using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IAnswerTemplateRepository
    {
        Task<AnswerTemplate?> GetByKeyAsync(string key);
    }
}
