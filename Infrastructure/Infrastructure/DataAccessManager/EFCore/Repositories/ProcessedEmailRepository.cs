using Application.Common.DTOs.Statistics;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccessManager.EFCore.Repositories
{
    public class ProcessedEmailRepository : CommandRepository<ProcessedEmail>, IProcessedEmailRepository
    {
        public ProcessedEmailRepository(CommandContext context)
            : base(context)
        {
        }

        public async Task<bool> ExistsByExternalIdAsync(
            string externalId,
            CancellationToken cancellationToken)
        {
            return await _context.ProcessedEmail.AnyAsync(x => x.ExternalId == externalId, cancellationToken);
        }

        public async Task SaveAsync(ProcessedEmail email, CancellationToken cancellationToken)
        {
            _context.ProcessedEmail.Add(email);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ProcessedDailyEmailCountDto>> GetDailyProcessedEmailCountAsync(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.ProcessedEmail.Where(e => e.ProcessedAt != null);

            if (from.HasValue)
            {
                query = query.Where(e => e.ProcessedAt >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(e => e.ProcessedAt <= to.Value);
            }

            return await query
                .GroupBy(e => e.ProcessedAt.Date)
                .Select(g => new ProcessedDailyEmailCountDto
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }
    }
}
