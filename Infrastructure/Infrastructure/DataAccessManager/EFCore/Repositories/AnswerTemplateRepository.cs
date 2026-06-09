using Application.Common.Extensions;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccessManager.EFCore.Repositories
{
    public class AnswerTemplateRepository : CommandRepository<AnswerTemplate>, IAnswerTemplateRepository
    {
        public AnswerTemplateRepository(CommandContext context)
            : base(context)
        {
        }

        public async Task<AnswerTemplate?> GetByKeyAsync(string key)
        {
            return await _context.Set<AnswerTemplate>().ApplyIsDeletedFilter().FirstOrDefaultAsync(x => x.Key == key);
        }
    }
}
