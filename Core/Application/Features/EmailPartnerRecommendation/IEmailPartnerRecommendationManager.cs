using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.EmailPartnerRecommendation
{
    public interface IEmailPartnerRecommendationManager
    {
        Task ProcessUnreadEmailsAsync(CancellationToken cancellationToken = default);
    }
}
