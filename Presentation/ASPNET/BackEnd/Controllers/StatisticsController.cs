using Application.Common.Services.Statistics;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET.BackEnd.Controllers
{
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly IEmailStatisticsService _emailStatisticsService;

        public StatisticsController(IEmailStatisticsService emailStatisticsService)
        {
            _emailStatisticsService = emailStatisticsService;
        }

        /// <summary>
        /// Naponta feldolgozott emailek száma
        /// </summary>
        [HttpGet("emails/processed/daily")]
        public async Task<IActionResult> GetDailyProcessedEmails([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var result = await _emailStatisticsService.GetDailyStatsAsync(from, to);
            return Ok(result);
        }
    }
}
