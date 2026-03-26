using Application.Common.DTOs.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services.Statistics
{
    public interface IEmailStatisticsService
    {
        Task<List<ProcessedDailyEmailCountDto>> GetDailyStatsAsync(DateTime? from, DateTime? to);
    }
}
