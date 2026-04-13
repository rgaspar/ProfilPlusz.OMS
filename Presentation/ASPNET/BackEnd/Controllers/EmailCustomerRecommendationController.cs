using Application.Features.EmailPartnerRecommendation;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET.BackEnd.Controllers
{
    [ApiController]
    [Route("api/email-customer-recommendations")]
    public class EmailCustomerRecommendationController : ControllerBase
    {
        private readonly IEmailCustomerRecommendationManager _manager;

        public EmailCustomerRecommendationController(
            IEmailCustomerRecommendationManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Feldolgozza az olvasatlan emaileket és partner ajánlás draftot készít.
        /// </summary>
        [HttpPost("process-unread")]
        public async Task<IActionResult> ProcessUnreadEmails(
            CancellationToken cancellationToken)
        {
            await _manager.ProcessUnreadEmailsAsync(cancellationToken);

            return Ok(new
            {
                Message = "Email feldolgozás elindítva."
            });
        }
    }
}
