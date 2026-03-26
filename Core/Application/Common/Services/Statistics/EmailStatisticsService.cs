using Application.Common.DTOs.Statistics;
using Application.Common.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services.Statistics
{
    public class EmailStatisticsService : IEmailStatisticsService
    {
        private readonly IProcessedEmailRepository _repository;

        public EmailStatisticsService(IProcessedEmailRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProcessedDailyEmailCountDto>> GetDailyStatsAsync(DateTime? from = null, DateTime? to = null)
        {
            return await _repository.GetDailyProcessedEmailCountAsync(from, to);
        }
    }
}
